﻿using log4net;
using org.apache.zookeeper;
using org.apache.zookeeper.data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ZkClient.Net
{
    public class ZkClient : Watcher, IDisposable
    {
        private readonly string _zkServers;
        private readonly int _connectionTimeout;
        private readonly int _oprationRetryTimeout;
        private Event.KeeperState _currentState;
        private readonly AutoResetEvent _stateChangEvent = new AutoResetEvent(false);
        private readonly object _zkEventLock = new object();
        private ZooKeeper _zk;
        private bool _isDisposed = false;

        private readonly List<IZkStateListener> _stateListener = new List<IZkStateListener>();

        private readonly ConcurrentDictionary<string, HashSet<IZkChildListener>> _childListener =
            new ConcurrentDictionary<string, HashSet<IZkChildListener>>();

        private readonly ConcurrentDictionary<string, HashSet<IZkDataListener>> _dataListener =
            new ConcurrentDictionary<string, HashSet<IZkDataListener>>();

        private static readonly ILog Logger = LogManager.GetLogger(typeof(ZkClient));

        /// <inheritdoc />
        /// <summary>
        /// 初始化zkclient
        /// </summary>
        /// <param name="zkOptions"></param>
        public ZkClient(ZkOptions zkOptions)
            : this(zkOptions.ZkServers, zkOptions.SessionTimeout, zkOptions.ConnectionTimeout,
                zkOptions.OprationRetryTimeout)
        {
           
        }

        /// <summary>
        /// 初始化zkclient
        /// </summary>
        /// <param name="zkServers">zookeeper服务器地址，192.168.2.103:2181,192.168.2.104:2181,192.168.2.105:2181</param>
        /// <param name="sessionTimout">客户端与zk服务器的session超时时间，单位毫秒</param>
        /// <param name="connectionTimeout">连接zk服务器的超时时间，单位毫秒</param>
        /// <param name="oprationRetryTimeout">执行操作的超时时间，单位毫秒</param>
        public ZkClient(string zkServers, int sessionTimout, int connectionTimeout, int oprationRetryTimeout)
        {
            _zkServers = zkServers;
            _connectionTimeout = connectionTimeout;
            _oprationRetryTimeout = oprationRetryTimeout;

            _zk = Create(zkServers, sessionTimout);

            Logger.Info($"ZkClient init, _zkServers: {_zkServers}, sessionTimout: {sessionTimout}, _connectionTimeout: {_connectionTimeout}");
        }

        private ZooKeeper Create(string connectionString, int sessionTimeout)
        {
            return new ZooKeeper(connectionString, sessionTimeout, this);
        }

        #region CRUD

        public async Task CreatePersistent(string path, bool createParents = true)
        {
            await CreatePersistent(path, createParents, ZooDefs.Ids.OPEN_ACL_UNSAFE);
        }

        public async Task CreatePersistent(string path, bool createParents, List<ACL> acl)
        {
            try
            {
                await CreatePersistent(path, "", acl);
            }
            catch (KeeperException.NodeExistsException e)
            {
                if (!createParents)
                {
                    throw;
                }
            }
            catch (KeeperException.NoNodeException)
            {
                var parentDir = path.Substring(0, path.LastIndexOf("/", StringComparison.Ordinal));
                await CreatePersistent(parentDir, createParents, acl);
                await CreatePersistent(path, createParents, acl);
            }
        }

        public async Task<string> CreatePersistent(string path, string data)
        {
            return await Create(path, data, ZooDefs.Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT);
        }

        public async Task<string> CreatePersistent(string path, string data, List<ACL> acl)
        {
            return await Create(path, data, acl, CreateMode.PERSISTENT);
        }

        public async Task<string> CreatePersistentSequential(string path, string data)
        {
            return await Create(path, data, ZooDefs.Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT_SEQUENTIAL);
        }

        public async Task<string> CreatePersistentSequential(string path, string data, List<ACL> acl)
        {
            return await Create(path, data, acl, CreateMode.PERSISTENT_SEQUENTIAL);
        }

        public async Task CreateEphemeral(string path)
        {
            await Create(path, "", ZooDefs.Ids.OPEN_ACL_UNSAFE, CreateMode.EPHEMERAL);
        }

        public async Task CreateEphemeral(string path, List<ACL> acl)
        {
            await Create(path, "", acl, CreateMode.EPHEMERAL);
        }


        public async Task<string> Create(string path, string data, CreateMode mode)
        {
            return await RetryUntilConnected(async () => await _zk.createAsync(path, Encoding.UTF8.GetBytes(data), ZooDefs.Ids.OPEN_ACL_UNSAFE, mode));
        }

        public async Task<string> Create(string path, string data, List<ACL> acl, CreateMode mode)
        {
            try
            {
                return await RetryUntilConnected(async () => await _zk.createAsync(path, Encoding.UTF8.GetBytes(data), acl, mode));
            }
            catch (KeeperException.NodeExistsException e)
            {
                return path;
            }
        }

        public async Task<string> GetData(string path)
        {
            var dataResult = await RetryUntilConnected(async () => await _zk.getDataAsync(path, HasListeners(path)));
            if (dataResult.Data != null && dataResult.Data.Length > 0)
            {
                return Encoding.UTF8.GetString(dataResult.Data);
            }
            return string.Empty;
        }

        public async Task<string> GetData(string path, bool watch)
        {
            var dataResult = await RetryUntilConnected(async () => await _zk.getDataAsync(path, watch));
            if (dataResult.Data != null && dataResult.Data.Length > 0)
            {
                return Encoding.UTF8.GetString(dataResult.Data);
            }
            return string.Empty;
        }

        public async Task<bool> SetData(string path, string data)
        {
            try
            {
                await RetryUntilConnected(async () => await _zk.setDataAsync(path, Encoding.UTF8.GetBytes(data)));
            }
            catch (KeeperException e)
            {
                return await Task.FromResult(false);
            }
            return await Task.FromResult(true);
        }

        public async Task<List<string>> GetChildren(string path)
        {
            var result = await RetryUntilConnected(async () => await _zk.getChildrenAsync(path, HasListeners(path)));
            return result.Children;
        }

        public async Task<List<string>> GetChildren(string path, bool watch)
        {
            var result = await RetryUntilConnected(async () => await _zk.getChildrenAsync(path, watch));
            return result.Children;
        }

        public async Task<bool> ExistsAsync(string path)
        {
            return await RetryUntilConnected(async () => await _zk.existsAsync(path, HasListeners(path)) != null);
        }

        public async Task<bool> ExistsAsync(string path, bool watch)
        {
            return await RetryUntilConnected(async () => await _zk.existsAsync(path, watch) != null);
        }

        public async Task<bool> DeleteRecursive(string path)
        {
            List<string> children;
            try
            {
                children = await GetChildren(path);
            }
            catch (KeeperException.NoNodeException e)
            {
                return true;
            }
            foreach (var subPath in children)
            {
                if (!await DeleteRecursive(path + "/" + subPath))
                {
                    return false;
                }
            }
            return await DeleteAsync(path);
        }

        public async Task<bool> DeleteAsync(string path)
        {
            try
            {
                await RetryUntilConnected(async () =>
                {
                    await _zk.deleteAsync(path);
                    return await Task.FromResult(true);
                });
            }
            catch (KeeperException.NoNodeException e)
            {
                return await Task.FromResult(false);
            }

            return await Task.FromResult(true);
        }

        #endregion

        #region 重试机制

        /// <summary>
        /// 等待Zookeeper变为某种状态
        /// </summary>
        /// <param name="keeperState"></param>
        /// <param name="waitTimeout"></param>
        /// <returns></returns>
        public bool WaitForKeeperState(Event.KeeperState keeperState, TimeSpan waitTimeout)
        {
            var stillWaiting = true;
            while (_currentState != keeperState)
            {
                if (!stillWaiting)
                {
                    return false;
                }

                Logger.Info($"waitForKeeperState: {keeperState}, currentState: {_currentState}");

                stillWaiting = _stateChangEvent.WaitOne(waitTimeout);

                if (_currentState == Event.KeeperState.AuthFailed)
                {
                    throw new KeeperException.AuthFailedException();
                }
            }
            return true;
        }

        /// <summary>
        /// 重试直到连接到Zookeeper服务器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="callable"></param>
        /// <returns></returns>
        public async Task<T> RetryUntilConnected<T>(Func<Task<T>> callable)
        {
            var operationStartTime = DateTime.Now;
            while (true)
            {
                try
                {
                    return await callable();
                }
                catch (KeeperException.ConnectionLossException)
                {
                    Logger.Info("ConnectionLossException, retry...");
                    await Task.Yield();
                    WaitForRetry();
                }
                catch (KeeperException.SessionExpiredException)
                {
                    Logger.Info("SessionExpiredException, retry...");
                    await Task.Yield();
                    WaitForRetry();
                }
                if (_oprationRetryTimeout > -1 && (DateTime.Now - operationStartTime).TotalMilliseconds > _oprationRetryTimeout)
                {
                    throw new TimeoutException("Operation cannot be retried because of retry timeout(" + _oprationRetryTimeout + " milli seconds)");
                }
            }
        }

        private void WaitForRetry()
        {
            if (_oprationRetryTimeout < 0)
            {
                WaitUntilConnected();
            }
            
            WaitUntilConnected(TimeSpan.FromMilliseconds(_oprationRetryTimeout));
        }

        /// <summary>
        /// 等待Zookeeper连接状态
        /// </summary>
        /// <returns></returns>
        public bool WaitUntilConnected()
        {
            return WaitForKeeperState(Event.KeeperState.SyncConnected, TimeSpan.MaxValue);
        }

        public bool WaitUntilConnected(TimeSpan waitTimeout)
        {
            return WaitForKeeperState(Event.KeeperState.SyncConnected, waitTimeout);
        }

        #endregion

        #region 订阅zookeeper事件

        public void SubscribeStateChange(IZkStateListener listener)
        {
            lock (this)
            {
                _stateListener.Add(listener);
            }
        }

        public void UnSubscribeStateChange(IZkStateListener listener)
        {
            lock (this)
            {
                _stateListener.Remove(listener);
            }
        }

        public List<string> SubscribeChildChange(string path, IZkChildListener listener)
        {
            lock (this)
            {
                _childListener.TryGetValue(path, out var childListeners);
                if (childListeners == null)
                {
                    childListeners = new HashSet<IZkChildListener>();
                }
                childListeners.Add(listener);
                _childListener[path] = childListeners;
            }

            return Task.Run(async () => await WatchForChildren(path)).Result;
        }

        public void UnSubscribeChildChange(string path, IZkChildListener listener)
        {
            lock (this)
            {
                _childListener.TryGetValue(path, out var childListeners);
                childListeners?.Remove(listener);
                _childListener[path] = childListeners;
            }
        }

        public void SubscribeDataChange(string path, IZkDataListener listener)
        {
            lock (this)
            {
                _dataListener.TryGetValue(path, out var dataListeners);
                if (dataListeners == null)
                {
                    dataListeners = new HashSet<IZkDataListener>();
                }
                dataListeners.Add(listener);
                _dataListener[path] = dataListeners;
            }

            Task.Run(async () => await WatchForData(path));
        }

        public void UnSubscribeDataChange(string path, IZkDataListener listener)
        {
            lock (this)
            {
                _dataListener.TryGetValue(path, out var dataListeners);
                dataListeners?.Remove(listener);
                _dataListener[path] = dataListeners;
            }
        }

        public void UnSubscribeAll()
        {
            lock (this)
            {
                _childListener.Clear();
                _dataListener.Clear();
                _stateListener.Clear();
            }
        }

        public async Task WatchForData(string path)
        {
            await RetryUntilConnected(async () => await ExistsAsync(path, true));
        }

        public async Task<List<string>> WatchForChildren(string path)
        {
            return await RetryUntilConnected(async () =>
            {
                await ExistsAsync(path, true);
                try
                {
                    return await GetChildren(path, true);
                }
                catch (KeeperException.NoNodeException)
                {

                }
                return null;
            });
        }

        #endregion

        #region 处理状态变更

        public override async Task process(WatchedEvent @event)
        {
            if (_isDisposed)
            {
                return;
            }
            var stateChanged = @event.getPath() == null;
            var dataChanged = @event.get_Type() == Event.EventType.NodeChildrenChanged
                              || @event.get_Type() == Event.EventType.NodeCreated
                              || @event.get_Type() == Event.EventType.NodeDeleted
                              || @event.get_Type() == Event.EventType.NodeDataChanged;

            if (stateChanged)
            {
                await ProcessStateChange(@event);   
            }
            else if (dataChanged)
            {
                await ProcessDataOrChildChange(@event);
            }
            else if (@event.getState() == Event.KeeperState.Expired)
            {
                // If the session expired we have to signal all conditions, because watches might have been removed and
                // there is no guarantee that those
                // We also have to notify all listeners that something might have changed
                Logger.Info("session过期");
                await FireAllEvents();
            }

        }

        /// <summary>
        /// 处理zk状态变更
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        private async Task ProcessStateChange(WatchedEvent @event)
        {
            SetCurrentState(@event.getState());
            //触发注册的StateChange事件
            FireStateChangedEvent(@event.getState());

            if (@event.getState() == Event.KeeperState.Expired)
            {
                Task.Run(async () => await Reconnect().ConfigureAwait(false))
                    .ConfigureAwait(false).GetAwaiter().GetResult();

                FireNewSessionEvent();
            }
            
            _stateChangEvent.Set();

            await Task.FromResult(0);
        }


        private async Task ProcessDataOrChildChange(WatchedEvent @event)
        {
            string path = @event.getPath();

            if (@event.get_Type() == Event.EventType.NodeChildrenChanged || @event.get_Type() == Event.EventType.NodeDeleted
                || @event.get_Type() == Event.EventType.NodeCreated)
            {
                _childListener.TryGetValue(path, out var zkChildListeners);
                if (zkChildListeners != null && zkChildListeners.Count > 0)
                {
                    await FireChildChangedEvents(path, zkChildListeners);
                }
            }

            if (@event.get_Type() == Event.EventType.NodeDataChanged || @event.get_Type() == Event.EventType.NodeDeleted
                || @event.get_Type() == Event.EventType.NodeCreated)
            {
                _dataListener.TryGetValue(path, out var dataListeners);
                if (dataListeners != null && dataListeners.Count > 0)
                {
                    await FireDataChangedEvents(path, dataListeners);
                }
            }
        }

        private async Task FireAllEvents()
        {
            foreach (var item in _dataListener)
            {
                await FireDataChangedEvents(item.Key, item.Value);
            }

            foreach (var item in _childListener)
            {
                await FireChildChangedEvents(item.Key, item.Value);
            }
        }

        private async Task FireDataChangedEvents(string path, HashSet<IZkDataListener> dataListeners)
        {
            try
            {
                // reinstall the watch
                foreach (var listener in dataListeners)
                {
                    try
                    {
                        await ExistsAsync(path);
                        var data = await GetData(path);
                        listener.HandleDataChange(path, data);
                    }
                    catch (KeeperException.NoNodeException e)
                    {
                        listener.HandleDataDeleted(path);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"FireDataChangedEvents exception: {e.ToString()}");
            }
        }

        private async Task FireChildChangedEvents(string path, HashSet<IZkChildListener> childListeners)
        {
            try
            {
                // reinstall the watch
                foreach (var listener in childListeners)
                {
                    try
                    {
                        // if the node doesn't exist we should listen for the root node to reappear
                        await ExistsAsync(path, true);
                        var children = await GetChildren(path, true);
                        listener.HandleChildChange(path, children);
                    }
                    catch (KeeperException.NoNodeException e)
                    {
                        listener.HandleChildChange(path, null);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"FireChildChangedEvents exception: {e.ToString()}");
            }
        }

       
        private void FireStateChangedEvent(Event.KeeperState state)
        {
            foreach (var listener in _stateListener)
            {
                Task.Factory.StartNew(() => listener.HandleStateChanged(state));
            }
        }

        private void FireNewSessionEvent()
        {
            foreach (var listener in _stateListener)
            {
                Task.Factory.StartNew(() => listener.HandleNewSession());
            }
        }

        private void FireSessionEstablishmentErrorEvent(Exception error)
        {
            foreach (var listener in _stateListener)
            {
                Task.Factory.StartNew(() => listener.HandleSessionEstablishmentError(error));
            }
        }

        /// <summary>
        /// 设置zk当前状态
        /// </summary>
        /// <param name="keeperState"></param>
        private void SetCurrentState(Event.KeeperState keeperState)
        {
            lock (_zkEventLock)
            {
                _currentState = keeperState;
            }
        }

        /// <summary>
        /// 重新连接zk
        /// </summary>
        /// <returns></returns>
        private async Task Reconnect()
        {
            if (!Monitor.TryEnter(_zkEventLock, TimeSpan.FromMilliseconds(_connectionTimeout)))
            {
                return;
            }
            try
            {
                if (_zk != null)
                {
                    await _zk.closeAsync();
                }
                _zk = Create(_zkServers, _connectionTimeout);
                Logger.Info("Reconnect");
            }
            catch (Exception ex)
            {
                FireSessionEstablishmentErrorEvent(ex);
                Logger.Error($"Reconnect exception: {ex.ToString()}");
            }
            finally
            {
                Monitor.Exit(_zkEventLock);
            }
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            _isDisposed = true;

            lock (_zkEventLock)
            {
                Task.Run(async () => await _zk.closeAsync().ConfigureAwait(false))
                    .ConfigureAwait(false).GetAwaiter().GetResult();
            }

            Logger.Info("Dispose");
        }

        #endregion

        #region 辅助

        private bool HasListeners(string path)
        {
            _dataListener.TryGetValue(path, out var dataListeners);
            if (dataListeners != null && dataListeners.Count > 0)
            {
                return true;
            }

            _childListener.TryGetValue(path, out var childListeners);
            if (childListeners != null && childListeners.Count > 0)
            {
                return true;
            }
            return false;
        }

        #endregion
    }
}
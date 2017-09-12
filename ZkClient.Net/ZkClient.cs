using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using org.apache.zookeeper;
using org.apache.zookeeper.data;
using System.Linq;
using System.Security.AccessControl;
using System.Text;

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

        private readonly Dictionary<string, HashSet<IZkChildListener>> _childListener =
            new Dictionary<string, HashSet<IZkChildListener>>();

        private readonly Dictionary<string, HashSet<IZkDataListener>> _dataListener =
            new Dictionary<string, HashSet<IZkDataListener>>();

        public ZkClient(string zkServers, int connectionTimeout, int oprationRetryTimeout)
        {
            _zkServers = zkServers;
            _connectionTimeout = connectionTimeout;
            _oprationRetryTimeout = oprationRetryTimeout;

            _zk = Create(zkServers, connectionTimeout);
        }

        private ZooKeeper Create(string connectionString, int sessionTimeout)
        {
            return new ZooKeeper(connectionString, sessionTimeout, this);
        }

        #region CRUD

        public async Task<string> Create(string path, string data, CreateMode mode)
        {
            return await RetryUntilConnected(async () => await _zk.createAsync(path, Encoding.UTF8.GetBytes(data), ZooDefs.Ids.OPEN_ACL_UNSAFE, mode));
        }

        public async Task<string> Create(string path, string data, List<ACL> acl, CreateMode mode)
        {
            return await RetryUntilConnected(async () => await _zk.createAsync(path, Encoding.UTF8.GetBytes(data), acl, mode));
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
                    await Task.Yield();
                    WaitForRetry();
                }
                catch (KeeperException.SessionExpiredException)
                {
                    await Task.Yield();
                    WaitForRetry();
                }
                if (_oprationRetryTimeout > -1 && (DateTime.Now - operationStartTime).Milliseconds > _oprationRetryTimeout)
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

        public async Task<List<string>> SubscribeChildChange(string path, IZkChildListener listener)
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
            return await WatchForChildren(path);
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

        public async Task SubscribeDataChange(string path, IZkDataListener listener)
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

            await WatchForData(path);
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
                await Reconnect();
            }

            _stateChangEvent.Set();
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
                        await ExistsAsync(path);
                        var children = await GetChildren(path);
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
                FireNewSessionEvent();
            }
            catch (Exception ex)
            {
                FireSessionEstablishmentErrorEvent(ex);
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
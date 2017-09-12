namespace ZkClient.Net
{
    public interface IZkDataListener
    {
        void HandleDataChange(string dataPath, string data);

        void HandleDataDeleted(string dataPath);
    }
}
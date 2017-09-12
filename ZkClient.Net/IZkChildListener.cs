using System.Collections.Generic;

namespace ZkClient.Net
{
    public interface IZkChildListener
    {
        void HandleChildChange(string parentPath, List<string> currentChildren);
    }
}
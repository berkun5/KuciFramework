using System.Collections.Generic;
using Kuci.Logger;

namespace Kuci.Core.DataManagement
{
    public class RemoteDataHandler
    {
        private readonly List<RemoteDataBase> _remoteDataHolder = new();

        public void AddDataClass(RemoteDataBase newData)
        {
            _remoteDataHolder.Add(newData);
        }
        
        public void LoadAllData()
        {
            DevLogger.Log($"Loading all data classes: {_remoteDataHolder.Count}");
            
            foreach (var dataClass in _remoteDataHolder)
            {
                dataClass.Load();
            }
        }
        
        public void SaveAllData()
        {
            foreach (var dataClass in _remoteDataHolder)
            {
                dataClass.Save();
            }
        }
    }
}

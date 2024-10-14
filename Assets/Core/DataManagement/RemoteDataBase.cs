using System.Collections.Generic;
using Kuci.Models;
using UnityEngine;

namespace Kuci.Core.DataManagement
{
    public abstract class RemoteDataBase
    {
        protected readonly Dictionary<string, object> Changes = new();
        protected abstract List<string> SaveFieldsKeys { get; }
        
        protected RemoteDataBase(DataModel dataModel)
        {
            dataModel.RemoteDataHandler.AddDataClass(this);
        }
        
        public void Save()
        {
            SetSave(GetChanges());
        }
        
        public void Load()
        {
            GetLoad(SaveFieldsKeys);
        }
        
        private void SetSave(IDictionary<string, object> updates)
        {
            foreach (var update in updates)
            {
                if (update.Value != null)
                {
                    PlayerPrefs.SetString(update.Key, update.Value.ToString());
                }
            }
            
            PlayerPrefs.Save();
        }
        
        private void GetLoad(List<string> keys)
        {
            var result = new Dictionary<string, string>(keys.Count);
            foreach (var key in keys)
            {
                var loadedStringData = PlayerPrefs.GetString(key);
                if (!string.IsNullOrEmpty(loadedStringData))
                {
                    result.Add(key, loadedStringData);
                }
            }

            DataLoaded(result);
        }

        private IDictionary<string, object> GetChanges()
        {
            Changes.Clear();
            PrepareChangesToSave();
            return Changes;
        }
        
        protected abstract void PrepareChangesToSave();
        
        protected abstract void DataLoaded(IDictionary<string, string> data);
    }
}
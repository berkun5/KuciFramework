using System;
using System.Collections.Generic;

namespace Kuci.Models
{
    public class ModelsHolder : IModelsHolder
    { 
        public HashSet<ModelBase> AllModels => _allModels;
        private readonly HashSet<ModelBase> _allModels = new();
        
        public T Create<T>(T model) where T : ModelBase
        {
            _allModels.Add(model);
            return model;
        }
        
        public T Get<T>() where T : ModelBase
        {
            foreach (var existingModel in _allModels)
            {
                if (existingModel.GetType() == typeof(T))
                {
                    return (T)existingModel;
                }
            }
            
            return null;
        }
        
        public void UpdateAll()
        {
            foreach (IUpdateListener updateListener in _allModels)
            {
                updateListener.Update();
            }
        }
        
        public void DisposeAll()
        {
            foreach (IDisposable model in _allModels)
            {
                model.Dispose();
            }
            
            _allModels.Clear();
        }
    }
}

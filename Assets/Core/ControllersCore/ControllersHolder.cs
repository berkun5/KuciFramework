using System;
using System.Collections.Generic;

namespace Kuci.Controllers
{
    public class ControllersHolder : IControllersHolder
    {
        private readonly HashSet<ControllerBase> _allControllers = new();
        private bool _allInitialized;
        
        public T Create<T>(T controller) where T : ControllerBase
        {
            foreach (var existingController in _allControllers)
            {
                if (existingController.GetType() == controller.GetType())
                {
                    return null;
                }
            }
            
            _allControllers.Add(controller);
            return controller;
        }
        
        public T Get<T>() where T : ControllerBase
        {
            foreach (var existingController in _allControllers)
            {
                if (existingController.GetType() == typeof(T))
                {
                    return (T)existingController;
                }
            }
            
            return null;
        }
        
        public void DisposeType<T>() where T : ControllerBase
        {
            foreach (var existingController in _allControllers)
            {
                if (existingController.GetType() == typeof(T))
                {
                    var controller = (IDisposable)existingController;
                    _allControllers.Remove(existingController);
                    controller.Dispose();
                    return;
                }
            }
            
        }
        
        public void InitAll()
        {
            foreach (var controller in _allControllers)
            {
                controller.OnInit();
            }

            _allInitialized = true;
        }

        public void DisposeAll()
        {
            foreach (IDisposable controller in _allControllers)
            {
                controller.Dispose();
            }
            
            _allControllers.Clear();
            _allInitialized = false;
        }
        
        public void UpdateAll()
        {
            if (!_allInitialized)
            {
                return;
            }
            
            foreach (IUpdateListener controller in _allControllers)
            {
                controller.Update();
            }
        }
    }
}
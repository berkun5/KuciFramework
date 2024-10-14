using System;
using System.Collections;

namespace Kuci.Models
{
    public abstract class ModelBase : IDisposable, IUpdateListener
    {
        protected bool DataLoaded;
        private bool _isInitialized;
        private Action _onLoadingCompletedCallback;
        void IDisposable.Dispose() => OnDispose();
        
        void IUpdateListener.Update()
        {
            if (!_isInitialized)
            {
                return;
            }
            
            OnUpdate();
        }

        public void LoadData(Action onLoadingCompleted = null)
        {
            _onLoadingCompletedCallback = onLoadingCompleted;
            Game.StartAppCoroutine(LoadBaseData());
        }
        
       /// <summary>
       /// all models are constructed and this model's data is loaded.
       /// </summary>
        protected virtual void OnInit()
        {
            
        }
        
        /// <summary>
        /// all models are constructed and all models` data are loaded.
        /// </summary>
        public virtual void OnDataLoaded()
        {
            DataLoaded = true;
        }

        /// <summary>
        /// Persistent Model Disposed OnDestroy, Scene Specific Models Disposed right before changing the scene(one frame)
        /// </summary>
        protected virtual void OnDispose()
        {

        }

       /// <summary>
       /// won't start until everything is loaded and OnDataLoaded fired
       /// </summary>
        protected virtual void OnUpdate()
        {
            
        }
        
        /// <summary>
        /// derived specific actions for data loading process
        /// </summary>
        protected virtual IEnumerator StartLoadData()
        {
            yield return null;
        }
        
        /// <summary>
        /// // all models are constructed and starting load this model's data
        /// </summary>
        private IEnumerator LoadBaseData()
        {
            yield return null; // generic
            yield return StartLoadData(); // derived specific
            
            OnInitializationCompleted();
        }
        
        private void OnInitializationCompleted()
        {
            OnInit();
            _isInitialized = true;
            _onLoadingCompletedCallback?.Invoke();
        }
    }
}


using System;

namespace Kuci.Controllers
{
    [Serializable]
    public abstract class ControllerBase : IDisposable, IUpdateListener
    {
        private bool _isInitialized;

        void IDisposable.Dispose() => OnDispose();

        void IUpdateListener.Update()
        {
            if (!_isInitialized)
            {
                return;
            }

            OnUpdate();
        }

        //maybe virtual OnEnable and OnDisable
        public virtual void OnInit()
        {
            _isInitialized = true;
        }

        /// <summary>
        /// Persistent Controllers Disposed OnDestroy, Scene Specific Controllers Disposed right before changing the scene(one frame)
        /// </summary>
        protected virtual void OnDispose()
        {

        }

        protected virtual void OnUpdate()
        {

        }
    }
}

using System;
using System.Collections.Generic;
using Kuci.Controllers;
using Kuci.Models;

namespace Kuci
{
    public abstract class SceneHandlerBase : IDisposable
    {
        public abstract HashSet<ModelBase> CreateSceneModels();
        public abstract HashSet<ControllerBase> CreateSceneControllers();
        public abstract void OnSceneHandlerReady();
        public void Dispose() { }
    }
}

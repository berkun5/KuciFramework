using System.Collections.Generic;
using Kuci.Controllers;
using Kuci.Logger;
using Kuci.Models;
using UnityEngine;

namespace Kuci
{
    public class GameUndefinedSceneHandler : SceneHandlerBase
    {
        private readonly IModelsHolder _persistentModelsHolder;
        private readonly UndefinedSceneReferences _undefinedSceneReferences;
        
        public GameUndefinedSceneHandler(IModelsHolder persistentModelsHolder)
        {
            _undefinedSceneReferences = Object.FindObjectOfType<UndefinedSceneReferences>();
            _persistentModelsHolder = persistentModelsHolder;
        }
        
        public override HashSet<ModelBase> CreateSceneModels()
        {
            var sceneModels = new HashSet<ModelBase>();
            //add scene specific models
            
            return sceneModels;
        }

        public override HashSet<ControllerBase> CreateSceneControllers()
        {
            var sceneControllers = new HashSet<ControllerBase>
            {
                //add  scene specific controllers
                //new UndefinedControllerExample(
                //    _persistentModelsHolder.Get<DataModel>(),
                //    _persistentModelsHolder.Get<UIModel>()),
            };
            
            return sceneControllers;
        }

        public override void OnSceneHandlerReady()
        {
            //do nothing
        }
    }
}

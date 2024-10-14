using Kuci.Models;
using Kuci.Scenes;

namespace Kuci.Controllers
{
    public class ScenesController : ControllerBase
    {
        private readonly ScenesModel _scenesModel;
        
        public ScenesController(ScenesModel scenesModel)
        {
            _scenesModel = scenesModel;
        }

        public override void OnInit()
        {
            base.OnInit();
            _scenesModel.CurrentSceneChanged += OnSceneChanged; 
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            _scenesModel.CurrentSceneChanged -= OnSceneChanged; 
        }
        
        private void OnSceneChanged(SceneId newScene, SceneId oldScene)
        {
            
        }

        private void TryChangeScene(SceneId requestedScene)
        { 
            _scenesModel.TryLoadScene(requestedScene);
        }
    }
}

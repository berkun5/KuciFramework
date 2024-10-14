using System;
using System.Collections;
using Kuci.Core.Extensions;
using Kuci.LocalData;
using Kuci.Logger;
using Kuci.Scenes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kuci.Models
{
    public class ScenesModel : ModelBase
    {
        public event Action<SceneId> ReadyToLoadSceneHandlers;
        public event Action ReadyToUnloadSceneHandlers;
        /// <summary>
        /// new scene, old scene
        /// </summary>
        public event Action<SceneId, SceneId> CurrentSceneChanged;
        public bool LoadingInProgress => _loadingInProgress;
        public SceneId CurrentActiveScene => _currentActiveScene;
        public SceneId PreviousScene => _previousScene;
        private readonly Array _allScenes = Enum.GetValues(typeof(SceneId));
        private readonly ScenesSettings _scenesSettings;
        private readonly double _minLoadSceneTime;
        private SceneId _currentActiveScene;
        private SceneId _previousScene;
        private double _previousLoadSceneTime;
        private bool _loadingInProgress;
        private Coroutine _sceneLoadingCoroutine;
        private SceneHandlerBase _activeSceneHandler;
        private bool _firstTimeLoaded;
        
        public ScenesModel(DataModel dataModel)
        {
            _scenesSettings = dataModel.LocalDataCollection.ScenesSettings;
            _minLoadSceneTime = _scenesSettings.MinSwitchSceneTime;
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            if (_sceneLoadingCoroutine != null)
            {
                Game.StopAppCoroutine(_sceneLoadingCoroutine);
                _loadingInProgress = false;
            }
        }

        public override void OnDataLoaded()
        {
            base.OnDataLoaded();
            TryLoadScene(SceneId.Persistent); // load first scene here by default
        }

        public SceneHandlerBase GetActiveSceneHandler()
        {
            return _activeSceneHandler;
        }
        
        public void SetActiveSceneHandler(SceneHandlerBase newSceneHandler)
        {
            _activeSceneHandler = newSceneHandler;
        }
        
        public void ForceLoadScene(SceneId requestedScene)
        {
            if (_sceneLoadingCoroutine != null)
            {
                Game.StopAppCoroutine(_sceneLoadingCoroutine);
                _loadingInProgress = false;
            }
            
            _previousLoadSceneTime = TimeModel.CurrentUnixTimeStamp + _minLoadSceneTime;
            _sceneLoadingCoroutine = Game.StartAppCoroutine(LoadSceneAsync(requestedScene));
        }
        
        public void TryLoadScene(SceneId requestedScene)
        {
            if (_loadingInProgress)
            {
                DevLogger.LogWarning("A Scene loading operation is already in progress. Please wait.");
                return;
            }
            
            if (TimeModel.CurrentUnixTimeStamp < _previousLoadSceneTime)
            {
                DevLogger.LogWarning("Can not load a Scene so frequently. Please wait briefly.");
                return;
            }
            
            _previousLoadSceneTime = TimeModel.CurrentUnixTimeStamp + _minLoadSceneTime;
            _sceneLoadingCoroutine = Game.StartAppCoroutine(LoadSceneAsync(requestedScene));
        }
        
        private IEnumerator LoadSceneAsync(SceneId requestedScene)
        {
            _loadingInProgress = true;
            _previousScene = _currentActiveScene;
            
            try
            {
                if (requestedScene is SceneId.Persistent or SceneId.Undefined)
                {
                    DevLogger.LogWarning($"Cannot load the ::{requestedScene.ToString()}:: Scene. Wrong request.");
                    _loadingInProgress = false;
                    yield break;
                }

                ReadyToUnloadSceneHandlers?.Invoke();
                var loadStartTime = TimeModel.CurrentTime;
                foreach (SceneId sceneId in _allScenes)
                {
                    if (sceneId is SceneId.Persistent or SceneId.Undefined)
                    {
                        continue;
                    }

                    var sceneName = _scenesSettings.GetSceneName(sceneId);
                    var isSceneCurrentlyLoaded = SceneManager.GetSceneByName(sceneName).isLoaded;

                    if (!isSceneCurrentlyLoaded)
                    {
                        continue;
                    }

                    var unloadSceneOperation = UnloadSceneAsyncOperation(sceneName);
                    while (unloadSceneOperation != null && !unloadSceneOperation.isDone)
                    {
                        yield return null;
                    }
                }

                var loadRequestedSceneOperation = LoadSceneAsyncOperation(_scenesSettings.GetSceneName(requestedScene));

                while (loadRequestedSceneOperation != null && !loadRequestedSceneOperation.isDone)
                {
                    yield return null;
                }

                var loadEndTime = TimeModel.CurrentTime - loadStartTime;
                DevLogger.Log($"Loading Scene {requestedScene} completed in {loadEndTime.ToSecondAndMillisecond()} seconds.");

                _currentActiveScene = requestedScene;
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(_scenesSettings.GetSceneName(requestedScene)));
                ReadyToLoadSceneHandlers?.Invoke(requestedScene);
                _loadingInProgress = false;
            }
            finally
            {
                var cannotLoadScene = requestedScene is SceneId.Persistent or SceneId.Undefined;
                if (!cannotLoadScene)
                {
                    CurrentSceneChanged?.Invoke(requestedScene, _previousScene);
                }
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(_scenesSettings.GetSceneName(requestedScene)));
                _loadingInProgress = false;
            }
        }

        private AsyncOperation LoadSceneAsyncOperation(string loadScene)
        {
            return SceneManager.LoadSceneAsync(loadScene, LoadSceneMode.Additive);
        }

        private AsyncOperation UnloadSceneAsyncOperation(string unloadScene)
        {
            return SceneManager.UnloadSceneAsync(unloadScene);
        }
    }
}

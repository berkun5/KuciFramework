using System;
using System.Collections;
using System.Collections.Generic;
using Kuci.Core.Extensions;
using Kuci.DataManagement;
using Kuci.Logger;
using Kuci.Models;
using Kuci.Controllers;
using Kuci.Scenes;
using Kuci.UI;
using UnityEngine;

namespace Kuci
{
    public class Game : MonoBehaviour
    {
        private static Game _game;
        [SerializeField] private LocalDataCollection _localDataCollection;
        [SerializeField] private PersistentSceneReferences _persistentSceneReferences;
        [SerializeField] private UIEntityPool _uiEntityPool;

        // persistent models
        private ModelsHolder _persistentModelsHolder;
        private ControllersHolder _persistentControllersHolder;

        // scene specific models
        private ModelsHolder _scenesModelsHolder;
        private ControllersHolder _scenesControllersHolder;
        private SceneHandlerBase _activeSceneHandler;

        private void Awake()
        {
            _game = this;
            _persistentModelsHolder = new ModelsHolder();
            _persistentControllersHolder = new ControllersHolder();
            _scenesModelsHolder = new ModelsHolder();
            _scenesControllersHolder = new ControllersHolder();

            CreatePersistentModels();
        }

        private void OnEnable()
        {
            _persistentModelsHolder.Get<ScenesModel>().ReadyToLoadSceneHandlers += OnSceneChangedLoadSceneHandler;
            _persistentModelsHolder.Get<ScenesModel>().ReadyToUnloadSceneHandlers += OnSceneChangedUnloadSceneHandlers;
        }

        private void OnDisable()
        {
            _persistentModelsHolder.Get<ScenesModel>().ReadyToLoadSceneHandlers -= OnSceneChangedLoadSceneHandler;
            _persistentModelsHolder.Get<ScenesModel>().ReadyToUnloadSceneHandlers -= OnSceneChangedUnloadSceneHandlers;
            StopAllCoroutines();
        }

        private void Start()
        {
            InitPersistentModels();
        }

        private void Update()
        {
            _persistentModelsHolder.UpdateAll();
            _persistentControllersHolder.UpdateAll();
            _scenesModelsHolder.UpdateAll();
            _scenesControllersHolder.UpdateAll();
        }

        private void OnDestroy()
        {
            _persistentModelsHolder.DisposeAll();
            _persistentControllersHolder.DisposeAll();
            _scenesModelsHolder.DisposeAll();
            _scenesControllersHolder.DisposeAll();
            StopAllCoroutines();
        }

        public static Coroutine StartAppCoroutine(IEnumerator enumerator)
        {
            return _game.StartCoroutine(enumerator);
        }

        public static void StopAppCoroutine(Coroutine coroutine)
        {
            _game.StopCoroutine(coroutine);
        }

        private void CreatePersistentModels()
        {
            _persistentModelsHolder.AllModels.Clear();

            //ui model has an exception on model creation, every other model might require ui model and it might require any other model as well,
            // so we are passing the _persistentModelsHolder and referencing any model on "OnDataLoaded", which is after every other model is ready.
            _persistentModelsHolder.Create(new UIModel(_uiEntityPool, _persistentModelsHolder, _persistentSceneReferences));

            _persistentModelsHolder.Create(new TimeModel());

            _persistentModelsHolder.Create(new DataModel(_localDataCollection));
            
            _persistentModelsHolder.Create(new ScenesModel(
                _persistentModelsHolder.Get<DataModel>()));

            _persistentModelsHolder.Create(new InputModel(
                _persistentModelsHolder.Get<DataModel>()));
        }

        private void OnSceneChangedUnloadSceneHandlers()
        {
            _scenesModelsHolder.DisposeAll();
            _scenesControllersHolder.DisposeAll();
            _activeSceneHandler?.Dispose();
        }
        
        private void OnSceneChangedLoadSceneHandler(SceneId newScene)
        {
            switch (newScene)
            {
                case SceneId.Undefined:
                    _activeSceneHandler = new GameUndefinedSceneHandler(_persistentModelsHolder);
                    break;

                default:
                    break;
            }

            if (_activeSceneHandler == null)
            {
                return;
            }
            
            var sceneModels = _activeSceneHandler.CreateSceneModels();

            foreach (var model in sceneModels)
            {
                _scenesModelsHolder.Create(model);
            }

            InitScenesModels();
            _persistentModelsHolder.Get<ScenesModel>().SetActiveSceneHandler(_activeSceneHandler);
        }

        private void InitScenesModels()
        {
            StartAppCoroutine(
                LoadAndInitializeModels(_scenesModelsHolder.AllModels, () =>
                {
                    var sceneControllers = _activeSceneHandler.CreateSceneControllers();
                    foreach (var controller in sceneControllers)
                    {
                        _scenesControllersHolder.Create(controller);
                    }

                    _scenesControllersHolder.InitAll();
                    _activeSceneHandler.OnSceneHandlerReady();
                }));
        }

        private void InitPersistentModels()
        {
            StartAppCoroutine(LoadAndInitializeModels(_persistentModelsHolder.AllModels, () =>
            {
                CreatePersistentControllers();
                _persistentControllersHolder.InitAll();
            }));
        }

        private IEnumerator LoadAndInitializeModels(HashSet<ModelBase> models, Action onComplete)
        {
            Dictionary<ModelBase, bool> modelReadyPool = new();

            foreach (var model in models)
            {
                modelReadyPool.TryAdd(model, false);
            }

            foreach (var model in models)
            {
                var startedLoading = false;

                while (modelReadyPool[model] == false)
                {
                    if (!startedLoading)
                    {
                        var derivedType = model.GetType();
                        var startTime = TimeModel.CurrentTime;
                        DevLogger.Log($"Started loading {derivedType.Name}.");

                        model.LoadData(() =>
                        {
                            var totalTime = (TimeModel.CurrentTime - startTime).ToSecondAndMillisecond();
                            DevLogger.Log(
                                $"{derivedType.Name} is loaded and ready! || Loaded in {totalTime} seconds.");
                            modelReadyPool[model] = true;
                        });
                    }

                    startedLoading = true;
                    yield return null;
                }
            }

            foreach (var model in models)
            {
                model.OnDataLoaded();
            }

            onComplete?.Invoke();
        }

        private void CreatePersistentControllers()
        {
            _persistentControllersHolder.Create(new ScenesController(
                _persistentModelsHolder.Get<ScenesModel>())
                
            //, add persistent controllers here
             );
        }
    };
}

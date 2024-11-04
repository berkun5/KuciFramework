using System;
using System.Collections.Generic;
using System.Linq;
using Kuci.Core.ReactiveProperty;
using Kuci.Logger;
using Kuci.UI;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Kuci.Models
{
    public class UIModel : ModelBase
    {
        public event Action<UIDepthLayerType, int> CanvasEntityCountChanged;
        public event Action<UIEntity> UIEntityHidden;
        public Dictionary<Canvas, UIDepthLayerConfig> CanvasConfigPair => _canvasConfigPair;
        public IReactiveProperty<bool> UIEntityWithPointersEnabled => _uiEntityWithPointersEnabled;
        private readonly Dictionary<Canvas, UIDepthLayerConfig> _canvasConfigPair = new();
        private readonly Dictionary<UIDepthLayerType, Canvas> _depthLayerCanvasPair = new();
        private readonly List<Type> _settingsUIEntities = new() 
        {
            //typeof(SettingsWindow),
        };
        private readonly UIEntityPool _uiEntityPool;
        private readonly PersistentSceneReferences _persistentSceneReferences;
        private readonly Dictionary<Type, List<UIEntity>> _activeEntities = new();
        private readonly Dictionary<Type, List<UIEntity>> _inactiveEntities = new();
        private readonly Dictionary<Type, GameObject> _gameUIByType = new();
        private readonly Dictionary<Guid, IDisposable> _uiEntityDisposeViewModels = new();
        private readonly Dictionary<Canvas, List<UIEntity>> _canvasEntityMap = new();
        private readonly IModelsHolder _persistentModelsHolder;
        private readonly ReactiveProperty<bool> _uiEntityWithPointersEnabled = new();
        private int _activeUIEntityWithPointersCount;
        
        public UIModel(UIEntityPool uiEntityPool, IModelsHolder persistentModelsHolder, 
            PersistentSceneReferences persistentSceneReferences)
        {
            _persistentModelsHolder = persistentModelsHolder;
            _persistentSceneReferences = persistentSceneReferences;
            _uiEntityPool = uiEntityPool;
        }

        protected override void OnInit()
        {
            base.OnInit();
        }

        public override void OnDataLoaded()
        {
            base.OnDataLoaded();
            var dataModel = _persistentModelsHolder.Get<DataModel>();

            TryAddCanvas(UIDepthLayerType.Unset, _persistentSceneReferences.MainCanvasReferences.GetCanvas(UIDepthLayerType.Unset),
                dataModel.LocalDataCollection.MainCanvasSettings.GetConfig(UIDepthLayerType.Unset));
            
            PrepareUIEntities();
        }

        public bool TryAddCanvas(UIDepthLayerType depthLayer, Canvas newCanvas,  UIDepthLayerConfig canvasConfig)
        {
            var success = _canvasConfigPair.TryAdd(newCanvas, canvasConfig) && 
                          _depthLayerCanvasPair.TryAdd(depthLayer, newCanvas);

            if (!success)
            {
                DevLogger.LogError("Can not add canvas.");
            }
            
            return success;
        }

        public bool TryRemoveCanvas(UIDepthLayerType depthLayer, Canvas removeCanvas)
        {
            var success = _canvasConfigPair.Remove(removeCanvas) && 
                          _depthLayerCanvasPair.Remove(depthLayer);

            if (!success)
            {
                DevLogger.LogError("Can not remove canvas.");
            }

            return success;
        }
        
        protected override void OnDispose()
        {
            base.OnDispose();
            foreach (var disposableViewModels in _uiEntityDisposeViewModels
                         .Where(disposableViewModels => disposableViewModels.Value != null))
            {
                disposableViewModels.Value.Dispose();
            }
            
            _uiEntityDisposeViewModels.Clear();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
        }
        
        public void ToggleSettingsWindows()
        {
            // check if any of the settings UI entities are active
            // we are looking for any settingsUIEntity type element in _activeEntities as value.
            // If there is, we are checking if all the instances are active and not destroyed for any other reason.
            // Usually UI Entities are not destroyed but scene specific UI might cause exceptions so we are checking it.
            var settingsActive = _settingsUIEntities.Any(settingsUIEntity => 
                _activeEntities.TryGetValue(settingsUIEntity, out var entities) && 
                entities.Any(entity => entity != null));
            
            if (settingsActive)
            {
                // hiding all type of settings window classes
                foreach (var settingsUIEntity in _settingsUIEntities)
                {
                    var method = typeof(UIModel).GetMethod("Hide")?.MakeGenericMethod(settingsUIEntity);
                    var parameters = method?.GetParameters();
                    var defaultValues = parameters?.Select(p => GetDefaultObjectParameterValue(p.ParameterType)).ToArray();
                    method?.Invoke(this, defaultValues);
                }
            }
            else
            {
                /*
                Show<SettingsWindow>(UIDepthLayerType.Layer300, window =>
                {
                    var viewModel = new SettingsWindowViewModel(this),
                        _xrOriginReference);
                    
                    window.Init(viewModel);
                    return viewModel;
                }, uniqueEntity: true);
                */
            }
        }
        
        // maybe I'll add show / hide strategy enum for special conditions such as hide all before show
        //right now only show on top flag
        public T Show<T>(UIDepthLayerType canvasDepthLayer, Func<T, IDisposable> onCreated = null,
            bool showOnTop = true,bool uniqueEntity = false) where T : UIEntity
        {
            if (uniqueEntity && IsEntityBeingShown<T>())
            {
                return null;
            }
            
            var uiEntityToShow = GetEntity<T>();
            
            if (uiEntityToShow == null)
            {
                DevLogger.LogError($"Failed to show UIEntity {typeof(T)} because Entity doesn't exist in UIPool scriptableObject.");
                return null;
            }

            var parentCanvas = _depthLayerCanvasPair[canvasDepthLayer];
            if (parentCanvas == null)
            {
                DevLogger.LogError($"Failed to show UIEntity {typeof(T)} because Target Canvas is null.");
                return null;
            }

            var uiEntityTransform = uiEntityToShow.transform;
            uiEntityTransform.SetParent(parentCanvas.transform);
            
            var siblingIndex = showOnTop ? parentCanvas.transform.childCount - 1 : 0;
            uiEntityTransform.SetSiblingIndex(siblingIndex);
            uiEntityToShow.gameObject.SetActive(false);
            
            uiEntityTransform.transform.localScale = Vector3.one;
            var uiEntityRect = uiEntityTransform as RectTransform;
            if (uiEntityRect != null)
            {
                uiEntityRect.CenterAndExpandToParent();
                uiEntityRect.localEulerAngles = Vector3.zero;
                uiEntityRect.localPosition = Vector3.zero;
            }
            
            var disposable = onCreated?.Invoke(uiEntityToShow);
            
            if (disposable != null)
            {
                _uiEntityDisposeViewModels.TryAdd(uiEntityToShow.UniqueId, disposable);
            }
            else
            {
                Debug.LogError($"ViewModel for {typeof(T)} must be IDisposable.");
            }
            
            if (uiEntityToShow.RequirePauseGame)
            {
                // handle pause game on show ui
            }
            
            if (!_canvasEntityMap.ContainsKey(parentCanvas))
            {
                _canvasEntityMap[parentCanvas] = new List<UIEntity>();
            }
            _canvasEntityMap[parentCanvas].Add(uiEntityToShow);
            CanvasEntityCountChanged?.Invoke(canvasDepthLayer, _canvasEntityMap[parentCanvas].Count);
            uiEntityToShow.gameObject.SetActive(true);
            return uiEntityToShow;
        }
        
        public void Hide<T>(Guid specificEntityOfType = default) where T : UIEntity
        {
            var entityType = typeof(T);
            if (_activeEntities.TryGetValue(entityType, out var entities))
            {
                var hasSpecificEntityOrder = specificEntityOfType != Guid.Empty;
                
                // find the first entity of the specified type or specific entity of the specified type
                var uiEntityToHide = hasSpecificEntityOrder ? 
                    entities.FirstOrDefault(e => e != null && e.UniqueId == specificEntityOfType) : 
                    entities.FirstOrDefault(e => e != null && e.GetType() == entityType);
                
                if (uiEntityToHide == null)
                {
                    // at this point the list either the list is empty, or contains only null elements so it's safe to clear
                    _activeEntities[entityType].Clear();
                    DevLogger.LogWarning($"Failed to hide UIEntity, because it's not being shown.");
                    return;
                }
                
                foreach (var canvasMap in _canvasEntityMap)
                {
                    if (!canvasMap.Value.Contains(uiEntityToHide))
                    {
                        continue;
                    }
                    
                    canvasMap.Value.Remove(uiEntityToHide);

                    foreach (var pair in _depthLayerCanvasPair)
                    {
                        if (pair.Value == canvasMap.Key)
                        {
                            CanvasEntityCountChanged?.Invoke(pair.Key, _canvasEntityMap[pair.Value].Count);
                            break;
                        }
                    }
                    break;
                }
                
                uiEntityToHide.gameObject.SetActive(false);
                _activeEntities[entityType].Remove(uiEntityToHide);
                
                if (_activeEntities[entityType].Count == 0)
                {
                    _activeEntities.Remove(entityType);
                }

                if (!_inactiveEntities.ContainsKey(entityType))
                {
                    _inactiveEntities.Add(entityType, new List<UIEntity>());
                }
                    
                _inactiveEntities[entityType].Add(uiEntityToHide);

                if (_uiEntityDisposeViewModels.Remove(uiEntityToHide.UniqueId, out var disposable))
                {
                    disposable.Dispose();
                }
                
                if (uiEntityToHide.RequirePauseGame)
                {
                    // handle unpause game on show ui
                }
                
                UIEntityHidden?.Invoke(uiEntityToHide);
            }
        }
        
        public int ActiveWindowCountOnCanvas(UIDepthLayerType depthLayer)
        {
            if (!_depthLayerCanvasPair.TryGetValue(depthLayer, out var targetCanvas))
            {
                Debug.LogError($"Canvas for depth layer {depthLayer} not found.");
                return 0;
            }

            if (_canvasEntityMap.TryGetValue(targetCanvas, out var entities))
            {
                return entities.Count(entity => entity != null && entity.gameObject.activeSelf);
            }

            return 0;
        }
        
        public bool IsEntityBeingShown<T>() where T : UIEntity
        {
            var entityType = typeof(T);
            
            if (_activeEntities.TryGetValue(entityType, out var entities))
            {
                return entities.Count(entity => entity != null && entity.gameObject.activeSelf) > 0;
            }

            return false;
        }
        
        private T GetEntity<T>() where T : UIEntity
        {
            var entityType = typeof(T);
            UIEntity inactiveEntity;

            if (_inactiveEntities.ContainsKey(entityType) && _inactiveEntities[entityType].Count > 0)
            {
                inactiveEntity = _inactiveEntities[entityType][0];

                // in case if it's destroyed for any reason, eg. scene change
                if (inactiveEntity == null)
                {
                    _inactiveEntities[entityType].RemoveAt(0);
                    inactiveEntity = InstantiateUIEntity(_gameUIByType[entityType]);
                }
                else
                {
                    _inactiveEntities[entityType].RemoveAt(0);
                }
            }
            else
            {
                inactiveEntity = InstantiateUIEntity(_gameUIByType[entityType]);
            }

            if (!_activeEntities.ContainsKey(entityType))
            {
                _activeEntities.Add(entityType, new List<UIEntity>());
            }

            _activeEntities[entityType].Add(inactiveEntity);
            inactiveEntity.gameObject.SetActive(false);

            return (inactiveEntity) as T;
        }
        
        private void PrepareUIEntities()
        {
            foreach (var uiPrefab in _uiEntityPool.Pool)
            {
                var type = uiPrefab.GetComponent<UIEntity>().GetType();
                
                if (!_gameUIByType.ContainsKey(type))
                {
                    _gameUIByType.Add(type, uiPrefab.gameObject);

                    if (!uiPrefab.PrepareOnInit)
                    {
                        continue;
                    }
                    
                    var warmedEntity = InstantiateUIEntity(uiPrefab.gameObject);
                    var warmedEntityTransform = warmedEntity.transform;
                    
                    warmedEntityTransform.SetParent(_persistentSceneReferences.MainCanvasReferences.transform);
                    warmedEntityTransform.localPosition = Vector3.zero;
                    warmedEntityTransform.gameObject.SetActive(false);
                        
                    var entityType = warmedEntity.GetType();
                    if (!_inactiveEntities.ContainsKey(entityType))
                    {
                        _inactiveEntities.Add(entityType, new List<UIEntity>());
                    }

                    _inactiveEntities[entityType].Add(warmedEntity);
                }
                else
                {
                    Debug.LogWarning($"Duplicated UIEntity in sceneUIPool of type {type}.");
                }
            }
        }
        
        private UIEntity InstantiateUIEntity(GameObject ui)
        {
            var entity = Object.Instantiate(ui);
            return entity.GetComponent<UIEntity>();
        }
        
        private object GetDefaultObjectParameterValue(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }
}

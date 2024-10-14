using System.Collections.Generic;
using Kuci.Logger;
using Kuci.UI;
using UnityEngine;

namespace Kuci.LocalData
{
    [CreateAssetMenu(fileName = "MainCanvasSettings", menuName = "ScriptableObjects/Settings/MainCanvasSettings", order = 99)]
    public class MainCanvasSettings : ScriptableObject, ISerializationCallbackReceiver
    {
        [Header("Sub Canvas Depth")]
        [SerializeField] private List<UIDepthLayerConfig> _depthLayerConfigs = new();
        private readonly Dictionary<UIDepthLayerType, UIDepthLayerConfig> _layerTypeConfigPair = new();
        
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            // do nothing
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            _layerTypeConfigPair.Clear();
            foreach (var config in _depthLayerConfigs)
            {
                _layerTypeConfigPair.TryAdd(config.DepthLayer, config);
            }
        }
        
        public UIDepthLayerConfig GetConfig(UIDepthLayerType requiredLayer)
        {
            if (_layerTypeConfigPair.TryGetValue(requiredLayer, out var requiredConfig))
            {
                return requiredConfig;
            }

            DevLogger.LogError("Required UI layer config doesn't exist in the settings list.");
            return null;
        }
    }
}
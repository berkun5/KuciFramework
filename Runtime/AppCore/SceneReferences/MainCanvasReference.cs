using System;
using System.Collections.Generic;
using Kuci.Logger;
using Kuci.UI;
using UnityEngine;

namespace Kuci.Controllers
{
    public class MainCanvasReference : MonoBehaviour, ISerializationCallbackReceiver, IDisposable
    {
        [SerializeField] private List<CanvasLayerPair> _canvasLayers = new();
        private readonly Dictionary<UIDepthLayerType, Canvas> _canvasLayerPair = new();
        
        void IDisposable.Dispose()
        {
            _canvasLayerPair.Clear();
        }
        
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            // do nothing
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            _canvasLayerPair.Clear();
            
            foreach (var pair in _canvasLayers)
            {
                _canvasLayerPair.TryAdd(pair.DepthLayer, pair.Canvas);
            }
        }

        public Canvas GetCanvas(UIDepthLayerType layerType)
        {
            if(_canvasLayerPair.TryGetValue(layerType, out var canvas))
            {
                return canvas;
            }
            
            DevLogger.LogError($"Canvas for {layerType} doesn't exist.");
            return null;
        } 
        
    }
}

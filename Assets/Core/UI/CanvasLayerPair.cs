using Kuci.UI;
using UnityEngine;

namespace Kuci.Controllers
{
    [System.Serializable]
    public class CanvasLayerPair
    {
        public UIDepthLayerType DepthLayer => _depthLayer;
        public Canvas Canvas => _canvas;
        
        [SerializeField] private UIDepthLayerType _depthLayer;
        [SerializeField] private Canvas _canvas;
    }
}

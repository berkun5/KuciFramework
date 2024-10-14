using UnityEngine;

namespace Kuci.UI
{
    [System.Serializable]
    public class UIDepthLayerConfig
    {
        public UIDepthLayerType DepthLayer => _depthLayer;
        public float LookAtCameraSpeed => _lookAtCameraSpeed;
        
        [SerializeField] private UIDepthLayerType _depthLayer;
        [Tooltip("Speed of the rotation of the layer around itself relative to the camera.")]
        [SerializeField] private float _lookAtCameraSpeed = 6f;
    }
}

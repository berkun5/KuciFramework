using Kuci.Controllers;

namespace Kuci
{
    using UnityEngine;

    public class PersistentSceneReferences : MonoBehaviour
    {
        public MainCanvasReference MainCanvasReferences => _mainCanvasReferences;
        [SerializeField] private MainCanvasReference _mainCanvasReferences;
    }
}
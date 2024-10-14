using Kuci.LocalData;
using UnityEngine;
using UnityEngine.Serialization;

namespace Kuci.DataManagement
{
    //root for all local data.
    [CreateAssetMenu(fileName = "LocalDataCollection", menuName = "ScriptableObjects/LocalDataCollection")]
    public class LocalDataCollection : ScriptableObject
    { 
        public InputsSettings InputSettings => _inputsSettings;
        public ScenesSettings ScenesSettings => _scenesSettings;
        public MainCanvasSettings MainCanvasSettings => _mainCanvasSettings;
        
        [SerializeField] private InputsSettings _inputsSettings;
        [SerializeField] private ScenesSettings _scenesSettings;
        [SerializeField] private MainCanvasSettings _mainCanvasSettings;
    }
}
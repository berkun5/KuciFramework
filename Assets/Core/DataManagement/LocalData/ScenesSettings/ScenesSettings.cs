using System.Collections.Generic;
using Kuci.Logger;
using Kuci.Scenes;
using UnityEngine;

namespace Kuci.LocalData
{
    [CreateAssetMenu(fileName = "ScenesSettings", menuName = "ScriptableObjects/Settings/Scenes", order = 99)]
    public class ScenesSettings : ScriptableObject, ISerializationCallbackReceiver
    {
        public float MinSwitchSceneTime => _minSwitchSceneTime;
        [SerializeField] private float _minSwitchSceneTime = 1;
        [SerializeField, Space(10)] private List<SceneIdPair> _scenes = new();
        private readonly Dictionary<SceneId, string> _allSceneIdPairs = new();

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            _allSceneIdPairs.Clear();
            
            foreach (var pair in _scenes)
            {
                _allSceneIdPairs.TryAdd(pair.SceneId, pair.SceneName);
            }
        }
        
        
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            //do nothing
        }
        
        public string GetSceneName(SceneId id)
        {
            if (_allSceneIdPairs.TryGetValue(id, out var sceneName))
            {
                return sceneName;
            } 
            
            DevLogger.LogError($"Scene name does not exists in the local settings. :: {id.ToString()}");
            return "unknownScene";
        }
    }
}
using Kuci.Scenes;
using UnityEngine;

[System.Serializable]
public class SceneIdPair
{
    public SceneId SceneId => _sceneId;
    public string SceneName => _sceneName;
    
    [SerializeField] private SceneId _sceneId;
    [SerializeField] private string _sceneName;
}

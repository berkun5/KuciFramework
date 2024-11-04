using System;
using Kuci.RemoteData;
using UnityEngine;
using UnityEngine.Serialization;

namespace Kuci.UI
{
    public class UIEntity : MonoBehaviour
    {
        public Guid UniqueId => _uniqueId;
        private readonly Guid _uniqueId = Guid.NewGuid();
        public bool PrepareOnInit => _prepareOnInit;
        public bool RequirePauseGame => _requirePauseGame;
        
        [SerializeField] private bool _prepareOnInit;
        [SerializeField] private bool _requirePauseGame;
        
        private void Awake()
        {
            var rectTrans = transform.GetComponent<RectTransform>();
            if (rectTrans != null)
            {
                rectTrans.anchorMin = 
                    rectTrans.anchorMax = 
                        rectTrans.pivot = new Vector2(0.5f, 0.5f);

                rectTrans.position = Vector3.zero;
            }
        }
    }
}
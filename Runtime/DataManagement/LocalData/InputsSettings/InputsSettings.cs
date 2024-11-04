using System;
using System.Collections.Generic;
using System.Linq;
using Kuci.Logger;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Kuci.LocalData
{
    [CreateAssetMenu(fileName = "InputSettings", menuName = "ScriptableObjects/Settings/Input", order = 99)]
    public class InputsSettings : ScriptableObject, ISerializationCallbackReceiver
    {
        public float MinInputHoldThreshold => _minInputHoldThreshold;
        public InputActionAsset InputActionAsset => _inputActionAsset;
        public List<UserInput> Inputs => _inputs;
        
        [SerializeField] private float _minInputHoldThreshold = 0.05f;
        [SerializeField] private InputActionAsset _inputActionAsset; 
        [SerializeField, Space(15)] private List<UserInput> _inputs = new();

        private readonly Dictionary<InputActionFunction, InputActionReference> _inputTypeActionReferenceMap = new();
        
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            // do nothing
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            _inputTypeActionReferenceMap.Clear();
            
            foreach (var inputPair in _inputs)
            {
                _inputTypeActionReferenceMap.Add(inputPair.Function, inputPair.InputActionReference);
            }
        }

        public InputActionFunction GetInputButtonType(Guid inputId)
        {
            foreach (var userInput in _inputs.Where(userInput => userInput.InputActionReference.action.id == inputId))
            {
                return userInput.Function;
            }
            
            DevLogger.LogError($"Given input does not exist in the input settings..");
            return InputActionFunction.Unknown;
        }
        
        public InputActionReference GetInputActionReference(InputActionFunction actionFunction)
        {
            if (_inputTypeActionReferenceMap.TryGetValue(actionFunction, out var actionReference))
            {
                return actionReference;
            }
            
            DevLogger.LogError($"Given InputActionFunction does not exist in the input settings..");
            return null;
        }
    }
}
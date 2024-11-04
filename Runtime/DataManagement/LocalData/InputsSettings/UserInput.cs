using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Kuci.LocalData
{
    [Serializable]
    public class UserInput
    {
        public InputActionReference InputActionReference => _inputActionReference;
        public InputActionFunction Function => _function;

        [SerializeField] private InputActionReference _inputActionReference;
        [SerializeField] private InputActionFunction _function;
    }
}
using System;
using System.Collections.Generic;
using Kuci.Core.Extensions;
using Kuci.LocalData;
using Kuci.Logger;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Kuci.Models
{
    public class InputModel : ModelBase
    {
        public HashSet<InputActionFunction> BlockedInputs => _blockedInputs;
        public event Action<HashSet<InputActionFunction>> BlockedValuesChanged;
        
        public event Action<InputActionFunction> AnyInputHoldStarted;
        public event Action<InputActionFunction> AnyInputHoldingContinuously;
        public event Action<InputActionFunction> AnyInputHoldEnded;

        // add all inputs actions
        public event Action ConfirmActionPressed;
        public event Action<Vector2> ConfirmActionPerformed;

        private readonly InputsSettings _inputsSettings;
        private readonly float _inputHoldThreshold;
        private readonly HashSet<InputActionFunction> _blockedInputs = new();
        private readonly Dictionary<InputActionFunction, Action<InputAction.CallbackContext>> _performActions = new();
        private readonly Dictionary<InputActionFunction, double> _holdingActions = new();
        private readonly List<InputActionFunction> _startedHoldingActions = new();
        private readonly DataModel _dataModel;
        
        public InputModel(DataModel dataModel)
        {
            _dataModel = dataModel;
            _inputsSettings = dataModel.LocalDataCollection.InputSettings;
            _inputHoldThreshold = _inputsSettings.MinInputHoldThreshold;
            
            SetPerformActions();
        }

        protected override void OnInit()
        {
            base.OnInit();
            if (_inputsSettings.InputActionAsset == null)
            {
                DevLogger.LogError("You need to create an InputActionAsset and assign to Input settings under LocalDataCollection.");
            }
            
            _inputsSettings.InputActionAsset.Enable();

            foreach (var userInput in _inputsSettings.Inputs)
            {
                userInput.InputActionReference.action.performed += _performActions[userInput.Function];
                userInput.InputActionReference.action.started += OnAnyInputStarted;
                userInput.InputActionReference.action.canceled += OnAnyInputCancelled;
            }
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            _inputsSettings.InputActionAsset.Disable();
            
            foreach (var userInput in _inputsSettings.Inputs)
            {
                userInput.InputActionReference.action.performed -= _performActions[userInput.Function];
                userInput.InputActionReference.action.started -= OnAnyInputStarted;
                userInput.InputActionReference.action.canceled -= OnAnyInputCancelled;
            }
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            foreach (var holdingInputPair in _holdingActions)
            {
                if (TimeModel.CurrentUnixTimeStamp >= holdingInputPair.Value)
                {
                    if (_startedHoldingActions.Remove(holdingInputPair.Key))
                    {
                        AnyInputHoldStarted?.Invoke(holdingInputPair.Key);
                    }
                    
                    AnyInputHoldingContinuously?.Invoke(holdingInputPair.Key);
                }
            }
        }
        
        public void BlockInputReadings(HashSet<InputActionFunction> blockingInputs)
        {
            var valuesChanged = false;
            foreach (var inputType in blockingInputs)
            {
                _blockedInputs.Add(inputType);
                valuesChanged = true;
            }

            if (valuesChanged)
            {
                BlockedValuesChanged?.Invoke(_blockedInputs);
            }
        }
        
        public void AllowInputReadings(HashSet<InputActionFunction> allowingInputs)
        {
            var valuesChanged = false;
            foreach (var inputType in allowingInputs)
            {
                _blockedInputs.Remove(inputType);
                valuesChanged = true;
            }
            
            if (valuesChanged)
            {
                BlockedValuesChanged?.Invoke(_blockedInputs);
            }
        }

        public void BlockAllInputReadings() => BlockInputReadings(EnumExtensions.ToHashSet<InputActionFunction>());
        
        public void AllowAllInputReadings() => AllowInputReadings(EnumExtensions.ToHashSet<InputActionFunction>());
        
        private void OnAnyInputStarted(InputAction.CallbackContext context)
        {
            var actionButton = _inputsSettings.GetInputButtonType(context.action.id);
            if (_blockedInputs.Contains(actionButton))
            {
                return;
            }
            
            var holdTriggerTime = TimeModel.CurrentUnixTimeStamp + _inputHoldThreshold;
            
            if (!_holdingActions.TryAdd(actionButton, holdTriggerTime))
            {
                _holdingActions[actionButton] = holdTriggerTime;
            }

            if (!_startedHoldingActions.Contains(actionButton))
            {
                _startedHoldingActions.Add(actionButton);
            }
        }

        private void OnAnyInputCancelled(InputAction.CallbackContext context)
        {
            var actionButton = _inputsSettings.GetInputButtonType(context.action.id);
            if (_blockedInputs.Contains(actionButton))
            {
                return;
            }
            
            if (_holdingActions.TryGetValue(actionButton, out var holdStartTime))
            {
                if (TimeModel.CurrentUnixTimeStamp >= holdStartTime)
                { 
                    AnyInputHoldEnded?.Invoke(actionButton);
                }
                
                _holdingActions.Remove(actionButton);
                _startedHoldingActions.Remove(actionButton);
            }
        }
        
        private void SetPerformActions()
        {
            // example actions
            TryAddPerformAction(InputActionFunction.ConfirmAction, OnConfirmActionPressed);
            TryAddPerformAction(InputActionFunction.ConfirmAction, OnRightThumbstickPerformed);
        }
        
        private void TryAddPerformAction(InputActionFunction function, Action<InputAction.CallbackContext> action)
        {
            _performActions.TryAdd(function, context =>
            {
                if (_blockedInputs.Contains(function))
                {
                    return;
                }
                action(context);
            });
        }
        
        private void OnConfirmActionPressed(InputAction.CallbackContext context) 
            => ConfirmActionPressed?.Invoke();
        
        private void OnRightThumbstickPerformed(InputAction.CallbackContext context) 
            => ConfirmActionPerformed?.Invoke(context.ReadValue<Vector2>());
    }
}

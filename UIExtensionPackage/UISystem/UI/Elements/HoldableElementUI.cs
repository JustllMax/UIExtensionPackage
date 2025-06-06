using System.Collections;
using NaughtyAttributes;
using UIExtensionPackage.ExtendedUI.Enums;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UIExtensionPackage.UISystem.UI.Elements
{

    /// <summary>
    /// Class represents base for holdable UI elements.
    /// </summary>
    public abstract class HoldableElementUI : SelectableUIElement<HoldableElementUI>
    {

        #region VARIABLES
        
        [FormerlySerializedAs("OnHoldStarted")]
        [Header("Hold Events")] [Space]
        
        [Foldout("Events")] [SerializeField] public UnityEvent onHoldStarted;
        [Foldout("Events")] [SerializeField] public UnityEvent<float> onHoldProgressedChanged;
        [Foldout("Events")] [SerializeField] public UnityEvent<float> onHoldReleased;
        [Foldout("Events")] [SerializeField] public UnityEvent onFullyLoaded;

        [Foldout("Config")] [SerializeField] [Tooltip("Amount of uses.\n-1 for infinite.")]
        private int _amountOfUse = -1;

        [Foldout("Config")] [SerializeField] [Tooltip("Scales with Time.unscaledDeltaTime if false")]
        private bool _useScaledDeltaTime;

        [Foldout("Config")] [SerializeField] [Tooltip("Should release of the object when progress is fully reached.")]
        private bool _releaseOnFullHold;

        
        [Foldout("Config")] [SerializeField]
        [Tooltip("Should fire OnHoldRelease events when not held and progress was not fully loaded.")]
        private bool _fireOnEarlyRelease = true;
        
        [Foldout("Config")] [SerializeField, ShowIf(nameof(ShouldShowSaveHoldTime))] 
        [Tooltip("Progress won't be reset back to 0 on early release.")]
        private bool _saveHoldTimeOnEarlyRelease = true;
        
        [Foldout("Config")] [SerializeField, ShowIf(nameof(_saveHoldTimeOnEarlyRelease))] 
        [Tooltip("Progress will now slowly revert back to 0 on early release.")]
        private bool _slowlyRegressOnEarlyRelease = true;
        
        [Header("Regress config")]
        [Foldout("Config")] [SerializeField]
        [Tooltip("When firing, should the progress be set instantly " +
                 "or slowly progress back to start position.")]
        private bool _resetProgressOnRelease;
        
        [Foldout("Config")] [SerializeField, Min(0.01f), ShowIf(nameof(ShouldShowEarlyFireReset))]
        private bool _resetProgressOnEarlyFire;

       
        [Foldout("Config")]
        [SerializeField, ShowIf(nameof(ShouldShowFullyFireReset))]
        [Tooltip("When fully loaded and released, should the progress be set instantly " +
                 "or slowly regress back to start position.")]
        private bool _resetProgressOnFullyLoaded = true;
        
        [Foldout("Config")]
        [SerializeField, Min(0.01f), ShowIf(nameof(ShouldShowRegressModifier))]
        [Tooltip("How much to speed up regress when not held. 1 is normal.\nExamples: 1.4, 0.51, 1.01")]
        private float _earlyReleaseRegressSpeedModifier = 1f;
        
        [Foldout("Config")]
        [SerializeField, Min(0.01f), ShowIf(nameof(ShouldShowFullRegressModifier))]
        [Tooltip("How much to speed up regress when fired OnRelease events. 1 is normal.\nExamples: 1.4, 0.51, 1.01")]
        private float _onFireRegressSpeedModifier = 2f;
        
        [Header("Holding Config")]
        [Foldout("Config")] [SerializeField]
        [Tooltip("Starting value for hold amount. In Seconds.")]
        private float _startHoldTime;
        
        [Foldout("Config")] [SerializeField] 
        [Tooltip("How long to hold for. In Seconds.")]
        private float _maxHoldTime = 1;
        
        [Foldout("Config")] [SerializeField, Min(0.01f)]
        [Tooltip("How much to speed up progress. 1 is normal.\nExamples: 1.4, 0.51, 1.01")]
        private float _progressSpeedModifier = 1f;
        
        [Foldout("Debug")] [SerializeField, ReadOnly]
        private bool _isHolding;

        [Foldout("Debug")] [SerializeField, ReadOnly]
        private float _holdDuration;

        [Foldout("Debug")] [SerializeField, ReadOnly]
        private bool _hasStartHoldInvokedFlag;

        [Foldout("Debug")] [SerializeField, ReadOnly]
        private bool _hasFullyLoadInvokedFlag;

        [Foldout("Debug")] [SerializeField, ReadOnly]
        private bool _hasFired;

        [Foldout("Debug")] [SerializeField, ReadOnly]
        private bool _hasReleasedFullyLoaded;

        #endregion

        #region ATTRIBUTES

        public bool IsHolding => _isHolding;
        private bool IsFullyLoaded => GetNormalizedProgress() >= 0.9875f;
        public float CurrentHoldDuration => _holdDuration;

        private float ModifiedProgressSpeed => _useScaledDeltaTime
            ? Time.deltaTime * _progressSpeedModifier
            : Time.unscaledDeltaTime * _progressSpeedModifier;

        private float RegressSpeed => _useScaledDeltaTime
            ? Time.deltaTime * _earlyReleaseRegressSpeedModifier
            : Time.unscaledDeltaTime * _earlyReleaseRegressSpeedModifier;

        private float FullRegressSpeed => _useScaledDeltaTime
            ? Time.deltaTime * _onFireRegressSpeedModifier
            : Time.unscaledDeltaTime * _onFireRegressSpeedModifier;

        public int AmountOfUse
        {
            get => _amountOfUse;
            private set => _amountOfUse = value;
        }

        public bool IsInfinite => _amountOfUse == -1;
        public bool HasUses => _amountOfUse > 0 || IsInfinite;

        #endregion
        

        protected virtual void Update()
        {
            if (!IsActive) return;
            if (!HasUses && !IsInteractionDisabled)
            {
                SetCanBeInteractedWith(false);
            }

            CheckIsHolding();
            CheckIsRegressing();
        }

        public override void HandleOnHoverExit()
        {
            base.HandleOnHoverExit();
            if (!CanBeInteractedWith || !IsActive) return;
            OnStopHolding();

        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (!CanBeInteractedWith || !IsActive) return;
            _isHolding = true;

        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            if (!CanBeInteractedWith || !IsActive) return;
            OnStopHolding();
        } 


        /// <summary>
        /// Method handles possible fire of events on release and resetting progress
        /// </summary>
        private void ReleaseHold()
        {
            //When released and fully loaded, fire event
            if (IsFullyLoaded)
            {
                OnFullHoldFire();
                return;
            }
            // Or when not fully loaded, check for early firing event
            if (_fireOnEarlyRelease)
            {
                OnEarlyHoldFire();
                return;
            }

            //Do not save progress => reset hold duration
            if (!_saveHoldTimeOnEarlyRelease)
                ResetHoldDuration();
        }

        /// <summary>
        /// Method implements logic for release of the element when fully progressed
        /// </summary>
        private void OnFullHoldFire()
        {
            //Set Flags
            _hasFired = true;
            _hasReleasedFullyLoaded = true;
            //Invoke Events
            InvokeOnHoldReleased();

            //Decrease amount of use
            if (!IsInfinite)
                _amountOfUse--;

            //Reset timers
            if (_resetProgressOnFullyLoaded)
            {
                ResetHoldDuration();
                ResetFlags();
                return;
            }

            //NOTE: Used in regressing hold
            // Set interaction state
            SetCanBeInteractedWith(false);
            // Set flag
            _isHolding = false;
        }
        
        /// <summary>
        /// Method implements logic for release of the element
        /// when not fully regressed and <see cref="_fireOnEarlyRelease"/> is set to true
        /// </summary>
        private void OnEarlyHoldFire()
        {
            //Set Flags
            _hasFired = true;
            //Invoke Events
            InvokeOnHoldReleased();

            //Decrease amount of use
            if (!IsInfinite)
                _amountOfUse--;

            //Set interaction state - used in regressing hold
            SetCanBeInteractedWith(false);
        }

        /// <summary>
        /// Method checks for possible regression
        /// </summary>
        private void CheckIsRegressing()
        {
            //If object is not being held, but it previously was, regress the hold duration   
            if (!_isHolding && _holdDuration > _startHoldTime)
            {
                RegressHold();
            }
        }

        /// <summary>
        /// Method checks is object is being held.
        /// </summary>
        private void CheckIsHolding()
        {
            //Check can the object be held
            if (!CanBeInteractedWith) return;

            if (InteractionState == InteractionState.Pressed)
            {
                ProgressHold();
            }

        }

        /// <summary>
        /// Method called when object is being held to progress hold duration.
        /// </summary>
        private void ProgressHold()
        {
            // Check for fire flag
            if (_hasFired) return;

            // Check flag to make sure that method inside is called once
            if (!_hasStartHoldInvokedFlag)
            {
                //Invoke Events
                InvokeOnHoldStarted();
            }

            //Update HoldDuration;
            UpdateHoldDuration();
        }

        /// <summary>
        /// Method that is being called when object is not held, to regress progress on holding.
        /// </summary>
        private void RegressHold()
        {
            // Check is hold duration above startHoldTime aka is being held
            if (_holdDuration > _startHoldTime)
            {
                // Check for fire flag
                if (_hasFired)
                {
                    if (CheckForInstantFireReset())
                    {
                        ResetFlags();
                        return;
                    }
                }

                // Check for slow regress option
                if (!_slowlyRegressOnEarlyRelease && !_hasFired)
                {
                    //Stop the progress in place, do not regress
                    if (_saveHoldTimeOnEarlyRelease) return;
                    
                    //Instantly regress progress to 0
                    if(_resetProgressOnRelease)
                    {
                        ResetFlags();
                        return;
                    }
                }
                
                
                //Slowly regress progress to 0

                //Check for which regress modifier to use
                if (_hasReleasedFullyLoaded)
                    _holdDuration -= FullRegressSpeed;
                else
                    _holdDuration -= RegressSpeed;

                // When fully regressed, reset object state
                if (_holdDuration <= _startHoldTime)
                {
                    _holdDuration = _startHoldTime;
                    // Check for uses left
                    if (HasUses)
                    {
                        ResetFlags();
                    }
                }
                InvokeOnHoldProgressedChanged();
            }
        }

        private bool CheckForInstantFireReset()
        {
            //Reset on early fire release
            if (_resetProgressOnEarlyFire && !_hasFullyLoadInvokedFlag) 
            {
                ResetHoldDuration();
                return true;
            }

            //Reset on full fire release
            if (_hasReleasedFullyLoaded && _resetProgressOnFullyLoaded && _hasFullyLoadInvokedFlag)
            {
                ResetHoldDuration();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Method for updating current hold progress.
        /// </summary>
        private void UpdateHoldDuration()
        {
            // Increase hold progress
            if (_holdDuration < _maxHoldTime)
            {
                _holdDuration += ModifiedProgressSpeed;
                InvokeOnHoldProgressedChanged();
            }

            // When fully loaded, set flags and stop increasing
            if (IsFullyLoaded)
            {
                _holdDuration = _maxHoldTime;
                if (!_hasFullyLoadInvokedFlag)
                {
                    InvokeOnFullyLoaded();
                }
                
                if (_releaseOnFullHold) ReleaseHold();
            }
        }

        /// <summary>
        /// Method used for stopping hold state of the object
        /// </summary>
        private void OnStopHolding() 
        {
            // If was held, release
            if (_isHolding) ReleaseHold();
            _isHolding = false;
        }

        protected override void HandleDisable()
        {
            if (_isHolding) OnStopHolding();
            base.HandleDisable();
        }

        /// <summary>
        /// Resets all flags, object is set to default state
        /// </summary>
        private void ResetFlags()
        {
            _hasFired = false;
            _hasReleasedFullyLoaded = false;
            _isHolding = false;
            SetStartFlag(false);
            SetFullyLoadedFlag(false);
            SetCanBeInteractedWith(true);
        }

        /// <summary>
        /// Method handles changing amount of usages
        /// </summary>
        public void ChangeUseAmount(int value)
        {
            if(!IsActive) return;
            AmountOfUse += value;
            if(AmountOfUse == 0 || AmountOfUse < -1) ZeroAmountOfUse();
            else StartCoroutine(WaitForHoldRegress());
        }
        
        /// <summary>
        /// Method handles setting amount of usages
        /// </summary>
        public void SetUseAmount(int value)
        {
            if(!IsActive) return;
            if (value == 0 || value < -1)
            {
                ZeroAmountOfUse();
                return;
            }
            AmountOfUse = value;
            StartCoroutine(WaitForHoldRegress());
        }
        
        /// <summary>
        /// Method sets the amount of use to 0 and disables interaction
        /// </summary>
        public void ZeroAmountOfUse()
        {
            if(!IsActive) return;
            AmountOfUse = 0;
            SetCanBeInteractedWith(false);
        }
        
        /// <summary>
        /// Coroutine makes sure to not reset flags when hold duration is regressing
        /// </summary>
        IEnumerator WaitForHoldRegress()
        {
            while (_holdDuration > _startHoldTime)
            {
                yield return new WaitForSeconds(0.1f);
            }
            ResetFlags();
        }

        #region UTILITY

        private void SetStartFlag(bool value) => _hasStartHoldInvokedFlag = value;
        private void SetFullyLoadedFlag(bool value) => _hasFullyLoadInvokedFlag = value;

        /// <summary>
        /// Resets timer for hold duration
        /// </summary>
        private void ResetHoldDuration()
        {
            _holdDuration = _startHoldTime;
            _hasFired = false;
            _hasStartHoldInvokedFlag = false;
            InvokeOnHoldProgressedChanged();
        }

        public float GetNormalizedProgress()
        {
            
            return ((_holdDuration - _startHoldTime) / (_maxHoldTime - _startHoldTime));
        }

        /// <summary>
        /// Method for invoking OnHoldStart event and setting flags
        /// </summary>
        private void InvokeOnHoldStarted()
        {
            SetStartFlag(true);
            onHoldStarted?.Invoke();
        }

        /// <summary>
        /// Method for invoking OnHoldProgress event with normalized progress value
        /// </summary>
        private void InvokeOnHoldProgressedChanged()
        {
            onHoldProgressedChanged?.Invoke(GetNormalizedProgress());
        }

        /// <summary>
        /// Method for invoking OnHoldRelease event and setting flag
        /// </summary>
        private void InvokeOnHoldReleased()
        {
            SetStartFlag(false);
            onHoldReleased?.Invoke(GetNormalizedProgress());
        }

        /// <summary>
        /// Method for invoking OnFullyLoaded event and setting flag
        /// </summary>
        private void InvokeOnFullyLoaded()
        {
            SetFullyLoadedFlag(true);
            onFullyLoaded?.Invoke();
        }

        #endregion

        protected override void OnDestroy()
        {
            base.OnDestroy();
            StopAllCoroutines();
        }

        #region INSPECTOR_UTILITIES

        private bool ShouldShowEarlyFireReset() => _fireOnEarlyRelease && !_resetProgressOnRelease;
        private bool ShouldShowFullyFireReset() => !_resetProgressOnRelease;
        private bool ShouldShowRegressModifier() => !_resetProgressOnEarlyFire || _slowlyRegressOnEarlyRelease; 
        private bool ShouldShowFullRegressModifier() => !_resetProgressOnFullyLoaded;  
        private bool ShouldShowSaveHoldTime() => !_fireOnEarlyRelease;
        protected override bool ShouldShowUnselectOnPointerUp() => false;
        protected override bool ShouldShowSelectEvents() => false;
        #endregion

        protected override void OnValidate()
        {
            base.OnValidate();

            if (_startHoldTime >= _maxHoldTime)
            {
                Debug.LogError($"Starting Hold Time {_startHoldTime} has to be smaller than Hold Time {_maxHoldTime}!" +
                               $"\nStarting Hold Time {_startHoldTime} set to {_startHoldTime -= 1f} ");
                _startHoldTime -= 1f;

            }

            // Fail safe 
            if (_resetProgressOnRelease)
            {
                _resetProgressOnFullyLoaded = true;
                _resetProgressOnEarlyFire = true;
            }
            // Fail safe 
            if (_fireOnEarlyRelease) _saveHoldTimeOnEarlyRelease = false;
            // Fail safe 
            if(!_fireOnEarlyRelease) _resetProgressOnEarlyFire = true; 
            // Fail safe 
            if (!_saveHoldTimeOnEarlyRelease) _slowlyRegressOnEarlyRelease = false;


        }
    }
}


using System.Collections;
using NaughtyAttributes;
using UIExtensionPackage.ExtendedUI.Enums;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UIExtensionPackage.UISystem.UI.Elements
{

    /// <summary>
    /// Class represents base for holdable UI elements.
    /// </summary>
    public abstract class HoldableElementUI : SelectableUIElement<HoldableElementUI>
    {

        #region VARIABLES
        

        [Header("Hold Events")] [Space]
        
        [Foldout("Events")] [SerializeField] public UnityEvent OnHoldStarted;
        [Foldout("Events")] [SerializeField] public UnityEvent<float> OnHoldProgressedChanged;
        [Foldout("Events")] [SerializeField] public UnityEvent<float> OnHoldReleased;
        [Foldout("Events")] [SerializeField] public UnityEvent OnFullyLoaded;

        [Foldout("Config")] [SerializeField] [Tooltip("Amount of uses.\n-1 for infinite.")]
        private int amountOfUse = -1;

        [Foldout("Config")] [SerializeField] [Tooltip("Scales with Time.unscaledDeltaTime if false")]
        private bool useScaledDeltaTime = false;

        [Foldout("Config")] [SerializeField] [Tooltip("Should release of the object when progress is fully reached.")]
        private bool releaseOnFullHold = false;


        [Foldout("Config")] [SerializeField]
        [Tooltip("Should fire OnHoldRelease events when not held and progress was not fully loaded.")]
        private bool fireOnEarlyRelease = true;

        [Foldout("Config")] [SerializeField, ShowIf(nameof(ShouldShowSaveHoldTime))] 
        [Tooltip("Progress won't be reset back to 0 on early release.")]
        private bool saveHoldTimeOnEarlyRelease = true;

        [Foldout("Config")] [SerializeField, ShowIf(nameof(saveHoldTimeOnEarlyRelease))] 
        [Tooltip("Progress will now slowly revert back to 0 on early release.")]
        private bool slowlyRegressOnEarlyRelease = true;

        [Header("Regress config")]
        [Foldout("Config")] [SerializeField]
        [Tooltip("When firing, should the progress be set instantly " +
                 "or slowly progress back to start position.")]
        private bool resetProgressOnRelease = false;
        
        [Foldout("Config")] [SerializeField, Min(0.01f), ShowIf(nameof(ShouldShowEarlyFireReset))]
        private bool resetProgressOnEarlyFire = false;

        [Foldout("Config")]
        [SerializeField, ShowIf(nameof(ShouldShowFullyFireReset))]
        [Tooltip("When fully loaded and released, should the progress be set instantly " +
                 "or slowly regress back to start position.")]
        private bool resetProgressOnFullyLoaded = true;

        [Foldout("Config")]
        [SerializeField, Min(0.01f), ShowIf(nameof(ShouldShowRegressModifier))]
        [Tooltip("How much to speed up regress when not held. 1 is normal.\nExamples: 1.4, 0.51, 1.01")]
        private float earlyReleaseRegressSpeedModifier = 1f;

        [Foldout("Config")]
        [SerializeField, Min(0.01f), ShowIf(nameof(ShouldShowFullRegressModifier))]
        [Tooltip("How much to speed up regress when fired OnRelease events. 1 is normal.\nExamples: 1.4, 0.51, 1.01")]
        private float onFireRegressSpeedModifier = 2f;

        [Header("Holding Config")]
        [Foldout("Config")] [SerializeField]
        [Tooltip("Starting value for hold amount. In Seconds.")]
        private float startHoldTime = 0f;

        [Foldout("Config")] [SerializeField] 
        [Tooltip("How long to hold for. In Seconds.")]
        private float maxHoldTime = 1;

        [Foldout("Config")] [SerializeField, Min(0.01f)]
        [Tooltip("How much to speed up progress. 1 is normal.\nExamples: 1.4, 0.51, 1.01")]
        private float progressSpeedModifier = 1f;
        

        [Foldout("Debug")] [SerializeField, ReadOnly]
        private bool isHolding;

        [Foldout("Debug")] [SerializeField, ReadOnly]
        private float _holdDuration = 0;

        [Foldout("Debug")] [SerializeField, ReadOnly]
        private bool _hasStartHoldInvokedFlag = false;

        [Foldout("Debug")] [SerializeField, ReadOnly]
        private bool _hasFullyLoadInvokedFlag = false;

        [Foldout("Debug")] [SerializeField, ReadOnly]
        private bool _hasFired = false;

        [Foldout("Debug")] [SerializeField, ReadOnly]
        private bool _hasReleasedFullyLoaded = false;

        #endregion

        #region ATTRIBUTES

        public bool IsHolding => isHolding;
        private bool IsFullyLoaded => GetNormalizedProgress() >= 0.9875f;
        public float CurrentHoldDuration => _holdDuration;

        private float ModifiedProgressSpeed => useScaledDeltaTime
            ? Time.deltaTime * progressSpeedModifier
            : Time.unscaledDeltaTime * progressSpeedModifier;

        private float RegressSpeed => useScaledDeltaTime
            ? Time.deltaTime * earlyReleaseRegressSpeedModifier
            : Time.unscaledDeltaTime * earlyReleaseRegressSpeedModifier;

        private float FullRegressSpeed => useScaledDeltaTime
            ? Time.deltaTime * onFireRegressSpeedModifier
            : Time.unscaledDeltaTime * onFireRegressSpeedModifier;

        public int AmountOfUse
        {
            get => amountOfUse;
            private set => amountOfUse = value;
        }

        public bool IsInfinite => amountOfUse == -1;
        public bool HasUses => amountOfUse > 0 || IsInfinite;

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
            isHolding = true;

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
            if (fireOnEarlyRelease)
            {
                OnEarlyHoldFire();
                return;
            }

            //Do not save progress => reset hold duration
            if (!saveHoldTimeOnEarlyRelease)
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
                amountOfUse--;

            //Reset timers
            if (resetProgressOnFullyLoaded)
            {
                ResetHoldDuration();
                ResetFlags();
                return;
            }

            //NOTE: Used in regressing hold
            // Set interaction state
            SetCanBeInteractedWith(false);
            // Set flag
            isHolding = false;
        }
        
        /// <summary>
        /// Method implements logic for release of the element
        /// when not fully regressed and <see cref="fireOnEarlyRelease"/> is set to true
        /// </summary>
        private void OnEarlyHoldFire()
        {
            //Set Flags
            _hasFired = true;
            //Invoke Events
            InvokeOnHoldReleased();

            //Decrease amount of use
            if (!IsInfinite)
                amountOfUse--;

            //Set interaction state - used in regressing hold
            SetCanBeInteractedWith(false);
        }

        /// <summary>
        /// Method checks for possible regression
        /// </summary>
        private void CheckIsRegressing()
        {
            //If object is not being held, but it previously was, regress the hold duration   
            if (!isHolding && _holdDuration > startHoldTime)
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
            if (_holdDuration > startHoldTime)
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
                if (!slowlyRegressOnEarlyRelease && !_hasFired)
                {
                    //Stop the progress in place, do not regress
                    if (saveHoldTimeOnEarlyRelease) return;
                    
                    //Instantly regress progress to 0
                    if(resetProgressOnRelease)
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
                if (_holdDuration <= startHoldTime)
                {
                    _holdDuration = startHoldTime;
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
            if (resetProgressOnEarlyFire && !_hasFullyLoadInvokedFlag) 
            {
                ResetHoldDuration();
                return true;
            }

            //Reset on full fire release
            if (_hasReleasedFullyLoaded && resetProgressOnFullyLoaded && _hasFullyLoadInvokedFlag)
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
            if (_holdDuration < maxHoldTime)
            {
                _holdDuration += ModifiedProgressSpeed;
                InvokeOnHoldProgressedChanged();
            }

            // When fully loaded, set flags and stop increasing
            if (IsFullyLoaded)
            {
                _holdDuration = maxHoldTime;
                if (!_hasFullyLoadInvokedFlag)
                {
                    InvokeOnFullyLoaded();
                }
                
                if (releaseOnFullHold) ReleaseHold();
            }
        }

        /// <summary>
        /// Method used for stopping hold state of the object
        /// </summary>
        private void OnStopHolding() 
        {
            // If was held, release
            if (isHolding) ReleaseHold();
            isHolding = false;
        }

        protected override void HandleDisable()
        {
            if (isHolding) OnStopHolding();
            base.HandleDisable();
        }

        /// <summary>
        /// Resets all flags, object is set to default state
        /// </summary>
        private void ResetFlags()
        {
            _hasFired = false;
            _hasReleasedFullyLoaded = false;
            isHolding = false;
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
            while (_holdDuration > startHoldTime)
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
            _holdDuration = startHoldTime;
            _hasFired = false;
            _hasStartHoldInvokedFlag = false;
            InvokeOnHoldProgressedChanged();
        }

        public float GetNormalizedProgress()
        {
            
            return ((_holdDuration - startHoldTime) / (maxHoldTime - startHoldTime));
        }

        /// <summary>
        /// Method for invoking OnHoldStart event and setting flags
        /// </summary>
        private void InvokeOnHoldStarted()
        {
            SetStartFlag(true);
            OnHoldStarted?.Invoke();
        }

        /// <summary>
        /// Method for invoking OnHoldProgress event with normalized progress value
        /// </summary>
        private void InvokeOnHoldProgressedChanged()
        {
            OnHoldProgressedChanged?.Invoke(GetNormalizedProgress());
        }

        /// <summary>
        /// Method for invoking OnHoldRelease event and setting flag
        /// </summary>
        private void InvokeOnHoldReleased()
        {
            SetStartFlag(false);
            OnHoldReleased?.Invoke(GetNormalizedProgress());
        }

        /// <summary>
        /// Method for invoking OnFullyLoaded event and setting flag
        /// </summary>
        private void InvokeOnFullyLoaded()
        {
            SetFullyLoadedFlag(true);
            OnFullyLoaded?.Invoke();
        }

        #endregion

        protected override void OnDestroy()
        {
            base.OnDestroy();
            StopAllCoroutines();
        }

        #region INSPECTOR_UTILITIES

        private bool ShouldShowEarlyFireReset() => fireOnEarlyRelease && !resetProgressOnRelease;
        private bool ShouldShowFullyFireReset() => !resetProgressOnRelease;
        private bool ShouldShowRegressModifier() => !resetProgressOnEarlyFire || slowlyRegressOnEarlyRelease; 
        private bool ShouldShowFullRegressModifier() => !resetProgressOnFullyLoaded;  
        private bool ShouldShowSaveHoldTime() => !fireOnEarlyRelease;
        protected override bool ShouldShowUnselectOnPointerUp() => false;
        protected override bool ShouldShowSelectEvents() => false;
        #endregion

        protected override void OnValidate()
        {
            base.OnValidate();

            if (startHoldTime >= maxHoldTime)
            {
                Debug.LogError($"Starting Hold Time {startHoldTime} has to be smaller than Hold Time {maxHoldTime}!" +
                               $"\nStarting Hold Time {startHoldTime} set to {startHoldTime -= 1f} ");
                startHoldTime -= 1f;

            }

            // Fail safe 
            if (resetProgressOnRelease)
            {
                resetProgressOnFullyLoaded = true;
                resetProgressOnEarlyFire = true;
            }
            // Fail safe 
            if (fireOnEarlyRelease) saveHoldTimeOnEarlyRelease = false;
            // Fail safe 
            if(!fireOnEarlyRelease) resetProgressOnEarlyFire = true; 
            // Fail safe 
            if (!saveHoldTimeOnEarlyRelease) slowlyRegressOnEarlyRelease = false;


        }
    }
}


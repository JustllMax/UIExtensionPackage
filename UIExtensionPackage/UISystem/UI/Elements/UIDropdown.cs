using System.Collections.Generic;
using TMPro;
using UIExtensionPackage.UISystem.Core.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace UIExtensionPackage.UISystem.UI.Elements
{
    /// <summary>
    /// Represents a dropdown UI element.
    /// </summary>
    [RequireComponent(typeof(TMP_Dropdown))]
    public abstract class UIDropdown : UIElement, IWithSetup
    {
        /// <summary>
        /// Wrapper for Unity dropdown component
        /// </summary>
        protected TMP_Dropdown UnityDropdown { get; private set; }

        /// <summary>
        /// Event that happens when the value of the dropdown is changed.
        /// </summary>
        public UnityEvent<int> OnValueChanged;
        /// <summary>
        /// Value of this dropdown (index of selected item)
        /// </summary>
        public int Value => UnityDropdown.value;

        private void Awake()
        {
            UnityDropdown = GetComponent<TMP_Dropdown>();
        }

        public virtual void SetUp()
        {
            UnityDropdown?.onValueChanged.AddListener((int value) =>OnValueChanged?.Invoke(value));
        }

        public virtual void TearDown()
        {
            UnityDropdown?.onValueChanged.RemoveAllListeners();
        }

        /// <summary>
        /// Adds an option to the Unity dropdown component.
        /// </summary>
        public void AddOption(string optionName) => UnityDropdown?.AddOptions(new List<string>() {optionName});


        /// <summary>
        /// Removes an option from the Unity dropdown component.
        /// </summary>
        public void RemoveOption(string optionName)
        {
            UnityDropdown?.options.RemoveAll(option => option.text == optionName);
            UnityDropdown?.RefreshShownValue();
        }

        /// <summary>
        /// Clears all options from the Unity dropdown component.
        /// </summary>
        public void ClearOptions() => UnityDropdown?.ClearOptions();

        /// <summary>
        /// Clears all options from the Unity dropdown component and sets new options.
        /// </summary>
        public void SetOptions(List<TMP_Dropdown.OptionData> options)
        {
            UnityDropdown?.ClearOptions();
            UnityDropdown?.AddOptions(options);
        }

        /// <summary>
        /// Clears all options from the Unity dropdown component and sets new options.
        /// </summary>
        public void SetOptions(List<string> options)
        {
            UnityDropdown?.ClearOptions();
            UnityDropdown?.AddOptions(options);
        }

        /// <summary>
        /// Sets the value of the Unity dropdown component.
        /// </summary>
        public void SetValue(int value) => UnityDropdown.value = value;


        //protected abstract void OnValueChanged(int value);
    }
}


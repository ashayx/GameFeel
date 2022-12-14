using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MoreMountains.Tools
{	
    /// <summary>
    /// A very simple class, meant to be used within a MMSceneLoading screen, to update a Text
    /// based on loading progress
    /// </summary>
    public class MMSceneLoadingTMPTextProgress : MonoBehaviour
    {
        /// the value to which the progress' zero value should be remapped to
        [Tooltip("the value to which the progress' zero value should be remapped to")]
        public float RemapMin = 0f;
        /// the value to which the progress' one value should be remapped to
        [Tooltip("the value to which the progress' one value should be remapped to")]
        public float RemapMax = 100f;
        /// the amount of decimals to display
        [Tooltip("the amount of decimals to display")]
        public int NumberOfDecimals = 0;

        protected TMP_Text _text;

        /// <summary>
        /// On Awake we grab our Text and store it
        /// </summary>
        protected virtual void Awake()
        {
            _text = this.gameObject.GetComponent<TMP_Text>();
        }
        
        /// <summary>
        /// Updates the Text with the progress value
        /// </summary>
        /// <param name="newValue"></param>
        public virtual void SetProgress(float newValue)
        {
            float remappedValue = MMMaths.Remap(newValue, 0f, 1f, RemapMin, RemapMax);
            float displayValue = MMMaths.RoundToDecimal(remappedValue, NumberOfDecimals);
            _text.text = displayValue.ToString(CultureInfo.InvariantCulture);
        }
    }
}

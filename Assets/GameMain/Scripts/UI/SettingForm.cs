using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine.Rendering.PostProcessing;
using System;
namespace Game
{
    public class SettingForm : UGuiForm
    {
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void _setDPR(float float1);
#endif


        public SwitchToggle m_FPSSwitch;
        public SwitchToggle m_ColorGradingSwitch;
        public SwitchToggle m_VignetteSwitch;
        public Slider m_Slider_Render;
        //public TextMeshProUGUI m_DPI;
        // Start is called before the first frame update
        public PostProcessVolume postProcessVolume;
        //public TMP_Dropdown dropdown;

        private AutoQualityManager autoQualityManager;
        private int volumeIndex = 0;
        private PostProcessEffectSettings ColorGradingItem;
        private PostProcessEffectSettings VignetteItem;
        void Start()
        {
            //m_FPSSwitch.SwitchOn.AddListener(OnHighFps);
            //m_FPSSwitch.SwitchOff.AddListener(OnLowFps);
            autoQualityManager = FindObjectOfType<AutoQualityManager>();

            ////渲染缩放
            m_Slider_Render.onValueChanged.AddListener(OnRenderChange);

        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            if (postProcessVolume == null)
            {
                postProcessVolume = FindObjectOfType<PostProcessVolume>();
            }

            m_FPSSwitch.IsOn = Application.targetFrameRate != 30;
            if (postProcessVolume != null)
            {
                foreach (var item in postProcessVolume.profile.settings)
                {

                    //dropdown.options.Add(new TMP_Dropdown.OptionData(item.name.Replace("(Clone)", "")));
                    if (item.GetType() == typeof(ColorGrading))
                    {
                        ColorGradingItem = item;

                    }

                    if (item.GetType() == typeof(Vignette))
                    {
                        VignetteItem = item;

                    }
                }
                m_ColorGradingSwitch.IsOn = ColorGradingItem.active;
                m_VignetteSwitch.IsOn = VignetteItem.active;
            }

        }

        //private void OnSelect(int index)
        //{
        //    volumeIndex = index;
        //    var item = postProcessVolume.profile.settings[index];
        //    m_Enable.isOn = item.active;
        //}

        //private void OnSetVolume(bool isOn)
        //{
        //    var item = postProcessVolume.profile.settings[volumeIndex];
        //    item.active = isOn;
        //}

        public void OpenColorGrading()
        {
            ColorGradingItem.active = true;
        }

        public void CloseColorGrading()
        {
            ColorGradingItem.active = false;
        }

        public void OpenVignette()
        {
            VignetteItem.active = true;
        }

        public void CloseVignette()
        {
            VignetteItem.active = false;
        }

        public void __setDPR(float float1)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
        _setDPR(float1);
#endif
        }

        void Update()
        {
            //m_DPI.text = autoQualityManager.dpi.ToString();
            //m_Slider_Render.value = autoQualityManager.dpi;

        }

        public void OnLowFps()
        {

            Application.targetFrameRate = 30;
        }

        public void OnHighFps()
        {

            Application.targetFrameRate = 60;
        }

        private void OnRenderChange(float scale)
        {
            autoQualityManager.dpi = scale;
            __setDPR(scale);
        }
    }
}
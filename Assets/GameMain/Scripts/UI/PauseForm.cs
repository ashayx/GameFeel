using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public partial class PauseForm : UGuiForm
    {
        protected override void OnInit(object userData)

        {
            base.OnInit(userData);

            GetBindComponents(gameObject);

            m_Button_Settings.onClick.AddListener(OnSetting);
            m_Button_Continue.onClick.AddListener(OnContinue);
            m_Button_Restart.onClick.AddListener(OnRestart);
            m_Button_GiveUp.onClick.AddListener(OnGiveUp);
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);


        }

        private void OnContinue()
        { 
            CorgiEngineEvent.Trigger(CorgiEngineEventTypes.UnPause);
            Close(false);
        }

        private void OnRestart()
        {
            Close(false);
            LevelManager.Instance.GotoLevel(AssetUtility.GetSceneAsset(SceneManager.GetActiveScene().name), true, true);
        }

        private void OnGiveUp()
        {
            Close(false);
            MMGameEvent.Trigger("Save");
            GameEntry.Event.Fire(null, ChangeSceneEventArgs.Create(GameEntry.Config.GetInt(Constant.GameData.HomeScene)));
        }

        private void OnSetting()
        {
            GameEntry.UI.OpenUIForm(EnumUIForm.SettingForm);
        }

        protected override void OnClose(bool isShutdown, object userData)

        {
            base.OnClose(isShutdown, userData);

        }
    }
}
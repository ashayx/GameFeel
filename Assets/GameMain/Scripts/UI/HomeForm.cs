using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game
{
    public class HomeForm : UGuiForm
    {
        public void StartGame()
        {
            m_ProcedureMenu.StartGame(Constant.GameData.GameScene);
            Close();
        }

        public void OpenStage()
        {
            GameEntry.UI.OpenUIForm(EnumUIForm.StageForm);
        }

        public void OpenSetting()
        {
            GameEntry.UI.OpenUIForm(EnumUIForm.SettingForm);
        }

        private ProcedureMenu m_ProcedureMenu = null;
        protected override void OnInit(object userData)

        {
            base.OnInit(userData);

            //GetBindComponents(gameObject);

            //InitHero();
            //InitEquip(GameData.instance.Equip);

        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            m_ProcedureMenu = (ProcedureMenu)userData;
            //GameEntry.Event.Subscribe(WebRequestSuccessEventArgs.EventId, OnWebRequestSuccess);

            //m_TxtMesh_Money.text = GameData.instance.Money.ToString();

            //m_Btn_SoundOn.onClick.AddListener(OnSoundOn);
            //m_Btn_SoundOff.onClick.AddListener(OnSoundoff);
            //m_Btn_Start.onClick.AddListener(OnStarGame);
            //m_Btn_Shop.onClick.AddListener(OnShopClick);
            //m_Btn_Card.onClick.AddListener(OnCard);
            //m_Btn_Setting.onClick.AddListener(OnSetting);

            //var boo = GameEntry.Sound.IsMuted("Music");
            //m_Btn_SoundOff.gameObject.SetActive(boo);
            //m_Btn_SoundOn.gameObject.SetActive(!boo);

            //m_Btn_Start2.onClick.AddListener(OnStarGame2);
            //GameEntry.Event.Subscribe(SelectEquipEventArgs.EventId, OnSelectEquip);
            //GameEntry.BuiltinData.PlayerUICamera.gameObject.SetActive(true);

        }

        protected override void OnClose(bool isShutdown, object userData)

        {
            base.OnClose(isShutdown, userData);
            //GameEntry.Event.Unsubscribe(WebRequestSuccessEventArgs.EventId, OnWebRequestSuccess);
            //m_Btn_SoundOn.onClick.RemoveListener(OnSoundOn);
            //m_Btn_SoundOff.onClick.RemoveListener(OnSoundoff);
            //m_Btn_Start.onClick.RemoveListener(OnStarGame);
            //m_Btn_Shop.onClick.RemoveListener(OnShopClick);
            //m_Btn_Card.onClick.RemoveListener(OnCard);
            //m_Btn_Setting.onClick.RemoveListener(OnSetting);

            //m_Btn_Start2.onClick.RemoveListener(OnStarGame2);
            //GameEntry.Event.Unsubscribe(SelectEquipEventArgs.EventId, OnSelectEquip);
            //GameEntry.BuiltinData.PlayerUICamera.gameObject.SetActive(false);
        }


        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)

        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

        }

    }
}
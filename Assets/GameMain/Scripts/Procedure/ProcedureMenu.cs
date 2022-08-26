//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameFramework.Event;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace Game
{
    public class ProcedureMenu : ProcedureBase
    {
        private bool m_StartGame = false;
        private string m_SceneName;

        public override bool UseNativeDialog
        {
            get
            {
                return false;
            }
        }

        public void StartGame(string sceneName)
        {
            m_StartGame = true;
            m_SceneName = sceneName;
        }

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            GameEntry.Event.Subscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);

            //GameData.instance.Startup();

            m_StartGame = false;
            //GameEntry.Sound.PlayMusic(EnumMusic.bgm);
            GameEntry.UI.OpenUIForm(EnumUIForm.HomeForm, this);
            //GameEntry.Entity.ShowEntity<EntityPlayer>(EnumEntity.EntityPlayer, (entity) =>
            //{

            //    //player = (EntityPlayer)entity.Logic;

            //}, PlayerData.Create(UnityEngine.Vector3Int.zero, true));


        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);

            GameEntry.Event.Unsubscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);

            //if (m_MenuForm != null)
            //{
            //    m_MenuForm.Close(isShutdown);
            //    m_MenuForm = null;
            //}
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (m_StartGame)
            {
                procedureOwner.SetData<VarInt32>("NextSceneId", GameEntry.Config.GetInt(m_SceneName));
                ChangeState<ProcedureChangeScene>(procedureOwner);
            }
        }

        private void OnOpenUIFormSuccess(object sender, GameEventArgs e)
        {
            OpenUIFormSuccessEventArgs ne = (OpenUIFormSuccessEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            //m_MenuForm = (MenuForm)ne.UIForm.Logic;
        }
    }
}

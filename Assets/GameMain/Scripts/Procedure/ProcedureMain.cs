//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System.Collections.Generic;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
using UnityEngine;
using GameFramework.Event;
using System;
using UnityTimer;
using MoreMountains.Tools;
using MoreMountains.CorgiEngine;

namespace Game
{
    public class ProcedureMain : ProcedureBase, MMEventListener<CorgiEngineEvent>
    {
        private bool m_ChangeScene = false;
        private ProcedureOwner m_ProcedureOwner;
        public bool Paused { get; set; }

        //private SettlementSystem settlementSystem;

        protected override void OnInit(ProcedureOwner procedureOwner)
        {
            base.OnInit(procedureOwner);
        }

        protected override void OnDestroy(ProcedureOwner procedureOwner)
        {
            base.OnDestroy(procedureOwner);
        }

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            m_ProcedureOwner = procedureOwner;
            m_ChangeScene = false;
            Paused = false;
            this.MMEventStartListening<CorgiEngineEvent>();
            GameEntry.Event.Subscribe(ChangeSceneEventArgs.EventId, OnChangeScene);


            //GameEntry.Sound.PlayMusic(EnumMusic.fight);

        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
            this.MMEventStartListening<CorgiEngineEvent>();
            this.MMEventStopListening<CorgiEngineEvent>();
            GameEntry.Event.Unsubscribe(ChangeSceneEventArgs.EventId, OnChangeScene);
            //GameEntry.Event.Unsubscribe(UseCardEventArgs.EventId, OnCardUse);
            ////GameEntry.Event.Unsubscribe(CardEndEventArgs.EventId, OnCardSettlement);
            //GameEntry.Event.Unsubscribe(RoundStateChangeEventArgs.EventId, OnRoundChange);
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (m_ChangeScene)
            {
                ChangeState<ProcedureChangeScene>(procedureOwner);
            }

            //if (m_Game && !m_Game.IsGameOver)
            //    m_Game.OnUpdate(elapseSeconds, realElapseSeconds);

        }

        public virtual void OnMMEvent(CorgiEngineEvent engineEvent)
        {
            switch (engineEvent.EventType)
            {
                //case CorgiEngineEventTypes.TogglePause:
                //    if (Paused)
                //    {
                //        UnPause();
                //    }
                //    else
                //    {
                //        Pause();
                //    }
                //    break;

                case CorgiEngineEventTypes.Pause:
                    Pause();
                    break;

                case CorgiEngineEventTypes.UnPause:
                    UnPause();
                    break;
            }
        }

        private void UnPause()
        {
            Paused = false;
            GameEntry.UI.CloseUIForm(EnumUIForm.PauseForm);
        }

        private void Pause()
        {
            Paused = true;
            GameEntry.UI.OpenUIForm(EnumUIForm.PauseForm);

        }

        private void OnChangeScene(object sender, GameEventArgs e)
        {
            ChangeSceneEventArgs ne = (ChangeSceneEventArgs)e;
            if (ne == null)
                return;

            m_ChangeScene = true;
            m_ProcedureOwner.SetData<VarInt32>("NextSceneId", ne.SceneId);
            CorgiEngineEvent.Trigger(CorgiEngineEventTypes.LevelEnd);
            CorgiEngineEvent.Trigger(CorgiEngineEventTypes.UnPause);
            CorgiEngineEvent.Trigger(CorgiEngineEventTypes.LoadNextScene);
        }

    }
}

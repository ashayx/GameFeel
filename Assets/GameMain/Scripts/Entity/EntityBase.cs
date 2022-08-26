using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameFramework;
using UnityEngine;
using UnityGameFramework.Runtime;
namespace Game
{
    public class EntityBase : EntityLogic, IPause
    {
        protected bool m_Pause = false;
        [SerializeField]
        protected EntityData m_EntityData = null;

        public int Id
        {
            get
            {
                return Entity.Id;
            }
        }

        public EnumEntity TypeId
        {
            get
            {
                return m_EntityData.TypeId;
            }
        }

        public Vector3Int Position
        {
            get
            {
                return m_EntityData.Position;
            }
            set
            {
                m_EntityData.Position = value;
            }
        }

        public int Money
        {
            get
            {
                return m_EntityData.Money;
            }
            set
            {
                m_EntityData.Money = value;
            }
        }

        public bool IsRun;

        public virtual void Pause()
        {
            m_Pause = true;
        }

        public virtual void Resume()
        {
            m_Pause = false;
        }

        protected override void OnShow(object userData)

        {
            base.OnShow(userData);

            m_EntityData = userData as EntityData;
            if (m_EntityData == null)
            {
                Log.Error("Entity data is invalid.");
                return;
            }

            //Name = Utility.Text.Format("[Entity {0}]", Id.ToString());
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
        }

        protected override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);
            if (m_EntityData != null)
            {
                ReferencePool.Release(m_EntityData);
            }
        }

        

    }
}
//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;
using UnityEngine;
using GameFramework;

namespace Game
{
    [Serializable]
    public class EntityData : IReference
    {
        [SerializeField]
        private EnumEntity m_TypeId = 0;

        [SerializeField]
        private Vector3Int m_Position = Vector3Int.zero;

        public int Money = 0;
        public int DieExp = 0;

        /// <summary>
        /// 实体类型编号。
        /// </summary>
        public EnumEntity TypeId
        {
            get
            {
                return m_TypeId;
            }
            set
            {
                m_TypeId = value;
            }
        }

        /// <summary>
        /// 实体坐标
        /// </summary>
        public Vector3Int Position
        {
            get
            {
                return m_Position;
            }
            set
            {
                m_Position = value;
            }
        }

        public object UserData
        {
            get;
            protected set;
        }

        public static EntityData Create(EnumEntity typeId, Vector3Int positon)
        {
            EntityData data = ReferencePool.Acquire<EntityData>();
            data.TypeId = typeId;
            //data.Id = GameEntry.Entity.GenerateSerialId();
            data.Position = positon;

            DREntity dREntity = GameEntry.DataTable.GetDataTable<DREntity>().GetDataRow((int)data.TypeId);
            data.Money = dREntity.Money;
            data.DieExp = dREntity.Exp;
            return data;
        }

        public virtual void Clear()
        {
            m_Position = Vector3Int.zero;
            UserData = null;
        }
    }
}

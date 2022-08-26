//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;
using GameFramework;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Game
{
    [Serializable]
    public class EffectData : EntityData
    {
        [SerializeField]
        private float m_KeepTime = 0f;

        public float KeepTime
        {
            get
            {
                return m_KeepTime;
            }
        }

        public static EffectData Create(Vector3Int positon, EnumEntity typeId)
        {
            EffectData data = ReferencePool.Acquire<EffectData>();
            data.TypeId = typeId;
            //data.Id = GameEntry.Entity.GenerateSerialId();
            data.m_KeepTime = 3f;
            data.Position = positon;
            return data;
        }



        public override void Clear()
        {
            base.Clear();
            m_KeepTime = 0;
            Log.Info("隐藏特效");
        }
    }
}

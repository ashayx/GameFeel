
using GameFramework;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Game
{
    /// <summary>
    /// 特效类。
    /// </summary>
    public class EntityEffect : EntityBase
    {
        [SerializeField]
        private EffectData m_UserData = null;

        private float m_ElapseSeconds = 0f;


        protected override void OnShow(object userData)

        {
            base.OnShow(userData);
            //transform.position = HexMapManager.instance.ConvertToWorldPositon(Position);

            m_UserData = userData as EffectData;
            if (m_UserData == null)
            {
                Log.Error("Effect data is invalid.");
                return;
            }
            m_ElapseSeconds = 0f;
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)

        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

            m_ElapseSeconds += elapseSeconds;
            if (m_ElapseSeconds >= m_UserData.KeepTime)
            {
                GameEntry.Entity.HideEntity(this);
            }
        }

        protected override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);
    
        }
    }
}

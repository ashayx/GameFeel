using GameFramework.ObjectPool;
using GameFramework;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Game
{
    public class PoolObject : ObjectBase
    {
        public static PoolObject Create(MonoBehaviour target)
        {
            PoolObject ob = ReferencePool.Acquire<PoolObject>();
            ob.Initialize(target);
            return ob;
        }

        public static PoolObject Create(GameObject target)
        {
            PoolObject ob = ReferencePool.Acquire<PoolObject>();
            ob.Initialize(target);
            return ob;
        }

        protected override void Release(bool isShutdown)
        {
            if (Target.GetType() == typeof(GameObject))
            {
                Object.Destroy(Target as GameObject);
            }
            else
            {
                Object.Destroy((Target as MonoBehaviour).gameObject);
            }
        }

        //protected override void OnSpawn()
        //{
        //}

        //protected override void OnUnspawn()
        //{

        //}

        public override void Clear()
        {
            base.Clear();
        }
    }
}


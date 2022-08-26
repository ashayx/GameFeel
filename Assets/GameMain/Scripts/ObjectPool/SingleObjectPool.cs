
using UnityGameFramework.Runtime;
using GameFramework.ObjectPool;
using UnityEngine;

namespace Game
{
    public class SingleObjectPool : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_CardTemplate = null;

        [SerializeField]
        private Transform m_InstanceRoot = null;

        [SerializeField]
        private int m_InstancePoolCapacity = 10;

        [SerializeField]
        private string m_PoolName = "Pool";

        private IObjectPool<PoolObject> m_ObjectPool = null;


        public Transform Root
        {
            get
            {
                return m_InstanceRoot;
            }
        }
        private void Start()
        {
            if (m_InstanceRoot == null)
            {
                m_InstanceRoot = transform;
            }
            m_ObjectPool = GameEntry.ObjectPool.CreateSingleSpawnObjectPool<PoolObject>(m_PoolName, m_InstancePoolCapacity);
        }

        public T Spawn<T>() where T : MonoBehaviour
        {
            T poolable = null;
            PoolObject poolObject = m_ObjectPool.Spawn();
            if (poolObject != null)
            {
                poolable = (T)poolObject.Target;
            }
            else
            {
                var go = Instantiate(m_CardTemplate, m_InstanceRoot);
                poolable = go.GetComponent<T>();
                if (poolable == null)
                {
                    Log.Error("GetComponent error {0}", typeof(T));
                    return null;
                }
                m_ObjectPool.Register(PoolObject.Create(poolable), true);
            }
            return poolable;
        }

        /// <summary>
        /// 普通的game object对象，不用挂载脚本
        /// </summary>
        /// <returns></returns>
        public GameObject Spawn()
        {
            GameObject poolable = null;
            PoolObject poolObject = m_ObjectPool.Spawn();
            if (poolObject != null)
            {
                poolable = (GameObject)poolObject.Target;
            }
            else
            {
                poolable = Instantiate(m_CardTemplate, m_InstanceRoot);
                m_ObjectPool.Register(PoolObject.Create(poolable), true);
            }
            return poolable;
        }

        public void Unspawn(GameObject poolable)
        {
            poolable.SetActive(false);
            m_ObjectPool.Unspawn(poolable);
        }

        public void Unspawn(MonoBehaviour poolable)
        {
            poolable.gameObject.SetActive(false);
            m_ObjectPool.Unspawn(poolable);
        }

        public void Release()
        {
            m_ObjectPool.ReleaseAllUnused();
        }
    }
}

using GameFramework;
using GameFramework.Event;
using UnityGameFramework.Runtime;
using System;
using System.Collections.Generic;

namespace Game
{
    public class EntityLoader : IReference
    {
        private Dictionary<int, Action<Entity>> dicCallback;
        private Dictionary<int, Entity> dicSerial2Entity;

        private List<int> tempList;

        public object Owner
        {
            get;
            private set;
        }

        public EntityLoader()
        {
            dicSerial2Entity = new Dictionary<int, Entity>();
            dicCallback = new Dictionary<int, Action<Entity>>();
            tempList = new List<int>();
            Owner = null;
        }

        public int ShowEntity<T>(EnumEntity enumEntity, Action<Entity> onShowSuccess, EntityData userData = null) where T : EntityLogic
        {
            return ShowEntity<T>((int)enumEntity, onShowSuccess, userData);
        }

        public int ShowEntity<T>(int entityId, Action<Entity> onShowSuccess, EntityData userData = null) where T : EntityLogic
        {
            int serialId =  GameEntry.Entity.GenerateSerialId();
            dicCallback.Add(serialId, onShowSuccess);
            GameEntry.Entity.ShowEntity<T>(serialId, entityId, userData);
            return serialId;
        }

        public bool HasEntity(int serialId)
        {
            return GetEntity(serialId) != null;
        }

        public Entity GetEntity(int serialId)
        {
            if (dicSerial2Entity.ContainsKey(serialId))
            {
                return dicSerial2Entity[serialId];
            }

            return null;
        }

        public IEnumerable<Entity> GetAllEntities()
        {
            return dicSerial2Entity.Values;
        }

        public void HideEntity(int serialId)
        {
            Entity entity = null;
            if (!dicSerial2Entity.TryGetValue(serialId, out entity))
            {
                Log.Error("Can find entity('serial id:{0}') ", serialId);
            }

            dicSerial2Entity.Remove(serialId);
            dicCallback.Remove(serialId);

            Entity[] entities = GameEntry.Entity.GetChildEntities(entity);
            if (entities != null)
            {
                foreach (var item in entities)
                {
                    //???Child Entity?????????Loader????????????????????????Loader??????
                    if (dicSerial2Entity.ContainsKey(item.Id))
                    {
                        HideEntity(item);
                    }
                    else//???Child Entity????????????Loader?????????????????????Parent Entity??????
                        GameEntry.Entity.DetachEntity(item);
                }
            }

            GameEntry.Entity.HideEntity(entity);
        }

        public void HideEntity(Entity entity)
        {
            if (entity == null)
                return;

            HideEntity(entity.Id);
        }

        public void HideEntity(EntityLogic entity)
        {
            if (entity == null)
                return;

            HideEntity(entity.Entity.Id);
        }

        public void HideAllEntity()
        {
            tempList.Clear();

            foreach (var entity in dicSerial2Entity.Values)
            {
                Entity parentEntity = GameEntry.Entity.GetParentEntity(entity);
                //???ParentEntity
                if (parentEntity != null)
                {
                    //???Parent Entity?????????Loader???????????????????????????Child Entity??????????????????????????????Parent Entity???GF???????????????Child Entity
                    if (dicSerial2Entity.ContainsKey(parentEntity.Id))
                    {
                        dicSerial2Entity.Remove(entity.Id);
                        dicCallback.Remove(entity.Id);
                    }
                    //???Parent Entity????????????Loader?????????????????????Parent Entity??????
                    else
                    {
                        GameEntry.Entity.DetachEntity(entity);
                    }
                }
            }

            foreach (var serialId in dicSerial2Entity.Keys)
            {
                tempList.Add(serialId);
            }

            foreach (var serialId in tempList)
            {
                HideEntity(serialId);
            }

            dicSerial2Entity.Clear();
            dicCallback.Clear();
        }

        private void OnShowEntitySuccess(object sender, GameEventArgs e)
        {
            ShowEntitySuccessEventArgs ne = (ShowEntitySuccessEventArgs)e;
            if (ne == null)
            {
                return;
            }

            Action<Entity> callback = null;
            if (!dicCallback.TryGetValue(ne.Entity.Id, out callback))
            {
                return;
            }

            dicSerial2Entity.Add(ne.Entity.Id, ne.Entity);
            callback?.Invoke(ne.Entity);
        }

        private void OnShowEntityFail(object sender, GameEventArgs e)
        {
            ShowEntityFailureEventArgs ne = (ShowEntityFailureEventArgs)e;
            if (ne == null)
            {
                return;
            }

            if (dicCallback.ContainsKey(ne.EntityId))
            {
                dicCallback.Remove(ne.EntityId);
                Log.Warning("{0} Show entity failure with error message '{1}'.", Owner.ToString(), ne.ErrorMessage);
            }
        }

        public static EntityLoader Create(object owner)
        {
            EntityLoader entityLoader = ReferencePool.Acquire<EntityLoader>();
            entityLoader.Owner = owner;
            GameEntry.Event.Subscribe(ShowEntitySuccessEventArgs.EventId, entityLoader.OnShowEntitySuccess);
            GameEntry.Event.Subscribe(ShowEntityFailureEventArgs.EventId, entityLoader.OnShowEntityFail);

            return entityLoader;
        }

        public void Clear()
        {
            Owner = null;
            dicSerial2Entity.Clear();
            dicCallback.Clear();
            GameEntry.Event.Unsubscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
            GameEntry.Event.Unsubscribe(ShowEntityFailureEventArgs.EventId, OnShowEntityFail);
        }
    }
}


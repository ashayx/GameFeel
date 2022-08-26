//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameFramework.DataTable;
using System;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Game
{
    public static class EntityExtension
    {
        // 关于 EntityId 的约定：
        // 0 为无效
        // 正值用于和服务器通信的实体（如玩家角色、NPC、怪等，服务器只产生正值）
        // 负值用于本地生成的临时实体（如特效、FakeObject等）
        private static int s_SerialId = 0;


        //public static void HideEntity(this EntityComponent entityComponent, EntityLogic entity)
        //{
        //    entityComponent.HideEntity(entity.Entity);
        //}

        public static void ShowEntity<T>(this EntityComponent entityComponent, int serialId, int entityId, EntityData userData = null)
        {
            entityComponent.ShowEntity(serialId, entityId, typeof(T), userData);
        }

        public static void ShowEntity(this EntityComponent entityComponent, int serialId, int entityId, Type logicType, EntityData data = null)
        {
            if (data == null)
            {
                Log.Warning("Data is invalid.");
                return;
            }

            IDataTable<DREntity> dtEntity = GameEntry.DataTable.GetDataTable<DREntity>();
            DREntity drEntity = dtEntity.GetDataRow((int)data.TypeId);
            if (drEntity == null)
            {
                Log.Warning("Can not load entity id '{0}' from data table.", data.TypeId.ToString());
                return;
            }

            entityComponent.ShowEntity(serialId, logicType, AssetUtility.GetEntityAsset(drEntity.AssetName), drEntity.Group, Constant.AssetPriority.EntityAsset, data);
        }

        public static int GenerateSerialId(this EntityComponent entityComponent)
        {
            return --s_SerialId;
        }

        public static EntityLoader entityLoader;
        public static int ShowEntity<T>(this EntityComponent entityComponen, EnumEntity enumEntity, Action<Entity> onShowSuccess, EntityData userData = null) where T : EntityLogic
        {
            if (entityLoader == null)
            {
                entityLoader = EntityLoader.Create(GameEntry.Entity);
            }
            return entityLoader.ShowEntity<T>(enumEntity, onShowSuccess, userData);
        }

        public static void HideEntity(this EntityComponent entityComponent, EntityLogic entity)
        {
            if (entityLoader == null)
            {
                Log.Error("entityLoader is null");
                return;
            }
            entityLoader.HideEntity(entity);
        }

        public static Entity[] GetEntities(this EntityComponent entityComponent, EnumEntity enumEntity)
        {
            return entityComponent.GetEntities(AssetUtility.GetEntityAsset(enumEntity.ToString()));
        }
    }
}

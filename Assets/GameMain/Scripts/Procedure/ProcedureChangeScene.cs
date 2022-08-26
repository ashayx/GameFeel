//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameFramework.DataTable;
using GameFramework.Event;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
using UnityEngine;
using MoreMountains.Tools;

namespace Game
{
    public class ProcedureChangeScene : ProcedureBase
    {
        private const int MenuSceneId = 2;

        private bool m_ChangeToMenu = false;

        public override bool UseNativeDialog
        {
            get
            {
                return false;
            }
        }

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);


            // 停止所有声音
            GameEntry.Sound.StopAllLoadingSounds();
            GameEntry.Sound.StopAllLoadedSounds();

            // 隐藏所有实体
            GameEntry.Entity.HideAllLoadingEntities();
            GameEntry.Entity.HideAllLoadedEntities();

            //// 卸载所有场景
            //string[] loadedSceneAssetNames = GameEntry.Scene.GetLoadedSceneAssetNames();
            //for (int i = 0; i < loadedSceneAssetNames.Length; i++)
            //{
            //    GameEntry.Scene.UnloadScene(loadedSceneAssetNames[i]);
            //}

            // 还原游戏速度
            GameEntry.Base.ResetNormalGameSpeed();
            // 关闭所有ui
            GameEntry.UI.CloseAllLoadedUIForms();
            // 释放对象池
            //GameEntry.PlatSegmentComponet.Release();

            int sceneId = procedureOwner.GetData<VarInt32>("NextSceneId");
            m_ChangeToMenu = sceneId == MenuSceneId;
            IDataTable<DRScene> dtScene = GameEntry.DataTable.GetDataTable<DRScene>();
            DRScene drScene = dtScene.GetDataRow(sceneId);
            if (drScene == null)
            {
                Log.Warning("Can not load scene '{0}' from data table.", sceneId.ToString());
                return;
            }

            //GameEntry.Scene.LoadScene(AssetUtility.GetSceneAsset(drScene.AssetName), Constant.AssetPriority.SceneAsset, this);
            //m_BackgroundMusicId = drScene.BackgroundMusicId;
            GFLoadingManager.LoadScene(AssetUtility.GetSceneAsset(drScene.AssetName));
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (!GFLoadingManager.LoadComplete)
            {
                return;
            }

            if (m_ChangeToMenu)
            {
                ChangeState<ProcedureMenu>(procedureOwner);
            }
            else
            {
                ChangeState<ProcedureMain>(procedureOwner);
            }
        }

    }
}

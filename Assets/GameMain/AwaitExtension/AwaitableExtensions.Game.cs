//using System;
//using System.Threading.Tasks;
//using Game;
//using UnityGameFramework.Runtime;

//namespace Game
//{
//    public partial class AwaitableExtensions
//    {

//        /// <summary>
//        /// 打开界面（可等待）
//        /// </summary>
//        public static Task<UIForm> OpenUIFormAsync(this UIComponent uiComponent, int uiFormId,
//            object userData = null)
//        {
//#if UNITY_EDITOR
//            TipsSubscribeEvent();
//#endif
//            int? serialId = uiComponent.OpenUIForm(uiFormId, userData);
//            if (serialId == null)
//            {
//                return Task.FromResult((UIForm)null);
//            }

//            var tcs = new TaskCompletionSource<UIForm>();
//            s_UIFormTcs.Add(serialId.Value, tcs);
//            return tcs.Task;
//        }
//        /// <summary>
//        /// 打开界面（可等待）
//        /// </summary>
//        public static Task<UIForm> OpenUIFormAsync(this UIComponent uiComponent, EnumUIForm uiFormId,
//            object userData = null)
//        {
//#if UNITY_EDITOR
//            TipsSubscribeEvent();
//#endif
//            int? serialId = uiComponent.OpenUIForm(uiFormId, userData);
//            if (serialId == null)
//            {
//                return Task.FromResult((UIForm)null);
//            }

//            var tcs = new TaskCompletionSource<UIForm>();
//            s_UIFormTcs.Add(serialId.Value, tcs);
//            return tcs.Task;
//        }

//        /// <summary>
//        /// 显示实体（可等待）
//        /// </summary>
//        public static Task<Entity> ShowEntityAsync(this EntityComponent entityComponent, Type logicType, string entityGroup, int priority,EntityData data)
//        {
//#if UNITY_EDITOR
//            TipsSubscribeEvent();
//#endif
//            var tcs = new TaskCompletionSource<Entity>();
//            s_EntityTcs.Add(data.Id, tcs);
//            entityComponent.ShowEntity(logicType, entityGroup, priority, data);
//            return tcs.Task;
//        }
//    }
//}
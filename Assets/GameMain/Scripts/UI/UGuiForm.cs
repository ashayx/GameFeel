//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace Game
{
    public abstract class UGuiForm : UIFormLogic
    {

        public const int DepthFactor = 100;
        private const float FadeTime = 0.3f;

        private static Font s_MainFont = null;
        private Canvas m_CachedCanvas = null;
        private CanvasGroup m_CanvasGroup = null;
        private List<Canvas> m_CachedCanvasContainer = new List<Canvas>();

        public int OriginalDepth
        {
            get;
            private set;
        }

        public int Depth
        {
            get
            {
                return m_CachedCanvas.sortingOrder;
            }
        }

        public void Close()
        {
            Close(false);
        }

        public void Close(bool ignoreFade)
        {
            StopAllCoroutines();

            if (ignoreFade)
            {
                GameEntry.UI.CloseUIForm(this);
            }
            else
            {
                StartCoroutine(CloseCo(FadeTime));
            }
        }

        public void PlayUISound(int uiSoundId)
        {
            //GameEntry.Sound.PlayUISound(uiSoundId);
        }

        public static void SetMainFont(Font mainFont)
        {
            if (mainFont == null)
            {
                Log.Error("Main font is invalid.");
                return;
            }

            s_MainFont = mainFont;
        }


        protected override void OnInit(object userData)

        {
            base.OnInit(userData);

            m_CachedCanvas = gameObject.GetOrAddComponent<Canvas>();
            m_CachedCanvas.overrideSorting = true;
            m_CachedCanvas.sortingLayerName = "UI";
            OriginalDepth = m_CachedCanvas.sortingOrder;

            m_CanvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();

            RectTransform transform = GetComponent<RectTransform>();
            transform.anchorMin = Vector2.zero;
            transform.anchorMax = Vector2.one;
            transform.anchoredPosition = Vector2.zero;
            transform.sizeDelta = Vector2.zero;

            gameObject.GetOrAddComponent<GraphicRaycaster>();


        }


        protected override void OnRecycle()

        {
            base.OnRecycle();
        }


        protected override void OnOpen(object userData)

        {
            base.OnOpen(userData);

            m_CanvasGroup.alpha = 0f;
            //StopAllCoroutines();
            //StartCoroutine(m_CanvasGroup.FadeToAlpha(1f, FadeTime));
            StartCoroutine(MMFade.FadeCanvasGroup(m_CanvasGroup, FadeTime, 1f, true));
        }


        protected override void OnClose(bool isShutdown, object userData)

        {
            base.OnClose(isShutdown, userData);
        }


        protected override void OnPause()

        {
            base.OnPause();
        }


        protected override void OnResume()

        {
            base.OnResume();

            m_CanvasGroup.alpha = 0f;
            StopAllCoroutines();
            //StartCoroutine(m_CanvasGroup.FadeToAlpha(1f, FadeTime));
            StartCoroutine(MMFade.FadeCanvasGroup(m_CanvasGroup, FadeTime, 1f, true));

        }


        protected override void OnCover()
        {
            base.OnCover();
        }


        protected override void OnReveal()

        {
            base.OnReveal();
        }

        protected override void OnRefocus(object userData)

        {
            base.OnRefocus(userData);
        }


        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)

        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
        }

        protected override void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)

        {
            int oldDepth = Depth;
            base.OnDepthChanged(uiGroupDepth, depthInUIGroup);
            int deltaDepth = UGuiGroupHelper.DepthFactor * uiGroupDepth + DepthFactor * depthInUIGroup - oldDepth + OriginalDepth;
            GetComponentsInChildren(true, m_CachedCanvasContainer);
            for (int i = 0; i < m_CachedCanvasContainer.Count; i++)
            {
                m_CachedCanvasContainer[i].sortingOrder += deltaDepth;
            }

            m_CachedCanvasContainer.Clear();
        }

        private IEnumerator CloseCo(float duration)
        {
            yield return MMFade.FadeCanvasGroup(m_CanvasGroup, duration, 0f, true);
            GameEntry.UI.CloseUIForm(this);
        }
    }
}

using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using UnityGameFramework.Runtime;
using GameFramework.Event;
using UnityEngine.Events;

namespace MoreMountains.Tools
{
    public class GFLoadingManager : MonoBehaviour
    {
        //是否加载完成
        public static bool LoadComplete;

        [Header("GameObjects")]
        [Tooltip("平滑的进度")]
        public ProgressEvent SetInterpolatedProgressValue;
        public UnityEvent OnLoadTransitionComplete;
        public int FaderID = 500;

        [Header("Time")]
        public float StartFadeDuration = 0.2f;
        public float ProgressBarSpeed = 2f;
        public float ExitFadeDuration = 0.2f;
        public float LoadCompleteDelay = 0f;
        public MMTweenType AniTween;

        protected static string _sceneToLoad = "";
        protected float _loadProgress = 0f;
        protected float _interpolatedLoadProgress;
        public static string LoadingScreenSceneName = "Assets/GameMain/Scenes/LoadingScene.unity";
        private static string[] loadedSceneAssetNames;
        private bool loadEnd = false;

        private static SceneComponent Scene;
        private static EventComponent Event;
        public static void LoadScene(string sceneToLoad)
        {
            LoadComplete = false;
            _sceneToLoad = sceneToLoad;
            Scene = UnityGameFramework.Runtime.GameEntry.GetComponent<SceneComponent>();
            Event = UnityGameFramework.Runtime.GameEntry.GetComponent<EventComponent>();
            loadedSceneAssetNames = Scene.GetLoadedSceneAssetNames();
            if (LoadingScreenSceneName != null)
            {
                Scene.LoadScene(LoadingScreenSceneName, 0);
                //GameEntry.Scene.SetSceneOrder(LoadingScreenSceneName, 999);
            }
        }

        protected virtual void Start()
        {
            AniTween = new MMTweenType(MMTween.MMTweenCurve.EaseOutCubic);

            if (!string.IsNullOrEmpty(_sceneToLoad))
            {
                StartCoroutine(LoadSequence());
            }
        }

        void OnEnable()
        {
            Event.Subscribe(LoadSceneSuccessEventArgs.EventId, OnLoadSceneSuccess);
            Event.Subscribe(LoadSceneFailureEventArgs.EventId, OnLoadSceneFailure);
            Event.Subscribe(LoadSceneUpdateEventArgs.EventId, OnLoadSceneUpdate);
            Event.Subscribe(LoadSceneDependencyAssetEventArgs.EventId, OnLoadSceneDependencyAsset);
        }

        void OnDisable()
        {
            Event.Unsubscribe(LoadSceneSuccessEventArgs.EventId, OnLoadSceneSuccess);
            Event.Unsubscribe(LoadSceneFailureEventArgs.EventId, OnLoadSceneFailure);
            Event.Unsubscribe(LoadSceneUpdateEventArgs.EventId, OnLoadSceneUpdate);
            Event.Unsubscribe(LoadSceneDependencyAssetEventArgs.EventId, OnLoadSceneDependencyAsset);
        }

        protected virtual void Update()
        {
            UpdateProgress();
        }

        /// <summary>
        /// Sends progress value via UnityEvents
        /// </summary>
        protected virtual void UpdateProgress()
        {

            _interpolatedLoadProgress = MMMaths.Approach(_interpolatedLoadProgress, _loadProgress, Time.deltaTime * ProgressBarSpeed);
            //Log.Info("update {0}",_interpolatedLoadProgress);
            SetInterpolatedProgressValue?.Invoke(_interpolatedLoadProgress);
        }

        protected virtual IEnumerator LoadSequence()
        {
            LoadComplete = false;
            InitiateLoad();
            //进入动画
            yield return EntryFade();
            //卸载旧场景
            yield return UnloadScenes();
            //加载新场景
            yield return LoadScenes();
            LoadComplete = true;
            //加载完成
            yield return LoadingComplete();
            //退出动画
            yield return ExitFade();
            //卸载loading场景
            yield return UnloadSceneLoader();

        }
        private IEnumerator UnloadScenes()
        {
            // 卸载所有场景
            for (int i = 0; i < loadedSceneAssetNames.Length; i++)
            {
                Scene.UnloadScene(loadedSceneAssetNames[i]);
                while (Scene.SceneIsUnloading(loadedSceneAssetNames[i]))
                {
                    yield return null;
                }
            }
        }

        private IEnumerator UnloadSceneLoader()
        {
            Scene.UnloadScene(LoadingScreenSceneName);
            yield return null;
        }

        private IEnumerator LoadScenes()
        {
            loadEnd = false;
            Scene.LoadScene(_sceneToLoad, 0);

            while (!loadEnd)
            {
                _loadProgress = 0.9f;
                yield return null;
            }

            _loadProgress = 1f;

            while (_interpolatedLoadProgress < 1)
            {
                yield return null;
            }
        }
        private IEnumerator LoadingComplete()
        {
            if (LoadCompleteDelay > 0)
            {
                OnLoadTransitionComplete?.Invoke();
                yield return new WaitForSeconds(LoadCompleteDelay);
            }
        }

        private IEnumerator EntryFade()
        {
            if (StartFadeDuration > 0)
            {
                MMFadeInEvent.Trigger(StartFadeDuration, AniTween, FaderID);
                yield return new WaitForSeconds(StartFadeDuration);
            }
        }

        private IEnumerator ExitFade()
        {
            if (ExitFadeDuration > 0)
            {
                MMFadeOutEvent.Trigger(ExitFadeDuration, AniTween, FaderID);
                yield return new WaitForSeconds(ExitFadeDuration);
            }
        }


        protected virtual void InitiateLoad()
        {
            //LoadingCompleteAnimation.alpha = 0;
            _loadProgress = 0;
            _interpolatedLoadProgress = 0;
        }

        private void OnLoadSceneSuccess(object sender, GameEventArgs e)
        {
            LoadSceneSuccessEventArgs ne = (LoadSceneSuccessEventArgs)e;
            loadEnd = true;
            Log.Info("Load scene '{0}' OK.", ne.SceneAssetName);
        }

        private void OnLoadSceneFailure(object sender, GameEventArgs e)
        {
            LoadSceneFailureEventArgs ne = (LoadSceneFailureEventArgs)e;
            Log.Error("Load scene '{0}' failure, error message '{1}'.", ne.SceneAssetName, ne.ErrorMessage);
        }

        private void OnLoadSceneUpdate(object sender, GameEventArgs e)
        {
            LoadSceneUpdateEventArgs ne = (LoadSceneUpdateEventArgs)e;
            //_loadProgress = ne.Progress;
            Log.Info("Load scene '{0}' update, progress '{1}'.", ne.SceneAssetName, ne.Progress.ToString("P2"));
        }

        private void OnLoadSceneDependencyAsset(object sender, GameEventArgs e)
        {
            LoadSceneDependencyAssetEventArgs ne = (LoadSceneDependencyAssetEventArgs)e;
            Log.Info("Load scene '{0}' dependency asset '{1}', count '{2}/{3}'.", ne.SceneAssetName, ne.DependencyAssetName, ne.LoadedCount.ToString(), ne.TotalCount.ToString());
        }
    }
}
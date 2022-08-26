//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameFramework;
using GameFramework.Resource;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace Game
{
    public static class AssetUtility
    {
        public static string GetConfigAsset(string assetName, bool fromBytes)
        {
            return Utility.Text.Format("Assets/GameMain/Configs/{0}.{1}", assetName, fromBytes ? "bytes" : "txt");
        }

        public static string GetDataTableAsset(string assetName, bool fromBytes)
        {
            return Utility.Text.Format("Assets/GameMain/DataTables/{0}.{1}", assetName, fromBytes ? "bytes" : "txt");
        }

        public static string GetDictionaryAsset(string assetName, bool fromBytes)
        {
            return Utility.Text.Format("Assets/GameMain/Localization/{0}/Dictionaries/{1}.{2}", GameEntry.Localization.Language.ToString(), assetName, fromBytes ? "bytes" : "xml");
        }

        public static string GetFontAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameMain/Fonts/{0}.ttf", assetName);
        }

        public static string GetSceneAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameMain/Scenes/{0}.unity", assetName);
        }

        public static string GetMusicAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameMain/Music/{0}.mp3", assetName);
        }

        public static string GetSoundAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameMain/Sounds/{0}.wav", assetName);
        }

        public static string GetEntityAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameMain/Entities/{0}.prefab", assetName);
        }

        public static string GetUIFormAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameMain/UI/UIForms/{0}.prefab", assetName);
        }

        public static string GetUISoundAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameMain/UI/UISounds/{0}.wav", assetName);
        }

        public static string GetCardIconAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameMain/Texture/Icon/Card/{0}.png", assetName);
        }

        public static string GetCardTypeIconAsset(int assetName)
        {
            return Utility.Text.Format("Assets/GameMain/Texture/Icon/CardType/CardType_{0}.png", assetName);
        }

        public static string GetEquipconAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameMain/Texture/Icon/Item/{0}.png", assetName);
        }

        public static void SetIcon(string url, Image image)
        {
            GameEntry.Resource.LoadAsset(url, typeof(Texture2D), new LoadAssetCallbacks((tempAssetName, asset, duration, userdata) =>
            {
                Texture2D t2d = (Texture2D)asset;
                //Log.Info("加载完成了");
                Sprite sprite = Sprite.Create(t2d, new Rect(0, 0, t2d.width, t2d.height), new Vector2(0.5f, 0.5f), 100);
                image.sprite = sprite;
            }, (tempAssetName, status, errorMessage, userdata) =>
            {
                Log.Error(new GameFrameworkException(errorMessage));
            }));
        }

        public static void SetIcon(string url, SpriteRenderer image)
        {
            GameEntry.Resource.LoadAsset(url, typeof(Texture2D), new LoadAssetCallbacks((tempAssetName, asset, duration, userdata) =>
            {
                Texture2D t2d = (Texture2D)asset;
                //Log.Info("加载完成了");
                Sprite sprite = Sprite.Create(t2d, new Rect(0, 0, t2d.width, t2d.height), new Vector2(0.5f, 0.5f), 100);
                image.sprite = sprite;
            }, (tempAssetName, status, errorMessage, userdata) =>
            {
                Log.Error(new GameFrameworkException(errorMessage));
            }));
        }
    }
}

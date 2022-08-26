using UnityEditor;
using UnityEngine;

public class TextureAutoSet : EditorWindow
{
    private string m_selectFolder = "";
    private PlatformType m_platformType = PlatformType.iPhone;
    private TextureImporterType m_textureImporterType = TextureImporterType.Default;
    private TextureImporterFormat m_importerFormat = TextureImporterFormat.ASTC_6x6;

    [MenuItem("Tools/Texture/Texture Auto Set")]
    private static void ShowPanel()
    {
        GetWindow<TextureAutoSet>().Show();
    }

    private void OnEnable()
    {
        m_selectFolder = PlayerPrefs.GetString("TextureSelectFolder");
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Label("路径:", GUILayout.Width(80));
        m_selectFolder = GUILayout.TextField(m_selectFolder);
        if (GUILayout.Button("选择文件夹", GUILayout.Width(120)))
        {
            var result = EditorUtility.OpenFolderPanel("选择文件夹", m_selectFolder, "");
            if (string.IsNullOrEmpty(result))
            {
                return;
            }

            m_selectFolder = result.Substring(result.IndexOf("Assets"));
            m_selectFolder = m_selectFolder.Replace('\\', '/');
            PlayerPrefs.SetString("TextureSelectFolder", m_selectFolder);
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        m_platformType = (PlatformType)EditorGUILayout.EnumPopup("平台 : ", m_platformType);
        GUILayout.Space(10);
        m_textureImporterType = (TextureImporterType)EditorGUILayout.EnumPopup("处理的资源类型 : ", m_textureImporterType);
        GUILayout.Space(10);
        m_importerFormat = (TextureImporterFormat)EditorGUILayout.EnumPopup("资源的压缩类型:", m_importerFormat);
        GUILayout.Space(10);
        if (GUILayout.Button("设置全部"))
        {
            SetAllTextureType();
        }
    }

    private void SetAllTextureType()
    {
        var index = 0;
        var assets = AssetDatabase.FindAssets("t:Texture", new string[] { m_selectFolder });
        foreach (var item in assets)
        {
            index++;
            var path = AssetDatabase.GUIDToAssetPath(item);
            // 显示进度
            EditorUtility.DisplayProgressBar($"当前资源：{index}/{assets.Length}", path, (float)index / assets.Length);
            // IOS平台设置
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null || importer.textureType != m_textureImporterType)
            {
                continue;
            }

            if (m_platformType == PlatformType.All)
            {
                string[] values = System.Enum.GetNames(typeof(PlatformType));
                for (int i = 1; i < values.Length; i++)
                {
                    var setting = importer.GetPlatformTextureSettings(values[i]);
                    // 设置成选择类型
                    setting.format = m_importerFormat;
                    setting.overridden = true;
                    importer.SetPlatformTextureSettings(setting);
                }
            }
            else
            {
                var setting = importer.GetPlatformTextureSettings(m_platformType.ToString());
                // 设置成选择类型
                setting.format = m_importerFormat;
                setting.overridden = true;
                importer.SetPlatformTextureSettings(setting);
            }

            // 保存设置
            AssetDatabase.ImportAsset(path);
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }
}

/// <summary>
///  The options for the platform string are "Standalone", "Web", "iPhone", "Android", "WebGL", "Windows Store Apps", "PS4", "XboxOne", "Nintendo Switch" and "tvOS".
/// </summary>
public enum PlatformType
{
    All,
    Android,
    iPhone,
    WebGL,
}
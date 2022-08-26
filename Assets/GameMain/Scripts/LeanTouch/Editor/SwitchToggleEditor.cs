
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[CustomEditor(typeof(SwitchToggle))]
[CanEditMultipleObjects]
public class SwitchToggleEditor : UnityEditor.Editor
{
    SerializedProperty m_CurrentSwitchState;
    SerializedProperty m_InitialState;


    protected void OnEnable()
    {
        m_InitialState = serializedObject.FindProperty("InitialState");

        m_CurrentSwitchState = serializedObject.FindProperty("CurrentSwitchState");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();

        serializedObject.Update();
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(m_CurrentSwitchState);

        if (EditorGUI.EndChangeCheck())
        {
            SwitchToggle toggle = serializedObject.targetObject as SwitchToggle;
            if (toggle.CurrentSwitchState == SwitchToggle.SwitchStates.Left)
            {
                toggle.SwitchToOn();
            }
            else
            {
                toggle.SwitchToOff();
            }
        }
        serializedObject.ApplyModifiedProperties();


    }
}

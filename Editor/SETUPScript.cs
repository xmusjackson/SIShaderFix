using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using System;

public class SIURPSetupAsset : ScriptableObject
{
    public string description = "Setup the URP for Schedule I Modding";
}

[CustomEditor(typeof(SIURPSetupAsset))]
public class SIURPSetupAssetEditor : Editor
{
    private GUIStyle wrapped_text;
    private GUIStyle bold_button;
    private GUIStyle header_style;

    private static ListRequest listRequest;
    private static AddRequest addRequest;

    private bool urp_installed = false;

    private bool urp_checking = false;

    private bool installing = false;

    private string button_text;

    public void Awake()
    {
        // Check if URP is installed
        listRequest = Client.List();
        EditorApplication.update += CheckProgress;
        urp_checking = true;
    }

    private void CheckProgress()
    {
        if (listRequest.IsCompleted)
        {
            if (listRequest.Status == StatusCode.Success)
            {
                foreach (var package in listRequest.Result)
                {
                    if (package.name == "com.unity.render-pipelines.universal")
                    {
                        urp_installed = true;
                        break;
                    }
                }
            }
            else if (listRequest.Status >= StatusCode.Failure)
            { 
            }
            EditorApplication.update -= CheckProgress;
            urp_checking = false;
            DefineURPSymbol();
        }
    }

    void DefineURPSymbol()
    {
        if (urp_installed)
        {
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
            if (!defines.Contains("HAS_URP_INSTALLED"))
            {
                defines += ";HAS_URP_INSTALLED";
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, defines);

                AssetDatabase.Refresh();
            }
        }
    }

    private void SetupStyles()
    {
        wrapped_text = new GUIStyle(EditorStyles.label)
        {
            richText = true,
            wordWrap = true
        };

        bold_button = new GUIStyle(GUI.skin.button)
        {
            fontStyle = FontStyle.Bold,
            fontSize = 12,
            fixedHeight = 30,
            stretchWidth = false
        };

        header_style = new GUIStyle(EditorStyles.largeLabel)
        {
            fontStyle = FontStyle.Bold
        };
    }

    public override void OnInspectorGUI()
    {
        if (urp_checking)
        {
            EditorGUILayout.LabelField("Checking for URP installation...", new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleCenter,
                stretchWidth = true
            });
            return;
        }

        if (wrapped_text == null || bold_button == null || header_style == null)
        {
            SetupStyles();
        }

        SIURPSetupAsset asset = (SIURPSetupAsset)target;

        // Top spacing
        GUILayout.Space(12);

        // Header title style

        EditorGUILayout.LabelField("🔧 Schedule I URP Setup", header_style);
        GUILayout.Space(10);

        EditorGUILayout.LabelField("By default, the Unity Editor is not setup the same way as the game." +
        " Because of this, any shaders that we include from the editor appear in game as invisible. This is ultimately" +
        " becuase the shader compiler isn't compiling our shaders with the correct settings and important shader passes" +
        " are being ignored completely when we export an Asset Bundle." +
        "\n\n <b>TL;DR: Click the button below if your objects don't show up in game.</b>", wrapped_text);

        GUILayout.Space(15);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (urp_installed)
        {
            button_text = "Setup URP for Schedule I Modding";
            if (GUILayout.Button(button_text, bold_button, GUILayout.Width(220)))
            {
                SetupURP();
                EditorUtility.SetDirty(asset);
            }

        }
        else
        {
            button_text = installing ? "Installing URP package..." : "Install URP for Schedule I Modding";
            if (GUILayout.Button(button_text, bold_button, GUILayout.Width(220)))
            {

                installing = true;

                addRequest = Client.Add("com.unity.render-pipelines.universal");
                EditorApplication.update += AddProgress;
            }
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
    }

    private void AddProgress()
    {
        if (addRequest.IsCompleted)
        {
            if (addRequest.Status == StatusCode.Success)
                Debug.Log("Installed: " + addRequest.Result.packageId);
            else if (addRequest.Status >= StatusCode.Failure)
                Debug.Log(listRequest.Error.message);

            EditorApplication.update -= AddProgress;
            installing = false;
            urp_installed = true;
            DefineURPSymbol();
        }
    }

    private void SetupURP()
    {
        const string urpAssetName = "SI-UniversalRenderPipelineAsset";
        const string globalSettingsName = "SI-UniversalRenderPipelineGlobalSettings";

        var helperType = Type.GetType("ScheduleIEditor.SIURPSetupHelper");

        if (helperType != null)
        {
            var method = helperType.GetMethod("TrySetupURP", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            bool success = (bool)method.Invoke(null, new object[] { urpAssetName, globalSettingsName });

            if (success)
                Debug.Log("URP setup complete!");
            else
                Debug.LogError("Failed to set up URP. Check asset names or asset loading.");
        }
        else
        {
            Debug.LogWarning("URP Setup Helper not found. URP may not be installed correctly.");
            urp_installed = false;
        }
    }
}
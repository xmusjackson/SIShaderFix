#if HAS_URP_INSTALLED
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ScheduleIEditor
{
    public static class SIURPSetupHelper
    {
        public static bool TrySetupURP(string urpAssetName, string globalSettingsName)
        {
            string[] urp_guids = AssetDatabase.FindAssets(urpAssetName, new[]{ "Assets/SIShaderFix/Assets" });
            string[] gs_guids = AssetDatabase.FindAssets(globalSettingsName, new[] { "Assets/SIShaderFix/Assets" });

            if (urp_guids.Length == 0 || gs_guids.Length == 0)
                return false;

            string urpPath = AssetDatabase.GUIDToAssetPath(urp_guids[0]);
            string gsPath = AssetDatabase.GUIDToAssetPath(gs_guids[0]);

            RenderPipelineAsset urpAsset = AssetDatabase.LoadAssetAtPath<RenderPipelineAsset>(urpPath);
            RenderPipelineGlobalSettings globalSettings = AssetDatabase.LoadAssetAtPath<RenderPipelineGlobalSettings>(gsPath);

            if (urpAsset == null || globalSettings == null)
                return false;

            GraphicsSettings.renderPipelineAsset = urpAsset;
            QualitySettings.renderPipeline = urpAsset;

            GraphicsSettings.RegisterRenderPipelineSettings<UniversalRenderPipeline>(globalSettings);

            return true;
        }
    }
}
#endif
using UnityEditor;

namespace Loju.Build
{
    public class StandaloneBuildConfig : BuildConfig
    {

        public bool IsServer = false;

        public StandaloneBuildConfig(BuildTarget target, BuildType type, string platformName, string appendToPath = null, BuildCompilationDefines defines = null) : base(target, BuildTargetGroup.Standalone, type, platformName, appendToPath, defines)
        {

        }

        public override void OnPreBuild(ref BuildOptions options)
        {
            base.OnPreBuild(ref options);

#if UNITY_2021_OR_NEWER
            EditorUserBuildSettings.standaloneBuildSubtarget = IsServer ? StandaloneBuildSubtarget.Server : StandaloneBuildSubtarget.Player;
#else
            options |= BuildOptions.EnableHeadlessMode;
#endif
        }

        public override void OnPostBuild(string pathToBuild)
        {
            base.OnPostBuild(pathToBuild);

#if UNITY_2021_OR_NEWER
            EditorUserBuildSettings.standaloneBuildSubtarget = StandaloneBuildSubtarget.Player;
#endif
        }

    }
}
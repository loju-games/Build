using UnityEditor;

namespace Loju.Build
{
    public class iOSBuildConfig : BuildConfig
    {

        public iOSBuildConfig(BuildTarget target, BuildType type, string platformName, string appendToPath = null, BuildCompilationDefines defines = null) : base(target, type, platformName, appendToPath, defines)
        {

        }

        public override void OnPreBuild(ref BuildOptions options)
        {
            base.OnPreBuild(ref options);

            EditorUserBuildSettings.iOSBuildConfigType = type == BuildType.Debug ? iOSBuildType.Debug : iOSBuildType.Release;
            options |= BuildOptions.AcceptExternalModificationsToPlayer;
        }

    }
}
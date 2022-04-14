using UnityEditor;

namespace Loju.Build
{
    /// Build config for iOS, includes additional build options and user build settings for the iOS platform.
    public class iOSBuildConfig : BuildConfig
    {

        public bool append;

        public iOSBuildConfig(BuildType type, string platformName, string appendToPath = null, BuildCompilationDefines defines = null, bool append = true) : base(BuildTarget.iOS, BuildTargetGroup.iOS, type, platformName, appendToPath, defines)
        {
            this.append = append;
        }

        public override void OnPreBuild(ref BuildOptions options)
        {
            base.OnPreBuild(ref options);

#if UNITY_2021_1_OR_NEWER
            EditorUserBuildSettings.iOSXcodeBuildConfig = type == BuildType.Debug ? XcodeBuildConfig.Debug : XcodeBuildConfig.Release;
#else
            //EditorUserBuildSettings.XcodeBuildConfig = type == BuildType.Debug ? XcodeBuildConfig.Debug : XcodeBuildConfig.Release;
#endif
            if (append) options |= BuildOptions.AcceptExternalModificationsToPlayer;
        }

    }
}
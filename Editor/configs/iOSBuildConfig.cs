using UnityEditor;

namespace Loju.Build
{
    /// Build config for iOS, includes additional build options and user build settings for the iOS platform.
    public class iOSBuildConfig : BuildConfig
    {

        public iOSBuildConfig(BuildType type, string platformName, string appendToPath = null, BuildCompilationDefines defines = null) : base(BuildTarget.iOS, type, platformName, appendToPath, defines)
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
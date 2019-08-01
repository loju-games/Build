using UnityEditor;

namespace Loju.Build
{
    public enum AndroidReleaseBuildType
    {
        ProjectExport,
        SplitAPK,
        AppBundle
    }

    /// Build config for Android, handles build output differently depending on build type. For debug it generates a single APK that's
    /// installed and run on an attached device, for release or beta it creates multiple APKs or an AppBundle that can be submitted to the
    /// play store.
    public class AndroidBuildConfig : BuildConfig
    {

        public AndroidArchitecture debugArchitecture = AndroidArchitecture.ARMv7;
        public AndroidArchitecture releaseArchitecture = AndroidArchitecture.All;
        public AndroidReleaseBuildType releaseBuildType = AndroidReleaseBuildType.AppBundle;

        public AndroidBuildConfig(BuildType type, string platformName, string appendToPath = null, BuildCompilationDefines defines = null) : base(BuildTarget.Android, type, platformName, appendToPath, defines)
        {

        }

        public override void OnPreBuild(ref BuildOptions options)
        {
            base.OnPreBuild(ref options);

            if (type == BuildType.Debug)
            {
                EditorUserBuildSettings.buildAppBundle = false;
                EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
                PlayerSettings.Android.targetArchitectures = debugArchitecture;
                PlayerSettings.Android.buildApkPerCpuArchitecture = false;
            }
            else
            {
                PlayerSettings.Android.targetArchitectures = releaseArchitecture;

                if (releaseBuildType == AndroidReleaseBuildType.ProjectExport)
                {
                    EditorUserBuildSettings.buildAppBundle = false;
                    EditorUserBuildSettings.exportAsGoogleAndroidProject = true;
                    PlayerSettings.Android.buildApkPerCpuArchitecture = false;
                }
                else if (releaseBuildType == AndroidReleaseBuildType.AppBundle)
                {
                    EditorUserBuildSettings.buildAppBundle = true;
                    EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
                    PlayerSettings.Android.buildApkPerCpuArchitecture = false;
                }
                else if (releaseBuildType == AndroidReleaseBuildType.SplitAPK)
                {
                    EditorUserBuildSettings.buildAppBundle = false;
                    EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
                    PlayerSettings.Android.buildApkPerCpuArchitecture = true;
                }
            }
        }

    }
}
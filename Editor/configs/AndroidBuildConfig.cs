using UnityEditor;

namespace Loju.Build
{
    public enum AndroidAPKBuildType
    {
        SingleAPK,
        ProjectExport,
        SplitAPK,
        AppBundle
    }

    /// Build config for Android, handles build output differently depending on build type. For debug it generates a single APK that's
    /// installed and run on an attached device, for release or beta it creates multiple APKs or an AppBundle that can be submitted to the
    /// play store.
    public class AndroidBuildConfig : BuildConfig
    {

        public AndroidArchitecture buildArchitecture = AndroidArchitecture.All;
        public AndroidAPKBuildType apkBuildType = AndroidAPKBuildType.SingleAPK;

        public AndroidBuildConfig(BuildType type, string platformName, string appendToPath = null, BuildCompilationDefines defines = null) : base(BuildTarget.Android, type, platformName, appendToPath, defines)
        {

        }

        public override void OnPreBuild(ref BuildOptions options)
        {
            base.OnPreBuild(ref options);

            PlayerSettings.Android.targetArchitectures = buildArchitecture;

            if (apkBuildType == AndroidAPKBuildType.SingleAPK)
            {
                EditorUserBuildSettings.buildAppBundle = false;
                EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
                PlayerSettings.Android.buildApkPerCpuArchitecture = false;
            }
            else if (apkBuildType == AndroidAPKBuildType.ProjectExport)
            {
                EditorUserBuildSettings.buildAppBundle = false;
                EditorUserBuildSettings.exportAsGoogleAndroidProject = true;
                PlayerSettings.Android.buildApkPerCpuArchitecture = false;
            }
            else if (apkBuildType == AndroidAPKBuildType.AppBundle)
            {
                EditorUserBuildSettings.buildAppBundle = true;
                EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
                PlayerSettings.Android.buildApkPerCpuArchitecture = false;
            }
            else if (apkBuildType == AndroidAPKBuildType.SplitAPK)
            {
                EditorUserBuildSettings.buildAppBundle = false;
                EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
                PlayerSettings.Android.buildApkPerCpuArchitecture = true;
            }
        }

    }
}
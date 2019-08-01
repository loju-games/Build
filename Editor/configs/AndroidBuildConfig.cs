using UnityEditor;

namespace Loju.Build
{
    public enum AndroidReleaseBuildType
    {
        ProjectExport,
        SplitAPK,
        AppBundle
    }

    public class AndroidBuildConfig : BuildConfig
    {

        public AndroidReleaseBuildType releaseBuildType = AndroidReleaseBuildType.AppBundle;

        public AndroidBuildConfig(BuildTarget target, BuildType type, string platformName, string appendToPath = null, BuildCompilationDefines defines = null) : base(target, type, platformName, appendToPath, defines)
        {

        }

        public override void OnPreBuild(ref BuildOptions options)
        {
            base.OnPreBuild(ref options);

            if (type == BuildType.Debug)
            {
                EditorUserBuildSettings.buildAppBundle = false;
                EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
                PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7;
                PlayerSettings.Android.buildApkPerCpuArchitecture = false;
            }
            else
            {
                PlayerSettings.Android.targetArchitectures = AndroidArchitecture.All;

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
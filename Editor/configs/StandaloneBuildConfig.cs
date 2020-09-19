using UnityEditor;

namespace Loju.Build
{
    public class StandaloneBuildConfig : BuildConfig
    {

        public StandaloneBuildConfig(BuildTarget target, BuildType type, string platformName, string appendToPath = null, BuildCompilationDefines defines = null) : base(target, BuildTargetGroup.Standalone, type, platformName, appendToPath, defines)
        {

        }

    }
}
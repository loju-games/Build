using UnityEditor;

namespace Loju.Build
{
    public enum BuildType
    {
        Debug,
        Beta,
        Release
    }

    public class BuildConfig
    {

        public readonly BuildTarget target;
        public readonly BuildType type;
        public readonly string platformName;
        public readonly BuildCompilationDefines defines;
        public readonly string appendToPath;

        public BuildConfig(BuildTarget target, BuildType type, string platformName, string appendToPath = null, BuildCompilationDefines defines = null)
        {
            this.target = target;
            this.type = type;
            this.platformName = platformName;
            this.defines = defines != null ? defines.Clone() : new BuildCompilationDefines();
            this.appendToPath = appendToPath;
        }

        public virtual void OnPreBuild(ref BuildOptions options)
        {
            EditorUserBuildSettings.development = type == BuildType.Debug;
        }

        public virtual void OnPostBuild()
        {

        }

    }
}
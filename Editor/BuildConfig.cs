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

        public BuildTarget target = BuildTarget.iOS;
        public BuildType type = BuildType.Debug;
        public string platformName = null;
        public BuildCompilationDefines defines = null;
        public string appendToPath = null;
        public string[] scenes = null;

        public BuildConfig(BuildTarget target, BuildType type, string platformName, string appendToPath = null, BuildCompilationDefines defines = null) : this(target, EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes), type, platformName, appendToPath, defines)
        {

        }

        public BuildConfig(BuildTarget target, string[] scenes, BuildType type, string platformName, string appendToPath = null, BuildCompilationDefines defines = null)
        {
            this.target = target;
            this.type = type;
            this.platformName = platformName;
            this.defines = defines != null ? defines.Clone() : new BuildCompilationDefines();
            this.appendToPath = appendToPath;
            this.scenes = scenes;
        }

        public virtual string GetFinalBuildPath(string directory)
        {
            if (!string.IsNullOrEmpty(appendToPath)) directory = System.IO.Path.Combine(directory, appendToPath);

            return directory;
        }

        public virtual void OnPreBuild(ref BuildOptions options)
        {
            EditorUserBuildSettings.development = type == BuildType.Debug;
        }

        public virtual void OnPostBuild(string pathToBuild)
        {

        }

    }
}
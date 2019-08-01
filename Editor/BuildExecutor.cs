using UnityEngine;
using UnityEditor;
using System.IO;

namespace Loju.Build
{

    public static class BuildExecutor
    {

        public static void Build(BuildConfig config)
        {
            string location = GetBuildLocation(config.platformName, config.type);
            if (!string.IsNullOrEmpty(config.appendToPath)) location = Path.Combine(location, config.appendToPath);

            BuildOptions options = BuildOptions.None;
            BuildTargetGroup buildGroup = BuildPipeline.GetBuildTargetGroup(config.target);
            SaveVersion(config);

            // setup compilation defines
            BuildCompilationDefines restoreDefines = new BuildCompilationDefines(PlayerSettings.GetScriptingDefineSymbolsForGroup(buildGroup));
            BuildCompilationDefines currentDefines = restoreDefines.Clone();
            currentDefines.AddAll(config.defines);

            switch (config.type)
            {
                case BuildType.Debug: currentDefines.Add("BUILD_DEBUG"); break;
                case BuildType.Beta: currentDefines.Add("BUILD_BETA"); break;
                case BuildType.Release: currentDefines.Add("BUILD_RELEASE"); break;
            }

            // setup pre-build
            config.OnPreBuild(ref options);

            if (config.type == BuildType.Debug || config.target == BuildTarget.iOS) options |= BuildOptions.AutoRunPlayer;
            if (config.type == BuildType.Debug && config.target != BuildTarget.Android) options |= BuildOptions.Development;

            // build
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildGroup, currentDefines.ToString());
            BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, location, config.target, options);

            // cleanup
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildGroup, restoreDefines.ToString());

            config.OnPostBuild();
        }

        public static void UpdateBuildNumber(BuildTarget target)
        {
            // update internal build numbers
            if (target == BuildTarget.iOS)
            {
                PlayerSettings.iOS.buildNumber = (int.Parse(PlayerSettings.iOS.buildNumber) + 1).ToString();
            }
            else if (target == BuildTarget.Android)
            {
                PlayerSettings.Android.bundleVersionCode = PlayerSettings.Android.bundleVersionCode + 1;
            }
        }

        private static void SaveVersion(BuildConfig config)
        {
            // bake version into build
            string buildNumber = "";
            if (config.target == BuildTarget.iOS)
            {
                buildNumber = PlayerSettings.iOS.buildNumber;
            }
            else if (config.target == BuildTarget.Android)
            {
                buildNumber = PlayerSettings.Android.bundleVersionCode.ToString();
            }

            BuildInfo info = new BuildInfo(Application.version, buildNumber, config.platformName);
            File.WriteAllText(Path.Combine(Application.streamingAssetsPath, "version.json"), JsonUtility.ToJson(info));
        }

        private static string GetBuildLocation(string platform, BuildType type)
        {
            string location = "Builds";

            // apply version
            switch (type)
            {
                case BuildType.Debug:
                    location = Path.Combine(location, "Development");
                    location = Path.Combine(location, platform);
                    break;
                case BuildType.Beta:
                    location = Path.Combine(location, "Beta");
                    location = Path.Combine(location, platform);
                    location = Path.Combine(location, Application.version);
                    break;
                case BuildType.Release:
                    location = Path.Combine(location, "Release");
                    location = Path.Combine(location, platform);
                    location = Path.Combine(location, Application.version);
                    break;
            }

            return location;
        }
    }

}
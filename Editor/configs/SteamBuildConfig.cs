using UnityEditor;
using System.IO;
using System.Diagnostics;
using System.Text;

namespace Loju.Build
{
    public sealed class SteamBuildConfig : BuildConfig
    {

        public string depotId;
        public string localPath = "*";
        public string depotPath = ".";
        public string recursive = "1";
        public string[] fileExclusion = new string[] { "*.pdb" };

        public SteamBuildConfig(string depotId, BuildTarget target, BuildType type, string platformName, string appendToPath = null, BuildCompilationDefines defines = null) : base(target, BuildTargetGroup.Standalone, type, platformName, appendToPath, defines)
        {
            this.depotId = depotId;
        }

        public SteamBuildConfig(string depotId, BuildTarget target, string[] scenes, BuildType type, string platformName, string appendToPath = null, BuildCompilationDefines defines = null) : base(target, BuildTargetGroup.Standalone, scenes, type, platformName, appendToPath, defines)
        {
            this.depotId = depotId;
        }

        public static string CreateBuildFiles(string appId, string outputPath, string buildDescription, string setBranchLive, bool preview, params SteamBuildConfig[] configs)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("\"appbuild\"");
            stringBuilder.AppendLine("{");

            stringBuilder.AppendLine($"\t\"appid\"	\"{appId}\"");
            stringBuilder.AppendLine($"\t\"desc\"	\"{buildDescription}\"");
            stringBuilder.AppendLine("\t\"buildoutput\"	\".\\output\\\"");
            stringBuilder.AppendLine("\t\"contentroot\"	\".\\content\\\"");
            stringBuilder.AppendLine($"\t\"setlive\"	\"{setBranchLive}\"");
            stringBuilder.AppendLine($"\t\"preview\"	\"{(preview ? "1" : "0")}\"");
            stringBuilder.AppendLine("\t\"local\"	\"\"");
            stringBuilder.AppendLine("\t\"depots\"");
            stringBuilder.AppendLine("\t{");

            int i = 0, l = configs.Length;
            for (; i < l; ++i)
            {
                SteamBuildConfig config = configs[i];
                string depotPath = CreateDepotVDF(config, outputPath);
                stringBuilder.AppendLine($"\t\t\"{config.depotId}\"	\"{depotPath}\"");
            }

            stringBuilder.AppendLine("\t}");
            stringBuilder.AppendLine("}");

            Directory.CreateDirectory(Path.Combine(outputPath, "content"));
            Directory.CreateDirectory(Path.Combine(outputPath, "output"));

            string appVDFPath = GetPathToAppVDF(outputPath, appId);
            File.WriteAllText(appVDFPath, stringBuilder.ToString());

            return appVDFPath;
        }

        private static string CreateDepotVDF(SteamBuildConfig config, string outputPath)
        {
            string pathToBuild = BuildExecutor.GetFinalBuildPath(config);
            string buildPath = Path.Combine(Directory.GetCurrentDirectory(), Path.GetDirectoryName(pathToBuild));
            string depotFileName = $"depot_build_{config.depotId}.vdf";
            string depotPath = Path.Combine(outputPath, depotFileName);

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("\"DepotBuildConfig\"");
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine($"\t\"DepotID\"   \"{config.depotId}\"");
            stringBuilder.AppendLine($"\t\"ContentRoot\"   \"{buildPath}\"");
            stringBuilder.AppendLine($"\t\"FileMapping\"");
            stringBuilder.AppendLine("\t{");
            stringBuilder.AppendLine($"\t\t\"LocalPath\" \"{config.localPath}\"");
            stringBuilder.AppendLine($"\t\t\"DepotPath\" \"{config.depotPath}\"");
            stringBuilder.AppendLine($"\t\t\"recursive\" \"{config.recursive}\"");
            stringBuilder.AppendLine("\t}");

            int i = 0, l = config.fileExclusion.Length;
            for (; i < l; ++i)
            {
                stringBuilder.AppendLine($"\t\"FileExclusion\"   \"{config.fileExclusion[i]}\"");
            }

            stringBuilder.AppendLine("}");

            File.WriteAllText(depotPath, stringBuilder.ToString());

            return depotFileName;
        }

        public static void AuthorizeSteam(string sdkPath, string steamGuardCode)
        {
#if UNITY_EDITOR_WIN
            string pathToCmd = Path.Combine(sdkPath, "tools/ContentBuilder/builder/steamcmd.exe");
#else
            string pathToCmd = Path.Combine(sdkPath, "tools/ContentBuilder/builder_osx/steamcmd.sh");
#endif
            string arguments = $"+set_steam_guard_code {steamGuardCode} +quit";

            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = pathToCmd,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                Arguments = arguments
            };

            Process process = new Process
            {
                StartInfo = startInfo
            };

            process.Start();
            process.WaitForExit();

            string output = process.StandardOutput.ReadToEnd();
            UnityEngine.Debug.Log(output);
            process.Dispose();
        }

        public static void PushToSteam(string sdkPath, string outputPath, string appId, string username, string password, bool execute = true)
        {
#if UNITY_EDITOR_WIN
            string pathToCmd = Path.Combine(sdkPath, "tools/ContentBuilder/builder/steamcmd.exe");
#else
            string pathToCmd = Path.Combine(sdkPath, "tools/ContentBuilder/builder_osx/steamcmd.sh");
#endif
            string arguments = $"+login {username} {password} +run_app_build_http {GetPathToAppVDF(outputPath, appId)} +quit";

            if (execute)
            {
                ProcessStartInfo startInfo = new ProcessStartInfo()
                {
                    FileName = pathToCmd,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    Arguments = arguments
                };

                Process process = new Process
                {
                    StartInfo = startInfo
                };

                process.Start();
                process.WaitForExit();

                string output = process.StandardOutput.ReadToEnd();
                UnityEngine.Debug.Log(output);
                process.Dispose();
            }
            else
            {
                UnityEngine.Debug.LogFormat("{0} {1}", pathToCmd, arguments);
            }
        }

        private static string GetPathToAppVDF(string outputPath, string appId)
        {
            return Path.Combine(outputPath, $"app_build_{appId}.vdf");
        }

    }
}
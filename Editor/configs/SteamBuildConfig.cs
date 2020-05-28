using UnityEditor;
using System.IO;
using System.Diagnostics;
using System.Text;

namespace Loju.Build
{
    public sealed class SteamBuildConfig : BuildConfig
    {

        public string depotId;

        public SteamBuildConfig(string depotId, BuildTarget target, BuildType type, string platformName, string appendToPath = null, BuildCompilationDefines defines = null) : base(target, type, platformName, appendToPath, defines)
        {
            this.depotId = depotId;
        }

        public SteamBuildConfig(string depotId, BuildTarget target, string[] scenes, BuildType type, string platformName, string appendToPath = null, BuildCompilationDefines defines = null) : base(target, scenes, type, platformName, appendToPath, defines)
        {
            this.depotId = depotId;
        }

        public static void CreateBuildFiles(string appId, string outputPath, string buildDescription, string setBranchLive, params SteamBuildConfig[] configs)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("\"appbuild\"");
            stringBuilder.AppendLine("{");

            stringBuilder.AppendLine($"\t\"appid\"	\"{appId}\"");
            stringBuilder.AppendLine($"\t\"desc\"	\"{buildDescription}\"");
            stringBuilder.AppendLine("\t\"buildoutput\"	\".\\output\\\"");
            stringBuilder.AppendLine("\t\"contentroot\"	\".\\content\\\"");
            stringBuilder.AppendLine($"\t\"setlive\"	\"{setBranchLive}\"");
            stringBuilder.AppendLine("\t\"preview\"	\"0\"");
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

            string path = Path.Combine(outputPath, $"app_build_{appId}.vdf");
            File.WriteAllText(path, stringBuilder.ToString());
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
            stringBuilder.AppendLine("\t\t\"LocalPath\" \"*\"");
            stringBuilder.AppendLine("\t\t\"DepotPath\" \".\"");
            stringBuilder.AppendLine("\t\t\"recursive\" \"1\"");
            stringBuilder.AppendLine("\t}");
            stringBuilder.AppendLine($"\t\"FileExclusion\"   \"*.pdb\"");
            stringBuilder.AppendLine("}");

            File.WriteAllText(depotPath, stringBuilder.ToString());

            return depotFileName;
        }

        public static void PushToSteam(string sdkPath, string appVDFPath, string username, string password)
        {
#if UNITY_EDITOR_WIN
            string pathToCmd = Path.Combine(sdkPath, "tools/ContentBuilder/builder/steamcmd.exe");
#else
            string pathToCmd = Path.Combine(sdkPath, "tools/ContentBuilder/builder_osx/steamcmd.sh");
#endif
            string arguments = $"+login {username} {password} +run_app_build_http {appVDFPath} +quit";

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

            Process myProcess = new Process
            {
                StartInfo = startInfo
            };

            UnityEngine.Debug.LogFormat("{0} {1}", pathToCmd, arguments);

            myProcess.Start();
            string output = myProcess.StandardOutput.ReadToEnd();
            UnityEngine.Debug.Log(output);
            myProcess.WaitForExit();
        }

    }
}
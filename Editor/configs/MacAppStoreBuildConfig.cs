using UnityEditor;
#if UNITY_STANDALONE_OSX
using UnityEditor.iOS.Xcode;
#endif
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

namespace Loju.Build
{
    /**
    * https://docs.unity3d.com/Manual/HOWTO-PortToAppleMacStore.html
    **/
    public class MacAppStoreBuildConfig : BuildConfig
    {

        private const string kDefaultEntitlements = "<?xml version=\"1.0\" encoding=\"UTF - 8\"?>\n<!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\" >\n<plist version = \"1.0\">\n<dict>\n<key>com.apple.security.app-sandbox</key><true/>\n</dict>\n</plist>";

        public string playerIconsPath = "Assets/UnityPlayer.iconset";
        public string provisioningProfileApplication = "3rd Party Mac Developer Application: DEVELOPER NAME";
        public string provisioningProfileInstaller = "3rd Party Mac Developer Installer: DEVELOPER NAME";

        public MacAppStoreBuildConfig(BuildType type, string platformName, string appendToPath = null, BuildCompilationDefines defines = null) : base(BuildTarget.StandaloneOSX, type, platformName, appendToPath, defines)
        {

        }

        public override void OnPreBuild(ref BuildOptions options)
        {
            base.OnPreBuild(ref options);

            if (base.type == BuildType.Debug) PlayerSettings.useMacAppStoreValidation = false;
            else PlayerSettings.useMacAppStoreValidation = true;
        }

        public override void OnPostBuild(string pathToBuild)
        {
            if (base.type == BuildType.Debug) return;

#if UNITY_STANDALONE_OSX
            string fileName = Path.GetFileName(pathToBuild);
            string applicationPath = Path.GetDirectoryName(pathToBuild);

            // create entitlements
            string entitlementsName = string.Concat(PlayerSettings.productName, ".entitlements");
            string entitlementsPath = Path.Combine(applicationPath, entitlementsName);

            File.WriteAllText(entitlementsPath, kDefaultEntitlements);

            // copy player icons
            if (!string.IsNullOrEmpty(playerIconsPath))
            {
                File.Copy(playerIconsPath, Path.Combine(pathToBuild, "Contents/Resources/PlayerIcon.icns"), true);
            }

            // run terminal commands
            ProcessStartInfo startInfo = new ProcessStartInfo("/bin/bash");
            startInfo.WorkingDirectory = applicationPath;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardOutput = true;

            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();

            // change permissions
            string cmdCHMOD = string.Format("chmod -R a+xr {0}", fileName);
            UnityEngine.Debug.Log(cmdCHMOD);

            process.StandardInput.WriteLine(cmdCHMOD);

            // Code sign plugins and app
            List<string> pluginPaths = new List<string>();
            FindPlugins(pathToBuild, fileName, pluginPaths);

            int i = 0, l = pluginPaths.Count;
            for (; i < l; ++i) PerformCodeSign(process, entitlementsName, pluginPaths[i], provisioningProfileApplication);

            PerformCodeSign(process, entitlementsName, fileName, provisioningProfileApplication);

            // build package
            string cmdBuild = string.Format("productbuild --component {0} /Applications --sign '{2}' {1}.pkg", fileName, PlayerSettings.productName, provisioningProfileInstaller);
            UnityEngine.Debug.Log(cmdBuild);

            process.StandardInput.WriteLine(cmdBuild);

            process.StandardInput.WriteLine("exit");
            process.StandardInput.Flush();

            UnityEngine.Debug.Log(process.StandardOutput.ReadToEnd());

            process.WaitForExit();
#endif
        }

        private static void FindPlugins(string pathToBuild, string fileName, List<string> results)
        {
            string pluginsPath = Path.Combine(pathToBuild, "Contents/Plugins/");
            string[] plugins = Directory.GetFiles(pluginsPath);
            int i = 0, l = plugins.Length;
            for (; i < l; ++i)
            {
                string path = plugins[i].Replace(pathToBuild, fileName);
                results.Add(path);
            }

            plugins = Directory.GetDirectories(pluginsPath, "*.bundle");
            l = plugins.Length;
            for (i = 0; i < l; ++i)
            {
                string path = plugins[i].Replace(pathToBuild, fileName);
                results.Add(path);
            }
        }

        private static void PerformCodeSign(Process process, string entitlementsName, string fileName, string provisioningProfileApplication)
        {
            string cmdCodeSign = string.Format("codesign -o runtime -f --deep -s '{2}' --entitlements {0} {1}", entitlementsName, fileName, provisioningProfileApplication);
            UnityEngine.Debug.Log(cmdCodeSign);

            process.StandardInput.WriteLine(cmdCodeSign);
        }

        public static void CreateMacIconSet(string iconsPath)
        {
            string workingDirectory = Directory.GetParent(iconsPath).ToString();

            // run terminal commands
            ProcessStartInfo startInfo = new ProcessStartInfo("/bin/bash");
            startInfo.WorkingDirectory = workingDirectory;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardOutput = true;

            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();

            process.StandardInput.WriteLine(string.Format("iconutil -c icns \"{0}\"", iconsPath));
            process.StandardInput.WriteLine("exit");
            process.StandardInput.Flush();

            process.WaitForExit();

            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/MacOS/Create Icon Set")]
        private static void MenuCreateIconSet()
        {
            string iconsPath = EditorUtility.OpenFolderPanel("Select Icon Set Folder", "", "UnityPlayer.iconset");
            if (!string.IsNullOrEmpty(iconsPath)) CreateMacIconSet(iconsPath);
        }

    }
}
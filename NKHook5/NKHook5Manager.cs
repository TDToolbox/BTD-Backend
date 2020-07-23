using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using BTD_Backend.Web;

namespace BTD_Backend.NKHook5
{
    /// <summary>
    /// Contains methods relating to NKHook5 management
    /// </summary>
    public class NKHook5Manager
    {
        public static string nkhDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\NKHook5";
        public static string nkhEXE = nkhDir + "\\NKHook5-Injector.exe";
        public static string gitURL = "https://api.github.com/repos/DisabledMallis/NKHook5/releases";

        /// <summary>
        /// Check if NKHook EXE exists
        /// </summary>
        /// <returns>Whether or not the NKHook exe exists</returns>
        public static bool DoesNkhExist() => File.Exists(nkhEXE);

        /// <summary>
        /// Open NKHook directory
        /// </summary>
        public static void OpenNkhDir()
        {
            if (!Directory.Exists(nkhDir))
            {
                Log.Output("Error! NKHook is not installed");
                return;
            }

            Process.Start(nkhDir);
        }

        /// <summary>
        /// Launch the NKHook exe
        /// </summary>
        public static void LaunchNKH()
        {
            if (!DoesNkhExist())
            {
                Log.Output("Unable to find NKHook5-Injector.exe. Failed to launch...");
                return;
            }
            Process.Start(nkhEXE);
        }

        /// <summary>
        /// Delete all of the NKHook5 files
        /// </summary>
        public static void UninstallNKHook()
        {
            if (!Directory.Exists(nkhDir))
                return;

            Directory.Delete(nkhDir, true);
        }

        /// <summary>
        /// Download the latest NKHook from the github releases section
        /// </summary>
        public static void DownloadNKH()
        {
            Log.Output("Downloading latest NKHook5...");

            string git_Text = WebHandler.ReadText_FromURL(gitURL);
            if (!Guard.IsStringValid(git_Text))
            {
                Log.Output("Failed to read release info for NKHook5");
                return;
            }

            BgThread.AddToQueue(new Thread(() =>
            {
                var gitApi = GitApi.FromJson(git_Text);
                foreach (var a in gitApi)
                {
                    foreach (var b in a.Assets)
                    {
                        string link = b.BrowserDownloadUrl.ToString();
                        if (!Guard.IsStringValid(link))
                            continue;

                        WebHandler.DownloadFile(link, nkhDir);
                    }
                }
                Log.Output("NKHook5 successfully downloaded!");
            }));
        }
    }
}

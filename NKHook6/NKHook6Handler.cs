using BTD_Backend.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTD_Backend.NKHook6
{
    public class NKHook6Handler
    {
        private static readonly string melonLoaderApiURL = "";
        public static void HandleUpdates(string downloadDir, string melonModHandler_path)
        {
            UpdateHandler update = new UpdateHandler()
            {
                GitApiReleasesURL = melonLoaderApiURL,
                ProjectExePath = melonModHandler_path,
                InstallDirectory = downloadDir,
                ProjectName = "NKHook6",
                UpdatedZipName = "NKHook6.zip"
            };
            update.DeleteUpdater();
            update.HandleUpdates(false, false, true);
        }

        private string GetGitText() => WebHandler.ReadText_FromURL(melonLoaderApiURL);

        private bool IsUpdate(string melonModHandler_path, string aquiredGitText)
        {
            if (!File.Exists(melonModHandler_path))
                return true;

            return UpdateHandler.IsUpdate(melonModHandler_path, "MelonLoader", aquiredGitText);
        }
    }
}

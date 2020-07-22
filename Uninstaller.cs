using BTD_Backend.Persistence;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTD_Backend
{
    /// <summary>
    /// Purpose is for completely uninstalling the program using the library
    /// </summary>
    public class Uninstaller
    {
        public static void Uninstall(bool deleteUserFiles = false)
        {
            if (deleteUserFiles)
            {
                if (Directory.Exists(UserData.MainSettingsDir))
                    Directory.Delete(UserData.MainSettingsDir);
            }
        }

        private void LaunchUninstaller()
        {

        }
    }
}

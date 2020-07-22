using BTD_Backend.IO;
using BTD_Backend.Persistence;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public static void UninstallMainProgram(bool deleteUserFiles = false)
        {
            if (deleteUserFiles)
            {
                if (Directory.Exists(UserData.MainSettingsDir))
                    Directory.Delete(UserData.MainSettingsDir, true);
            }

            Windows.DeleteDirCMD(Environment.CurrentDirectory);

            /*var filename = new FileInfo(UserData.MainProgramExePath);
            if (Windows.IsProgramRunning(filename, out Process proc))
                Windows.KillProcess(filename);*/
        }
    }
}

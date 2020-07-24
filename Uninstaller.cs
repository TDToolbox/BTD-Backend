﻿using BTD_Backend.IO;
using BTD_Backend.Persistence;
using System;
using System.Diagnostics;
using System.IO;

namespace BTD_Backend
{
    /// <summary>
    /// Purpose is for completely uninstalling the program using the library
    /// </summary>
    public class Uninstaller
    {
        /// <summary>
        /// Uninstall the main/executing program that is using BTD Backend
        /// </summary>
        /// <param name="deleteUserFiles"></param>
        public static void UninstallMainProgram(bool deleteUserFiles = false)
        {
            if (deleteUserFiles)
            {
                if (Directory.Exists(UserData.MainSettingsDir))
                    Directory.Delete(UserData.MainSettingsDir, true);
            }

            Windows.DeleteDirCMD(Environment.CurrentDirectory);

            var filename = new FileInfo(UserData.MainProgramExePath);
            if (Windows.IsProgramRunning(filename, out Process proc))
                Windows.KillProcess(filename);
        }
    }
}

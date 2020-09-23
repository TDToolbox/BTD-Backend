﻿using MelonLoader;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BTD_Backend.Web;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BTD_Backend.NKHook6
{
    public class MelonModHandling
    {
        private static readonly string melonLoaderApiURL = "https://api.github.com/repos/HerpDerpinstine/MelonLoader/releases";
        public static void HandleUpdates(string downloadDir, string melonModHandler_path)
        {
            UpdateHandler update = new UpdateHandler()
            {
                GitApiReleasesURL = melonLoaderApiURL,
                ProjectExePath = melonModHandler_path,
                InstallDirectory = downloadDir,
                ProjectName = "MelonLoader",
                UpdatedZipName = "MelonLoader.zip",
                GitFileIndexsToDownload = new List<int>() { 1 }
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

        /// <summary>
        /// Get the Assembly of the mod that is calling NKHook6 code
        /// </summary>
        /// <returns></returns>
        public static System.Reflection.Assembly GetCallingMod() => GetMod(1);

        /// <summary>
        /// Get Assembly of mod at index
        /// </summary>
        /// <param name="index">StackTrace index of the mod you want to get Asm for.</param>
        /// <returns></returns>
        public static System.Reflection.Assembly GetMod(int index)
        {
            StackFrame[] frames = new StackTrace().GetFrames();
            var asmNames = (from f in frames
                            select f.GetMethod().ReflectedType.Assembly
                                     ).Distinct().ToList();

            return asmNames[index];
        }

        /// <summary>
        /// Get the MelonInfo of the mod that is calling NKHook6 code
        /// </summary>
        /// <returns></returns>
        public static MelonInfoAttribute GetCallingModInfo() => GetModInfo(GetCallingMod());

        /// <summary>
        /// Get the MelonInfo for the mod located at "index" in the StackTrace
        /// </summary>
        /// <param name="index">Location of mod in StackTrace</param>
        /// <returns></returns>
        public static MelonInfoAttribute GetModInfo(int index) => GetModInfo(GetMod(index));

        /// <summary>
        /// Get the MelonInfo for the mod located at "index" in the StackTrace
        /// </summary>
        /// <param name="filePath">FilePath to the file you want to get MelonModInfo from</param>
        /// <returns></returns>
        public static MelonInfoAttribute GetModInfo(string filePath) =>
            GetModInfo(System.Reflection.Assembly.LoadFrom(filePath));

        /// <summary>
        /// Get the MelonInfo of the mod with the provided Assembly
        /// </summary>
        /// <param name="modAssembly">Assembly of the mod you want to get MelonInfo for</param>
        /// <returns></returns>
        public static MelonInfoAttribute GetModInfo(System.Reflection.Assembly modAssembly)
        {
            MelonInfoAttribute callingMod = null;

            var cust = MelonInfoAttribute.GetCustomAttributes(modAssembly);
            foreach (var item in cust)
            {
                if (item is MelonInfoAttribute)
                    callingMod = (MelonInfoAttribute)item;
            }

            return callingMod;
        }

        /// <summary>
        /// Return whether or not the mod at stacktrace "index" is a valid melon mod
        /// </summary>
        /// <param name="index">Stacktrace index of mod.</param>
        /// <returns></returns>
        public static bool IsValidMelonMod(int index) => GetModInfo(index) != null;

        /// <summary>
        /// Return whether or not the mod at "filePath" is a valid melon mod
        /// </summary>
        /// <param name="filePath">filePath of mod you want MelonMod info for</param>
        /// <returns></returns>
        public static bool IsValidMelonMod(string filePath) =>
            GetModInfo(System.Reflection.Assembly.LoadFrom(filePath)) != null;

        /// <summary>
        /// Return whether or not the mod Assembly for your mod is a valid melon mod
        /// </summary>
        /// <param name="modAssembly">Assembly of the mod you want MelonModInfo for</param>
        /// <returns></returns>
        public static bool IsValidMelonMod(System.Reflection.Assembly modAssembly) => GetModInfo(modAssembly) != null;
    }
}

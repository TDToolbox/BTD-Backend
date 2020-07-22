﻿using BTD_Backend.IO;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace BTD_Backend.Game
{
    /// <summary>
    /// Methods relating to Steam and Steam game info. Made by Danny Boi
    /// </summary>
    public class SteamUtils
    {
        private const UInt64 BTD5AppID = 306020;
        private const string BTD5Name = "BloonsTD5";
        
        private const UInt64 BTDBAppID = 444640;
        private const string BTDBName = "Bloons TD Battles";
        
        private const UInt64 BMCAppID = 1252780;
        private const string BMCName = "Bloons Monkey City";
        
        private const UInt64 BTD6AppID = 960090;
        private const string BTD6Name = "BloonsTD6";
        
        private const UInt64 BTDATAppID = 979060;
        private const string BTDATName = "Bloons Adventure Time TD";
        
        private const UInt64 NKArchiveAppID = 1275350;
        private const string NKArchiveName = "Ninja Kiwi Archive";

        private static Dictionary<UInt64, string> steamGames = new Dictionary<UInt64, string>
        {{BTD5AppID, BTD5Name}, {BTDBAppID, BTDBName}, {BMCAppID, BMCName}, {BTD6AppID, BTD6Name},
            {BTDATAppID, BTDATName}, {NKArchiveAppID, NKArchiveName}};

        private static Dictionary<GameType, UInt64> steamGames_appID_fromGame = new Dictionary<GameType, UInt64>
        {{GameType.BTD5, BTD5AppID}, {GameType.BTDB, BTDBAppID}, {GameType.BMC, BMCAppID}, {GameType.BTD6, BTD6AppID},
            {GameType.BTDAT, BTDATAppID}, {GameType.NKArchive, NKArchiveAppID}};

        /// <summary>
        /// For processing strings so they are in the correct format for this class
        /// </summary>
        private class Utils
        {
            // Takes any quotation marks out of a string.
            public static string StripQuotes(string str) => str.Replace("\"", "");

            // Convert Unix path to Windows path
            public static string UnixToWindowsPath(string UnixPath) =>
                UnixPath.Replace("/", "\\").Replace("\\\\", "\\");
        }

        /// <summary>
        /// Check if any of the game is running
        /// </summary>
        /// <param name="game">Which game to get the ID for</param>
        /// <returns>Whether or not the game is running</returns>
        public static bool IsGameRunning(GameType game) => IsGameRunning(steamGames_appID_fromGame[game]);

        /// <summary>
        /// Check if any of the game is running
        /// </summary>
        /// <param name="appid">The steam appID for the game you want to check</param>
        /// <returns>Whether or not the game is running</returns>
        public static bool IsGameRunning(UInt64 appid)
        {
            int isGameRunning = (int)Registry.GetValue(Registry.CurrentUser +
                "\\Software\\Valve\\Steam\\Apps\\" + appid, "Running", null);
            if (isGameRunning == 1) // Cant type true because its of type System.Bool.
                return true;

            return false;
        }

        /// <summary>
        /// Use the steam game's app ID to check if the game is installed
        /// </summary>
        /// <param name="game">The game you are checking</param>
        /// <returns>true or false, whether or not the game is installed</returns>
        public static bool IsGameInstalled(GameType game) => IsGameInstalled(steamGames_appID_fromGame[game]);

        /// <summary>
        /// Use the steam game's app ID to check if the game is installed
        /// </summary>
        /// <param name="appid">Steam game's app ID</param>
        /// <returns>true or false, whether or not the game is installed</returns>
        public static bool IsGameInstalled(UInt64 appid)
        {
            int isGameInstalled = (int)Registry.GetValue(Registry.CurrentUser +
                "\\Software\\Valve\\Steam\\Apps\\" + appid, "Installed", null);
            if (isGameInstalled != 1)
                return false;

            return true;
        }

        /// <summary>
        /// Get steam directory from registry
        /// </summary>
        /// <returns>returns steam directory or null</returns>
        public static string GetSteamDir() => (string)Registry.GetValue(Registry.CurrentUser +
                "\\Software\\Valve\\Steam", "SteamPath", null);

        /// <summary>
        /// Check if steam is installed by checking if it's game directory exists
        /// </summary>
        /// <returns>true or false, whether or not steam directory exists</returns>
        public static bool IsSteamInstalled() => GetSteamDir() != null;

        /// <summary>
        /// Get game directory from steam ID
        /// </summary>
        /// <param name="appid">steam ID for the game you want the directory for</param>
        /// <returns>Game directory for game</returns>
        public static string GetGameDir(UInt64 appid) => GetGameDir(appid, steamGames[appid]);

        /// <summary>
        /// Get game directory from Game Enum
        /// </summary>
        /// <param name="game">Game to get directory for</param>
        /// <returns>Game directory for game</returns>
        public static string GetGameDir(GameType gameType) => GetGameDir(steamGames_appID_fromGame[gameType]);

        /// <summary>
        /// Get game directory from steam app ID and game name
        /// </summary>
        /// <param name="appid">steam ID for the game you want the directory for</param>
        /// <param name="gameName">Name of the game you want the directory for</param>
        /// <returns>Game directory for game</returns>
        public static string GetGameDir(UInt64 appid, string gameName)
        {
            string steamDir = GetSteamDir();
            if (steamDir == null)
            {
                Log.Output("Failed to find steam in registry!");
                return null;
            }

            if (!IsGameInstalled(appid))
            {
                Log.Output(gameName + " is not installed!");
                return null;
            }

            //
            // Get game Directory...
            string configFileDir = steamDir + "\\steamapps\\libraryfolders.vdf";
            List<string> SteamLibDirs = new List<string>();
            SteamLibDirs.Add(Utils.UnixToWindowsPath(steamDir)); // This steam Directory is always here.
            string[] configFile = File.ReadAllLines(configFileDir);
            for (int i = 0; i < configFile.Length; i++)
            {
                // To Example lines are
                // 	"ContentStatsID"		"-4535501642230800231"
                // "1"     "C:\\SteamLibrary"
                // So, we scan for the items in quotes, if the first one is numeric, 
                // then the second one will be a steam library.
                Regex reg = new Regex("\".*?\"");
                MatchCollection matches = reg.Matches(configFile[i]);
                for (int match = 0; match < matches.Count; match++)
                {
                    if (match == 1)
                        SteamLibDirs.Add(Utils.UnixToWindowsPath(Utils.StripQuotes(matches[match].Value.ToString())));

                    if (match == 0)
                    {
                        if (!int.TryParse(Utils.StripQuotes(matches[match].Value.ToString()), out int n))
                            break;
                    }
                }
            }

            for (int i = 0; i < SteamLibDirs.Count; i++)
            {
                string GameFolder = (SteamLibDirs[i] + "\\steamapps\\common\\" + gameName);
                if (Directory.Exists(GameFolder))
                {
                    Log.Output("Found " + gameName + " directory at: " + GameFolder);
                    return GameFolder;
                }
            }

            Log.Output(gameName + "'s Directory not found!");
            return null;
        }

        /// <summary>
        /// Use steam validator to validate the game
        /// </summary>
        /// <param name="gameType"></param>
        public static void ValidateGame(GameType gameType)
        {
            var utils = new SteamUtils();
            var args = new SteamUtilsEventArgs();
            args.Game = gameType;

            if (!IsGameInstalled(gameType))
            {
                Log.Output("Failed to validate " + gameType + ". Failed to find game directory through steam.");
                utils.OnFailedToValidateGame(args);
                return;
            }

            if (IsGameRunning(gameType))
                Windows.TerminateProcess(GameInfo.GetGame(gameType).ProcName);

            var appID = steamGames_appID_fromGame[gameType];
            Process.Start("steam://validate/" + appID);


            
            //utils.OnGameFinishedValidating(args);
        }


        #region Events
        public static event EventHandler<SteamUtilsEventArgs> GameFinishedValidating;
        public static event EventHandler<SteamUtilsEventArgs> FailedToValidateGame;

        public class SteamUtilsEventArgs : EventArgs
        {
            public GameType Game { get; set; }
        }

        public void OnGameFinishedValidating(SteamUtilsEventArgs e)
        {
            EventHandler<SteamUtilsEventArgs> handler = GameFinishedValidating;
            if (handler != null)
                handler(this, e);
        }

        public void OnFailedToValidateGame(SteamUtilsEventArgs e)
        {
            EventHandler<SteamUtilsEventArgs> handler = FailedToValidateGame;
            if (handler != null)
                handler(this, e);
        }
        #endregion
    }
}

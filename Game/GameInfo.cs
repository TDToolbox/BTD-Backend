using BTD_Backend.Game;
using System.Collections.Generic;

namespace BTD_Backend.Game
{
    /// <summary>
    /// Contains game specific info for each game. Includes exe and process names, directories, save files, passwords, etc.
    /// </summary>
    public class GameInfo
    {
        #region Properties
        /// <summary>
        /// A singleton list containing all of the games, each with their relevant info
        /// </summary>
        private static List<GameInfo> games;
        public static List<GameInfo> Games
        {
            get 
            {
                if (games == null)
                {
                    GameInfo info = new GameInfo();
                    info.InitializeGameInfo();
                }

                return games; 
            }
        }

        /// <summary>
        /// The GameType for this specific game. Example: GameType.BTD5
        /// </summary>
        public GameType Type { get; set; }

        /// <summary>
        /// The name of the game's exe. Example: "BTD5-Win.exe"
        /// </summary>
        public string EXEName { get; set; }

        /// <summary>
        /// The main directory for the game, same as the one that contains the exe. 
        /// Example: "C:\Program Files (x86)\Steam\steamapps\common\BloonsTD5"
        /// </summary>
        public string GameDir { get; set; }

        /// <summary>
        /// The directory that contains the save files. Example: "C:\Program Files (x86)\Steam\userdata\50223118\306020"
        /// </summary>
        public string SaveDir { get; set; }

        /// <summary>
        /// The name of the game's process, as seen in task manager. 
        /// Example: Bloons TD5 Game
        /// </summary>
        public string ProcName { get; set; }

        /// <summary>
        /// The name of the game's jet file, as seen in it's Assets folder. Example: "BTD5.jet"
        /// </summary>
        public string JetName { get; set; }

        /// <summary>
        /// The password for the game's jet file. Example: "Q%_{6#Px]]"
        /// </summary>
        public string JetPassword { get; set; }
        #endregion


        /// <summary>
        /// Get a specific game from the Games list
        /// </summary>
        /// <param name="type">The game you want to get</param>
        /// <returns></returns>
        public static GameInfo GetGame(GameType type)
        {
            GameInfo result = null;
            foreach (var game in Games)
                if (game.Type == type)
                    result = game;
            return result;
        }

        /// <summary>
        /// Initialize game info for each game in GameType.
        /// </summary>
        private void InitializeGameInfo()
        {
            var btd5 = new GameInfo
            {
                Type = GameType.BTD5,
                EXEName = "BTD5-Win.exe",
                GameDir = SteamUtils.GetGameDir(GameType.BTD5),
                SaveDir = "",
                ProcName = "Bloons TD5 Game",
                JetName = "BTD5.jet",
                JetPassword = "Q%_{6#Px]]"
            };

            var btdb = new GameInfo
            {
                Type = GameType.BTDB,
                EXEName = "Battles-Win.exe",
                GameDir = SteamUtils.GetGameDir(GameType.BTDB),
                SaveDir = "",
                ProcName = "Bloons TD Battles",
                JetName = "data.jet",
                JetPassword = ""
            };

            var bmc = new GameInfo
            {
                Type = GameType.BMC,
                EXEName = "MonkeyCity-Win.exe",
                GameDir = SteamUtils.GetGameDir(GameType.BMC),
                SaveDir = "",
                ProcName = "Bloons Monkey City Game (32 bit)",
                JetName = "data.jet",
                JetPassword = "Q%_{6#Px]]"
            };

            var btd6 = new GameInfo
            {
                Type = GameType.BTD6,
                EXEName = "BloonsTD6.exe",
                GameDir = SteamUtils.GetGameDir(GameType.BTD6),
                SaveDir = "",
                ProcName = "BloonsTD6.exe",
                JetName = "",
                JetPassword = ""
            };

            var btdat = new GameInfo
            {
                Type = GameType.BTDAT,
                EXEName = "btdadventuretime.exe",
                GameDir = SteamUtils.GetGameDir(GameType.BTDAT),
                SaveDir = "",
                ProcName = "btdadventuretime.exe",
                JetName = "",
                JetPassword = ""
            };

            var nkArchive = new GameInfo
            {
                Type = GameType.NKArchive,
                EXEName = "Ninja Kiwi Archive.exe",
                GameDir = SteamUtils.GetGameDir(GameType.NKArchive),
                SaveDir = "",
                ProcName = "Ninja Kiwi Archive",
                JetName = "",
                JetPassword = ""
            };

            games = new List<GameInfo>() { btd5, btdb, bmc, btd6, btdat, nkArchive };
        }
    }
}

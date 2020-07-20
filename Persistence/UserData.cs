using BTD_Backend.Game;
using BTD_Backend.IO;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;

namespace BTD_Backend.Persistence
{
    /// <summary>
    /// Manages persistence data related to user, such as game locations, if they are a new user, etc.
    /// This data is not intended to be configurable, therefore this is not the same as config/settings
    /// </summary>
    public class UserData
    {
        private static UserData data;
        public static string MainSettingsDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\BTD Toolbox";

        public static string userdataFilePath = MainSettingsDir + "\\userdata.json";


        /// <summary>
        /// Manages the last known version of the executing program
        /// </summary>
        public string MainProgramVersion { get; set; } = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion;
        
        /// <summary>
        /// Has the user just updated the executing program?
        /// </summary>
        public bool RecentUpdate { get; set; } = true;
        
        /// <summary>
        /// Is this this a new user?
        /// </summary>
        public bool NewUser { get; set; } = true;


        /// <summary>
        /// BTD5 Data
        /// </summary>
        #region BTD5
        public string BTD5Dir { get; set; } = SteamUtils.GetGameDir(GameType.BTD5);
        public string BTD5Version { get; set; } = FileIO.GetFileVersion(SteamUtils.GetGameDir(GameType.BTD5) + "\\BTD5-Win.exe");
        public string BTD5BackupDir { get; set; } = Environment.CurrentDirectory + "\\Backups\\BTD5";
        #endregion

        /// <summary>
        /// BTDB Data
        /// </summary>
        #region BTDB
        public string BTDBDir { get; set; } = SteamUtils.GetGameDir(GameType.BTDB);
        public string BTDBVersion { get; set; } = FileIO.GetFileVersion(SteamUtils.GetGameDir(GameType.BTDB) + "\\Battles-Win.exe");
        public string BTDBBackupDir { get; set; } = Environment.CurrentDirectory + "\\Backups\\BTDB";
        #endregion

        /// <summary>
        /// Bloons Monkey City Data
        /// </summary>
        #region
        public string BMCDir { get; set; } = SteamUtils.GetGameDir(GameType.BMC);
        public string BMCVersion { get; set; } = FileIO.GetFileVersion(SteamUtils.GetGameDir(GameType.BMC) + "\\MonkeyCity-Win.exe");
        public string BMCBackupDir { get; set; } = Environment.CurrentDirectory + "\\Backups\\BMC";
        #endregion

        /// <summary>
        /// BTD6 Data
        /// </summary>
        #region
        public string BTD6Dir { get; set; } = SteamUtils.GetGameDir(GameType.BTD6);
        public string BTD6Version { get; set; } = FileIO.GetFileVersion(SteamUtils.GetGameDir(GameType.BTD6) + "\\MonkeyCity-Win.exe");
        public string BTD6BackupDir { get; set; } = Environment.CurrentDirectory + "\\Backups\\BTD6";
        #endregion

        public static UserData LoadUserData()
        {
            if (!File.Exists(userdataFilePath))
            {
                data = new UserData();
                return data;
            }

            string json = File.ReadAllText(userdataFilePath);


            if (json == "null" || !Guard.IsJsonValid(json))
            {
                Log.Output("Userdata has invalid json, generating a new one.");
                data = new UserData();
                return data;
            }
            data = JsonConvert.DeserializeObject<UserData>(json);
            return data;
        }

        public static void SaveUserData()
        {
            if (data == null)
                LoadUserData();

            string output = JsonConvert.SerializeObject(data, Formatting.Indented);

            StreamWriter serialize = new StreamWriter(userdataFilePath, false);
            serialize.Write(output);
            serialize.Close();
        }
    }
}

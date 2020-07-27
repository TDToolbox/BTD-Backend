using BTD_Backend.Game;
using BTD_Backend.IO;
using BTD_Backend.Save_editing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        public static string MainProgramName;
        public static string MainProgramExePath;
        public static string MainSettingsDir;
        public static string UserDataFilePath;
        
        public static UserData Instance;


        /// <summary>
        /// Manages the last known version of the executing program
        /// </summary>
        public string MainProgramVersion { get; set; } = "";
        
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
        private static GameInfo btd5 = GameInfo.GetGame(GameType.BTD5);
        public string BTD5Dir { get; set; } = btd5.GameDir;
        public string BTD5Version { get; set; } = FileIO.GetFileVersion(btd5.GameDir + "\\" + btd5.EXEName);
        public string BTD5SaveDir { get; set; } = SaveHandler.GetSavePath(GameType.BTD5);
        public string BTD5BackupDir { get; set; } = Environment.CurrentDirectory + "\\Backups\\" + btd5.Type.ToString();
        #endregion

        /// <summary>
        /// BTDB Data
        /// </summary>
        #region BTDB
        private static GameInfo btdb = GameInfo.GetGame(GameType.BTDB);
        public string BTDBDir { get; set; } = btdb.GameDir;
        public string BTDBVersion { get; set; } = FileIO.GetFileVersion(btdb.GameDir + "\\" + btdb.EXEName);
        public string BTDBSaveDir { get; set; } = SaveHandler.GetSavePath(GameType.BTDB);
        public string BTDBBackupDir { get; set; } = Environment.CurrentDirectory + "\\Backups\\" + btdb.Type.ToString();
        #endregion

        /// <summary>
        /// Bloons Monkey City Data
        /// </summary>
        #region
        private static GameInfo bmc = GameInfo.GetGame(GameType.BMC);
        public string BMCDir { get; set; } = bmc.GameDir;
        public string BMCVersion { get; set; } = FileIO.GetFileVersion(bmc.GameDir + "\\" + bmc.EXEName);
        public string BMCSaveDir { get; set; } = SaveHandler.GetSavePath(GameType.BMC);
        public string BMCBackupDir { get; set; } = Environment.CurrentDirectory + "\\Backups\\" + bmc.Type.ToString();
        #endregion

        /// <summary>
        /// BTD6 Data
        /// </summary>
        #region BTD6
        private static GameInfo btd6 = GameInfo.GetGame(GameType.BTD6);
        public string BTD6Dir { get; set; } = btd6.GameDir;
        public string BTD6Version { get; set; } = FileIO.GetFileVersion(btd6.GameDir + "\\" + btd6.EXEName);
        public string BTD6SaveDir { get; set; } = SaveHandler.GetSavePath(GameType.BTD6);
        public string BTD6BackupDir { get; set; } = Environment.CurrentDirectory + "\\Backups\\" + btd6.Type.ToString();
        #endregion

        /// <summary>
        /// Bloons Adventure Time Data
        /// </summary>
        #region BTDAT
        private static GameInfo btdat = GameInfo.GetGame(GameType.BTDAT);
        public string BTDATDir { get; set; } = btdat.GameDir;
        public string BTDATVersion { get; set; } = FileIO.GetFileVersion(btdat.GameDir + "\\" + btdat.EXEName);
        public string BTDATSaveDir { get; set; } = SaveHandler.GetSavePath(GameType.BTDAT);
        public string BTDATBackupDir { get; set; } = Environment.CurrentDirectory + "\\Backups\\" + btdat.Type.ToString();
        #endregion

        /// <summary>
        /// NKArchive Data
        /// </summary>
        #region BTDAT
        private static GameInfo nkArchive = GameInfo.GetGame(GameType.NKArchive);
        public string NKArchiveDir { get; set; } = nkArchive.GameDir;
        public string NKArchiveVersion { get; set; } = FileIO.GetFileVersion(nkArchive.GameDir + "\\" + nkArchive.EXEName);
        public string NKArchiveBackupDir { get; set; } = Environment.CurrentDirectory + "\\Backups\\" + nkArchive.Type.ToString();

        
        public List<string> PreviousProjects { get; set; }
        #endregion


        #region Constructors
        public UserData()
        {
            if (!Guard.IsStringValid(MainProgramName))
                throw new MainProgramNameNotSet();

            if (!Guard.IsStringValid(MainProgramExePath))
                throw new MainProgramExePathNotSet();

            if (!Guard.IsStringValid(MainSettingsDir))
                MainSettingsDir = Environment.CurrentDirectory;

            if (!Guard.IsStringValid(MainProgramVersion))
                MainProgramVersion = FileVersionInfo.GetVersionInfo(MainProgramExePath).FileVersion;

            if (!Directory.Exists(MainSettingsDir))
                Directory.CreateDirectory(MainSettingsDir);

            if (!Guard.IsStringValid(UserDataFilePath))
                UserDataFilePath = MainSettingsDir + "\\userdata.json";

            if (PreviousProjects == null)
                PreviousProjects = new List<string>();
        }

        #endregion

        /// <summary>
        /// Open the main settings directory
        /// </summary>
        public static void OpenSettingsDir()
        {
            if (Instance == null)
                Instance = new UserData();

            if (!Directory.Exists(MainSettingsDir))
                Directory.CreateDirectory(MainSettingsDir);

            Process.Start(MainSettingsDir);
        }

        /// <summary>
        /// Load userdata from file
        /// </summary>
        /// <returns>The loaded userdata</returns>
        public static UserData LoadUserData()
        {
            if (Instance == null)
                Instance = new UserData();

            if (!File.Exists(UserDataFilePath))
                return Instance;

            string json = File.ReadAllText(UserDataFilePath);
            if (json == "null" || !Guard.IsJsonValid(json))
            {
                Log.Output("Userdata has invalid json, generating a new one.");
                return Instance = new UserData();
            }
            
            return Instance = JsonConvert.DeserializeObject<UserData>(json);
        }

        /// <summary>
        /// Save userdata to file
        /// </summary>
        public static void SaveUserData()
        {
            if (Instance == null)
                LoadUserData();

            string output = JsonConvert.SerializeObject(Instance, Formatting.Indented);

            StreamWriter serialize = new StreamWriter(UserDataFilePath, false);
            serialize.Write(output);
            serialize.Close();
        }


        #region Exceptions
        /// <summary>
        /// Throw exception if developer forgot to set the MainProgramName property.
        /// The property needs to be set so userdata can be saved correctly
        /// </summary>
        public class MainProgramNameNotSet : Exception
        {
            public override string Message
            {
                get 
                {
                    return "Did not set the MainProjectName property in UserData. Unable " +
                            "to save user data.";
                }
            }
        }

        /// <summary>
        /// Throw exception if developer forgot to set the MainProgramExePath property.
        /// The property needs to be set so the program can save the version number for the executing program
        /// </summary>
        public class MainProgramExePathNotSet : Exception
        {
            public override string Message
            {
                get
                {
                    return "Did not set the MainProgramExePath property in UserData. Unable " +
                            "to get version number of the executing program";
                }
            }
        }
        #endregion
    }
}

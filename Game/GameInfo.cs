namespace BTD_Backend.Game
{
    public class GameInfo
    {
        #region BTD5 Info
        public static GameType Btd5GameType = GameType.BTD5;
        public static string BTD5ExeName = "BTD5-Win.exe";
        public static string BTD5Dir = SteamUtils.GetGameDir(GameType.BTD5);
        public static string BTD5SaveLoc = "";
        public static string BTD5ProcName = "Bloons TD5 Game";
        public static string BTD5JetName = "BTD5.jet";
        public static string BTD5JetPass = "Q%_{6#Px]]";
        #endregion

        #region BTDB Info
        public static GameType BtdbGameType = GameType.BTDB;
        public static string BTDBExeName = "Battles-Win.exe";
        public static string BTDBDir = SteamUtils.GetGameDir(GameType.BTDB);
        public static string BTDBSaveLoc = "";
        public static string BTDBProcName = "Bloons TD Battles";
        public static string BTDBJetName = "data.jet";
        public static string BTDBJetPass = "";
        #endregion

        #region BMCInfo
        public static GameType BmcGameType = GameType.BMC;
        public static string BMCExeName = "MonkeyCity-Win.exe";
        public static string BMCDir = SteamUtils.GetGameDir(GameType.BMC);
        public static string BMCSaveLoc = "";
        public static string BMCProcName = "Bloons Monkey City Game (32 bit)";
        public static string BMCJetName = "data.jet";
        public static string BMCJetPass = "Q%_{6#Px]]";
        #endregion

        #region BTD6 Info
        public static GameType Btd6GameType = GameType.BTD6;
        public static string BTD6ExeName = "BloonsTD6.exe";
        public static string BTD6Dir = SteamUtils.GetGameDir(GameType.BTD6);
        public static string BTD6ProcName = "BloonsTD6.exe";
        #endregion

        #region BTDAdventureTime Info
        public static GameType BtdATGameType = GameType.BTDAT;
        public static string BTDATExeName = "btdadventuretime.exe";
        public static string BTDATDir = SteamUtils.GetGameDir(GameType.BTDAT);
        public static string BTDATProcName = "btdadventuretime.exe";
        #endregion

    }
}

using BTD_Backend.Game;
using System.IO;
using System.Collections.Generic;

namespace BTD_Backend.Save_editing
{
    public class SaveHandler
    {
        public static string GetSavePath(GameType game)
        {
            List<string> result = new List<string>();
            var appID = SteamUtils.GetGameID(game);
            try
            {
                var dirs = Directory.GetDirectories(SteamUtils.GetSteamDir() + "\\userdata", "*", SearchOption.AllDirectories);
                foreach (var dir in dirs)
                {
                    if (!dir.Contains(appID.ToString()))
                        continue;

                    return dir.Replace("/", "\\");
                }
            }
            catch
            { return ""; }

            return "";
        }
    }
}

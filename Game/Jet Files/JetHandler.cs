using BTD_Backend.IO;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTD_Backend.Game.Jet_Files
{
    public class JetHandler
    {
        /// <summary>
        /// Find the correct password for the Jet, or identify that the password is completely unknown
        /// </summary>
        /// <param name="zip">The custom Zip object whose Archive you want the password for</param>
        /// <returns>The password or null if password not found</returns>
        public static string FindJetPassword(Zip zip) => FindJetPassword(zip.Archive);

        /// <summary>
        /// Find the correct password for the Jet, or identify that the password is completely unknown
        /// </summary>
        /// <param name="filepath">The filepath to the ZipFile you want the password for</param>
        /// <returns>The password or null if password not found</returns>
        public static string FindJetPassword(string filepath)
        {
            if (!File.Exists(filepath))
            {
                Log.Output("Error! Can't get Jet password for \"" + filepath + "\" . That file does not exist!");
                return null;
            }
            return FindJetPassword(new ZipFile(filepath));
        }

        /// <summary>
        /// Find the correct password for the Jet, or identify that the password is completely unknown
        /// </summary>
        /// <param name="jet">The ZipFile you want the password for</param>
        /// <returns>The password or null if password not found</returns>
        public static string FindJetPassword(ZipFile jet)
        {
            var passwords = JetPassword.GetPasswords();
            foreach (var pass in passwords)
            {
                var result = Zip.IsPasswordCorrect(jet, pass);
                if (!result)
                    continue;

                return pass;
            }
            return null;
        }        
    }
}

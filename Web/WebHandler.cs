using System;
using System.IO;
using System.Net;
using System.Threading;

namespace BTD_Backend.Web
{
    public class WebHandler
    {
        /// <summary>
        /// Download file from URL. Needs to be run on a thread if using UI, otherwise the UI will freeze
        /// </summary>
        /// <param name="url">download url</param>
        /// <param name="dest">file destination. Just needs directory, doesn't need file name</param>
        public static void DownloadFile(string url, string dest)
        {
            var client = new WebClient();

            string tempDir = Environment.CurrentDirectory + "\\temp";
            string tempDest = tempDir + "\\file";

            string[] split = url.Split('/');
            string file = split[split.Length - 1];
            dest = dest + "\\" + file;

            if (!Directory.Exists(tempDir))
                Directory.CreateDirectory(tempDir);

            for (int i = 0; i < 150; i++)
            {
                try { client.DownloadFile(url, tempDest); break; }
                catch { }
                Thread.Sleep(50);
            }

            if (File.Exists(dest))
                File.Delete(dest);

            FileInfo f = new FileInfo(dest);
            var destDir = f.FullName.Replace(f.Name, "");
            if (!Directory.Exists(destDir))
                Directory.CreateDirectory(destDir);

            File.Move(tempDest, dest);
            Directory.Delete(tempDir, true);
        }

        /// <summary>
        /// Downloads string of text from the URL. Needs to be run on a thread if using UI, otherwise the UI will freeze
        /// </summary>
        /// <param name="url">URL to get string of text from</param>
        /// <returns>String of text or nothing</returns>
        public static string ReadText_FromURL(string url)
        {
            WebClient client = new WebClient();
            client.Headers.Add("user-agent", " Mozilla/5.0 (Windows NT 6.1; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0");

            string result = "";
            for (int i = 0; i <= 250; i++)
            {
                try
                {
                    result = client.DownloadString(url);
                    if (!Guard.IsStringValid(result))
                        continue;

                    break;
                }
                catch { Thread.Sleep(100); }
            }
            return result;
        }
    }
}

﻿using System;
using System.IO;
using System.Net;

namespace BTD_Backend
{
    public class Guard
    {
        /// <summary>
        /// Checks if string is empty or null. 
        /// </summary>
        /// <param name="text">String to check</param>
        /// <returns>bool whether or not string is valid</returns>
        public static bool IsStringValid(string text)
        {
            if (text != "" && text != null)
                return true;
            return false;
        }

        /// <summary>
        /// Check if url is valid/exists
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public static bool IsWebsiteValid(string url)
        {
            WebClient wc = new WebClient();
            try
            {
                string HTMLSource = wc.DownloadString(url);
                return true;
            }
            catch (Exception) { return false; }
        }

        /// <summary>
        /// Check if text is valid json
        /// </summary>
        /// <param name="text">Text to check</param>
        /// <returns>Whether or not text is valid json</returns>
        public static bool IsJsonValid(string text)
        {
            if (!IsStringValid(text))
                return false;

            try
            {
                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                dynamic result = serializer.DeserializeObject(text);
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// Check if FileInfo file contains valid json
        /// </summary>
        /// <param name="file">FileInfo to check</param>
        /// <returns>Whether or not FileInfo file contains valid json</returns>
        public static bool IsJsonValid(FileInfo file) => IsJsonValid(File.ReadAllText(file.FullName));
    }
}
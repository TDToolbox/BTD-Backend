﻿using System;

namespace BTD_Backend
{
    public class Log
    {
        #region Properties

        /// <summary>
        /// Singleton instance of this class
        /// </summary>
        private static Log instance;
        public static Log Instance
        {
            get
            {
                if (instance == null)
                    instance = new Log();

                return instance;
            }
        }
        #endregion

        #region Events
        public event EventHandler<LogEvents> MessageLogged;

        public class LogEvents : EventArgs
        {
            public string Message { get; set; }
        }

        /// <summary>
        /// When a message has been sent to the Output() function
        /// </summary>
        /// <param name="e">LogEvent args containing the output message</param>
        public void OnMessageLogged(LogEvents e)
        {
            EventHandler<LogEvents> handler = MessageLogged;
            if (handler != null)
                handler(this, e);
        }

        #endregion


        /// <summary>
        /// Passes message to OnMessageLogged for Event Handling.
        /// </summary>
        /// <param name="text">Message to output to user</param>
        public static void Output(string text)
        {
            LogEvents args = new LogEvents();
            args.Message = ">> " + text + Environment.NewLine;
            Instance.OnMessageLogged(args);
        }
    }
}
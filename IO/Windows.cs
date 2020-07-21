using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Cryptography;

namespace BTD_Backend.IO
{
    /// <summary>
    /// Contains Windows related methods, like terminate a process
    /// </summary>
    public class Windows
    {
        /// <summary>
        /// Close process if it's opened
        /// </summary>
        /// <param name="procName">Name of the process to close. It's the name as the exe name without the ".exe"</param>
        public static void TerminateProcess(string procName)
        {
            bool procFound = false;
            var args = new WindowsEventArgs();
            args.ProcessName = procName;

            Log.Output("Attempting to close " + procName);
            var processes = Process.GetProcessesByName(procName);
            foreach (var process in processes)
            {
                process.Kill();
                procFound = true;
                new Windows().OnProcessClosed(args);
            }

            if (!procFound)
                new Windows().OnFailedToCloseProc(args);
        }


        #region Events
        public static event EventHandler<WindowsEventArgs> ProcessClosed;
        public static event EventHandler<WindowsEventArgs> FailedToCloseProc;

        public class WindowsEventArgs : EventArgs
        {
            /// <summary>
            /// Name of process that was closed or attempted to close
            /// </summary>
            public string ProcessName { get; set; }

        }

        /// <summary>
        /// Fire when a process has been closed
        /// </summary>
        /// <param name="e">args containing the name of the process that was closed</param>
        public void OnProcessClosed(WindowsEventArgs e)
        {
            EventHandler<WindowsEventArgs> handler = ProcessClosed;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Fires when a process failed to close
        /// </summary>
        /// <param name="e">args containing the name of the process that failed to close</param>
        public void OnFailedToCloseProc(WindowsEventArgs e)
        {
            EventHandler<WindowsEventArgs> handler = FailedToCloseProc;
            if (handler != null)
                handler(this, e);
        }

        #endregion
    }
}

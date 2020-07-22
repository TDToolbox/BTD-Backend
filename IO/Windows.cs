using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
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
        /// <param name="procID">The ID of the process you are trying to close</param>
        public static void KillProcess(int procID)
        {
            var args = new WindowsEventArgs();
            args.ProcessID = procID;
            var process = GetProcess(procID);

            if (process == null)
            {
                new Windows().OnFailedToCloseProc(args);
                return;
            }

            process.Kill();
            new Windows().OnProcessClosed(args);
        }


        /// <summary>
        /// Used for searching for processes. 
        /// </summary>
        public enum ProcType
        {
            ProcessName,
            WindowTitle
        }

        /// <summary>
        /// Close process if it's opened
        /// </summary>
        /// <param name="name">ProcName or WindowTitle of the process to close. 
        /// ProcName is the same as the exe name without the ".exe". WindowTitle is the title of the window</param>
        /// <param name="type">Which part of the process are you looking for, to find and close process</param>
        public static void KillProcess(string name, ProcType type = ProcType.ProcessName)
        {
            var args = new WindowsEventArgs();
            var process = GetProcess(name, type);

            if (type == ProcType.ProcessName)
                args.ProcessName = name;
            else
                args.ProcessWindowTitle = name;

            if (process == null)
            {
                new Windows().OnFailedToCloseProc(args);
                return;
            }

            process.Kill();
            new Windows().OnProcessClosed(args);
        }

        /// <summary>
        /// Get Process from running processes
        /// </summary>
        /// <param name="procID">The ID of the process you are searching for.</param>
        /// <returns>The process that was found, or null if not found</returns>
        public static Process GetProcess(int procID)
        {
            var processes = Process.GetProcesses();
            foreach (var process in processes)
                if (process.Id == procID)
                    return process;

            return null;
        }

        /// <summary>
        /// Get Process from running processes
        /// </summary>
        /// <param name="type">The part of the process you are searching for, as in ProcessName, ProcessID, WindowTitle.
        /// <param name="name">The name of the process you are searching for.</param>
        /// <returns>The process that was found, or null if not found</returns>
        public static Process GetProcess(string name, ProcType type = ProcType.ProcessName)
        {
            var processes = Process.GetProcesses();
            foreach (var process in processes)
            {
                if (type == ProcType.ProcessName)
                {
                    if (process.ProcessName == name)
                        return process;
                }
                else if (type == ProcType.WindowTitle)
                {
                    if (process.MainWindowTitle == name)
                        return process;
                }
            }
            return null;
        }

        /// <summary>
        /// Check if a process is running
        /// </summary>
        /// <param name="procID">The ID of the process you are searching for.</param>
        /// <param name="process">The process that was found. Returns null if process is not found</param>
        /// <returns>True or false, whether or not the process is running</returns>
        public static bool IsProgramRunning(int procID, out Process process)
        {
            process = GetProcess(procID);
            return process != null;
        }

        /// <summary>
        /// Check if a process is running
        /// </summary>
        /// <param name="type">The part of the process you are searching for. 
        /// By ProcessName, ProcessID, and MainWindowTytle</param>
        /// <param name="name">The text of the process you are searching for.</param>
        /// <param name="process">The process that was found. Returns null if process is not found</param>
        /// <returns>True or false, whether or not the process is running</returns>
        public static bool IsProgramRunning(string name, out Process process, ProcType type = ProcType.ProcessName)
        {
            process = GetProcess(name, type);
            return process != null;
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
            
            /// <summary>
            /// Window Title for the process you are looking for/attempting to close
            /// </summary>
            public string ProcessWindowTitle { get; set; }
            
            /// <summary>
            /// Process ID of the process you are looking for/attempting to close
            /// </summary>
            public int ProcessID { get; set; }

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

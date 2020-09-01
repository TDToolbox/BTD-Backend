using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BTD_Backend.Natives
{
    public class Injector
    {
        //Code from https://github.com/erfg12/memory.dll/blob/master/Memory/memory.cs
        public static void InjectDll(String strDllName, Process procToInject)
        {
            IntPtr bytesout;
            
            try
            {
                var test = procToInject.Modules;
                if (test == null)
                    return;
            }
            catch (Exception e) 
            { 
                if (e.Message.ToLower().Contains("access"))
                {
                    Log.Output("An exception prevented the mods from being injected... \nException: " + e.Message, OutputType.Both);
                    Log.Output("Try re-opening the prgram as Admin", OutputType.Both);
                    return;
                }
                if (e.Message.ToLower().Contains("readprocess") || e.Message.ToLower().Contains("writeprocess"))
                {
                    Log.Output("An exception prevented the mods from being injected... \nException: " + e.Message, OutputType.Both);
                    Log.Output("Your virus protection might be preventing the injection from working. Try adding an exeption to" +
                        " the program, or disable your virus protection while using the program.", OutputType.Both);
                    return;
                }
                Log.Output("An exception prevented the mods from being injected... \nException: " + e.Message, OutputType.Both);
                return;
            }
            foreach (ProcessModule pm in procToInject.Modules)
            {
                if (pm.ModuleName.StartsWith("inject", StringComparison.InvariantCultureIgnoreCase))
                    return;
            }

            if (!procToInject.Responding)
                return;

            IntPtr pHandle = Win32.OpenProcess(0x1F0FFF, true, procToInject.Id);

            int lenWrite = strDllName.Length + 1;
            UIntPtr allocMem = Win32.VirtualAllocEx(pHandle, (UIntPtr)null, (uint)lenWrite, 0x00001000 | 0x00002000, 0x04);

            Win32.WriteProcessMemory(pHandle, allocMem, strDllName, (UIntPtr)lenWrite, out bytesout);
            UIntPtr injector = Win32.GetProcAddress(Win32.GetModuleHandle("kernel32.dll"), "LoadLibraryA");

            if (injector == null)
                return;

            IntPtr hThread = Win32.CreateRemoteThread(pHandle, (IntPtr)null, 0, injector, allocMem, 0, out bytesout);
            if (hThread == null)
                return;

            int Result = Win32.WaitForSingleObject(hThread, 10 * 1000);
            if (Result == 0x00000080L || Result == 0x00000102L)
            {
                if (hThread != null)
                    Win32.CloseHandle(hThread);
                return;
            }
            Win32.VirtualFreeEx(pHandle, allocMem, (UIntPtr)0, 0x8000);

            if (hThread != null)
                Win32.CloseHandle(hThread);

            return;
        }
    }
}

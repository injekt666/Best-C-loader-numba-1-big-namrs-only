using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Management;
using System.IO;
using System.Net;
using System.Globalization;
using System.Resources;
using System.Runtime.InteropServices;
using System.Net.Sockets;
using System.Web;
namespace injector

{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        [DllImport("kernel32")]
        public static extern IntPtr CreateRemoteThread(
         IntPtr hProcess,
         IntPtr lpThreadAttributes,
         uint dwStackSize,
         UIntPtr lpStartAddress, // raw Pointer into remote process
         IntPtr lpParameter,
         uint dwCreationFlags,
         out IntPtr lpThreadId
       );

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(
            UInt32 dwDesiredAccess,
            Int32 bInheritHandle,
            Int32 dwProcessId
            );

        [DllImport("kernel32.dll")]
        public static extern Int32 CloseHandle(
        IntPtr hObject
        );

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern bool VirtualFreeEx(
            IntPtr hProcess,
            IntPtr lpAddress,
            UIntPtr dwSize,
            uint dwFreeType
            );

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
        public static extern UIntPtr GetProcAddress(
            IntPtr hModule,
            string procName
            );

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern IntPtr VirtualAllocEx(
            IntPtr hProcess,
            IntPtr lpAddress,
            uint dwSize,
            uint flAllocationType,
            uint flProtect
            );

        [DllImport("kernel32.dll")]
        static extern bool WriteProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            string lpBuffer,
            UIntPtr nSize,
            out IntPtr lpNumberOfBytesWritten
        );

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(
            string lpModuleName
            );

        [DllImport("kernel32", SetLastError = true, ExactSpelling = true)]
        internal static extern Int32 WaitForSingleObject(
            IntPtr handle,
            Int32 milliseconds
            );

        public Int32 GetProcessId(String proc)
        {
            Process[] ProcList;
            ProcList = Process.GetProcessesByName(proc);
            return ProcList[0].Id;
        }


        public void InjectDLL(IntPtr hProcess, String strDLLName)
        {
            IntPtr bytesout;

            Int32 LenWrite = strDLLName.Length + 1;
            IntPtr AllocMem = (IntPtr)VirtualAllocEx(hProcess, (IntPtr)null, (uint)LenWrite, 0x1000, 0x40);

            WriteProcessMemory(hProcess, AllocMem, strDLLName, (UIntPtr)LenWrite, out bytesout);
            UIntPtr Injector = (UIntPtr)GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");

            if (Injector == null)
            {
                MessageBox.Show(" Injector Error! \n ");
                return;
            }
            IntPtr hThread = (IntPtr)CreateRemoteThread(hProcess, (IntPtr)null, 0, Injector, AllocMem, 0, out bytesout);
            if (hThread == null)
            {
                MessageBox.Show(" hThread [ 1 ] Error! \n ");
                return;
            }
            int Result = WaitForSingleObject(hThread, 10 * 1000);
            if (Result == 0x00000080L || Result == 0x00000102L || Result == 0xFFFFFFFF)
            {
                MessageBox.Show(" hThread [ 2 ] Error! \n ");
                if (hThread != null)
                {
                    CloseHandle(hThread);
                }
                return;
            }
            Thread.Sleep(1000);
            VirtualFreeEx(hProcess, AllocMem, (UIntPtr)0, 0x8000);

            if (hThread != null)
            {
                CloseHandle(hThread);
            }
            return;
        }



        string HWID;
        private void Form1_Load(object sender, EventArgs e)
        {
            string localIP;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }

            label1.Text = "IP Address: " + localIP;
            HWID = System.Security.Principal.WindowsIdentity.GetCurrent().User.Value;
            label2.Text = "HardWare ID: " + HWID;

            purves();
            yourstatus();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            try
            {
                string pprocess = "csgo";

                if(Process.GetProcessesByName(pprocess).Length == 1)
                {
                    try
                    {
                        Int32 process = GetProcessId(pprocess);
                        if(process >= 0)
                        {
                            IntPtr hProcess = (IntPtr)OpenProcess(0x1F0FFF, 1, process);
                            
                            try
                            {
                                if(process == null)
                                {
                                    var button = MessageBoxButtons.OK;
                                    var icon = MessageBoxIcon.Error;
                                    string text = "Purve$ Injector by neplo!";
                                    MessageBox.Show("Injection failed", text, button, icon);
                                    return;
                                }
                                else
                                {
                                    string filename = openFileDialog1.FileName;
                                    var button = MessageBoxButtons.OK;
                                    var icon = MessageBoxIcon.Information;
                                    string text = "Purve$ Injector by neplo!";
                                    InjectDLL(hProcess, filename);
                                    MessageBox.Show("DLL has been injected!", text, button, icon);
                                }
                            }
                            catch(Exception ex)
                            {
                                var button = MessageBoxButtons.OK;
                                var icon = MessageBoxIcon.Error;
                                string text = "Purve$ Injector by neplo!";
                                MessageBox.Show("Injection Error: " + ex, text, button, icon);
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        var button = MessageBoxButtons.OK;
                        var icon = MessageBoxIcon.Error;
                        string text = "Purve$ Injector by neplo!";
                        MessageBox.Show("Injection Error: " + ex, text, button, icon);
                    }
                }
                else
                {
                    var button = MessageBoxButtons.OK;
                    var icon = MessageBoxIcon.Error;
                    string text = "Purve$ Injector by neplo!";
                    MessageBox.Show("Error: Please start CS:GO First", text, button, icon);
                    return;
                }
            }
            catch(Exception ex)
            {
                var button = MessageBoxButtons.OK;
                var icon = MessageBoxIcon.Error;
                string text = "Purve$ Injector by neplo!";
                MessageBox.Show("Injection Error: " + ex, text, button, icon);
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "DLL Files (*.dll)|*.dll|All files (*.*)|*.*";
            string filename;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filename = openFileDialog1.FileName;
                string path = "";
                string result;

                result = Path.GetFileNameWithoutExtension(filename);

                label5.Text = "DLL Name: " + result;
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void purves()
        {
            try
            {
                using(var client = new WebClient())
                {
                    string purves = "https://purves.cc/beta_tools/";
                    using(client.OpenRead(purves))
                    {
                        label3.Text = "Purve$ Status: Offline";
                    }
                }
            }
            catch(WebException webex)
            {
                label3.Text = "Purve$ Status: Online";
            }
        }


        private void yourstatus()
        {
            try
            {
                using (var client = new WebClient())
                {
                    string purves = "https://google.com";
                    using (client.OpenRead(purves))
                    {
                        label4.Text = "Your Status: Online";
                    }
                }
            }
            catch (WebException webex)
            {
                label4.Text = "Your Status: Offline";
            }
        }

        private void antiobs()
        {
            string obs = "obs64";

            if(Process.GetProcessesByName(obs).Length == 1)
            {
                label1.Text = "Hidden due to recording with OBS Studio";
                label2.Text = "Hidden due to recording with OBS Studio";
            }
            else
            {
                string localIP;
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                {
                    socket.Connect("8.8.8.8", 65530);
                    IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                    localIP = endPoint.Address.ToString();
                }

                label1.Text = "IP Address: " + localIP;
                HWID = System.Security.Principal.WindowsIdentity.GetCurrent().User.Value;
                label2.Text = "HardWare ID: " + HWID;
            }
        }

        private void Label4_Click(object sender, EventArgs e)
        {

        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            string csgo = "csgo";

            if(Process.GetProcessesByName(csgo).Length == 1)
            {
                label6.Text = "CS:GO Working: true";
            }
            else
            {
                label6.Text = "CS:GO Working: false";
            }

            label7.Text = "Current Date: " + DateTime.Now.ToLongDateString();
            label8.Text = "Current Time: " + DateTime.Now.ToLongTimeString();
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://purves.cc/beta_tools/u.php?id=2438");
        }
    }
}

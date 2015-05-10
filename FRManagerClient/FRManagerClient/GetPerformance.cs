using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.IO;
using System.Diagnostics;
using System.Management;
using System.Speech.Synthesis;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FRManagerClient
{
    public static class GetPerformance
    {        
        public static JsonDataObject data;
                
        #region Get() - Get the required information and write to a json file
        public static void Get()
        {
            data = new JsonDataObject();
            Identify();
            Query();
            Counters();
            string json = JsonConvert.SerializeObject(data);
            WriteToJsonFile(json);           
        }
        #endregion

        #region Identify() - Unique Identification  of the user
        public static void Identify()
        {
            //Get the current time in UTC
            data.utc = DateTime.UtcNow;            
            
            // Get the Name of HOST
            data.hostName = Dns.GetHostName();

            //Get the current logged in user of the system
            data.loggedInUserName = Environment.UserName;

            //Get the Mac Address
            string macAddresses = string.Empty;
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    macAddresses += string.Join(":", (from addr in nic.GetPhysicalAddress().GetAddressBytes() select addr.ToString("X2")).ToArray());
                }

            }
            data.macAddress = macAddresses;

            // Get the IP
            IPHostEntry host = Dns.GetHostEntry(data.hostName);
            data.ipAddress = string.Join(",", (from ip in host.AddressList where ip.AddressFamily == AddressFamily.InterNetwork select ip.ToString()).ToList());        
        }
        #endregion

        #region Counters() - Initializing and getting data through performance counters
        public static void Counters()
        {
            #region Initialize Counters
            //This will create performance counter object to get the total system up time and initialize it. 
            PerformanceCounter systemUpTime = new PerformanceCounter("System", "System Up Time");
            systemUpTime.NextValue();
            
            //This will create performance counter object to get total percentage processor load and initialize it. 
            PerformanceCounter cpu = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            cpu.NextSample();

            //This will create performance counter object to get the threads and initialize it. 
            PerformanceCounter threads = new PerformanceCounter("System", "Threads");
            threads.NextValue();

            //This will create performance counter object to get the processes and initialize it. 
            PerformanceCounter processes = new PerformanceCounter("System", "Processes");
            processes.NextValue();
            
            //This will create performance counter object to get total available memory in megabytes and initialize it. 
            PerformanceCounter memory = new PerformanceCounter("Memory", "Available MBytes");
            memory.NextValue();       

            //This will create performance counter object to get the percentage disk time and initialize it. 
            PerformanceCounter diskTime = new PerformanceCounter("PhysicalDisk", "% Disk Time", "_Total");
            diskTime.NextValue();

            //This will create performance counter object to get the percentage idle time and initialize it. 
            PerformanceCounter idleTime = new PerformanceCounter("PhysicalDisk", "% Idle Time", "_Total");
            idleTime.NextValue();

            //This will create performance counter object to get the disk bytes/sec and initialize it. 
            PerformanceCounter diskBytes = new PerformanceCounter("PhysicalDisk", "Disk Bytes/sec", "_Total");
            diskBytes.NextValue();

            //This will create performance counter object to get the disk transfers/sec and initialize it. 
            PerformanceCounter diskTransfers = new PerformanceCounter("PhysicalDisk", "Disk Transfers/sec", "_Total");
            diskTransfers.NextValue();

            //This will create performance counter object to get the average disk sec/transfer and initialize it. 
            PerformanceCounter avgDiskSecsTransfers = new PerformanceCounter("PhysicalDisk", "Avg. Disk sec/Transfer", "_Total");
            avgDiskSecsTransfers.NextValue();

            //This will create performance counter object to get the average disk bytes/transfer and initialize it. 
            PerformanceCounter avgDiskBytesTransfers = new PerformanceCounter("PhysicalDisk", "Avg. Disk Bytes/Transfer", "_Total");
            avgDiskBytesTransfers.NextValue();

            //This will create performance counter object to get the average disk queue length and initialize it. 
            PerformanceCounter avgDiskQueueLength = new PerformanceCounter("PhysicalDisk", "Avg. Disk Queue Length", "_Total");
            avgDiskQueueLength.NextValue();

            #endregion

            #region System Up Time
            //Create time span object
            TimeSpan t = TimeSpan.FromSeconds(systemUpTime.NextValue());
            data.upTime = new SystemUpTime
            {
                TotalDays = (int)t.TotalDays,
                Hours = (int)t.Hours,
                Minutes = (int)t.Minutes,
                Seconds = (int)t.Seconds
            };
            #endregion

            #region CPU
            //Get the total percentage processor load
            CounterSample cs1 = cpu.NextSample();
            Thread.Sleep(1000);
            CounterSample cs2 = cpu.NextSample();
            data.percentCpuLoad = (int)CounterSample.Calculate(cs1, cs2);
            WarningCPU(data.percentCpuLoad);
            #endregion

            #region Threads
            //Get the number of threads running
            data.threads = (int)threads.NextValue();
            #endregion

            #region Processes
            //Get the number of processes running
            data.processes = (int)processes.NextValue();
            #endregion

            #region Memory
            //Get the available memory in MegaBytes
            data.availableMemoryInMB = (int)memory.NextValue();
            WarningMemory(data.availableMemoryInMB);
            #endregion

            #region Disk Time
            //Get the percentage disk time
            data.percentDiskTime = (int)diskTime.NextValue();
            #endregion

            #region Disk Idle Time
            //Get the percentage disk idle time
            data.percentIdleTime = (int)idleTime.NextValue();
            #endregion

            #region Disk Throughput (Disk KB/s)
            //Get the rate at bytes are transferred
            //if the rate is high, then good
            data.diskKiloBytesSec = (int)(diskBytes.NextValue() / 1024);
            #endregion

            #region Disk Utilization (Disk Transfers/s)
            //Get the number of reads and writes completed per second
            //if diskTransfersSec > 50 , bottleneck might be developing 
            data.diskTransfersSec = (int)diskTransfers.NextValue();
            #endregion

            #region Disk Failures Check (Avg Disk s/transfer)
            //Get the average time for a transfer
            //if high continuously, retrying requests due to lengthy queing 
            //or disk failure 
            data.avgDiskSecsTransfer = Math.Round(avgDiskSecsTransfers.NextValue(), 3);
            #endregion

            #region Disk Efficiency Check (Avg Disk KB/transfer)
            //Get the size of read and write operations
            //if high, it is efficient
            data.avgDiskKiloBytesTransfer = (int)(avgDiskBytesTransfers.NextValue()/1024);
            #endregion

            #region Disk Bottleneck Check (Avg Disk Queue Length)
            //Get the number fo requests that are queued and waiting, as well as requests in service
            //If more than 2 requests continuously waiting, disk might be bottleneck 
            data.avgDiskQueueLength = Math.Round(avgDiskQueueLength.NextValue(),3);
            #endregion

            #region Dispose Counters
            systemUpTime.Close();
            cpu.Close();
            threads.Close();
            processes.Close();
            memory.Close();
            diskTime.Close();
            idleTime.Close();
            diskBytes.Close();
            diskTransfers.Close();
            avgDiskSecsTransfers.Close();
            avgDiskBytesTransfers.Close();
            avgDiskQueueLength.Close();
            #endregion
        }
        #endregion

        #region Query() - Management Object Query
        public static void Query()
        {
            // Get the Logical Disk Size and free space
            //data.disk = selectQuery("Win32_LogicalDisk", "Size"); 
            
        }        
        #endregion

        #region WarningCPU() - Warning CPU Load
        public static void WarningCPU(int finalCpuCounter)
        {            
            //Warning generator to alert the user when processor usage is higher
            if (finalCpuCounter > 85)
            {                
                String messagecpu = String.Format("Warning your CPU Load is {0} percent", finalCpuCounter);
                Speak.speak(messagecpu, VoiceGender.Female);
            }
        
        }
        #endregion

        #region WarningMemory() - Warning Available Memory
        public static void WarningMemory(int availableMemory)
        {
            //Warning generator to alert the user when available memory is lesser
            if (availableMemory < 200)
            {
                String messagememory = String.Format("Available Memory {0} mega bytes", availableMemory);
                Speak.speak(messagememory, VoiceGender.Female);
            }
        }
        #endregion

       /* public static LinkedList<ManagementQuery> selectQuery(String query, String property)
        #region Function when only the query and property is given
        {
            LinkedList<ManagementQuery> result = new LinkedList<ManagementQuery>();
            //Create a Management Object Searcher object to get the information from Windows Management Instrumentation(WMI)
            ManagementObjectSearcher searchQuery = new ManagementObjectSearcher("SELECT * FROM " + query);

            //Get Management Object one by one
            foreach (ManagementObject mngObj in searchQuery.Get())
            {
                //Name of the current Management Object
                ManagementQuery x = new ManagementQuery();
                x.name = mngObj["Name"].ToString();
                x.propertyName = property;
                x.value = mngObj.Properties[property].Value.ToString();
                result.AddLast(x); 
            }
            //Release the resources consumed by the Management Object Searcher object
            searchQuery.Dispose();
            return result;
        }
        #endregion  */ 

        public static void WriteToJsonFile(String Message)
        {
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\DataLogFile.json", true);
                sw.WriteLine(Message);
                sw.Flush();
                sw.Close();
            }
            catch
            { }
        }
    }
}


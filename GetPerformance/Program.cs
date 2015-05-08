using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Management;
using System.Threading;
using System.Speech.Synthesis;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;

namespace GetPerformance
{
    class Program
    {

        private static SpeechSynthesizer speech = new SpeechSynthesizer();
         
        public static void Main(string[] args)
        {    
            //Initialize the speed of the voice for the Warning !!!
            int speed = 2;

            //Initialize the Mac Address 
            string macAddresses = string.Empty;

            #region Start
            //Speak out the greeting to the user
            speak("Welcome to your Workplace", VoiceGender.Female);
            Thread.Sleep(2);
            speak("Please enter your user name and password", VoiceGender.Female);
            #endregion

            #region Unique Identification
            //Get the current logged in user of the system
            Console.WriteLine(System.Security.Principal.WindowsIdentity.GetCurrent().Name);

            //Get the Mac Address                       
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    macAddresses += string.Join(":", (from addr in nic.GetPhysicalAddress().GetAddressBytes() select addr.ToString("X2")).ToArray());                  
                }
 
            }
            Console.WriteLine(macAddresses);

            // Retrive the Name of HOST
            string hostName = Dns.GetHostName();            
            Console.WriteLine(hostName);  
                           
           // Get the IP
            IPHostEntry host = Dns.GetHostEntry(hostName);
            string myIP = string.Join(",", (from ip in host.AddressList where ip.AddressFamily == AddressFamily.InterNetwork select ip.ToString()).ToList());
            Console.WriteLine("My IP Address is :" + myIP);
            Console.ReadKey();
            
            #endregion

            #region Performance Counters

            //This will create performance counter object to get total percentage processor load and initialize it. 
            PerformanceCounter cpu = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            cpu.NextSample();

            //This will create performance counter object to get total available memory in megabytes and initialize it. 
            PerformanceCounter memory = new PerformanceCounter("Memory", "Available MBytes");
            memory.NextValue();

            //This will create performance counter object to get the total system up time and initialize it. 
            PerformanceCounter systemUpTime = new PerformanceCounter("System", "System Up Time");
            systemUpTime.NextValue();

            //This will create performance counter object to get the processes and initialize it. 
            PerformanceCounter processes = new PerformanceCounter("System", "Processes");
            processes.NextValue();

            //This will create performance counter object to get the threads and initialize it. 
            PerformanceCounter threads = new PerformanceCounter("System", "Threads");
            threads.NextValue(); 

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

            #region Program Logic 1
            //Create time span object
            TimeSpan t = TimeSpan.FromSeconds(systemUpTime.NextValue());
            String uptime = String.Format("The current system up time is {0} days {1} hours {2} minutes and {3} seconds",
                    (int)t.TotalDays,
                    (int)t.Hours,
                    (int)t.Minutes,
                    (int)t.Seconds                    
                    );
            
            Console.WriteLine(uptime);
            //Speak out current system up time
            speak(uptime, VoiceGender.Female,2);

            // Get the Logical Disk size and free space
            selectQuery("Win32_LogicalDisk", "Size");
            selectQuery("Win32_LogicalDisk", "FreeSpace");
            selectQuery("Win32_ComputerSystem", "TotalPhysicalMemory");
            
            #endregion

            #region Program Logic 2
            while (true)
            {
                //Get the total percentage processor load
                CounterSample cs1 = cpu.NextSample();
                Thread.Sleep(1000);
                CounterSample cs2 = cpu.NextSample();
                int finalCpuCounter = (int)CounterSample.Calculate(cs1,cs2);

                Console.WriteLine("CPU Load : " + finalCpuCounter + "%");

                //Get the available memory in megabytes
                int availableMemory = (int)memory.NextValue();
                
                Console.WriteLine("Available Memory : " + availableMemory + " MB");

                int noProcesses = (int)processes.NextValue();
                int noThreads = (int)threads.NextValue();
                int percentDiskTime = (int)diskTime.NextValue();
                int percentIdleTime = (int)idleTime.NextValue();
                int diskKiloBytesSec = (int)(diskBytes.NextValue() / 1024);
                int diskTransfersSec = (int)diskTransfers.NextValue();
                double diskFailureCheckAvgDiskSecsTransfers = Math.Round(avgDiskSecsTransfers.NextValue(), 3);
                int diskEfficiencyCheckAvgDiskKiloBytesTransfer = (int)(avgDiskBytesTransfers.NextValue() / 1024);
                double diskBottleneckCheckAvgDiskQueueLength = Math.Round(avgDiskQueueLength.NextValue(), 3); 



                Console.WriteLine("Processes : " + noProcesses);
                Console.WriteLine("Threads : " + noThreads);
                Console.WriteLine("Disk Time: " + percentDiskTime + "%");
                Console.WriteLine("Idle Time : " + percentIdleTime + "%");
                Console.WriteLine("Throughput - Disk Kilo Bytes/Sec  : " + diskKiloBytesSec + " KB");
                Console.WriteLine("Disk Utilization - Disk Transfers/Sec  : " + diskTransfersSec + " Transfers/s");
                Console.WriteLine("Disk Failure Check - Avg Disk Secs/Transfers  : " + diskFailureCheckAvgDiskSecsTransfers + " sec/Transfer");
                Console.WriteLine("Disk Efficiency Check - Avg Disk Kilo Bytes Transfer : " + diskEfficiencyCheckAvgDiskKiloBytesTransfer + " KB/Transfer");
                Console.WriteLine("Disk Bottleneck Check - Avg Disk Queue Length : " + diskBottleneckCheckAvgDiskQueueLength + " requests");
                Console.WriteLine("Available Memory : " + availableMemory + " MB");

                //Warning generator to alert the user when processor usage is higher
                if (finalCpuCounter > 85)
                {
                    if (finalCpuCounter == 100)
                    {
                        if (speed < 5)
                        {
                            speed++;
                        }
                        String messagecpu = String.Format("Warning your CPU Load is 100 percent");
                        speak(messagecpu, VoiceGender.Female, speed);
                    }

                    else
                    {
                        String messagecpu = String.Format("CPU Load {0} percent", finalCpuCounter);
                        speak(messagecpu,VoiceGender.Female,2);
                    }
                }

                //Warning generator to alert the user when available memory is lesser
                if (availableMemory < 200)
                {
                    String messagememory = String.Format("Available Memory {0} mega bytes", availableMemory);
                    speak(messagememory, VoiceGender.Female, 2);
                }
                Console.WriteLine();
            }
            #endregion
        }

        #region Speak Functions
        public static void speak(String message)
        {
            speech.Speak(message);        
        }

        public static void speak(String message, VoiceGender voice)
        {
            speech.SelectVoiceByHints(voice);
            speak(message); 
        }
                
        public static void speak(String message, VoiceGender voice, int rate)
        {
            speech.Rate = rate;
            speak(message,voice);            
        }
        #endregion
        
        public static void selectQuery(String query, String currentObj, String property)
        #region Function when only the query,object and property is given
        {
          //Create a Management Object Searcher object to get the information from Windows Management Instrumentation(WMI)
          ManagementObjectSearcher searchQuery = new ManagementObjectSearcher("SELECT * FROM " + query);
           
          //Get Management Object one by one
          foreach (ManagementObject mngObj in searchQuery.Get())
          {
            if (mngObj["Name"].ToString() == currentObj)
            {
              //Name of the current Management Object
              Console.WriteLine(mngObj["Name"].ToString());
              Console.WriteLine("----------------------------------------------------------------");

              //Display the value of the property
              Console.WriteLine(property +" : " + mngObj.Properties[property].Value);       
            }
            break;           
          }
          
          //Release the resources consumed by the Management Object Searcher object
          searchQuery.Dispose();
        } 
        #endregion 

        public static void selectQuery(String query, String property)
        #region Function when only the query and property is given
        {
          //Create a Management Object Searcher object to get the information from Windows Management Instrumentation(WMI)
          ManagementObjectSearcher searchQuery = new ManagementObjectSearcher("SELECT * FROM " + query);
           
          //Get Management Object one by one
          foreach (ManagementObject mngObj in searchQuery.Get())
          {
            //Name of the current Management Object
            Console.WriteLine(mngObj["Name"].ToString());
            Console.WriteLine("----------------------------------------------------------------");

            //Display the value of the property
            Console.WriteLine(property +" : " + mngObj.Properties[property].Value);
            Console.WriteLine();
          }
            
          //Release the resources consumed by the Management Object Searcher object
          searchQuery.Dispose();                  
       }
       #endregion   
  
        public static void selectQuery(String query)
        #region Function when only the query is given
        {
          //Create a Management Object Searcher object to get the information from Windows Management Instrumentation(WMI)
          ManagementObjectSearcher searchQuery = new ManagementObjectSearcher ("SELECT * FROM " + query);
           
          //Get Management Objects one by one
          foreach (ManagementObject mngObj in searchQuery.Get())
          {
             //Name of the current Management Object
             Console.WriteLine(mngObj["Name"].ToString());
             Console.WriteLine("----------------------------------------------------------------");

             //Get Property Data Objects one by one
             foreach (PropertyData property in mngObj.Properties)
             {
                 if (property.Value != null && property.Value.ToString() != "")     //Display only the property data objects which have value
                 {
                     Console.Write(property.Name + " : ");

                     switch (property.Value.GetType().ToString())                   //Display value considering the property data object's value type
                     {
                         case "System.String[]":
                             string[] str = (string[])property.Value;
                             string str1 = string.Empty;
                             foreach (string s in str)
                             {
                                 str1 += s + " ";
                             }

                             Console.WriteLine(str1);
                             break;

                         case "System.UInt16[]":
                             ushort[] shortstr = (ushort[])property.Value;
                             string shortstr1 = string.Empty;
                             foreach (ushort shrts in shortstr)
                             {
                                 shortstr1 += shrts.ToString() + " ";
                             }
                             Console.WriteLine(shortstr1);
                             break;

                         default:
                             Console.WriteLine(property.Value.ToString());
                             break;
                     }
                 }
                
             }

          }
          //Release the resources consumed by the Management Object Searcher object
          searchQuery.Dispose();         
        }
        #endregion          

   }

}
           







     
        

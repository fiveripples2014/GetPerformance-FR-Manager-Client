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

namespace GetPerformance
{
    class Program
    {
       /* #region Hide Console
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int HIDE = 0;
        #endregion*/

        private static SpeechSynthesizer speech = new SpeechSynthesizer();
        
        public static void Main(string[] args)
        {
           /* 
            var handle = GetConsoleWindow();
            // Hide
            ShowWindow(handle, HIDE); */ 
            
            //Initialize the speed of the voice for the Warning !!!
            int speed = 2;

            #region Start
            //Speak out the greeting to the user
            speak("Welcome to your Workplace");
            Thread.Sleep(2);
            speak("Please enter your user name and password");
            #endregion

            #region Unique Identification
            //Get the current logged in user of the system
            Console.WriteLine(System.Security.Principal.WindowsIdentity.GetCurrent().Name);

            //Get the Mac Address 
            string macAddresses = string.Empty;
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    macAddresses += nic.GetPhysicalAddress().ToString();
                    break;
                }
            }
            Console.WriteLine(macAddresses);

            //Get the IP address of the computer
             string name = Dns.GetHostName();
             try
             {
                 IPAddress[] addrs = Dns.GetHostEntry(name).AddressList;
                 foreach (IPAddress addr in addrs)
                 Console.WriteLine("{0}/{1}", name, addr);
             }
             catch (Exception e)
             {
                 Console.WriteLine(e.Message);
             }

            string hostName = Dns.GetHostName(); // Retrive the Name of HOST
            Console.WriteLine(hostName);
           // Get the IP
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            Console.WriteLine("My IP Address is :"+myIP);
            Console.ReadKey();
       
            
           /* try
            {
                 IPHostEntry hostname = Dns.GetHostByName("localhost");
                   IPAddress[] ip = hostname.AddressList;
                   Console.WriteLine(ip[0].ToString());

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }*/
            #endregion

            #region Performance Counters

            //This will create performance counter object to get total percentage processor load and initialize it. 
            PerformanceCounter cpu = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            cpu.NextSample();

            //This will create performance counter object to get total available memory in megabytes and initialize it. 
            PerformanceCounter memory = new PerformanceCounter("Memory", "Available MBytes");
            memory.NextValue();

            //This will create performance counter object to get the total system up time and initialize it. 
            PerformanceCounter systemuptime = new PerformanceCounter("System", "System Up Time");
            systemuptime.NextValue();    
            #endregion

            #region Program Logic 1
            //Create time span object
            TimeSpan t = TimeSpan.FromSeconds(systemuptime.NextValue());
            String uptime = String.Format("The current system up time is {0} days {1} hours {2} minutes and {3} seconds",
                    (int)t.TotalDays,
                    (int)t.Hours,
                    (int)t.Minutes,
                    (int)t.Seconds                    
                    );
            
            Console.WriteLine(uptime);
            //Speak out current system up time
            speak(uptime, VoiceGender.Male,2);

            // Get the Logical Disk size and free space
            selectQuery("Win32_LogicalDisk", "Size");
            selectQuery("Win32_LogicalDisk", "FreeSpace");
            
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
                int availablememory = (int)memory.NextValue();
                
                Console.WriteLine("Available Memory : " + availablememory + " MB");

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
                        speak(messagecpu, VoiceGender.Male, speed);
                    }

                    else
                    {
                        String messagecpu = String.Format("CPU Load {0} percent", finalCpuCounter);
                        speak(messagecpu,VoiceGender.Male,2);
                    }
                }

                //Warning generator to alert the user when available memory is lesser
                if (availablememory < 200)
                {
                    String messagememory = String.Format("Available Memory {0} mega bytes", availablememory);
                    speak(messagememory, VoiceGender.Male, 2);
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
                             string str1 = "";
                             foreach (string s in str)
                             {
                                 str1 += s + " ";
                             }

                             Console.WriteLine(str1);
                             break;

                         case "System.UInt16[]":
                             ushort[] shortstr = (ushort[])property.Value;
                             string shortstr1 = "";
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
           

/*server = "192.168.10.221";
database = "restaurantdb";
uid = "root";
password = "";
string connectionString;
connectionString = "SERVER=" + server + "; PORT = 3306 ;" + "DATABASE=" + database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";
mycon = new MySqlConnection(connectionString);
*/





     
        

using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace FRManagerClient
{
    public class JsonDataObject
    {
        public DateTime utc { get; set; }
        public string hostName { get; set; }
        public Object loggedInUserName { get; set; }
        public string macAddress { get; set; }
        public string ipAddress { get; set; }
        public SystemUpTime upTime { get; set; }
        public Dictionary<string, Dictionary<string, Object>> disk { get; set; }
        public String memory; 
        public int percentCpuLoad { get; set; }
        public int threads { get; set; }
        public int processes { get; set; }
        public int availableMemoryInMB { get; set; }
        public int percentDiskTime { get; set; }
        public int percentIdleTime { get; set; }
        public int diskKiloBytesSec { get; set; }
        public int diskTransfersSec { get; set; }
        public double avgDiskSecsTransfer { get; set; }
        public int avgDiskKiloBytesTransfer { get; set; }
        public double avgDiskQueueLength { get; set; }
        public string runningApplications { get; set; }
    }

    public class SystemUpTime
    {
        public int totalDays { get; set; }
        public int hours { get; set; }
        public int minutes { get; set; }
        public int seconds { get; set; }
    }
   
    public class Partition
    {
        public string partitionName;
        public Object sizeInGB;
        public Object freeSpaceInGB;
    }
}


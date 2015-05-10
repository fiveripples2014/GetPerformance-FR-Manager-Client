using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRManagerClient
{
    public class JsonDataObject
    {
        public DateTime utc { get; set; }
        public string hostName { get; set; }
        public string loggedInUserName { get; set; }
        public string macAddress { get; set; }
        public string ipAddress { get; set; }
        public SystemUpTime upTime { get; set; }
        //public LinkedList<ManagementQuery> disk { get; set; }
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
    }

    public class SystemUpTime
    {
        public int TotalDays { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public int Seconds { get; set; }
    }
   
    /*public class ManagementQuery{
        public string name { get; set; }
        public string propertyName { get; set; }
        public string value { get; set; }
    }*/
}


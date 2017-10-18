using System;
using System.Threading;
using Microsoft.AspNet.SignalR;
using MSSQLScreen.Models;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Data;
using System.Web;
using System.Web.UI;

namespace MSSQLScreen.Hubs
{

    public class ChartDataUpdate
    {
        private readonly static Lazy<ChartDataUpdate> _instance = new Lazy<ChartDataUpdate>(() => new ChartDataUpdate());
        readonly int _updateInterval = 1000;
        private System.Threading.Timer timer;
        private volatile bool _sendingChartData = false;
        private readonly object _chartUpateLock = new object();
        SaveDataToDb saveData = new SaveDataToDb();
        GetMemoryFromSys getMemory = new GetMemoryFromSys();
        GetCPUFromSP getCPU = new GetCPUFromSP();

        private ChartDataUpdate()
        {

        }

        public static ChartDataUpdate Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        // Calling this method starts the Timer    
        public void GetChartData()
        {
            timer = new System.Threading.Timer(ChartTimerCallBack, null, _updateInterval, _updateInterval);

        }
        private void ChartTimerCallBack(object state)
        {
            if (_sendingChartData)
            {
                return;
            }
            lock (_chartUpateLock)
            {
                if (!_sendingChartData)
                {
                    _sendingChartData = true;
                    SendChartData();
                    SetChartData();
                    _sendingChartData = false;
                }
            }
        }

        private void SetChartData()
        {
            saveData.SetMemory();
            GetAllClients().All.UpdateChart(saveData);
            

        }

        private void SendChartData()
        {
            getMemory.GetMemory();
            getCPU.GetCPU();
            GetAllClients().All.getData(getMemory, getCPU);
        }

        private static dynamic GetAllClients()
        {
            return GlobalHost.ConnectionManager.GetHubContext<ChartHub>().Clients;
        }
    }

    public class SaveDataToDb
    {
        private ApplicationDbContext _context;

        public SaveDataToDb()
        {
            _context = new ApplicationDbContext();
        }

        public void SetMemory()
        {
            Uri uri = new Uri(GlobalVariable.URI);
            string ip_address = HttpUtility.ParseQueryString(uri.Query).Get("ip_address");
            var getserver = _context.ServerLists.Single(c => c.IPAddress == ip_address);
            var resourceInDB = _context.ResourceUsages.SingleOrDefault(c => c.ServerListId == getserver.Id);

            int getmemory = GetMemoryFromSys.memory;
            int getcpu = GetCPUFromSP.cpu;

            if (resourceInDB == null)
            {

                var usage = new ResourceUsage
                {
                    ProcessorUsage = getcpu,
                    AvailableMemory = getmemory,
                    ServerListId = getserver.Id
                };
                _context.ResourceUsages.Add(usage);
                _context.SaveChanges();
            }
            else
            {
                resourceInDB.ProcessorUsage = getcpu;
                resourceInDB.AvailableMemory = getmemory;
                _context.SaveChanges();
            }

        }
    }


    public class GetMemoryFromSys
    {
        [JsonProperty("availableMemory")]
        private static int availableMemory;

        public static int memory { get; set; }

        private ApplicationDbContext _context;

        public GetMemoryFromSys()
        {
            _context = new ApplicationDbContext();
        }


        public void GetMemory()
        {
    
            Uri uri = new Uri(GlobalVariable.URI);
            string ip_address = HttpUtility.ParseQueryString(uri.Query).Get("ip_address");
            var getserver = _context.ServerLists.Single(c => c.IPAddress == ip_address);
            using (SqlConnection sql = new SqlConnection("server=" + getserver.IPAddress + ";" + "user id=" + getserver.UserId + ";" + "password=" + getserver.Password + ";"))
            {
                sql.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT physical_memory_in_use_kb FROM sys.dm_os_process_memory", sql))
                {

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        availableMemory = Convert.ToInt32(reader[0]) / 1024;
                        memory = Convert.ToInt32(reader[0]) / 1024;
                    }
                }
            }

        }

    }

    public class GetCPUFromSP
    {
        [JsonProperty("cpuUsage")]
        private static int cpuUsage;

        public static int cpu { get; set; }

        private ApplicationDbContext _context;

        public GetCPUFromSP()
        {
            _context = new ApplicationDbContext();
        }


        public void GetCPU()
        {
            Uri uri = new Uri(GlobalVariable.URI);
            string ip_address = HttpUtility.ParseQueryString(uri.Query).Get("ip_address");
            var getserver = _context.ServerLists.Single(c => c.IPAddress == ip_address);
            using (SqlConnection sql = new SqlConnection("server=" + getserver.IPAddress + ";" + "user id=" + getserver.UserId + ";" + "password=" + getserver.Password + ";"))
            {
                sql.Open();
                using (SqlCommand cmd = new SqlCommand("sp_monitor", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlDataReader reader = cmd.ExecuteReader();

                    reader.NextResult();
                    while (reader.Read())
                    {
                        int length = reader[0].ToString().IndexOf("(");
                        cpuUsage = Convert.ToInt32(reader[0].ToString().Substring(0, length));
                        cpu = Convert.ToInt32(reader[0].ToString().Substring(0, length));
                    }
                }

            }

           
            
        }
    }

}
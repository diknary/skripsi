using System;
using System.Threading;
using Microsoft.AspNet.SignalR;
using MSSQLScreen.Models;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Data;

namespace MSSQLScreen.Hubs
{
    public class ChartDataUpdate
    {
        private readonly static Lazy<ChartDataUpdate> _instance = new Lazy<ChartDataUpdate>(() => new ChartDataUpdate());
        readonly int _updateInterval = 1000;
        private Timer timer;
        private volatile bool _sendingChartData = false;
        private readonly object _chartUpateLock = new object();
        SaveDataToDb saveData = new SaveDataToDb();
        GetMemoryFromDb getMemory = new GetMemoryFromDb();
        GetCPUFromDb getCPU = new GetCPUFromDb();

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
            timer = new Timer(ChartTimerCallBack, null, _updateInterval, _updateInterval);

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
            PerformanceCounter MemoryCounter = new PerformanceCounter("Memory", "Available MBytes");
            PerformanceCounter CPUcounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            var resourceInDB = _context.ResourceUsages.SingleOrDefault();
            CPUcounter.NextValue();
            Thread.Sleep(1000);
            CPUcounter.NextValue();
            Thread.Sleep(1000);
            if (resourceInDB == null)
            {
                var usage = new ResourceUsage
                {
                    ProcessorUsage = CPUcounter.NextValue(),
                    AvailableMemory = MemoryCounter.NextValue()
                };
                _context.ResourceUsages.Add(usage);
                _context.SaveChanges();
            }
            else
            {
                resourceInDB.ProcessorUsage = CPUcounter.NextValue();
                resourceInDB.AvailableMemory = MemoryCounter.NextValue();
                _context.SaveChanges();
            }
        }
    }

    public class GetMemoryFromDb
    {
        [JsonProperty("availableMemory")]
        private static int availableMemory;

        public void GetMemory()
        {

            SqlConnection sql = new SqlConnection("server=192.168.0.15;user id=sa;password=sukaati;");

            using (SqlCommand cmd = new SqlCommand("SELECT physical_memory_in_use_kb FROM sys.dm_os_process_memory", sql))
            {
                sql.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    availableMemory = Convert.ToInt32(reader[0]) / 1024;
                }
            }
            sql.Close();
        }

    }

    public class GetCPUFromDb
    {
        [JsonProperty("cpuUsage")]
        private static int cpuUsage;

        public void GetCPU()
        {

            SqlConnection sql = new SqlConnection("server=192.168.0.15;user id=sa;password=sukaati;");

            using (SqlCommand cmd = new SqlCommand("sp_monitor", sql))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                sql.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                reader.NextResult();
                while (reader.Read())
                {
                    int length = reader[0].ToString().IndexOf("(");
                    cpuUsage = Convert.ToInt32(reader[0].ToString().Substring(0, length));
                }
            }
            sql.Close();
            
        }
    }

}
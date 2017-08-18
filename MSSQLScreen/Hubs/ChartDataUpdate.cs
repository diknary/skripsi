using System;
using System.Threading;
using Microsoft.AspNet.SignalR;
using MSSQLScreen.Models;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;

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
        private static float availableMemory;

        public void GetMemory()
        {

            PerformanceCounter MemoryCounter = new PerformanceCounter("Memory", "Available MBytes");

            availableMemory = MemoryCounter.NextValue();
        }

    }

    public class GetCPUFromDb
    {
        [JsonProperty("cpuUsage")]
        private static float cpuUsage;

        public void GetCPU()
        {

            PerformanceCounter CPUcounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

            CPUcounter.NextValue();
            Thread.Sleep(1000);
            CPUcounter.NextValue();
            Thread.Sleep(1000);
            cpuUsage = CPUcounter.NextValue();
        }
    }

}
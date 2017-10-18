using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace MSSQLScreen.Hubs
{
    [HubName("chartHub")]
    public class ChartHub : Hub
    {
        private readonly ChartDataUpdate _ChartInstance;
        public ChartHub() : this(ChartDataUpdate.Instance) { }

        public ChartHub(ChartDataUpdate ChartInstance)
        {
            _ChartInstance = ChartInstance;
        }

        public void InitSaveData()
        {
            SaveDataToDb saveData = new SaveDataToDb();
            saveData.SetMemory();
            
            Clients.All.UpdateChart(saveData);

            _ChartInstance.GetChartData();

            
        }

        public void InitGetData()
        {
            GetMemoryFromSys getMemory = new GetMemoryFromSys();
            getMemory.GetMemory();
            GetCPUFromSP getCPU = new GetCPUFromSP();
            getCPU.GetCPU();

            Clients.All.getData(getMemory, getCPU);

            _ChartInstance.GetChartData();
        }

    }
}
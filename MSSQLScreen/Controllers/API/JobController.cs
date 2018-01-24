using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web.Http;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo.Agent;
using MSSQLScreen.Models;
using System.Data.SqlClient;

namespace MSSQLScreen.Controllers.API
{
    public class JobController : ApiController
    {
        private ApplicationDbContext _context;

        public JobController()
        {
            _context = new ApplicationDbContext();
        }

        //Convert Runstatusoutcome
        private string RunStatus(string id)
        {
            switch (id)
            {
                case "0":
                    return "Failed";
                case "1":
                    return "Succeeded";
                case "2":
                    return "Retry";
                case "3":
                    return "Canceled";
                default:
                    return "0";

            }
        }

        //Convert Runduration to string
        private int RunDuration(int status)
        {
            int hour = (status / 10000) * 3600;
            int minutes = (status / 100 % 100) * 60;
            int seconds = (status % 100);
            int duration = hour + minutes + seconds;
            return duration;
        }

        [APIAuthorize]
        [HttpGet]
        [Route("api/job/{server_id}")]
        public IEnumerable<JobList> GetJob(int server_id)
        {
            var getserver = _context.ServerLists.SingleOrDefault(c => c.Id== server_id);
            byte[] dcrpt = Convert.FromBase64String(getserver.Password);
            SqlConnection sql = new SqlConnection("server=" + getserver.IPAddress + ";" + "user id=" + getserver.UserId + ";" + "password=" + ASCIIEncoding.ASCII.GetString(dcrpt)  + ";");
            ServerConnection conn = new ServerConnection(sql);
            Server server = new Server(conn);
            JobCollection jobs = server.JobServer.Jobs;
            jobs.Refresh();
            //Insert job from dbo.sysjob to MSSQLScreen table
            foreach (Job job in jobs)
            {

                var joblistInDb = _context.JobLists.SingleOrDefault(c => c.JobId == job.JobID.ToString());

                if (joblistInDb == null)
                {
                    var joblist = new JobList
                    {
                        JobId = job.JobID.ToString(),
                        Name = job.Name,
                        IsEnabled = job.IsEnabled,
                        JobStatus = job.CurrentRunStatus.ToString(),
                        LastRunOutcome = job.LastRunOutcome.ToString(),
                        LastRun = job.LastRunDate.ToString("yyyy-MM-dd HH:mm:ss:fff"),
                        NextRun = job.NextRunDate.ToString("yyyy-MM-dd HH:mm:ss:fff"),
                        Scheduled = job.HasSchedule,
                        ServerListId = getserver.Id
                    };
                    _context.JobLists.Add(joblist);
                    _context.SaveChanges();
                }
                else
                {
                    joblistInDb.IsEnabled = job.IsEnabled;
                    joblistInDb.JobStatus = job.CurrentRunStatus.ToString();
                    joblistInDb.LastRunOutcome = job.LastRunOutcome.ToString();
                    joblistInDb.LastRun = job.LastRunDate.ToString("yyyy-MM-dd HH:mm:ss:fff");
                    joblistInDb.NextRun = job.NextRunDate.ToString("yyyy-MM-dd HH:mm:ss:fff");
                    joblistInDb.Scheduled = job.HasSchedule;
                    _context.SaveChanges();

                }

            }

            var joblists = _context.JobLists.ToList();
            foreach (var joblist in joblists)
            {
                var jobhistory = server.JobServer.Jobs[joblist.Name];
                jobhistory.Refresh();

                var jobfilter = new JobHistoryFilter();
                var jobhistories = jobhistory.EnumHistory(jobfilter);

                foreach (DataRow row in jobhistories.Rows.Cast<DataRow>())
                {
                    joblist.Duration = RunDuration(Convert.ToInt32(row["RunDuration"]));
                    _context.SaveChanges();
                    break;
                }
            }
            
            return _context.JobLists.OrderByDescending(c => c.Duration).ToList();
        }

        [APIAuthorize]
        [HttpGet]
        [Route("api/job/{server_id}/{job_id}")]
        public IEnumerable<JobHistory> GetJobHistory(int server_id, int job_id)
        {
            //Delete job history in MSSQLScreen table
            var jobhistoryInDb = _context.JobDetails.Where(c => c.JobListId == job_id).ToList();
            foreach (var jobhis in jobhistoryInDb)
            {
                _context.JobDetails.Remove(jobhis);
                _context.SaveChanges();
            }


            //Insert job history from dbo.syshistory to MSSQLScreen table
            var joblistInDb = _context.JobLists.SingleOrDefault(c => c.Id == job_id);

            var getserver = _context.ServerLists.SingleOrDefault(c => c.Id == server_id);
            byte[] dcrpt = Convert.FromBase64String(getserver.Password);
            SqlConnection sql = new SqlConnection("server=" + getserver.IPAddress + ";" + "user id=" + getserver.UserId + ";" + "password=" + ASCIIEncoding.ASCII.GetString(dcrpt) + ";");
            ServerConnection conn = new ServerConnection(sql);
            Server server = new Server(conn);
            var job = server.JobServer.Jobs[joblistInDb.Name];
            job.Refresh();

            var jobfilter = new JobHistoryFilter();
            var jobhistories = job.EnumHistory(jobfilter);

            foreach (DataRow row in jobhistories.Rows.Cast<DataRow>())
            {
                var jobrunhistory = new JobHistory
                {
                    JobId = joblistInDb.JobId,
                    RunDate = row["RunDate"].ToString(),
                    JobListId = joblistInDb.Id,
                    StepName = row["StepName"].ToString(),
                    StepId = Convert.ToInt32(row["StepId"]),
                    Duration = RunDuration(Convert.ToInt32(row["RunDuration"])),
                    RunOutcome = RunStatus(row["RunStatus"].ToString())
                };
                _context.JobDetails.Add(jobrunhistory);
                _context.SaveChanges();

            }
            return _context.JobDetails.Where(c => c.JobListId == job_id).Include(c => c.JobList).ToList();
        }

    }
}

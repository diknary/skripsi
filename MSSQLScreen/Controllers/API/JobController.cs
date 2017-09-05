using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo.Agent;
using MSSQLScreen.Models;

namespace MSSQLScreen.Controllers.API
{
    public class JobController : ApiController
    {
        static readonly string SqlServer = @"DESKTOP-PQD9KKN";

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
        private string RunDuration(int status)
        {
            string hour = Convert.ToString(status / 10000) + ":";
            string minutes = Convert.ToString(status / 100 % 100) + ":";
            string seconds = Convert.ToString(status % 100);
            string duration = hour + minutes + seconds;
            return duration;
        }

        [APIAuthorize]
        [HttpGet]
        [Route("api/job/getjob")]
        public IEnumerable<JobList> GetJob()
        {
            ServerConnection conn = new ServerConnection(SqlServer);
            Server server = new Server(conn);
            JobCollection jobs = server.JobServer.Jobs;
            jobs.Refresh();
            //Insert job from dbo.sysjob to MSSQLScreen table
            foreach (Job job in jobs)
            {

                var jobactivityInDb = _context.JobLists.SingleOrDefault(c => c.JobId == job.JobID.ToString());

                if (jobactivityInDb == null)
                {
                    var jobactivity = new JobList
                    {
                        JobId = job.JobID.ToString(),
                        Name = job.Name,
                        IsEnabled = job.IsEnabled,
                        JobStatus = job.CurrentRunStatus.ToString(),
                        LastRunOutcome = job.LastRunOutcome.ToString(),
                        LastRun = job.LastRunDate.ToString("yyyy-MM-dd HH:mm:ss:fff"),
                        NextRun = job.NextRunDate.ToString("yyyy-MM-dd HH:mm:ss:fff"),
                        Scheduled = job.HasSchedule
                    };
                    _context.JobLists.Add(jobactivity);
                    _context.SaveChanges();
                }
                else
                {
                    jobactivityInDb.IsEnabled = job.IsEnabled;
                    jobactivityInDb.JobStatus = job.CurrentRunStatus.ToString();
                    jobactivityInDb.LastRunOutcome = job.LastRunOutcome.ToString();
                    jobactivityInDb.LastRun = job.LastRunDate.ToString("yyyy-MM-dd HH:mm:ss:fff");
                    jobactivityInDb.NextRun = job.NextRunDate.ToString("yyyy-MM-dd HH:mm:ss:fff");
                    jobactivityInDb.Scheduled = job.HasSchedule;
                    _context.SaveChanges();

                }

            }

            return _context.JobLists.ToList();
        }

        [APIAuthorize]
        [HttpGet]
        [Route("api/job/jobhistory/{id}")]
        public IEnumerable<JobRunHistory> GetJobHistory(int id)
        {
            //Delete job history in MSSQLScreen table
            var jobhistoryInDb = _context.JobRunHistories.Where(c => c.JobActivityId == id).ToList();
            foreach (var jobhis in jobhistoryInDb)
            {
                _context.JobRunHistories.Remove(jobhis);
                _context.SaveChanges();
            }


            //Insert job history from dbo.syshistory to MSSQLScreen table
            var jobactivityInDb = _context.JobLists.SingleOrDefault(c => c.Id == id);

            ServerConnection conn = new ServerConnection(SqlServer);
            Server server = new Server(conn);
            var job = server.JobServer.Jobs[jobactivityInDb.Name];
            job.Refresh();

            var jobfilter = new JobHistoryFilter();
            var jobhistories = job.EnumHistory(jobfilter);

            foreach (DataRow row in jobhistories.Rows.Cast<DataRow>())
            {
                var jobrunhistory = new JobRunHistory
                {
                    JobId = jobactivityInDb.JobId,
                    RunDate = row["RunDate"].ToString(),
                    JobActivityId = jobactivityInDb.Id,
                    StepName = row["StepName"].ToString(),
                    StepId = Convert.ToInt32(row["StepId"]),
                    Duration = RunDuration(Convert.ToInt32(row["RunDuration"])),
                    RunOutcome = RunStatus(row["RunStatus"].ToString())
                };
                _context.JobRunHistories.Add(jobrunhistory);
                _context.SaveChanges();

            }
            return _context.JobRunHistories.Where(c => c.JobActivityId == id).Include(c => c.JobActivity).ToList();
        }

    }
}

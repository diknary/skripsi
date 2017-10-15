using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo.Agent;
using MSSQLScreen.Models;
using System.Data.SqlClient;
using System.Security.Claims;

namespace MSSQLScreen.Controllers
{
    public class JobController : Controller
    {
        private ApplicationDbContext _context;

        public JobController()
        {
            _context = new ApplicationDbContext();
        }

        //Convert Runstatusoutcome
        private string RunStatus(string status)
        {
            switch (status)
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

        [WebAuthorize]
        //[Route("job/migratejob/{ip_address}")]
        public ActionResult MigrateJob(string IP)
        {
            var identity = (ClaimsIdentity)User.Identity;
            int adminId = Int32.Parse(((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type == "AdminId").Value);
            var getserver = _context.ServerLists.SingleOrDefault(c => c.IPAddress == IP);
            SqlConnection sql = new SqlConnection("server=" + getserver.IPAddress + ";" + "user id=" + getserver.UserId + ";" + "password=" + getserver.Password + ";");
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
                        ServerListId = getserver.Id,
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

            return RedirectToAction("GetJob", "Job", new { server_id = getserver.Id });
        }

        [WebAuthorize]
        //[Route("job/getjob/{server_id}")]
        public ActionResult GetJob(int server_id)
        {
            var JobLists = _context.JobLists.Where(c => c.ServerListId == server_id).ToList();
            return View(JobLists);
        }

        [WebAuthorize]
        //[Route("job/jobhistory/{server_id}/{job_id}")]
        public ActionResult JobHistory(int server_id, int job_id)
        {

            //Delete job history in MSSQLScreen table
            var jobhistoryInDb = _context.JobRunHistories.Where(c => c.JobListId == job_id);
            _context.JobRunHistories.RemoveRange(jobhistoryInDb);
            _context.SaveChanges();

            //Insert job history from dbo.syshistory to MSSQLScreen table
            var joblistInDb = _context.JobLists.SingleOrDefault(c => c.Id == job_id);

            var getserver = _context.ServerLists.SingleOrDefault(c => c.Id == server_id);
            SqlConnection sql = new SqlConnection("server=" + getserver.IPAddress + ";" + "user id=" + getserver.UserId + ";" + "password=" + getserver.Password + ";");
            ServerConnection conn = new ServerConnection(sql);
            Server server = new Server(conn);
            var job = server.JobServer.Jobs[joblistInDb.Name];
            job.Refresh();

            var jobfilter = new JobHistoryFilter();
            var jobhistories = job.EnumHistory(jobfilter);

            foreach (DataRow row in jobhistories.Rows.Cast<DataRow>())
            {
                var jobrunhistory = new JobRunHistory
                {
                    JobId = joblistInDb.JobId,
                    RunDate = row["RunDate"].ToString(),
                    JobListId = joblistInDb.Id,
                    StepName = row["StepName"].ToString(),
                    StepId = Convert.ToInt32(row["StepId"]),
                    Duration = RunDuration(Convert.ToInt32(row["RunDuration"])),
                    RunOutcome = RunStatus(row["RunStatus"].ToString())
                };
                _context.JobRunHistories.Add(jobrunhistory);
                _context.SaveChanges();

            }
            //Pass data from MSSQLScreen table job history to view
            var viewjobrunhistory = _context.JobRunHistories.Where(c => c.JobListId == job_id).Include(c => c.JobList).ToList();

            return View(viewjobrunhistory);

        }
    }
}
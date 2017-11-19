using MSSQLScreen.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace MSSQLScreen.Controllers
{
    public class DownloadController : Controller
    {
        private ApplicationDbContext _context;

        public DownloadController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: Download
        public ActionResult ToExcel(int id)
        {
            DataSet ds = new DataSet("New_DataSet");
            DataTable dt = new DataTable("New_DataTable");

            ds.Locale = System.Threading.Thread.CurrentThread.CurrentCulture;
            dt.Locale = System.Threading.Thread.CurrentThread.CurrentCulture;

            var jobhistoryInDb = _context.JobDetails.Where(c => c.JobListId == id);
            var joblistInDb = _context.JobLists.Single(c => c.Id == id);
            dt.Columns.Add("Id", typeof(System.Int32));
            dt.Columns.Add("JobId", typeof(System.String));
            dt.Columns.Add("StepId", typeof(System.Int32));
            dt.Columns.Add("StepName", typeof(System.String));
            dt.Columns.Add("Duration", typeof(System.String));
            dt.Columns.Add("RunOutcome", typeof(System.String));
            dt.Columns.Add("RunDate", typeof(System.String));

            foreach (var element in jobhistoryInDb)
            {
                var row = dt.NewRow();
                row["Id"] = element.Id;
                row["JobId"] = element.JobId;
                row["StepId"] = element.StepId;
                row["StepName"] = element.StepName;
                row["Duration"] = element.Duration;
                row["RunOutcome"] = element.RunOutcome;
                row["RunDate"] = element.RunDate;
                dt.Rows.Add(row);
            }

            ds.Tables.Add(dt);

            Directory.CreateDirectory(HttpRuntime.AppDomainAppPath + "DownloadableFiles");
            string filePath = HttpRuntime.AppDomainAppPath + "DownloadableFiles" + "/" + joblistInDb.Name + ".xls";
            FileInfo file = new FileInfo(filePath);
            if (file.Exists)
            {
                file.Delete();
                ExcelLibrary.DataSetHelper.CreateWorkbook(filePath, ds);

            }
            else
            {
                ExcelLibrary.DataSetHelper.CreateWorkbook(filePath, ds);
            }
            Response.Clear();
            Response.ClearHeaders();
            Response.ClearContent();
            Response.AddHeader("Content-Disposition", "attachment; filename=" + file.Name);
            Response.AddHeader("Content-Length", file.Length.ToString());
            Response.AddHeader("Content-Type", "application/Excel");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.Flush();
            Response.TransmitFile(file.FullName);
            Response.End();

            return View();
        }
    }
}
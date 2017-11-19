using System.Linq;
using System.Web.Http;
using MSSQLScreen.Models;
using System.Data.SqlClient;
using System;
using System.Data;

namespace MSSQLScreen.Controllers.API
{
    public class ResourceController : ApiController
    {
        private ApplicationDbContext _context;

        private static int availableMemory, cpuUsage;
        private static long availableSpace, totalSpace;

        public ResourceController()
        {
            _context = new ApplicationDbContext();
        }

        [APIAuthorize]
        [HttpGet]
        [Route("api/resource/{server_id}")]
        public IHttpActionResult GetResources(int server_id)
        {

            var getserver = _context.ServerLists.Single(c => c.Id == server_id);
            var resourceInDB = _context.ResourceUsages.SingleOrDefault(c => c.ServerListId == server_id);

            using (SqlConnection sql = new SqlConnection("server=" + getserver.IPAddress + ";" + "user id=" + getserver.UserId + ";" + "password=" + getserver.Password + ";"))
            {
                sql.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT physical_memory_in_use_kb FROM sys.dm_os_process_memory", sql))
                {
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        availableMemory = Convert.ToInt32(reader[0]) / 1024;
                    }
                }
            }

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
                    }
                }

            }

            if (resourceInDB == null)
            {

                var usage = new ResourceUsage
                {
                    ProcessorUsage = cpuUsage,
                    AvailableMemory = availableMemory,
                    ServerListId = server_id
                };
                _context.ResourceUsages.Add(usage);
                _context.SaveChanges();
            }
            else
            {
                resourceInDB.ProcessorUsage = cpuUsage;
                resourceInDB.AvailableMemory = availableMemory;
                _context.SaveChanges();
            }

            return Ok(_context.ResourceUsages.SingleOrDefault(c => c.ServerListId == server_id));
        }

        [APIAuthorize]
        [HttpGet]
        [Route("api/resource/{drive_name}/{server_id}")]
        public IHttpActionResult GetDriveSpace(string drive_name, int server_id)
        {
            var getserver = _context.ServerLists.Single(c => c.Id == server_id);
            var diskusageInDb = _context.DiskUsages.SingleOrDefault(c => c.DriveName == drive_name && c.ServerListId == server_id);

            using (SqlConnection sql = new SqlConnection("server=" + getserver.IPAddress + ";" + "user id=" + getserver.UserId + ";" + "password=" + getserver.Password + ";"))
            {
                sql.Open();
                using (SqlCommand cmd = new SqlCommand("EXEC [master].[dbo].[xp_cmdshell] " + "'fsutil volume diskfree " + drive_name + ":'", sql))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    int counter = 0;
                    while (reader.Read())
                    {
                        int start = reader[0].ToString().IndexOf(":") + 2;
                        int lastindex = reader[0].ToString().Length;
                        if (counter == 1)
                        {
                            totalSpace = Convert.ToInt64(reader[0].ToString().Substring(start, lastindex - start)) / 1073741824;
                            break;
                        }
                        else
                        {
                            availableSpace = Convert.ToInt64(reader[0].ToString().Substring(start, lastindex - start)) / 1073741824;
                            counter++;
                        }
                        
                    }
                }    

            }

            if (diskusageInDb == null)
            {

                var usage = new DiskUsage
                {
                    AvailabeSpace = availableSpace,
                    TotalSpace = totalSpace,
                    DriveName = drive_name,
                    ServerListId = server_id
                };
                _context.DiskUsages.Add(usage);
                _context.SaveChanges();
            }
            else
            {
                diskusageInDb.AvailabeSpace = availableSpace;
                diskusageInDb.TotalSpace = totalSpace;
                _context.SaveChanges();
            }

            return Ok(_context.DiskUsages.SingleOrDefault(c => c.DriveName == drive_name && c.ServerListId == server_id));
        }
    }
}

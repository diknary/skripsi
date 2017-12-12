using System.Linq;
using System.Web.Http;
using MSSQLScreen.Models;
using System.Data.SqlClient;
using System;
using System.Data;
using System.Collections.Generic;

namespace MSSQLScreen.Controllers.API
{
    public class ResourceController : ApiController
    {
        private ApplicationDbContext _context;

        private static int availableMemory, cpuUsage;
        private string[] driveName;
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
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            availableMemory = Convert.ToInt32(reader[0]) / 1024;
                        }
                    }
                        
                }

                using (SqlCommand cmd = new SqlCommand("sp_monitor", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.NextResult();
                        while (reader.Read())
                        {
                            int length = reader[0].ToString().IndexOf("(");
                            cpuUsage = Convert.ToInt32(reader[0].ToString().Substring(0, length));
                        }
                    }
                       
                }

                using (SqlCommand cmd = new SqlCommand("EXEC xp_cmdshell N'fsutil fsinfo drives'", sql))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        int counter = 0;
                        while (reader.Read())
                        {
                            if (counter == 1)
                            {
                                string drivesName = reader[0].ToString();
                                driveName = drivesName.Split('\\', ':');
                                break;
                            }
                            else
                                counter++;

                        }

                    }

                }

                for (int i = 1; i < driveName.Length - 2; i += 2)
                {
                    var tmp = driveName[i];
                    var driveInDb = _context.DiskUsages.SingleOrDefault(c => c.DriveName == tmp);

                    if (driveInDb != null)
                    {
                        using (SqlCommand cmd2 = new SqlCommand("EXEC xp_cmdshell " + "'fsutil volume diskfree " + tmp + ":'", sql))
                        {
                            using (SqlDataReader reader2 = cmd2.ExecuteReader())
                            {
                                int ctr = 0;
                                while (reader2.Read())
                                {
                                    try
                                    {
                                        int start = reader2[0].ToString().IndexOf(":") + 2;
                                        int lastindex = reader2[0].ToString().Length;
                                        if (ctr == 1)
                                        {
                                            totalSpace = Convert.ToInt64(reader2[0].ToString().Substring(start, lastindex - start)) / 1073741824;
                                            break;
                                        }
                                        else
                                        {
                                            availableSpace = Convert.ToInt64(reader2[0].ToString().Substring(start, lastindex - start)) / 1073741824;
                                            ctr++;
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        totalSpace = 0;
                                        availableSpace = 0;
                                        break;
                                    }

                                }
                                driveInDb.AvailabeSpace = availableSpace;
                                driveInDb.TotalSpace = totalSpace;
                                _context.SaveChanges();
                            }

                        }
                    }
                    else
                    {
                        using (SqlCommand cmd2 = new SqlCommand("EXEC xp_cmdshell " + "'fsutil volume diskfree " + tmp + ":'", sql))
                        {
                            using (SqlDataReader reader2 = cmd2.ExecuteReader())
                            {
                                int ctr = 0;
                                while (reader2.Read())
                                {
                                    try
                                    {
                                        int start = reader2[0].ToString().IndexOf(":") + 2;
                                        int lastindex = reader2[0].ToString().Length;
                                        if (ctr == 1)
                                        {
                                            totalSpace = Convert.ToInt64(reader2[0].ToString().Substring(start, lastindex - start)) / 1073741824;
                                            break;
                                        }
                                        else
                                        {
                                            availableSpace = Convert.ToInt64(reader2[0].ToString().Substring(start, lastindex - start)) / 1073741824;
                                            ctr++;
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        totalSpace = 0;
                                        availableSpace = 0;
                                        break;
                                    }

                                }
                                var disk = new DiskUsage
                                {
                                    DriveName = driveName[i],
                                    AvailabeSpace = availableSpace,
                                    TotalSpace = totalSpace,
                                    ServerListId = server_id
                                };
                                _context.DiskUsages.Add(disk);
                                _context.SaveChanges();
                            }

                        }
                    }

                }
            }

            if (resourceInDB == null)
            {

                var usage = new ResourceUsage
                {
                    CPUBusy = cpuUsage,
                    AvailableMemory = availableMemory,
                    ServerListId = server_id
                };
                _context.ResourceUsages.Add(usage);
                _context.SaveChanges();
            }
            else
            {
                resourceInDB.CPUBusy = cpuUsage;
                resourceInDB.AvailableMemory = availableMemory;
                _context.SaveChanges();
            }

            var resourceView = new ResourceViewModel
            {
                ResourcesUsage = _context.ResourceUsages.SingleOrDefault(c => c.ServerListId == server_id),
                DrivesSpace = _context.DiskUsages.Where(c => c.ServerListId == server_id).ToList()
            };

            return Ok(resourceView);
        }

    }
}

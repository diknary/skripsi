using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSQLScreen.Models
{
    public class DiskUsage
    {
        public int Id { get; set; }

        public string DriveName { get; set; }

        public long AvailabeSpace { get; set; }

        public long TotalSpace { get; set; }

        public ServerList ServerList { get; set; }

        public int? ServerListId { get; set; }
    }
}
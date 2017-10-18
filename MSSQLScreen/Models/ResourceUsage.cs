using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Highsoft.Web.Mvc.Charts;

namespace MSSQLScreen.Models
{
    public class ResourceUsage
    {
        public int Id { get; set; }

        public int ProcessorUsage { get; set; }

        public int AvailableMemory { get; set; }

        public ServerList ServerList { get; set; }

        public int? ServerListId { get; set; }
    }
}
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

        public float ProcessorUsage { get; set; }

        public float AvailableMemory { get; set; }
    }
}
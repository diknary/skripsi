using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSSQLScreen.Models
{
    public class ResourceViewModel
    {
        public ResourceUsage ResourcesUsage { get; set; }

        public IEnumerable<DiskUsage> DrivesSpace { get; set; }
    }
}
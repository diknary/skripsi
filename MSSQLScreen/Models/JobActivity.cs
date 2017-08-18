using System.Collections.Generic;

namespace MSSQLScreen.Models
{
    public class JobActivity
    {
        public int Id { get; set; }

        public string JobId { get; set; }

        public string Name { get; set; }

        public string JobStatus { get; set; }

        public bool IsEnabled { get; set; }

        public string LastRunOutcome { get; set; }

        public string LastRun { get; set; }

        public string NextRun { get; set; }

        public bool Scheduled { get; set; }
    }
}
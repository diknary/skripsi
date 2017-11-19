using System.Collections.Generic;

namespace MSSQLScreen.Models
{
    public class JobList
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
        
        public int Duration { get; set; }

        public ServerList ServerList { get; set; }

        public int? ServerListId { get; set; }

    }
}
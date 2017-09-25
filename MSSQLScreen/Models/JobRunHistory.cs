using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MSSQLScreen.Models
{
    public class JobRunHistory
    {
        public int Id { get; set; }

        public string JobId { get; set; }

        public int? StepId { get; set; }

        public string StepName { get; set; }

        public string Duration { get; set; }

        public string RunOutcome { get; set; }

        public string RunDate { get; set; }

        public string ResourceId { get; set; }

        public JobList JobList { get; set; }

        public int? JobListId { get; set; }

    }
}
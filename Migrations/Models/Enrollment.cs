using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLearningSystem.Models
{
    public enum Status
    {
        //0
        Unordered,
        //1
        Ordered,
        //2
        Approved,
        //3
        WaitingForPayment
    }
    public class Enrollment
    {
        [ForeignKey(nameof(Course))]
        public int CourseID { get; set; }

        public virtual Course Course { get; set; }

        [ForeignKey(nameof(ApplicationUser))]
        public string StudentID { get; set; }

        public virtual ApplicationUser Student { get; set; }

        [Required]
        public Status Status { get; set; }

    }
}

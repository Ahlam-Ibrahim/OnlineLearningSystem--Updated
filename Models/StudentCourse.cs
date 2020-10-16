using System;
using System.Collections.Generic;
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
    public class StudentCourse
    {
        public int CourseID { get; set; }
        public virtual Course Course { get; set; }

        public string StudentID { get; set; }
        public virtual ApplicationUser Student { get; set; }
        public Status Status { get; set; }

    }
}

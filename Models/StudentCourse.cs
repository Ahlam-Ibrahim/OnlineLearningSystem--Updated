using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLearningSystem.Models
{
    public class StudentCourse
    {
        public int CourseID { get; set; }
        public virtual Course Course { get; set; }

        public string UserName { get; set; }
        public virtual ApplicationUser Student { get; set; }

    }
}

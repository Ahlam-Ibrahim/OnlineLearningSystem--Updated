using OnlineLearningSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLearningSystem.Dtos
{
    public class EnrollmentDto
    {
        public int CourseID { get; set; }

        public string StudentID { get; set; }
        public Status Status { get; set; }
    }
}

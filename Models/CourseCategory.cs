using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLearningSystem.Models
{
    public class CourseCategory
    {
        public int CourseID { get; set; }
        public virtual Course Course { get; set; }

        public int CategoryID { get; set; }
        public virtual Category Category { get; set; }
    }
}

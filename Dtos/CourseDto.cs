using OnlineLearningSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLearningSystem.Dtos
{
    public class CourseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double Duration { get; set; }
        public Location Location { get; set; }
        public DateTime DateCreated { get; set; }
        public double Price { get; set; }
        public string CoursePicturePath { get; set; }
        public string CoursePreviewVideoPath { get; set; }

    }
}

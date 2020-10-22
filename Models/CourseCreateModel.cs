using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLearningSystem.Models
{
    public class CourseCreateModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public double Duration { get; set; }
        public double Price { get; set; }
        public DateTime DateCreated { get; set; }
        public Location Location { get; set; }
        public IFormFile CoursePicture { get; set; }
        public IFormFile CoursePreviewVideo { get; set; }
    }
}

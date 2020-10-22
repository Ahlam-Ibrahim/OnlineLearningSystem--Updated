using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using OnlineLearningSystem.Models;

namespace OnlineLearningSystem.Models
{
    public enum Location
    {
        Unspecified,
        Online,
        Offline,
        Recorded
        //Unspecified: 0 - to prevent assigning the default 0 to others when updating
        //Online: Like a Zoom meeting - through link : 1
        //Offline: attending physically : 2
        //Recrded: recorded videos : 3
    }
    public class Course
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double Duration { get; set; }
        public double Price { get; set; }
        public DateTime DateCreated { get; set; }
        public Location Location { get; set; }
        public string CoursePicturePath { get; set; }
        public string CoursePreviewVideoPath { get; set; }

        public virtual ICollection<Section> Sections { get; set; }
        public virtual ICollection<CourseCategory> CourseCategories { get; set; }
        public virtual ICollection<Enrollment> Enrollments { get; set; }




    }
}

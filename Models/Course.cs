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
        Online,
        Offline,
        Recorded
        //Online: Like a Zoom meeting - through link : 0
        //Offline: attending physically : 1
        //Recrded: recorded videos : 2
    }

    public class Course
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public int Duration { get; set; }
        public DateTime DateCreated { get; set; }

        [Required(ErrorMessage = "location type is required.")]
        public Location Location { get; set; }
        
        public virtual ICollection<Section> Sections { get; set; }
        public virtual ICollection<CourseCategory> CourseCategories { get; set; }
        public virtual ICollection<StudentCourse> StudentCourses { get; set; }




    }
}

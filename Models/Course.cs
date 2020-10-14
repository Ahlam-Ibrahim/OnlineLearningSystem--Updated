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
        //[EnumMember(Value = "Online")]
        Online,
        //[EnumMember(Value = "Offline")]
        Offline,
        //[EnumMember(Value = "Recorded")]
        Recorded
        //Online: Like a Zoom meeting - through link : 0
        //Offline: attending physically : 1
        //Recrded: recorded videos : 2
    }
    public enum Status
    {
        //[EnumMember(Value = "Ordered")]
        Ordered,
        //[EnumMember(Value = "Approved")]
        Approved,
        //[EnumMember(Value = "WaitingForPayment")]
        WaitingForPayment
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

        // TO DO: some problems in enum classes

        //[JsonProperty("Location")]
        //[Required(ErrorMessage = "Location type is required.")]
        public Location Location { get; set; }

        //[JsonProperty("Status")]
        //[Required(ErrorMessage = "Status type is required.")]
        public Status Status { get; set; }

        public virtual ICollection<Section> Sections { get; set; }
        public virtual ICollection<CourseCategory> CourseCategories { get; set; }
        public virtual ICollection<StudentCourse> StudentCourses { get; set; }




    }
}

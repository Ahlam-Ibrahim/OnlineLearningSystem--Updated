using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace OnlineLearningSystem.Models
{
    public class Video
    {
        //The Video class represents the content of the Course
        //If the Course is online/offline - Zoom Meeting/physically attending -
        //then then the title will be used to display the details/link of the course
        //If the course is recorded - Videos - then the title and the videoPath will be used 

        [Required]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string VideoPath { get; set; }
        public virtual Section Section { get; set; }
    }
}

using OnlineLearningSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLearningSystem.Dtos
{
    public class SectionDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ICollection<Video> Videos { get; set; }
    }
}

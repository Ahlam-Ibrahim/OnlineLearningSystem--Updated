using Microsoft.AspNetCore.Builder;
using OnlineLearningSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLearningSystem.Services
{
    public class VideoRepository : IVideoRepository
    {
        private ApplicationDbContext _videoContext;

        public VideoRepository(ApplicationDbContext videoContext)
        {
            _videoContext = videoContext;
        }

        public bool CreateVideo(Video video)
        {
            _videoContext.Add(video);
            return Save();
        }

        public bool DeleteVideo(Video video)
        {
            _videoContext.Remove(video);
            return Save();
        }

        public Video GetVideo(int videoId)
        {
            return _videoContext.Videos.Where(c => c.Id == videoId).FirstOrDefault();
        }
        
        public ICollection<Video> GetAllVideosFromSection(int sectionId)
        {
            return _videoContext.Videos.Where(v => v.Section.Id == sectionId).ToList();
        }

        public bool Save()
        {
            var saved = _videoContext.SaveChanges();
            return saved >= 0 ? true : false;
        }

        public bool UpdateVideoOfASection(Video video)
        {
            _videoContext.Update(video);
            return Save();
        }

        public bool VideoExists(int id)
        {
            return _videoContext.Videos.Any(v => v.Id == id);
        }
    }
}

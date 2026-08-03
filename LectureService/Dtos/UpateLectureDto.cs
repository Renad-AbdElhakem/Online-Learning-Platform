using Microsoft.AspNetCore.Http;

namespace Content_Service.Dtos
{
    public class UpateLectureDto
    {
        public IFormFile ?VideoLecture { get; set; }

        public string ?LectureName { get; set; } 

        public string? Description { get; set; }

        public Guid? GroupId { get; set; }
    }
}

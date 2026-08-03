namespace LectureService.Dtos
{
    public class UploadNewLecture
    {
        public IFormFile VideoLecture { get; set; }

        public string LectureName { get; set; }

        public string? Description { get; set; }
        public Guid GroupId { get; set; }
    }
}

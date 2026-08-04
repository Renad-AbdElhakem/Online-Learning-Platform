namespace Content_Service.Dtos
{
    public class UpdateLectureMaterial
    {
        public IFormFile? LectureMaterialPath { get; set; } 

        public string? MaterialName { get; set; } 

        public string? Description { get; set; }
        public Guid? LectureId { get; set; }

    }
}

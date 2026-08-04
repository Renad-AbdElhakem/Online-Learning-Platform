namespace Content_Service.Dtos
{
    public class MaterialResponseDto
    {
        public Guid Id { get; set; }
        public string LectureMaterialPath { get; set; } 

        public string MaterialName { get; set; }

        public string? Description { get; set; }

        public DateTime UploadedAt { get; set; } 

        public DateTime? ModifiedAt { get; set; }

        
    }
}

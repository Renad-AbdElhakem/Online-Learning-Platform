namespace AssignmentService.Dtos
{
    public class AssignmentResponseDto
    {
        public string AssignmentName { get; set; }
        public string? Notes { get; set; }
        public int Grade { get; set; }
        public DateTime StartAt { get; set; }
        public int DurationInDays { get; set; }
        public DateTime EndAt { get; set; }
        public Guid GroupId { get; set; }
        public DateOnly UploadedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}

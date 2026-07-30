namespace GroupsCourseService.Dtos
{
    public class EnrollmentResponseDto
    {
        public int Id { get; set; }
        public Guid StudentId { get; set; }
        public Guid GroupId { get; set; }
        public DateOnly EnrollmentDate { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
    }
}

namespace GroupsCourseService.Dtos
{
    public class CreateEnrollmentDto
    {
        public Guid StudentId { get; set; }
        public Guid GroupId { get; set; }
        public string PaymentStatus { get; set; }
    }
}

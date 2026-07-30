namespace GroupsCourseService.Model
{
    public class Enrollment
    {
        public int Id { get; set; }
        public DateOnly EnrollmentDate { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public Guid GroupId { get; set; }
        public Group Group { get; set; }
        public Guid StudentId { get; set; }

    }
}

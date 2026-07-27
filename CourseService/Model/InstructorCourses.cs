namespace CourseService.Model
{
    public class InstructorCourses
    {
        public int Id { get; set; }

        public DateOnly CreatedAt { get; set; } = new DateOnly();
        public bool IsActive { get; set; } = true;
        public string ?status { get; set; }

        public Guid InstructorId { get; set; }
        //Navigation
        public Guid CourseId { get; set; }
        public Course Course { get; set; }
    }
}

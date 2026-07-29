namespace GroupsCourseService.Dtos
{
    public class RequestCreateNewGroup
    {
        public string GroupName { get; set; }
        public string Description { get; set; } = string.Empty;
        public int NumberOfStudentAllow { get; set; }
     
        public DateOnly StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string Statuts { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        public Guid CourseId { get; set; }
        public Guid InstructorId { get; set; }
    }
}

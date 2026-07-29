namespace GroupsCourseService.Dtos
{
    public class UpdateGroupAssignmentDto
    {
        public Guid? InstructorId { get; set; }
        public Guid? CourseId { get; set; }
    }
}

namespace GroupsCourseService.Dtos
{
    public class UpdateGroupStatusDto
    {
        public bool IsActive { get; set; } 
        public string Status { get; set; }
        public DateTime EndDate { get; set; }
    }
}

namespace CourseService.Dtos
{
    public class RequestUpdateCourse
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Level { get; set; }
        public decimal? Price { get; set; }
        public bool? IsActive { get; set; }
        public string? Status { get; set; }
        public TimeSpan? Duration { get; set; }
    }
}

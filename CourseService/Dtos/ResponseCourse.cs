namespace CourseService.Dtos
{
    public class ResponseCourse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Level { get; set; }
        public decimal? Price { get; set; }
        public bool? IsActive { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public TimeSpan? Duration { get; set; }
        public Guid CatelogeId { get; set; }
    }
}

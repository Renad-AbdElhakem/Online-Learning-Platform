namespace AuthService.Dtos
{
    public class ResponseUserAuth
    {
        public string Email { get; set; }
        public bool IsEmailConfirmed { get; set; } = false;
        public int RoleId { get; set; }
    }
}

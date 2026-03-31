namespace Task_Management_App.Models
{
    public class RefreshTokenMod
    {
        public int Id { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime Expires { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public string UserId { get; set; } = string.Empty;
        public AppUserModel? User { get; set; }
    }
}

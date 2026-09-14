namespace MusicApi.Response
{
    public class LoginResponse
    {
        public string ? AccessToken { get; set; } 
        public string ? RefreshToken { get; set; } 
        public UserResponse ? User { get; set; }
    }
}

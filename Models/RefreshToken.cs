namespace MusicApi.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string ? UserName { get; set; }  
        public string ? Token { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}

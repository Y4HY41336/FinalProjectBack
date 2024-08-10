namespace FinalProject.Models
{
    public class Like
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }

        public int PostId { get; set; }
        public Post Post { get; set; }

        public string UserId { get; set; }
        public AppUser User { get; set; }
    }
}

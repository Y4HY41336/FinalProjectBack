namespace FinalProject.Models;

public class PostTrend
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; }


    public int TrendId { get; set; }
    public Trend Trend { get; set; }

    public int PostId { get; set; }
    public Post Post { get; set; }

}

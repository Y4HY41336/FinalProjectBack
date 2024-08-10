namespace FinalProject.Models;

public class Trend
{
    public int Id { get; set; }
    public string TrendName { get; set; }
    public int PostCount { get; set; }
    public DateTime CreatedDate { get; set; }


    public ICollection<PostTrend> PostTrends { get;}
}

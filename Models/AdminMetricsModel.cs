
namespace blogAppApi.Models;
using System.ComponentModel.DataAnnotations;


public class AdminMetrics
{
    [Key]
    public int AdminMetricsId { get; set; }

    public DateTime Date { get; set; }

    public int TotalBlogPosts { get; set; }

    public int TotalUpvotes { get; set; }

    public int TotalDownvotes { get; set; }

    public int TotalComments { get; set; }

 
    public ICollection<Blog> Top10Post { get; set; }

   
    public ICollection<User> TopBlogger { get; set; }
}
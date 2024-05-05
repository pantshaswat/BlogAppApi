
using System.ComponentModel.DataAnnotations;
using blogAppApi.Models;

public class Comment
    {
        public int CommentId { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime CommentedDate { get; set; }

        public int UserId { get; set; }
        public User? Commenter { get; set; }

        public int BlogId { get; set; }
        public Blog? Blog { get; set; }

        public ICollection<Reaction>? Reactions { get; set; }
    }
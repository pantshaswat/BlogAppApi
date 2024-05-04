
namespace blogAppApi.Models;
using System.ComponentModel.DataAnnotations;


public class Reaction
    {
        public int ReactionId { get; set; }

        [Required]
        [MaxLength(10)]
        public string Type { get; set; } // "Upvote" or "Downvote"

        public int UserId { get; set; }
        public User Reactor { get; set; }

        public int? BlogId { get; set; }
        public Blog Blog { get; set; }

        public int? CommentId { get; set; }
        public Comment Comment { get; set; }
    }
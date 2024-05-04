namespace blogAppApi.Models;
using System.ComponentModel.DataAnnotations;


public class Blog
    {
        public int BlogId { get; set; }

        [Required]
        [MaxLength(150)]
        public string Title { get; set; }

        [Required]
        public string Body { get; set; }

     
        public ICollection<string> Images { get; set; }

        public DateTime PostedDate { get; set; }

        public int UserId { get; set; }
        public User Author { get; set; }

        public ICollection<Reaction> Reactions { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }


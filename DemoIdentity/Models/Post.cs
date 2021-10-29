using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DemoIdentity.Models
{
    [Table("Post")]
    public class Post
    {
        [Key]
        [Display(Name = "PostId")]
        public int Id { set; get; }

        [Required]
        [Display(Name = "Name")]
        public string Title { set; get; }

        public string Slug { set; get; }

        [DataType(DataType.Text)]
        public string Content { set; get; }

        public DateTime CreatedAt { set; get; }

        public DateTime UpdatedAt { set; get; }

        public int? CategoryId { set; get; }

        [ForeignKey("CategoryId")]
        public Category Category { set; get; }
    }
}

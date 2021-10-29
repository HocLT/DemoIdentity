using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DemoIdentity.Models
{
    [Table("Category")]
    public class Category
    {
        // Key: mô tả thuộc tính là khóa chính
        [Key]
        [Display(Name = "CategoryId")]
        public int Id { set; get; }

        public int? ParentId { set; get; }

        [Required]
        [Display(Name = "Name")]
        public string Title { set; get; }

        public string Slug { set; get; }

        [DataType(DataType.Text)]
        public string Content { set; get; }

        // mô tả List Category con
        public ICollection<Category> CategoryChildren { set; get; }

        // mô ta Category cha
        [ForeignKey("ParentId")]
        public Category ParentCategory { set; get; }

        public ICollection<Post> Posts { set; get; }
    }
}

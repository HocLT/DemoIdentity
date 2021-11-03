using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DemoIdentity.Models
{
    [Table("Product")]
    public class Product
    {
        [Key]
        public int Id { set; get; }
        public string Name { set; get; }
        public string Description { set; get; }
        [DataType(DataType.Currency)]
        public decimal Price { set; get; }
    }
}

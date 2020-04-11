using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyProject.Models
{
    public class Product
    {
        [Key]
        public int ProdunctId { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        [MaxLength(150)]
        public string Description { get; set; }
        [Required]
        public bool OutofStock { get; set; }
        [Required]
        public string ImageUrl { get; set; }
        [Required]
        public double Price { get; set; }
    }
}

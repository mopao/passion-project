using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace passion_project.Models
{
    //this represents a stock entity
    public class Stock
    {
        [Key]
        public int stockId { get; set; }
        [Required]
        public double size { get; set; }
        [Required]
        public int quantity { get; set; }
        [Required]
        public DateTime createdDate { get; set; }
        [ForeignKey("item")]
        public int itemId { get; set; }
        public Item item { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace passion_project.Models
{
    public enum Gender
    {
        Men,
        Women
    }
    //this represents an item entity
    public class Item
    {
        [Key]
        public int itemId { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        [Required]
        public string color { get; set; }
        [Required]
        public Gender gender { get; set; }
        [Required]
        public decimal price { get; set; }
        public string composition { get; set; }
        public string details { get; set; }
        [Required]
        public string image { get; set; }
        [Required]
        [DisplayName("Registered Datetime")]
        public DateTime createdDate { get; set; }
        [ForeignKey("brand")]
        [DisplayName("Brand")]
        public int brandId { get; set; }
        public Brand brand { get; set; }
        public IEnumerable <Stock> stocks { get; set; }


    }
}
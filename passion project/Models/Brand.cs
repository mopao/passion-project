using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace passion_project.Models
{   
    //this class describe a brand entity
    public class Brand
    {
        [Key]
        public int brandId { get; set; }
        [Required]
        public string name { get; set; }
        [DisplayName("Date and Time of registration")]
        [Required]
        public DateTime createdDate { get; set; }
        public ICollection<Item> items { get; set; }

    }

    
}
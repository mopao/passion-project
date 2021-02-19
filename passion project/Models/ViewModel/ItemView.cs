using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace passion_project.Models.ViewModel
{
    public class ItemView
    {
        public IEnumerable<Brand> brands { set; get; }
        public List<Gender> genders { set; get; }
        public Item item { set; get; }
    }
}
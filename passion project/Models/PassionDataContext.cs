using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace passion_project.Models
{
    public class PassionDataContext: DbContext
    {
        public PassionDataContext() : base("name=PassionDataContext") { }

        //set the models as tables in our database.
        public DbSet<Brand> brands { get; set; }
        public DbSet<Item> items { get; set; }
        public DbSet<Stock> stocks { get; set; }


    }

}
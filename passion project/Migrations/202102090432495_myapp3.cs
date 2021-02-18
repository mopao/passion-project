namespace passion_project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class myapp3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Brands", "name", c => c.String(nullable: false));
            AlterColumn("dbo.Items", "color", c => c.String(nullable: false));
            AlterColumn("dbo.Items", "image", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Items", "image", c => c.String());
            AlterColumn("dbo.Items", "color", c => c.String());
            AlterColumn("dbo.Brands", "name", c => c.String());
        }
    }
}

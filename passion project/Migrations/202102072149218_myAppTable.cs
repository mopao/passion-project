namespace passion_project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class myAppTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Brands",
                c => new
                    {
                        brandId = c.Int(nullable: false, identity: true),
                        name = c.String(),
                        createdDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.brandId);
            
            CreateTable(
                "dbo.Items",
                c => new
                    {
                        itemId = c.Int(nullable: false, identity: true),
                        name = c.String(),
                        code = c.String(),
                        color = c.String(),
                        gender = c.Int(nullable: false),
                        price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        composition = c.String(),
                        details = c.String(),
                        image = c.String(),
                        createdDate = c.DateTime(nullable: false),
                        brandId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.itemId)
                .ForeignKey("dbo.Brands", t => t.brandId, cascadeDelete: true)
                .Index(t => t.brandId);
            
            CreateTable(
                "dbo.Stocks",
                c => new
                    {
                        stockId = c.Int(nullable: false, identity: true),
                        size = c.Double(nullable: false),
                        quantity = c.Int(nullable: false),
                        createdDate = c.DateTime(nullable: false),
                        itemId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.stockId)
                .ForeignKey("dbo.Items", t => t.itemId, cascadeDelete: true)
                .Index(t => t.itemId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Stocks", "itemId", "dbo.Items");
            DropForeignKey("dbo.Items", "brandId", "dbo.Brands");
            DropIndex("dbo.Stocks", new[] { "itemId" });
            DropIndex("dbo.Items", new[] { "brandId" });
            DropTable("dbo.Stocks");
            DropTable("dbo.Items");
            DropTable("dbo.Brands");
        }
    }
}

namespace TvTamer.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlternateNames : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AlternateNames",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                        SortOrder = c.Int(nullable: false),
                        SeriesId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TvSeries", t => t.SeriesId)
                .Index(t => t.Name, unique: true, name: "IX_TVSeriesAlternateName")
                .Index(t => t.SeriesId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AlternateNames", "SeriesId", "dbo.TvSeries");
            DropIndex("dbo.AlternateNames", new[] { "SeriesId" });
            DropIndex("dbo.AlternateNames", "IX_TVSeriesAlternateName");
            DropTable("dbo.AlternateNames");
        }
    }
}

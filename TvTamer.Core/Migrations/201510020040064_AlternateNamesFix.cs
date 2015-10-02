namespace TvTamer.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlternateNamesFix : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.TvSeries", new[] { "SeriesId" });
            RenameColumn("dbo.TvSeries", "SeriesId", "TvDbSeriesId");
            CreateIndex("dbo.TvSeries", "TvDbSeriesId", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.TvSeries", new[] { "TvDbSeriesId" });
            RenameColumn("dbo.TvSeries", "TvDbSeriesId", "SeriesId");
            CreateIndex("dbo.TvSeries", "SeriesId", unique: true);
        }
    }
}

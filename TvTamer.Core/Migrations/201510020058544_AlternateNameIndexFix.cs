namespace TvTamer.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlternateNameIndexFix : DbMigration
    {
        public override void Up()
        {
            //RenameColumn(table: "dbo.AlternateNames", name: "TvSeries_Id", newName: "SeriesId");
            //RenameIndex(table: "dbo.AlternateNames", name: "IX_TvSeries_Id", newName: "IX_SeriesId");
        }
        
        public override void Down()
        {
           // RenameIndex(table: "dbo.AlternateNames", name: "IX_SeriesId", newName: "IX_TvSeries_Id");
            //RenameColumn(table: "dbo.AlternateNames", name: "SeriesId", newName: "TvSeries_Id");
        }
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace TvTamer.Core.Models
{
    public class AlternateName
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
    }

    public class AlternateNameMap : EntityTypeConfiguration<AlternateName>
    {

        public AlternateNameMap()
        {
            this.HasKey(t => t.Id);
            this.Property(t => t.Name).HasMaxLength(255).IsRequired()
                .HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute("IX_TVSeriesAlternateName", 1) { IsUnique = true })); ;

            this.Property(t => t.SortOrder).IsRequired();

        }

    }
}
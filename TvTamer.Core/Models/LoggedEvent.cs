using System;
using System.Data.Entity.ModelConfiguration;

namespace TvTamer.Core.Models
{

    public class LoggedEvent
    {
        public int Id { get; set; }
        public string EventType { get; set; }
        public DateTime EventTime { get; set; }
        public string Message { get; set; }

    }

    public class LoggedEventMap : EntityTypeConfiguration<LoggedEvent>
    {

        public LoggedEventMap()
        {
            this.HasKey(t => t.Id);
            this.Property(t => t.EventType).IsRequired().HasMaxLength(15);
            this.Property(t => t.Message).HasMaxLength(255);
            this.Property(t => t.EventTime).IsRequired();
        }

    }
}

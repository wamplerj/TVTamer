using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvTamer.Core.Models
{
    public enum LoggedEventType
    {
        DatabaseUpdate = 1
    }

    public class LoggedEvent
    {

        public int Id { get; set; }
        public LoggedEventType EventType { get; set; }
        public DateTime EventTime { get; set; }
        public string Message { get; set; }

    }

    public class LoggedEventMap : EntityTypeConfiguration<LoggedEvent>
    {

        public LoggedEventMap()
        {
            this.HasKey(t => t.Id);
            this.Property(t => t.EventType).IsRequired();
            this.Property(t => t.Message).HasMaxLength(255);
            this.Property(t => t.EventTime).IsRequired();
        }

    }
}

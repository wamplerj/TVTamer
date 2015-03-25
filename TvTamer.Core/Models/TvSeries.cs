using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace TvTamer.Core.Models
{
    public class TvSeries
    {

        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime FirstAired { get; set; }
        public string Summary { get; set; }
        public string Status { get; set; }

    }
}

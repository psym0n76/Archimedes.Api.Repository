using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Archimedes.Api.Repository
{
    public class PriceLevel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public DateTime TimeStamp { get; set; }

        public double BidPrice { get; set; }

        public double BidPriceRange { get; set; }

        public double AskPrice { get; set; }

        public double AskPriceRange { get; set; }
        
        public string Strategy { get; set; }

        public string TradeType { get; set; }

        public string Active { get; set; }

        public string Market { get; set; }

        public string Granularity { get; set; }

        public DateTime LastUpdated {get; set;}
    }
}
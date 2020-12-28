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

        public string Market { get; set; }

        public string Granularity { get; set; }

        public bool Active { get; set; }

        public bool OutsideOfRange { get; set; }

        public bool LevelBroken { get; set; }

        public bool Trade { get; set; }

        public string BuySell { get; set; }

        public string CandleType { get; set; }



        public string Strategy { get; set; }

        [Column(TypeName = "decimal(18,5)")]
        public double BidPrice { get; set; }

        [Column(TypeName = "decimal(18,5)")]
        public double BidPriceRange { get; set; }

        [Column(TypeName = "decimal(18,5)")]
        public double AskPrice { get; set; }

        [Column(TypeName = "decimal(18,5)")]
        public double AskPriceRange { get; set; }

        public DateTime LastLevelBrokenDate { get; set; }
        public int BookedTrades { get; set; }
        public int LevelsBroken { get; set; }


        public DateTime TimeStamp { get; set; }
        public DateTime LastUpdated {get; set;}
    }
}
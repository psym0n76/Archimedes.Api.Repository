using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Archimedes.Api.Repository
{
    public class Candle
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }


        public string Market { get; set; }

        public string MarketId { get; set; }

        public string Granularity { get; set; }


        [Column(TypeName = "decimal(18,5)")]
        public decimal BidOpen { get; set; }

        [Column(TypeName = "decimal(18,5)")]
        public decimal BidClose { get; set; }


        [Column(TypeName = "decimal(18,5)")]
        public decimal BidHigh { get; set; }

        [Column(TypeName = "decimal(18,5)")]
        public decimal BidLow { get; set; }

        [Column(TypeName = "decimal(18,5)")]
        public decimal AskOpen { get; set; }

        [Column(TypeName = "decimal(18,5)")]
        public decimal AskClose { get; set; }

        [Column(TypeName = "decimal(18,5)")]
        public decimal AskHigh { get; set; }

        [Column(TypeName = "decimal(18,5)")]
        public decimal AskLow { get; set; }


        public double TickQty { get; set; }

        public DateTime TimeStamp { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
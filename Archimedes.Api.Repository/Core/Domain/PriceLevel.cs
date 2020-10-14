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

        public double Price { get; set; }

        public double PriceRange { get; set; }

        public string TradeType { get; set; }

        public string Active { get; set; }

        public string Market { get; set; }

        public string Granularity { get; set; }

        public DateTime LastUpdated {get; set;}
    }
}
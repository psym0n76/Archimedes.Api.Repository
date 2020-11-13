using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Archimedes.Api.Repository
{
    public class Price
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long Id { get; set; }

        public string  Market { get; set; }

        public string  Granularity { get; set; }

        [Column(TypeName = "decimal(18,5)")]
        public decimal Bid { get; set; }

        [Column(TypeName = "decimal(18,5)")]
        public decimal Ask { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}
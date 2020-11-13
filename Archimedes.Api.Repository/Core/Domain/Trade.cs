using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Archimedes.Api.Repository
{
    public class Trade
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public string BuySell { get; set; }
        public string Market { get; set; }
        public string Strategy { get; set; }

        public bool Success { get; set; }

        [Column(TypeName = "decimal(18, 5)")] public decimal EntryPrice { get; set; }
        [Column(TypeName = "decimal(18, 5)")] public decimal TargetPrice { get; set; }
        [Column(TypeName = "decimal(18, 5)")] public decimal ClosePrice { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
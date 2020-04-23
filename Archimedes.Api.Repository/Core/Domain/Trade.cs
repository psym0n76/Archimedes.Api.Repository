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

        public string Direction { get; set; }
        public string Granularity { get; set; }
        public string Market { get; set; }
        public string Strategy { get; set; }

        [Column(TypeName = "decimal(5, 5)")] public decimal OpenPrice { get; set; }
        [Column(TypeName = "decimal(5, 5)")] public decimal ClosePrice { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
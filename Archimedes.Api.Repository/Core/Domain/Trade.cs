﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Archimedes.Api.Repository
{
    public class Trade
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public string TradeGroupId { get; set; }

        public string Market { get; set; }
        public string Strategy { get; set; }
        public DateTime PriceLevelTimestamp { get; set; }
        public string BuySell { get; set; }

        [Column(TypeName = "decimal(18, 5)")] public decimal TargetPrice { get; set; }

        [Column(TypeName = "decimal(18, 5)")] public decimal EntryPrice { get; set; }

        [Column(TypeName = "decimal(18, 5)")] public decimal ClosePrice { get; set; }

        [Column(TypeName = "decimal(18, 5)")] public decimal Price { get; set; }

        public double RiskReward { get; set; }

        public bool Success { get; set; }

        public DateTime TimeStamp { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
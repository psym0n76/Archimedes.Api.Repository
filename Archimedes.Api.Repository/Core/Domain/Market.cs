using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Archimedes.Api.Repository
{
    public class Market
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public int Interval { get; set; }
        public string TimeFrame { get; set; }
        public bool Active { get; set; }
        public DateTime MaxDate { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
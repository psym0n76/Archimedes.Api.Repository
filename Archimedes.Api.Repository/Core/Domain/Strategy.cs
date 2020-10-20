using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Archimedes.Api.Repository
{
    public class Strategy
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public string  Name { get; set; }

        public string  Market { get; set; }

        public string  Granularity { get; set; }

        public bool  Active { get; set; }

        public DateTime StartDate { get; set; }  

        public DateTime EndDate { get; set; }  

        public double Count { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
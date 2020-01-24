using System;

namespace Archimedes.Fx.Api.Repository
{
    public class Price
    {
        /// <summary>
        /// The price timestamp.
        /// </summary>
        public long Id { get; set; }


        /// <summary>
        /// The market
        /// </summary>
        public string  Market { get; set; }

        /// <summary>
        /// The market
        /// </summary>
        public string  Granularity { get; set; }

        /// <summary>
        /// The candle BidOpen price.
        /// </summary>
        public double BidOpen { get; set; }

        /// <summary>
        /// The candle BidClose price.
        /// </summary>
        public double BidClose { get; set; }

        /// <summary>
        /// The candle BidHigh price.
        /// </summary>
        public double BidHigh { get; set; }

        /// <summary>
        /// The candle BidLow price.
        /// </summary>
        public double BidLow { get; set; }

        /// <summary>
        /// The candle AskOpen price.
        /// </summary>
        public double AskOpen { get; set; }

        /// <summary>
        /// The candle AskClose price.
        /// </summary>
        public double AskClose { get; set; }

        /// <summary>
        /// The candle BidHigh price.
        /// </summary>
        public double AskHigh { get; set; }

        /// <summary>
        /// The candle AskLow price.
        /// </summary>
        public double AskLow { get; set; }

        /// <summary>
        /// The candle tick quantity value.
        /// </summary>
        public double TickQty { get; set; }

        /// <summary>
        /// The price timestamp.
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}
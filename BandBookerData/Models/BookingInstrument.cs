using System;
using System.Collections.Generic;

namespace BandBookerData.Models
{
    public partial class BookingInstrument
    {
        public int BookingInstrumentId { get; set; }
        public int BookingId { get; set; }
        public int InstrumentId { get; set; }

        public virtual Booking Booking { get; set; }
        public virtual Instrument Instrument { get; set; }
    }
}

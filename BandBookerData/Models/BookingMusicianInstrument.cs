using System;
using System.Collections.Generic;

namespace BandBookerData.Models
{
    public partial class BookingMusicianInstrument
    {
        public int BookingMusicianInstrumentId { get; set; }
        public int BookingId { get; set; }
        public int MusicianId { get; set; }
        public int? InstrumentId { get; set; }
        public bool? Response { get; set; }
        public DateTime? ResponseDate { get; set; }
        public string ResponseReason { get; set; }

        public virtual Booking Booking { get; set; }
        public virtual Instrument Instrument { get; set; }
        public virtual Musician Musician { get; set; }
    }
}

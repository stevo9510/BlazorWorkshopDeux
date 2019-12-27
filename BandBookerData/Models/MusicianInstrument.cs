using System;
using System.Collections.Generic;

namespace BandBookerData.Models
{
    public partial class MusicianInstrument
    {
        public int MusicianInstrumentId { get; set; }
        public int MusicianId { get; set; }
        public int InstrumentId { get; set; }

        public virtual Instrument Instrument { get; set; }
        public virtual Musician Musician { get; set; }
    }
}

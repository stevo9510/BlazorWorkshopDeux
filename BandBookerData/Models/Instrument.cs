using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BandBookerData.Models
{
    public partial class Instrument
    {
        public Instrument()
        {
            BookingInstrument = new HashSet<BookingInstrument>();
            BookingMusicianInstrument = new HashSet<BookingMusicianInstrument>();
            MusicianInstrument = new HashSet<MusicianInstrument>();
        }

        public int InstrumentId { get; set; }
        
        [Required]
        [StringLength(50, ErrorMessage = "Name is too long.")]
        public string Name { get; set; }

        public virtual ICollection<BookingInstrument> BookingInstrument { get; set; }
        public virtual ICollection<BookingMusicianInstrument> BookingMusicianInstrument { get; set; }
        public virtual ICollection<MusicianInstrument> MusicianInstrument { get; set; }
    }
}

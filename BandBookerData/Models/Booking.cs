using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BandBookerData.Models
{
    public partial class Booking
    {
        public Booking()
        {
            BookingInstrument = new HashSet<BookingInstrument>();
            BookingMusicianInstrument = new HashSet<BookingMusicianInstrument>();
        }

        public int BookingId { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Name is too long.")] 
        public string Name { get; set; }
        
        public bool Booked { get; set; }
        public DateTime? DateBooked { get; set; }
        public int? VenueId { get; set; }
        public DateTime DateAndTime { get; set; }
        public string PayOffering { get; set; }
        public int? BandId { get; set; }

        public virtual Band Band { get; set; }
        public virtual Venue Venue { get; set; }
        public virtual ICollection<BookingInstrument> BookingInstrument { get; set; }
        public virtual ICollection<BookingMusicianInstrument> BookingMusicianInstrument { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BandBookerData.Models
{
    public partial class Musician
    {
        public Musician()
        {
            BandMusician = new HashSet<BandMusician>();
            BookingMusicianInstrument = new HashSet<BookingMusicianInstrument>();
            MusicianInstrument = new HashSet<MusicianInstrument>();
        }

        public int MusicianId { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Name is too long.")]
        public string Name { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Email is too long.")]
        [EmailAddress(ErrorMessage = "Not a valid Email Address")]
        public string Email { get; set; }

        [Phone]
        [StringLength(50, ErrorMessage = "Phone number is too long.")]
        public string MobilePhone { get; set; }
        
        public string Bio { get; set; }

        [StringLength(255, ErrorMessage = "Photo URL is too long.")]
        public string PhotoUrl { get; set; }

        public virtual ICollection<BandMusician> BandMusician { get; set; }
        public virtual ICollection<BookingMusicianInstrument> BookingMusicianInstrument { get; set; }
        public virtual ICollection<MusicianInstrument> MusicianInstrument { get; set; }
    }
}

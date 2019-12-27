using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BandBookerData.Models
{
    public partial class Band
    {
        public Band()
        {
            BandMusician = new HashSet<BandMusician>();
            Booking = new HashSet<Booking>();
        }

        public int BandId { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Name is too long.")] 
        public string Name { get; set; }

        public string Description { get; set; }

        public virtual ICollection<BandMusician> BandMusician { get; set; }
        public virtual ICollection<Booking> Booking { get; set; }
    }
}

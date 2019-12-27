using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BandBookerData.Models
{
    public partial class Venue
    {
        public Venue()
        {
            Booking = new HashSet<Booking>();
        }

        public int VenueId { get; set; }
        
        [Required]
        [StringLength(50, ErrorMessage = "Name is too long.")]
        public string Name { get; set; }
        
        public string Description { get; set; }

        [StringLength(50, ErrorMessage = "Contact Name is too long.")]
        public string ContactName { get; set; }

        [EmailAddress(ErrorMessage = "Not a valid Email Address")]
        [StringLength(50, ErrorMessage = "Email is too long.")]
        public string ContactEmail { get; set; }

        [Phone]
        public string ContactMobilePhone { get; set; }

        [StringLength(255, ErrorMessage = "Photo URL is too long.")]
        public string PhotoUrl { get; set; }

        [StringLength(50, ErrorMessage = "Address is too long.")]
        public string Address { get; set; }

        [StringLength(50, ErrorMessage = "Address is too long.")]
        public string Address2 { get; set; }

        [StringLength(50, ErrorMessage = "City is too long.")]
        public string City { get; set; }

        [StringLength(50, ErrorMessage = "State is too long.")]
        public string State { get; set; }

        [StringLength(20, ErrorMessage = "Zip is too long.")]
        public string Zip { get; set; }
        
        [Phone]
        public string Phone { get; set; }
        
        [Url]
        public string Website { get; set; }

        public virtual ICollection<Booking> Booking { get; set; }
    }
}

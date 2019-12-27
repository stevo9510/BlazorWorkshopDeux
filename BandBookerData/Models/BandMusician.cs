using System;
using System.Collections.Generic;

namespace BandBookerData.Models
{
    public partial class BandMusician
    {
        public int BandMusicianId { get; set; }
        public int BandId { get; set; }
        public int MusicianId { get; set; }

        public virtual Band Band { get; set; }
        public virtual Musician Musician { get; set; }
    }
}

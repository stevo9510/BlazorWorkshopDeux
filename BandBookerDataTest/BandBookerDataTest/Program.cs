using System;
using System.Linq;
using BandBookerData;
using BandBookerData.Models;


namespace BandBookerDataTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // Un-comment to start over
            //DataManager.DeleteAll();


            // Add Guitar
            var guitar = (from i in DataManager.Instruments
                          where i.Name == "Guitar"
                          select i).FirstOrDefault();
            if (guitar == null)
            {
                guitar = new Instrument()
                {
                    Name = "Guitar"
                };
                DataManager.AddInstrument(guitar);
            }


            // Change the name from Guitar to Electric Guitar
            //guitar.Name = "Electric Guitar";
            //guitar = DataManager.UpdateInstrument(guitar);


            Console.WriteLine("Instrument: {0} with InstrumentId {1}",
                guitar.Name, guitar.InstrumentId);


            // Add Keyboards
            var keyboards = (from i in DataManager.Instruments
                             where i.Name == "Keyboards"
                             select i).FirstOrDefault();
            if (keyboards == null)
            {
                keyboards = new Instrument()
                {
                    Name = "Keyboards"
                };
                DataManager.AddInstrument(keyboards);
            }
            Console.WriteLine("Instrument: {0} with InstrumentId {1}",
              keyboards.Name, keyboards.InstrumentId);


            // Delete the guitar
            //DataManager.DeleteInstrument(guitar);
            //Console.WriteLine("Instrument: {0} deleted", guitar.Name);


            // Musician
            var musician = (from m in DataManager.Musicians
                            where m.Name == "Harry Chap"
                            select m).FirstOrDefault();
            if (musician == null)
            {
                musician = new Musician()
                {
                    Name = "Harry Chap",
                    Bio = "Harry Chap plays a mean guitar and sings his butt off",
                    Email = "harry@harrychap.com",
                    MobilePhone = "888-555-1212",
                    PhotoUrl = "",
                };
                musician = DataManager.AddMusician(musician, guitar);
            }


            //string message = DataManager.DeleteMusician(musician, true);
            //if (message != "")
            //    Console.WriteLine(message);
            //else
            //    Console.WriteLine("Musician {0} deleted", musician.Name);
            //return;


            //musician.Name = "Hairy Chap";
            //musician = DataManager.UpdateMusician(musician);


            Console.WriteLine("Musician: {0}", musician.Name);
            foreach (var mi in musician.MusicianInstrument)
            {
                Console.WriteLine("  plays {0}", mi.Instrument.Name);
            }


            // Band
            var band = (from x in DataManager.Bands
                        where x.Name == "The Harry Chap Band"
                        select x).FirstOrDefault();
            if (band == null)
            {
                band = new Band()
                {
                    Name = "The Harry Chap Band",
                    Description = "These guys can really jam!"
                };
                band = DataManager.AddBand(band, musician);
            }
            Console.WriteLine("Band: {0}", band.Name);


            // Venue
            var venue = (from x in DataManager.Venues
                         where x.Name == "The Spotted Horse"
                         select x).FirstOrDefault();
            if (venue == null)
            {
                venue = new Venue()
                {
                    Name = "The Spotted Horse"
                };
                venue = DataManager.AddVenue(venue);
            }
            Console.WriteLine("Venue: {0}", venue.Name);




            // Booking
            var booking = (from x in DataManager.Bookings
                           where x.BandId == band.BandId
                           select x).FirstOrDefault();
            if (booking == null)
            {
                booking = new Booking()
                {
                    Name = "Harry Chap Band at the Spotted Horse",
                    BandId = band.BandId,
                    VenueId = venue.VenueId,
                    DateAndTime = DateTime.Now.AddDays(30),
                    PayOffering = "50 bucks"
                };
                booking = DataManager.AddBooking(booking);
            }
            Console.WriteLine("Booking: {0} at {1} on {2}",
                     band.Name, venue.Name,
                     booking.DateAndTime.ToLongDateString());
        }
    }
}
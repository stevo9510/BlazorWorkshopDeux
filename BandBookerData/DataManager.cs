using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using BandBookerData.Models;
using System.Linq;

namespace BandBookerData
{
    public class DataManager
    {
        /// <summary>
        /// A handy utility to delete bookings
        /// </summary>
        public static void DeleteAllBookings()
        {
            using (var context = new BandBookerContext())
            {

                foreach (var item in context.BookingMusicianInstrument.ToArray())
                {
                    context.BookingMusicianInstrument.Remove(item);
                }
                context.SaveChanges();

                foreach (var item in context.BookingInstrument.ToArray())
                {
                    context.BookingInstrument.Remove(item);
                }
                context.SaveChanges();


                foreach (var item in context.Booking.ToArray())
                {
                    context.Booking.Remove(item);
                }
                context.SaveChanges();
            }
        }

        /// <summary>
        /// This is dangerous! Deletes all records from all tables.
        /// Good for testing.
        /// </summary>
        public static void DeleteAll()
        {
            using (var context = new BandBookerContext())
            {
                foreach (var item in context.BookingMusicianInstrument.ToArray())
                {
                    context.BookingMusicianInstrument.Remove(item);
                }
                context.SaveChanges();

                foreach (var item in context.BookingInstrument.ToArray())
                {
                    context.BookingInstrument.Remove(item);
                }
                context.SaveChanges();


                foreach (var item in context.Booking.ToArray())
                {
                    context.Booking.Remove(item);
                }
                context.SaveChanges();

                foreach (var item in context.Venue.ToArray())
                {
                    context.Venue.Remove(item);
                }
                context.SaveChanges();

                foreach (var item in context.BandMusician.ToArray())
                {
                    context.BandMusician.Remove(item);
                }
                context.SaveChanges();

                foreach (var item in context.Band.ToArray())
                {
                    context.Band.Remove(item);
                }
                context.SaveChanges();

                foreach (var mi in context.MusicianInstrument.ToArray())
                {
                    context.MusicianInstrument.Remove(mi);
                }
                context.SaveChanges();

                foreach (var item in context.Musician.ToArray())
                {
                    context.Musician.Remove(item);
                }
                context.SaveChanges();

                foreach (var item in context.Instrument.ToArray())
                {
                    context.Instrument.Remove(item);
                }
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Return all instruments
        /// </summary>
        public static List<Instrument> Instruments
        {
            get
            {
                using (var context = new BandBookerContext())
                {
                    return context.Instrument.ToList<Instrument>();
                }
            }
        }

        /// <summary>
        /// Add an instrument
        /// </summary>
        /// <param name="Instrument"></param>
        /// <returns></returns>
        public static Instrument AddInstrument(Instrument Instrument)
        {
            using (var context = new BandBookerContext())
            {
                context.Instrument.Add(Instrument);
                context.SaveChanges();
            }
            return Instrument;
        }

        /// <summary>
        /// Update an instrument
        /// </summary>
        /// <param name="Instrument"></param>
        /// <returns></returns>
        public static Instrument UpdateInstrument(Instrument Instrument)
        {
            using (var context = new BandBookerContext())
            {
                var inst = (from x in context.Instrument
                            where x.InstrumentId == Instrument.InstrumentId
                            select x).FirstOrDefault();
                if (inst != null)
                {
                    inst.Name = Instrument.Name;
                    context.SaveChanges();
                    return inst;
                }
            }
            return Instrument;
        }

        /// <summary>
        /// Delete an instrument
        /// </summary>
        /// <param name="Instrument">the instrument to delete</param>
        /// <param name="RemoveAllAssociations">Set to true to force cascading delete</param>
        /// <returns></returns>
        public static string DeleteInstrument(Instrument Instrument,
            bool RemoveAllAssociations = false)
        {
            try
            {
                using (var context = new BandBookerContext())
                {
                    var musicianInstruments =
                        from x in context.MusicianInstrument
                        where x.InstrumentId == Instrument.InstrumentId
                        select x;

                    var bookingMusicianInstruments =
                        from x in context.BookingMusicianInstrument
                        where x.InstrumentId == Instrument.InstrumentId
                        select x;

                    if (RemoveAllAssociations)
                    {
                        foreach (var child in musicianInstruments)
                        {
                            context.MusicianInstrument.Remove(child);
                        }

                        foreach (var bmi in bookingMusicianInstruments)
                        {
                            context.BookingMusicianInstrument.Remove(bmi);
                        }
                        context.SaveChanges();
                    }
                    else
                    {
                        if (musicianInstruments.Count() > 0)
                        {
                            return "The instrument can not be deleted because " +
                                "one or more musicians plays it.";
                        }
                        else if (bookingMusicianInstruments.Count() > 0)
                        {
                            return "The instrument can not be deleted because " +
                                "it is required at one or more gigs.";
                        }

                    }
                    var inst = (from x in context.Instrument
                                where x.InstrumentId == Instrument.InstrumentId
                                select x).FirstOrDefault();
                    if (inst != null)
                    {
                        context.Instrument.Remove(inst);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }

        /// <summary>
        /// Return all musicians with related data
        /// </summary>
        public static List<Musician> Musicians
        {
            get
            {
                using (var context = new BandBookerContext())
                {
                    var list = context.Musician
                        .Include(m => m.MusicianInstrument)
                        .ThenInclude(i => i.Instrument)
                        .Include(m => m.BookingMusicianInstrument)
                        .ThenInclude(bmi => bmi.Musician)
                        .ToList<Musician>();
                    return list;
                }
            }
        }

        /// <summary>
        /// Add a musician with optional instruments
        /// </summary>
        /// <param name="Musician"></param>
        /// <param name="Instrument">Optional instrument to add</param>
        /// <param name="Instruments">Optional list of instruments to add</param>
        /// <returns></returns>
        public static Musician AddMusician(Musician Musician,
            Instrument Instrument = null,
            List<Instrument> Instruments = null)
        {
            var musicianInstruments = Musician.MusicianInstrument.ToArray();
            if (musicianInstruments.Length > 0)
            {
                Musician.MusicianInstrument.Clear();
                Instruments = new List<Instrument>();
                foreach (var mi in musicianInstruments)
                {
                    Instruments.Add(mi.Instrument);
                }
            }

            using (var context = new BandBookerContext())
            {
                context.Musician.Add(Musician);
                context.SaveChanges();
            }
            if (Instruments != null)
            {
                foreach (var inst in Instruments)
                {
                    AddInstrumentToMusician(Musician, inst);
                }
            }
            else if (Instrument != null)
            {
                AddInstrumentToMusician(Musician, Instrument);
            }
            using (var context = new BandBookerContext())
            {
                return (from x in Musicians
                        where x.MusicianId == Musician.MusicianId
                        select x).FirstOrDefault();
            }
        }

        /// <summary>
        /// Adds an instrument to a musician
        /// </summary>
        /// <param name="Musician"></param>
        /// <param name="Instrument"></param>
        public static void AddInstrumentToMusician(Musician Musician,
            Instrument Instrument)
        {
            using (var context = new BandBookerContext())
            {
                var m = (from x in context.Musician
                         where x.MusicianId == Musician.MusicianId
                         select x).FirstOrDefault();
                var currentMI = (from x in context.MusicianInstrument
                                 where x.MusicianId == Musician.MusicianId
                                 && x.InstrumentId == Instrument.InstrumentId
                                 select x).FirstOrDefault();
                if (currentMI == null)
                {
                    var inst = (from x in context.Instrument
                                where x.InstrumentId == Instrument.InstrumentId
                                select x).FirstOrDefault();
                    var mi = new MusicianInstrument()
                    {
                        Musician = m,
                        Instrument = inst
                    };
                    m.MusicianInstrument.Add(mi);
                    context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Bookings define a list of instruments that are required.
        /// Since more than one band member can play an instrument,
        /// This returns a list of musicians in the band that play
        /// the required instruments
        /// </summary>
        /// <param name="Booking"></param>
        /// <returns></returns>
        public static List<Musician> GetMusicainsToInviteToBooking(Booking Booking)
        {
            var list = new List<Musician>();

            var dbBooking = (from x in Bookings
                             where x.BookingId == Booking.BookingId
                             select x).FirstOrDefault();

            foreach (var bm in dbBooking.Band.BandMusician)
            {
                foreach (var mi in bm.Musician.MusicianInstrument)
                {
                    var inst = (from x in dbBooking.BookingInstrument
                                where x.InstrumentId == mi.InstrumentId
                                select x).FirstOrDefault();
                    if (inst != null)
                    {
                        list.Add(mi.Musician);
                        break;
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// Updates a musicain
        /// </summary>
        /// <param name="Musician"></param>
        /// <returns></returns>
        public static Musician UpdateMusician(Musician Musician)
        {
            Musician dbMusician;

            using (var context = new BandBookerContext())
            {
                dbMusician = (from x in context.Musician
                              where x.MusicianId == Musician.MusicianId
                              select x).FirstOrDefault();
                if (dbMusician != null)
                {
                    dbMusician.Name = Musician.Name;
                    dbMusician.Bio = Musician.Bio;
                    dbMusician.Email = Musician.Email;
                    dbMusician.MobilePhone = Musician.MobilePhone;
                    dbMusician.PhotoUrl = Musician.PhotoUrl;

                    // MusicianInstruments
                    var mis = from x in context.MusicianInstrument
                              where x.MusicianId == dbMusician.MusicianId
                              select x;

                    foreach (var mi in mis.ToArray())
                    {
                        context.MusicianInstrument.Remove(mi);
                    }
                    context.SaveChanges();
                }
            }
            foreach (var mi in Musician.MusicianInstrument)
            {
                AddInstrumentToMusician(Musician, mi.Instrument);
            }
            return (from x in Musicians
                    where x.MusicianId == Musician.MusicianId
                    select x).First();
        }

        /// <summary>
        /// Deletes a musician, optionally removing all associations
        /// </summary>
        /// <param name="Musician"></param>
        /// <param name="RemoveAllAssociations"></param>
        /// <returns></returns>
        public static string DeleteMusician(Musician Musician,
            bool RemoveAllAssociations = false)
        {
            try
            {
                using (var context = new BandBookerContext())
                {
                    var bands = from x in context.BandMusician
                                where x.MusicianId == Musician.MusicianId
                                select x;
                    var musicianInstruments =
                        from x in context.MusicianInstrument
                        where x.MusicianId == Musician.MusicianId
                        select x;
                    var bookingMusicianInstruments =
                        from x in context.BookingMusicianInstrument
                        where x.MusicianId == Musician.MusicianId
                        select x;

                    if (RemoveAllAssociations)
                    {
                        foreach (var child in bands)
                        {
                            context.BandMusician.Remove(child);
                        }

                        foreach (var child in musicianInstruments)
                        {
                            context.MusicianInstrument.Remove(child);
                        }

                        foreach (var child in bookingMusicianInstruments)
                        {
                            context.BookingMusicianInstrument.Remove(child);
                        }

                        context.SaveChanges();
                    }
                    else
                    {
                        if (bands.Count() > 0)
                        {
                            return "The musician can not be deleted because " +
                                "they belong to one or more bands.";
                        }
                        else if (musicianInstruments.Count() > 0)
                        {
                            return "The musician can not be deleted because " +
                                "they have associated instruments.";
                        }
                        else if (bookingMusicianInstruments.Count() > 0)
                        {
                            return "The musician can not be deleted " +
                                "because they have gigs.";
                        }
                    }

                    var inst = (from x in context.Musician
                                where x.MusicianId == Musician.MusicianId
                                select x).FirstOrDefault();
                    if (inst != null)
                    {
                        context.Musician.Remove(inst);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }

        /// <summary>
        /// Returns all bands
        /// </summary>
        public static List<Band> Bands
        {
            get
            {
                using (var context = new BandBookerContext())
                {
                    var list = context.Band
                        .Include(x => x.BandMusician)
                        .ThenInclude(bm => bm.Musician)
                        .ThenInclude(bm => bm.MusicianInstrument)
                        .ThenInclude(mi => mi.Instrument)
                        .ToList<Band>();
                    return list;
                }
            }
        }

        /// <summary>
        /// Adds a band with optional musicians
        /// </summary>
        /// <param name="Band"></param>
        /// <param name="Musician"></param>
        /// <param name="Musicians"></param>
        /// <returns></returns>
        public static Band AddBand(Band Band, Musician Musician = null,
            List<Musician> Musicians = null)
        {
            var bandMusicians = Band.BandMusician.ToArray();
            if (bandMusicians.Length > 0)
            {
                Band.BandMusician.Clear();
                Musicians = new List<Musician>();
                foreach (var bm in bandMusicians)
                {
                    Musicians.Add(bm.Musician);
                }
            }

            using (var context = new BandBookerContext())
            {
                context.Band.Add(Band);
                context.SaveChanges();
            }
            if (Musicians != null)
            {
                foreach (var m in Musicians)
                {
                    AddMusicianToBand(Band, m);
                }
            }
            else if (Musician != null)
            {
                AddMusicianToBand(Band, Musician);
            }
            using (var context = new BandBookerContext())
            {
                return (from x in Bands
                        where x.BandId == Band.BandId
                        select x).FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets all the instruments played in a band
        /// </summary>
        /// <param name="Band"></param>
        /// <returns></returns>
        public static List<Instrument> GetBandInstruments(Band Band)
        {
            var instruments = new List<Instrument>();
            var bandWithMusicians = (from x in Bands
                                     where x.BandId == Band.BandId
                                     select x).FirstOrDefault();
            foreach (var bm in bandWithMusicians.BandMusician)
            {
                foreach (var i in bm.Musician.MusicianInstrument)
                {
                    // only add if not already there
                    if (!instruments.Contains(i.Instrument))
                        instruments.Add(i.Instrument);
                }
            }
            return instruments;
        }

        /// <summary>
        /// Adds a musician to a band
        /// </summary>
        /// <param name="Band"></param>
        /// <param name="Musician"></param>
        /// <returns></returns>
        public static Band AddMusicianToBand(Band Band, Musician Musician)
        {
            using (var context = new BandBookerContext())
            {
                var dbBand =
                    (from x in context.Band
                     where x.BandId == Band.BandId
                     select x).FirstOrDefault();

                var currentBM =
                    (from x in context.BandMusician
                     where x.BandId == Band.BandId
                     && x.MusicianId == Musician.MusicianId
                     select x).FirstOrDefault();

                if (currentBM == null)
                {
                    var dbMusician =
                        (from x in context.Musician
                         where x.MusicianId == Musician.MusicianId
                         select x).FirstOrDefault();
                    var bm = new BandMusician()
                    {
                        Band = dbBand,
                        Musician = dbMusician
                    };
                    dbBand.BandMusician.Add(bm);
                    context.SaveChanges();
                }
                var thisband = (from x in Bands
                                where x.BandId == Band.BandId
                                select x).FirstOrDefault();
                return thisband;
            }

        }

        /// <summary>
        /// Updates a band
        /// </summary>
        /// <param name="Band"></param>
        /// <returns></returns>
        public static Band UpdateBand(Band Band)
        {
            Band dbBand;

            using (var context = new BandBookerContext())
            {
                dbBand = (from x in context.Band
                          where x.BandId == Band.BandId
                          select x).FirstOrDefault();

                if (dbBand != null)
                {
                    dbBand.Name = Band.Name;
                    dbBand.Description = Band.Description;

                    //BandMusicians
                    var bms = from x in context.BandMusician
                              where x.BandId == dbBand.BandId
                              select x;

                    foreach (var bm in bms.ToArray())
                    {
                        context.BandMusician.Remove(bm);
                    }
                    context.SaveChanges();
                }
            }
            foreach (var bm in Band.BandMusician)
            {
                AddMusicianToBand(Band, bm.Musician);
            }
            return (from x in Bands
                    where x.BandId == Band.BandId
                    select x).First();
        }

        /// <summary>
        /// Get all the bookings that this musician has responded to
        /// </summary>
        /// <param name="Musician"></param>
        /// <returns></returns>
        public static List<Booking> GetAllBookingsByMusician(Musician Musician)
        {
            var list = new List<Booking>();
            using (var context = new BandBookerContext())
            {
                // find all the bands this musician belongs to
                var bands = from x in BandMusicians
                            where x.MusicianId == Musician.MusicianId
                            select x.Band;

                foreach (var band in bands)
                {
                    // find all bookings for this band
                    var bookings = from x in Bookings
                                   where x.BandId == band.BandId
                                   select x;

                    foreach (var booking in bookings)
                    {
                        list.Add(booking);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Get bookings a musician has responded to, 
        /// optionaly specifing whether to filter by 
        /// being booked or not booked
        /// </summary>
        /// <param name="Musician"></param>
        /// <param name="Booked">Optional</param>
        /// <returns></returns>
        public static List<Booking> GetBookingsByMusician(Musician Musician,
            bool Booked = false)
        {
            var bookings = GetAllBookingsByMusician(Musician);
            var list = from x in bookings
                       where x.Booked == Booked
                       select x;
            return list.ToList<Booking>();
        }

        /// <summary>
        /// Get a list of Instruments given a booking and musician
        /// </summary>
        /// <param name="Booking"></param>
        /// <param name="Musician"></param>
        /// <returns></returns>
        public static List<Instrument> GetInstrumentsByBookingAndMusician(
            Booking Booking, Musician Musician)
        {
            var list = new List<Instrument>();

            var myInstruments = from x in Musician.MusicianInstrument
                                select x.Instrument;

            var requiredInstrumentIds = from x in Booking.BookingInstrument
                                        select x.InstrumentId;

            foreach (var instrument in myInstruments)
            {
                if (requiredInstrumentIds.Contains(instrument.InstrumentId))
                {
                    list.Add(instrument);
                }
            }

            return list;
        }

        /// <summary>
        /// Gets a list of bookings that a musician has not responed to
        /// </summary>
        /// <param name="Musician"></param>
        /// <returns></returns>
        public static List<Booking> GetBookingsMusicianNeedsToRespondTo(
            Musician Musician)
        {
            var bookings = GetAllBookingsByMusician(Musician);
            var list = new List<Booking>();

            foreach (var booking in bookings)
            {
                var responses =
                    from x in booking.BookingMusicianInstrument
                    where x.MusicianId == Musician.MusicianId
                    && x.ResponseDate != null
                    select x;

                if (responses.Count() == 0)
                {
                    if (!list.Contains(booking))
                    {
                        var instruments =
                            GetInstrumentsByBookingAndMusician(booking, Musician);
                        if (instruments.Count > 0)
                        {
                            list.Add(booking);
                        }
                    }
                }
            }
            return list.ToList<Booking>();

        }

        /// <summary>
        /// Gets bookings by musician,
        /// optionally specifying to filter by
        /// booked or not booked,
        /// and whether the response is yes or no
        /// </summary>
        /// <param name="Musician"></param>
        /// <param name="Booked"></param>
        /// <param name="Response"></param>
        /// <returns></returns>
        public static List<Booking> GetBookingsByMusician(Musician Musician,
            bool Booked, bool Response)
        {
            var bookings = GetBookingsByMusician(Musician, Booked);
            var list = new List<Booking>();
            foreach (var booking in bookings)
            {
                foreach (var bmi in booking.BookingMusicianInstrument)
                {
                    if (bmi.MusicianId == Musician.MusicianId & bmi.Response == Response)
                    {
                        list.Add(booking);
                    }
                }
            }
            return list.ToList<Booking>();
        }

        /// <summary>
        /// Gets all the musicians in a band
        /// </summary>
        public static List<BandMusician> BandMusicians
        {
            get
            {
                var list = new List<BandMusician>();
                using (var context = new BandBookerContext())
                {
                    list = (from x in context.BandMusician
                            .Include(bm => bm.Band)
                            .Include(bm => bm.Musician)
                            select x).ToList<BandMusician>();
                }
                return list;
            }
        }

        /// <summary>
        /// Deletes a band,
        /// optionally removing associations
        /// </summary>
        /// <param name="Band"></param>
        /// <param name="RemoveAllAssociations"></param>
        /// <returns></returns>
        public static string DeleteBand(Band Band, bool RemoveAllAssociations)
        {
            try
            {
                using (var context = new BandBookerContext())
                {
                    var matches = from x in context.BandMusician
                                  where x.BandId == Band.BandId
                                  select x;
                    if (RemoveAllAssociations)
                    {
                        foreach (var child in matches)
                        {
                            context.BandMusician.Remove(child);
                        }
                        context.SaveChanges();
                    }
                    else
                    {
                        if (matches.Count() > 0)
                        {
                            return "The band can not be deleted " +
                                "because musicians belong to it.";
                        }
                    }

                    var inst = (from x in context.Band
                                where x.BandId == Band.BandId
                                select x).FirstOrDefault();
                    if (inst != null)
                    {
                        context.Band.Remove(inst);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }

        /// <summary>
        /// Return all venues
        /// </summary>
        public static List<Venue> Venues
        {
            get
            {
                using (var context = new BandBookerContext())
                {
                    return context.Venue.ToList<Venue>();
                }
            }
        }

        /// <summary>
        /// Adds a venue
        /// </summary>
        /// <param name="venue"></param>
        /// <returns></returns>
        public static Venue AddVenue(Venue venue)
        {
            using (var context = new BandBookerContext())
            {
                context.Venue.Add(venue);
                context.SaveChanges();

            }
            return venue;
        }

        /// <summary>
        /// Updates a venue
        /// </summary>
        /// <param name="Venue"></param>
        /// <returns></returns>
        public static Venue UpdateVenue(Venue Venue)
        {
            using (var context = new BandBookerContext())
            {
                var venue = (from x in context.Venue
                             where x.VenueId == Venue.VenueId
                             select x).FirstOrDefault();

                if (venue != null)
                {
                    venue.Name = Venue.Name;
                    venue.ContactName = Venue.ContactName;
                    venue.ContactEmail = Venue.ContactEmail;
                    venue.ContactMobilePhone = Venue.ContactMobilePhone;
                    venue.Address = Venue.Address;
                    venue.Address2 = Venue.Address2;
                    venue.City = Venue.City;
                    venue.Description = Venue.Description;
                    venue.Phone = Venue.Phone;
                    venue.State = Venue.State;
                    venue.Zip = Venue.Zip;
                    venue.Website = Venue.Website;
                    venue.PhotoUrl = Venue.PhotoUrl;
                    context.SaveChanges();
                    return venue;
                }
            }
            return Venue;
        }

        /// <summary>
        /// Deteles a venue,
        /// optionally deleting associations
        /// </summary>
        /// <param name="Venue"></param>
        /// <param name="RemoveAllAssociations"></param>
        /// <returns></returns>
        public static string DeleteVenue(Venue Venue, bool RemoveAllAssociations)
        {
            try
            {
                using (var context = new BandBookerContext())
                {
                    var bookings = from x in context.Booking
                                   where x.VenueId == Venue.VenueId
                                   select x;

                    if (RemoveAllAssociations)
                    {
                        foreach (var child in bookings)
                        {
                            DeleteBooking(child, true);
                        }
                    }
                    else
                    {
                        if (bookings.Count() > 0)
                        {
                            return "The venue can not be deleted because " +
                                "there are gigs booked there.";
                        }
                    }

                    var inst = (from x in context.Venue
                                where x.VenueId == Venue.VenueId
                                select x).FirstOrDefault();

                    if (inst != null)
                    {
                        context.Venue.Remove(inst);
                        context.SaveChanges();
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Get all bookings
        /// </summary>
        public static List<Booking> Bookings
        {
            get
            {
                using (var context = new BandBookerContext())
                {
                    var list = context.Booking
                        .Include(x => x.Venue)
                        .Include(x => x.Band)
                        .ThenInclude(x => x.BandMusician)
                        .ThenInclude(x => x.Musician)
                        .ThenInclude(x => x.MusicianInstrument)
                        .ThenInclude(x => x.Instrument)
                        .Include(x => x.BookingMusicianInstrument)
                        .ThenInclude(bmi => bmi.Instrument)
                        .Include(x => x.BookingMusicianInstrument)
                        .ThenInclude(bmi => bmi.Musician)
                        .Include(x => x.BookingInstrument)
                        .ThenInclude(x => x.Instrument)
                        .ToList<Booking>();
                    return list;
                }
            }
        }

        /// <summary>
        /// Get All BookingMusicianInstruments
        /// </summary>
        public static List<BookingMusicianInstrument> BookingMusicianInstruments
        {
            get
            {
                using (var context = new BandBookerContext())
                {
                    var list = context.BookingMusicianInstrument
                        .Include(bmi => bmi.Booking)
                        .Include(bmi => bmi.Instrument)
                        .Include(bmi => bmi.Musician)
                        .ToList<BookingMusicianInstrument>();
                    return list;
                }
            }
        }

        /// <summary>
        /// Add a booking
        /// </summary>
        /// <param name="Booking"></param>
        /// <returns></returns>
        public static Booking AddBooking(Booking Booking)
        {
            var instruments = new List<Instrument>();
            var musicianinstruments = new List<BookingMusicianInstrument>();
            Band Band = null;
            Venue Venue = null;

            if (Booking.Band != null)
            {
                Band = Booking.Band;
                Booking.Band = null;
            }

            if (Booking.Venue != null)
            {
                Venue = Booking.Venue;
                Booking.Venue = null;
            }

            var bookingInstruments =
                Booking.BookingInstrument.ToArray();
            if (bookingInstruments.Length > 0)
            {
                Booking.BookingInstrument.Clear();
                foreach (var mi in bookingInstruments)
                {
                    instruments.Add(mi.Instrument);
                }
            }

            var bookingMusicianInstruments =
                Booking.BookingMusicianInstrument.ToArray();
            if (bookingMusicianInstruments.Length > 0)
            {
                Booking.BookingMusicianInstrument.Clear();
                foreach (var bmi in bookingMusicianInstruments)
                {
                    musicianinstruments.Add(bmi);
                }
            }

            using (var context = new BandBookerContext())
            {
                context.Booking.Add(Booking);
                context.SaveChanges();
            }


            foreach (var instrument in instruments)
            {
                AddRequiredInstrumentToBooking(Booking, instrument);
            }

            foreach (var musicianinstrument in musicianinstruments)
            {
                AddMusicianAndInstrumentToBooking(Booking,
                    musicianinstrument.Musician,
                    musicianinstrument.Instrument);
            }

            if (Venue != null || Band != null)
            {
                using (var context = new BandBookerContext())
                {
                    Booking = (from x in context.Booking
                               where x.BookingId == Booking.BookingId
                               select x).FirstOrDefault();

                    if (Venue != null)
                    {
                        Booking.Venue = Venue;
                    }

                    if (Band != null)
                    {
                        Booking.Band = Band;
                    }

                    context.SaveChanges();
                }
            }

            return (from x in Bookings
                    where x.BookingId == Booking.BookingId
                    select x).First();
        }

        /// <summary>
        /// Add a required instrument to a booking
        /// </summary>
        /// <param name="booking"></param>
        /// <param name="instrument"></param>
        public static void AddRequiredInstrumentToBooking(Booking booking,
            Instrument instrument)
        {
            Booking dbBooking;

            using (var context = new BandBookerContext())
            {
                dbBooking =
                    (from x in context.Booking
                     where x.BookingId == booking.BookingId
                     select x).FirstOrDefault();

                var currentBI =
                    (from x in context.BookingInstrument
                     where x.BookingId == booking.BookingId
                     && x.InstrumentId == instrument.InstrumentId
                     select x).FirstOrDefault();

                if (currentBI == null)
                {
                    var dbInstrument =
                        (from x in context.Instrument
                         where x.InstrumentId == instrument.InstrumentId
                         select x).FirstOrDefault();

                    var bi = new BookingInstrument()
                    {
                        Booking = dbBooking,
                        Instrument = dbInstrument
                    };
                    dbBooking.BookingInstrument.Add(bi);
                    context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// A musician calls this to decline a booking
        /// that they have been invited to
        /// </summary>
        /// <param name="Booking"></param>
        /// <param name="Musician"></param>
        /// <param name="Reason"></param>
        public static void MusicianCannotPlayGig(Booking Booking,
            Musician Musician, string Reason)
        {
            using (var context = new BandBookerContext())
            {
                var dbBmi =
                    (from x in context.BookingMusicianInstrument
                     where x.BookingId == Booking.BookingId
                     && x.MusicianId == Musician.MusicianId
                     select x).FirstOrDefault();

                if (dbBmi != null)
                {
                    dbBmi.Response = false;
                    dbBmi.ResponseDate = DateTime.Now;
                    dbBmi.ResponseReason = Reason;
                }
                else
                {
                    var dbBooking = (from x in context.Booking
                                     where x.BookingId ==
                                     Booking.BookingId
                                     select x).FirstOrDefault();

                    var bmi = new BookingMusicianInstrument()
                    {
                        BookingId = Booking.BookingId,
                        MusicianId = Musician.MusicianId,
                        Response = false,
                        ResponseDate = DateTime.Now,
                        ResponseReason = Reason
                    };
                    dbBooking.BookingMusicianInstrument.Add(bmi);
                }
                context.SaveChanges();
            }
        }

        /// <summary>
        /// A musician calls this to confirm that they will
        /// play the given instrument at the given booking
        /// </summary>
        /// <param name="Booking"></param>
        /// <param name="Musician"></param>
        /// <param name="Instrument"></param>
        public static void AddMusicianAndInstrumentToBooking(Booking Booking,
            Musician Musician, Instrument Instrument)
        {
            Booking dbBooking;

            using (var context = new BandBookerContext())
            {
                dbBooking = (from x in context.Booking
                             where x.BookingId == Booking.BookingId
                             select x).FirstOrDefault();

                var currentBmi = (from x in context.BookingMusicianInstrument
                                  where x.BookingId == Booking.BookingId
                                  && x.MusicianId == Musician.MusicianId
                                  select x).FirstOrDefault();

                if (currentBmi != null)
                {
                    currentBmi.InstrumentId = Instrument.InstrumentId;
                    currentBmi.Response = true;
                    currentBmi.ResponseReason = "";
                    currentBmi.ResponseDate = DateTime.Now;
                }
                else
                {
                    var dbMusician =
                        (from x in context.Musician
                         where x.MusicianId == Musician.MusicianId
                         select x).FirstOrDefault();

                    var dbInstrument =
                        (from x in context.Instrument
                         where x.InstrumentId == Instrument.InstrumentId
                         select x).FirstOrDefault();

                    var bmi = new BookingMusicianInstrument()
                    {
                        Booking = dbBooking,
                        Musician = dbMusician,
                        Instrument = dbInstrument,
                        Response = true,
                        ResponseReason = "",
                        ResponseDate = DateTime.Now
                    };
                    dbBooking.BookingMusicianInstrument.Add(bmi);
                }
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Updates a booking
        /// </summary>
        /// <param name="Booking"></param>
        /// <returns></returns>
        public static Booking UpdateBooking(Booking Booking)
        {
            Booking dbBooking;

            using (var context = new BandBookerContext())
            {
                dbBooking = (from x in context.Booking
                             where x.BookingId == Booking.BookingId
                             select x).FirstOrDefault();

                if (dbBooking != null)
                {
                    dbBooking.Name = Booking.Name;
                    dbBooking.Booked = Booking.Booked;
                    dbBooking.DateAndTime = Booking.DateAndTime;
                    dbBooking.DateBooked = Booking.DateBooked;
                    dbBooking.PayOffering = Booking.PayOffering;
                    dbBooking.VenueId = Booking.VenueId;
                    dbBooking.BandId = Booking.BandId;

                    //BookingInstruments
                    var bis = from x in context.BookingInstrument
                              where x.BookingId == dbBooking.BookingId
                              select x;

                    foreach (var bi in bis.ToArray())
                    {
                        context.BookingInstrument.Remove(bi);
                    }

                    //BookingMusicianInstruments
                    var bmis = from x in context.BookingMusicianInstrument
                               where x.BookingId == dbBooking.BookingId
                               select x;

                    foreach (var bmi in bmis.ToArray())
                    {
                        context.BookingMusicianInstrument.Remove(bmi);
                    }

                    context.SaveChanges();
                }
            }
            foreach (var bi in Booking.BookingInstrument)
            {
                AddRequiredInstrumentToBooking(Booking,
                    bi.Instrument);
            }

            foreach (var bmi in Booking.BookingMusicianInstrument)
            {
                AddMusicianAndInstrumentToBooking(Booking,
                    bmi.Musician, bmi.Instrument);
            }

            return (from x in Bookings
                    where x.BookingId == Booking.BookingId
                    select x).First();
        }

        /// <summary>
        /// Deletes a booking,
        /// optionally deleting associations
        /// </summary>
        /// <param name="Booking"></param>
        /// <param name="RemoveAllAssociations"></param>
        /// <returns></returns>
        public static string DeleteBooking(Booking Booking,
            bool RemoveAllAssociations)
        {
            try
            {
                using (var context = new BandBookerContext())
                {
                    var musicianInstruments =
                        from x in context.BookingMusicianInstrument
                        where x.BookingId == Booking.BookingId
                        select x;

                    var bookingInstruments =
                        from x in context.BookingInstrument
                        where x.BookingId == Booking.BookingId
                        select x;


                    if (RemoveAllAssociations)
                    {
                        foreach (var child in musicianInstruments)
                        {
                            context.BookingMusicianInstrument.Remove(child);
                        }

                        foreach (var child in bookingInstruments)
                        {
                            context.BookingInstrument.Remove(child);
                        }

                        context.SaveChanges();
                    }
                    else
                    {
                        if (musicianInstruments.Count() > 0)
                        {
                            return "This booking can not be deleted because " +
                                "it has musicians associated with it";
                        }
                        else if (bookingInstruments.Count() > 0)
                        {
                            return "This booking can not be deleted because " +
                                "it has required instruments associated with it";
                        }
                    }

                    var booking = (from x in context.Booking
                                   where x.BookingId == Booking.BookingId
                                   select x).FirstOrDefault();

                    if (booking != null)
                    {
                        context.Booking.Remove(booking);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }
    }
}

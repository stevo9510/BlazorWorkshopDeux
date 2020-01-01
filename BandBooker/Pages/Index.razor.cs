using BandBooker.Data;
using BandBookerData;
using BandBookerData.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BandBooker.Pages
{
    public class IndexCode : ComponentBase
    {
        private HubConnection connection;
        protected bool isAllowedToView = false;
        protected bool loggedIn = false;
        protected bool isMusician = false;
        
        [Inject]
        public IConfiguration Configuration { get; set; }

        [Inject]
        public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        protected async override Task OnInitializedAsync()
        {
            AuthenticationState authenticationState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

            ClaimsPrincipal user = authenticationState.User;
            string email = user.Identity.Name;
            if (email == null)
                return;

            loggedIn = true;
            if (user.IsInRole("musician"))
            {
                isMusician = true;
            }

            if (!user.IsInRole("manager"))
                return;
            isAllowedToView = true;

            string baseURL = Configuration["BaseURL"];

            allInstruments = await ApiService.GetInstruments();

            connection = new HubConnectionBuilder()
                .WithUrl($"{baseURL}adminHub")
                .Build();

            connection.On<int, string>("ReceiveSyncMusicianBio", this.OnReceiveSyncMusicianBio);

            try
            {
                await connection.StartAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #region Instruments

        protected InstrumentEditor instrumentEditor;
        protected bool disableInstrumentList = false;
        protected bool disableEditInstrument = true;
        protected string instrumentErrorMessage;
        protected Instrument selectedInstrument;
        protected List<Instrument> allInstruments = new List<Instrument>();

        protected void OnInstrumentSelected(ChangeEventArgs changeEventArgs)
        {
            if (int.TryParse(changeEventArgs.Value?.ToString(), out int instrumentId))
            {
                selectedInstrument = allInstruments.Where(inst => inst.InstrumentId == instrumentId).FirstOrDefault();
                string instrumentName = selectedInstrument?.Name;
                disableEditInstrument = false;
            }
            else
            {
                disableEditInstrument = true;
            }
        }

        protected void InstrumentEditor_AddInstrumentClicked(Instrument instrument)
        {
            DataManager.AddInstrument(instrument);
            EnableInteraction();
            allInstruments = DataManager.Instruments;
            selectedInstrument = allInstruments.Where(inst => inst.InstrumentId == instrument.InstrumentId).FirstOrDefault();
            instrumentEditor.Hide();
        }

        protected void InstrumentEditor_UpdateInstrumentClicked(string message)
        {
            selectedInstrument = DataManager.UpdateInstrument(selectedInstrument);
            allInstruments = DataManager.Instruments;

            EnableInteraction();
            instrumentEditor.Hide();
        }

        protected void InstrumentEditor_CancelClicked(string message)
        {
            if (selectedInstrument != null)
            {
                selectedInstrument = allInstruments.Where(inst => inst.InstrumentId == selectedInstrument.InstrumentId).FirstOrDefault();
            }

            instrumentErrorMessage = message;
            EnableInteraction();
            instrumentEditor.Hide();
        }

        protected async Task NewInstrumentClicked()
        {
            instrumentErrorMessage = string.Empty;
            DisableInteraction();
            selectedInstrument = new Instrument();
            await instrumentEditor.Initialize(true);
        }

        protected async Task EditInstrumentClicked()
        {
            DisableInteraction();
            await instrumentEditor.Initialize(false);
        }

        protected void DeleteInstrumentClicked()
        {
            if (selectedInstrument != null)
            {
                instrumentErrorMessage = DataManager.DeleteInstrument(selectedInstrument);
                if (string.IsNullOrWhiteSpace(instrumentErrorMessage))
                {
                    allInstruments.Remove(selectedInstrument);
                    selectedInstrument = allInstruments.FirstOrDefault();
                }
            }
        }

        private void EnableInteraction()
        {
            disableEditInstrument = selectedInstrument == null;
            disableInstrumentList = false;
        }

        private void DisableInteraction()
        {
            disableEditInstrument = true;
            disableInstrumentList = true;
        }

        #endregion

        #region Musicians

        protected string musicianErrorMessage = "";
        protected Musician selectedMusician;
        protected MusicianEditor musicianEditor;
        protected bool disableMusicianControls = false;
        protected bool disableMusicianEditButton = true;

        protected void EditMusicianCancelled(string message)
        {
            if (selectedMusician != null)
            {
                selectedMusician =
                   (from x in DataManager.Musicians
                    where x.MusicianId == selectedMusician.MusicianId
                    select x).FirstOrDefault();


            }
            disableMusicianControls = false;
            disableMusicianEditButton = (selectedMusician == null);
        }

        protected void MusicianAdded(Musician musician)
        {
            DataManager.AddMusician(musician);
            disableMusicianControls = false;
            disableMusicianEditButton = (selectedMusician == null);
            musicianEditor.Hide();
            musicianErrorMessage = "";
        }

        protected void MusicianUpdated(Musician musician)
        {
            selectedMusician = DataManager.UpdateMusician(musician);
            disableMusicianControls = false;
            disableMusicianEditButton = false;
            musicianEditor.Hide();
        }

        protected void MusicianSelected(Microsoft.AspNetCore.Components.ChangeEventArgs args)
        {
            SelectMusicianById(Convert.ToInt32(args.Value));
        }

        void SelectMusicianById(int id)
        {
            musicianErrorMessage = "";


            selectedMusician =
               (from x in DataManager.Musicians
                where x.MusicianId.ToString() == id.ToString()
                select x).FirstOrDefault();


            if (selectedMusician != null)
            {
                disableMusicianEditButton = false;
            }
            else
                disableMusicianEditButton = true;
        }

        protected async Task NewMusicianButtonClick()
        {
            musicianErrorMessage = "";
            disableMusicianControls = true;
            disableMusicianEditButton = true;
            await musicianEditor.NewMusician(DataManager.Instruments);
        }

        protected async Task EditMusicianButtonClick()
        {
            disableMusicianControls = true;
            disableMusicianEditButton = true;
            await musicianEditor.EditMusician(DataManager.Instruments,
              selectedMusician);
        }

        protected void DeleteMusicianButtonClick()
        {
            if (selectedMusician != null)
            {
                musicianErrorMessage = DataManager.DeleteMusician(selectedMusician, false);
                if (DataManager.Musicians.Count == 1)
                {
                    selectedMusician = DataManager.Musicians.First();
                }
            }
        }

        protected async Task MusicianBioUpdated(string bio)
        {
            if (selectedMusician != null)
            {
                await connection.InvokeAsync("SyncMusicianBio", selectedMusician.MusicianId, bio);
            }
        }

        private void OnReceiveSyncMusicianBio(int musicianId, string bio)
        {
            Musician musician = DataManager.Musicians.Where(m => m.MusicianId == musicianId).FirstOrDefault();

            if (musician != null)
            {
                musicianEditor.UpdateMusicianBio(musicianId, bio);
            }
        }

        #endregion

        #region Bands

        protected string bandErrorMessage = "";
        protected Band selectedBand;
        protected BandEditor bandEditor;
        protected bool disableBandControls = false;
        protected bool disableBandEditButton = true;

        protected void EditBandCancelled(string message)
        {
            if (selectedBand != null)
            {
                selectedBand = (from x in DataManager.Bands
                                where x.BandId == selectedBand.BandId
                                select x).FirstOrDefault();
            }

            disableBandControls = false;
            disableBandEditButton = (selectedBand == null);
            bandErrorMessage = "";
        }

        protected void BandAdded(Band band)
        {
            DataManager.AddBand(band);
            disableBandControls = false;
            disableBandEditButton = (selectedBand == null);
            bandEditor.Hide();
        }

        protected void BandUpdated(Band band)
        {
            selectedBand = DataManager.UpdateBand(band);
            disableBandControls = false;
            disableBandEditButton = false;
            bandEditor.Hide();
        }

        protected void BandSelected(ChangeEventArgs args)
        {
            SelectBandById(Convert.ToInt32(args.Value));
        }

        void SelectBandById(int Id)
        {
            bandErrorMessage = "";

            selectedBand = (from x in DataManager.Bands
                            where x.BandId.ToString() == Id.ToString()
                            select x).FirstOrDefault();

            disableBandEditButton = selectedBand == null;
        }

        protected async Task NewBandButtonClick()
        {
            bandErrorMessage = "";
            disableBandControls = true;
            disableBandEditButton = true;
            await bandEditor.NewBand(DataManager.Musicians);
        }

        protected async Task EditBandButtonClick()
        {
            disableBandControls = true;
            disableBandEditButton = true;
            await bandEditor.EditBand(DataManager.Musicians, selectedBand);
        }

        protected void DeleteBandButtonClick()
        {
            var msg = DataManager.DeleteBand(selectedBand, false);
            if (msg == "")
            {
                selectedBand = null;
                disableBandEditButton = true;
            }
            else
            {
                bandErrorMessage = msg;
            }
        }

        #endregion

        #region Venues

        protected Venue selectedVenue;
        protected VenueEditor venueEditor;
        protected bool disableVenueControls = false;
        protected bool disableVenueEditButton = true;
        protected string venueErrorMessage = "";

        protected void EditVenueCancelled(string message)
        {
            if (selectedVenue != null)
            {
                selectedVenue = (from x in DataManager.Venues
                                 where x.VenueId == selectedVenue.VenueId
                                 select x).FirstOrDefault();
            }
            venueErrorMessage = message;
            disableVenueControls = false;
            disableVenueEditButton = (selectedVenue == null);
        }

        protected void VenueAdded(Venue venue)
        {
            try
            {
                DataManager.AddVenue(venue);
                disableVenueControls = false;
                disableVenueEditButton = (selectedVenue == null);
                venueEditor.Hide();
            }
            catch (Exception ex)
            {
                venueErrorMessage = ex.Message;
            }
        }

        protected void VenueUpdated(Venue venue)
        {
            try
            {
                selectedVenue = DataManager.UpdateVenue(venue);
                disableVenueControls = false;
                disableVenueEditButton = (selectedVenue == null);
                venueEditor.Hide();
            }
            catch (Exception ex)
            {
                venueErrorMessage = ex.Message;
            }
        }

        protected async Task NewVenueButtonClick()
        {
            venueErrorMessage = "";
            disableVenueControls = true;
            disableVenueEditButton = true;
            await venueEditor.NewVenue();
        }

        protected async Task EditVenueButtonClick()
        {
            disableVenueControls = true;
            disableVenueEditButton = true;
            await venueEditor.EditVenue(selectedVenue);
        }

        protected void DeleteVenueButtonClick()
        {
            string msg = DataManager.DeleteVenue(selectedVenue, false);
            if (msg == "")
            {
                selectedVenue = null;
                disableVenueEditButton = true;
            }
            else
            {
                venueErrorMessage = msg;
            }
        }

        protected void VenueSelected(Microsoft.AspNetCore.Components.ChangeEventArgs args)
        {
            venueErrorMessage = "";

            selectedVenue = (from x in DataManager.Venues
                             where x.VenueId.ToString() == args.Value.ToString()
                             select x).FirstOrDefault();
            if (selectedVenue != null)
                disableVenueEditButton = false;
            else
                disableVenueEditButton = true;
        }

        #endregion

        #region Bookings

        protected string bookingErrorMessage = "";
        protected Booking selectedBooking;
        protected BookingEditor bookingEditor;
        protected bool disableBookingControls = false;
        protected bool disableBookingEditButton = true;

        protected void ContactMusicians(Booking booking)
        {
            // send email to each musician
            string subject = "Potential Gig for " + booking.Band.Name;
            string body = booking.Band.Name + " at "
+ booking.Venue.Name + " on "
+ booking.DateAndTime.ToLongDateString() + " at "
+ booking.DateAndTime.ToShortTimeString()
+ " \n\nPlease go to {YOUR WEBSITE} to confirm or decline this gig.";

            var musicians = DataManager.GetMusicainsToInviteToBooking(booking);

            // TODO: Send Email here to all musicians

            bookingErrorMessage = "Musicians have been contacted!";
            bookingEditor.Hide();
            disableBookingControls = false;
            disableBookingEditButton = false;
        }

        protected void EditBookingCancelled(string message)
        {
            if (selectedBooking != null)
            {
                selectedBooking = (from x in DataManager.Bookings
                                   where x.BookingId == selectedBooking.BookingId
                                   select x).FirstOrDefault();
            }

            disableBookingControls = false;
            disableBookingEditButton = false;
            bandErrorMessage = "";
        }

        protected void BookingAdded(Booking booking)
        {
            DataManager.AddBooking(booking);
            disableBookingControls = false;
            disableBookingEditButton = (selectedBooking == null);
            bookingEditor.Hide();
        }

        protected void BookingUpdated(Booking booking)
        {
            selectedBooking = DataManager.UpdateBooking(booking);
            disableBookingControls = false;
            disableBookingEditButton = false;
            bookingEditor.Hide();
        }

        protected void BookingSelected(Microsoft.AspNetCore.Components.ChangeEventArgs args)
        {
            SelectBookingById(Convert.ToInt32(args.Value));
        }

        void SelectBookingById(int id)
        {
            bookingErrorMessage = "";

            selectedBooking = (from x in DataManager.Bookings
                               where x.BookingId.ToString() == id.ToString()
                               select x).FirstOrDefault();
            if (selectedBooking != null)
            {
                disableBookingEditButton = false;
            }
            else
                disableBookingEditButton = true;
        }

        protected async Task NewBookingButtonClick()
        {
            var Bands = DataManager.Bands;
            if (Bands.Count == 0)
            {
                bookingErrorMessage = "Please add a band first.";
                return;
            }

            var Venues = DataManager.Venues;
            if (Venues.Count == 0)
            {
                bookingErrorMessage = "Please add a venue first.";
                return;
            }

            bookingErrorMessage = "";
            disableBookingControls = true;
            disableBookingEditButton = true;
            await bookingEditor.NewBooking(Bands, Venues);
        }

        protected async Task EditBookingButtonClick()
        {
            disableBookingControls = true;
            disableBookingEditButton = true;
            await bookingEditor.EditBooking(DataManager.Bands,
                DataManager.Venues, selectedBooking);
        }

        protected void BandWasBooked(Booking booking)
        {
            // send email to each musician
            string subject = "New Booking for " + booking.Band.Name;
            string body = booking.Band.Name + " has been booked at "
+ booking.Venue.Name + " on "
+ booking.DateAndTime.ToLongDateString() + " at "
+ booking.DateAndTime.ToShortTimeString()
+ " with the following musicians:\n\n";
            foreach (var bmi in booking.BookingMusicianInstrument)
            {
                body += bmi.Musician.Name + " ("
                    + bmi.Instrument.Name + ")\n";
            }

            // TODO: Send Email Here to all BookingMusicians

            bookingErrorMessage = "Emails have been sent!";
            bookingEditor.Hide();

            // Update the booking now that the Booked bool has been set
            DataManager.UpdateBooking(booking);
        }

        protected void DeleteBookingButtonClick()
        {
            var msg = DataManager.DeleteBooking(selectedBooking, false);
            bookingErrorMessage = "";
            if (msg == "")
            {
                selectedBooking = null;
                disableBookingEditButton = true;
            }
            else
            {
                bookingErrorMessage = msg;
            }
        }

        #endregion

    }
}

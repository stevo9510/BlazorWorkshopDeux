using BandBookerData.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BandBooker.Pages
{
    public class BookingEditorCode : ComponentBase
    {
        [Inject]
        private IJSRuntime Js { get; set; }

        protected List<Band> AllBands { get; set; }
        protected List<Venue> AllVenues { get; set; }
        protected List<Instrument> BandInstruments { get; set; } = new List<Instrument>();
        protected List<Instrument> SelectedInstruments { get; set; } = new List<Instrument>();

        [Parameter]
        public EventCallback<string> CancelPressed { get; set; }

        [Parameter]
        public EventCallback<Booking> BookingAdded { get; set; }

        [Parameter]
        public EventCallback<Booking> BookingUpdated { get; set; }

        [Parameter]
        public EventCallback<Booking> ContactMusicians { get; set; }

        [Parameter]
        public EventCallback<Booking> BandWasBooked { get; set; }

        protected Booking Booking { get; set; } = new Booking();
        protected bool showBookingPanel = false;
        protected bool adding = false;
        protected string submitButtonText = "";

        protected async Task InviteMusicians()
        {
            await ContactMusicians.InvokeAsync(this.Booking);
        }

        protected void BandSelected(ChangeEventArgs args)
        {
            var id =  Convert.ToInt32(args.Value);
            var band = (from x in AllBands
                        where x.BandId == id
                        select x).FirstOrDefault();
            LoadBand(band);
        }

        void LoadBand(Band band)
        {
            Booking.BandId = band.BandId;
            Booking.Band = band;
            LoadBandInstruments();
        }

        protected void LoadBandInstruments()
        {
            BandInstruments.Clear();
            var band = (from x in AllBands
                        where x.BandId == Booking.BandId
                        select x).FirstOrDefault();
            foreach (var bm in band.BandMusician)
            {
                foreach (var i in bm.Musician.MusicianInstrument)
                {
                    BandInstruments.Add(i.Instrument);
                }
            }
        }

        protected void VenueSelected(ChangeEventArgs args)
        {
            Booking.VenueId = Convert.ToInt32(args.Value);
            Booking.Venue = (from x in AllVenues
                             where x.VenueId == Booking.VenueId
                             select x).FirstOrDefault();
        }

        public void Hide()
        {
            showBookingPanel = false;
        }

        public async Task BookBand()
        {
            Booking.Booked = true;
            Booking.DateBooked = DateTime.Now;
            await BandWasBooked.InvokeAsync(Booking);
        }

        public async Task NewBooking(List<Band> bands, List<Venue> venues)
        {
            BandInstruments = new List<Instrument>();
            SelectedInstruments = new List<Instrument>();

            AllBands = bands;
            AllVenues = venues;
            if (AllBands.Count == 0 || AllVenues.Count == 0)
            {
                return;
            }

            this.Booking = new Booking();
            this.Booking.Venue = AllVenues.FirstOrDefault();
            this.Booking.VenueId = this.Booking.Venue.VenueId;
            this.Booking.Band = AllBands.FirstOrDefault();
            this.Booking.BandId = this.Booking.Band.BandId;
            this.Booking.DateAndTime = DateTime.Now;
            LoadBandInstruments();

            adding = true;
            submitButtonText = "Add";
            showBookingPanel = true;
            await Js.InvokeVoidAsync("SetFocus", "booking.name");
        }

        public async Task EditBooking(List<Band> bands, List<Venue> venues, Booking booking)
        {
            this.Booking = booking;
            BandInstruments = new List<Instrument>();
            SelectedInstruments = new List<Instrument>();
            foreach (var bi in booking.BookingInstrument)
            {
                SelectedInstruments.Add(bi.Instrument);
            }
            AllBands = bands;
            AllVenues = venues;
            if (booking.Band != null)
            {
                LoadBand(booking.Band);
            }
            adding = false;
            submitButtonText = "Update";
            showBookingPanel = true;
            await Js.InvokeVoidAsync("SetFocus", "booking.name");
        }

        protected async Task SubmitButtonPressed()
        {
            // Clear booking's instruments and add the selected instruments
            this.Booking.BookingInstrument.Clear();
            foreach (var instrument in SelectedInstruments)
            {
                this.Booking.BookingInstrument.Add(
                    new BookingInstrument
                    {
                        Instrument = instrument,
                        InstrumentId = instrument.InstrumentId,
                        Booking = this.Booking,
                        BookingId = this.Booking.BookingId
                    });
            }

            if (adding)
            {
                await BookingAdded.InvokeAsync(this.Booking);
            }
            else
            {
                await BookingUpdated.InvokeAsync(this.Booking);
            }
        }

        protected async Task CancelButtonPressed()
        {
            showBookingPanel = false;
            await CancelPressed.InvokeAsync("");
        }
    }
}

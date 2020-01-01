using BandBookerData;
using BandBookerData.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BandBooker.Pages
{
    public class MusicianPageCode : ComponentBase
    {
        
        [Inject]
        private AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        protected Musician musician;
        protected MusicianEditor musicianEditor;
        protected List<Booking> newBookings;
        protected List<Booking> bookedBookings;
        protected List<Booking> allBookings;
        protected List<Instrument> myInstrumentsForSelectedBooking;
        protected Booking selectedNewBooking;
        protected Booking selectedBookedBooking;
        protected Booking selectedAnyBooking;
        protected Booking bookingToConfirm;
        protected BookingDetails newBookingDetails;
        protected BookingDetails bookedBookingDetails;
        protected BookingDetails anyBookingDetails;
        protected BookingResponse newBookingResponse;
        protected BookingResponse bookedBookingResponse;
        protected BookingResponse anyBookingResponse;
        protected string email = "";
        protected bool isAllowedToView = false;

        protected override void OnInitialized()
        {
            AuthenticationState authState = AuthenticationStateProvider
            .GetAuthenticationStateAsync().GetAwaiter().GetResult();

            ClaimsPrincipal user = authState.User;
            email = user.Identity.Name;
            if (email == null)
                return;
            if (!user.IsInRole("musician"))
                return;
            isAllowedToView = true;
            LoadMusician();
        }

        protected void MusicianAcceptedBooking(Instrument instrument)
        {
            DataManager.AddMusicianAndInstrumentToBooking(
                bookingToConfirm, musician, instrument);
            newBookingDetails.Hide();
            bookedBookingDetails.Hide();
            anyBookingDetails.Hide();
            LoadMusician();
        }

        protected void MusicianRejectedBooking(string reason)
        {
            DataManager.MusicianCannotPlayGig(
                bookingToConfirm, musician, reason);
            newBookingDetails.Hide();
            bookedBookingDetails.Hide();
            anyBookingDetails.Hide();
            LoadMusician();
        }

        protected void BookingResponseCancelled(string args)
        {
            newBookingDetails.Hide();
            bookedBookingDetails.Hide();
            anyBookingDetails.Hide();
        }

        protected void CancelReasonGivenForDecliningGig()
        {
            newBookingDetails.Hide();
        }

        protected void SelectedNewBookingClicked()
        {
            newBookingDetails.Show();
        }

        protected void SelectedBookedBookingClicked()
        {
            bookedBookingDetails.Show();
        }

        protected void SelectedAnyBookingClicked()
        {
            anyBookingDetails.Show();
        }

        protected void MusicianRespondedYes(Booking booking)
        {
            if (booking == selectedNewBooking)
            {
                newBookingResponse.ShowYes();
            }
            else if (booking == selectedBookedBooking)
            {
                bookedBookingResponse.ShowYes();
            }
            else if (booking == selectedAnyBooking)
            {
                anyBookingResponse.ShowYes();
            }
        }

        protected void MusicianRespondedNo(Booking booking)
        {
            if (booking == selectedNewBooking)
            {
                newBookingResponse.ShowNo();
            }
            else if (booking == selectedBookedBooking)
            {
                bookedBookingResponse.ShowNo();
            }
            else if (booking == selectedAnyBooking)
            {
                anyBookingResponse.ShowNo();
            }
        }


        protected void NewBookingSelected(ChangeEventArgs args)
        {
            bookingToConfirm = (from x in newBookings
                                where x.BookingId == Convert.ToInt32(args.Value)
                                select x).FirstOrDefault();
            selectedNewBooking = bookingToConfirm;
            myInstrumentsForSelectedBooking =
                DataManager.GetInstrumentsByBookingAndMusician(
                    bookingToConfirm, musician);
        }

        protected void BookedBookingSelected(Microsoft.AspNetCore.Components.ChangeEventArgs args)
        {
            bookingToConfirm = (from x in bookedBookings
                                where x.BookingId == Convert.ToInt32(args.Value)
                                select x).FirstOrDefault();
            selectedBookedBooking = bookingToConfirm;
            myInstrumentsForSelectedBooking =
                DataManager.GetInstrumentsByBookingAndMusician(
                    bookingToConfirm, musician);
        }

        protected void AnyBookingSelected(ChangeEventArgs args)
        {
            bookingToConfirm = (from x in allBookings
                                where x.BookingId == Convert.ToInt32(args.Value)
                                select x).FirstOrDefault();
            selectedAnyBooking = bookingToConfirm;
            myInstrumentsForSelectedBooking =
                DataManager.GetInstrumentsByBookingAndMusician(
                    bookingToConfirm, musician);
        }

        protected async Task EditProfile()
        {
            await musicianEditor.EditMusician(DataManager.Instruments, musician);
        }

        protected void CancelPressed(string message)
        {
            musicianEditor.Hide();
        }

        protected void MusicianUpdated(Musician musician)
        {
            DataManager.UpdateMusician(musician);
            LoadMusician();
            musicianEditor.Hide();
        }

        protected void LoadMusician()
        {

            if (!isAllowedToView)
                return;

            musician = (from x in DataManager.Musicians
                        where x.Email == email
                        select x).FirstOrDefault();

            if (musician == null)
                return;

            allBookings = DataManager.GetAllBookingsByMusician(musician);
            newBookings = DataManager.GetBookingsMusicianNeedsToRespondTo(musician);

            if (bookingToConfirm != null)
            {
                bookingToConfirm = (from x in allBookings
                                    where x.BookingId == bookingToConfirm.BookingId
                                    select x).FirstOrDefault();
                if (bookingToConfirm != null)
                {
                    myInstrumentsForSelectedBooking =
                        DataManager.GetInstrumentsByBookingAndMusician(
                            bookingToConfirm, musician);
                }
            }

            bookedBookings = DataManager.GetBookingsByMusician(musician, true);
        }
    }
}

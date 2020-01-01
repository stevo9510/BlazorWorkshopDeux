using BandBookerData.Models;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace BandBooker.Pages
{
    public class BookingDetailsCode : ComponentBase
    {
        [Parameter]
        public Booking Booking { get; set; }

        [Parameter]
        public bool ShowResponseButtons { get; set; } = false;

        [Parameter]
        public EventCallback<Booking> MusicianYesResponse { get; set; }

        [Parameter]
        public EventCallback<Booking> MusicianNoResponse { get; set; }

        protected bool showBookingDetails = false;

        public void Show()
        {
            showBookingDetails = true;
        }

        public void Hide()
        {
            showBookingDetails = false;
        }

        protected async Task RespondYesToBookingRequest()
        {
            await MusicianYesResponse.InvokeAsync(Booking);
        }

        protected async Task RespondNoToBookingRequest()
        {
            await MusicianNoResponse.InvokeAsync(Booking);
        }

        protected void CloseBookingDetails()
        {
            showBookingDetails = false;
        }
    }
}

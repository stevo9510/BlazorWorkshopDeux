using BandBookerData.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BandBooker.Pages
{
    public class BookingResponseCode : ComponentBase
    {

        [Parameter]
        public List<Instrument> MusiciansInstruments { get; set; }

        [Parameter]
        public EventCallback<Instrument> MusicianAcceptedBooking { get; set; }
        [Parameter]
        public EventCallback<string> MusicianRejectedBooking { get; set; }
        [Parameter]
        public EventCallback<string> ResponseCancelled { get; set; }

        protected string responseReason = "";
        protected bool showYesResponsePanel = false;
        protected bool showNoResponsePanel = false;

        public void Hide()
        {
            showYesResponsePanel = false;
            showNoResponsePanel = false;
        }

        public void ShowYes()
        {
            showYesResponsePanel = true;
            showNoResponsePanel = false;
        }

        public void ShowNo()
        {
            showYesResponsePanel = false;
            showNoResponsePanel = true;
        }

        protected async Task GigInstrumentSelected(ChangeEventArgs args)
        {
            var instrument = (from x in MusiciansInstruments
                              where x.InstrumentId == Convert.ToInt32(args.Value)
                              select x).FirstOrDefault();
            await MusicianAcceptedBooking.InvokeAsync(instrument);
            Hide();
        }

        protected async Task ReasonGivenForDecliningGig()
        {
            await MusicianRejectedBooking.InvokeAsync(responseReason);
            Hide();
        }

        protected async Task CancelReasonGivenForDecliningGig()
        {
            await ResponseCancelled.InvokeAsync("");
            Hide();
        }
    }
}

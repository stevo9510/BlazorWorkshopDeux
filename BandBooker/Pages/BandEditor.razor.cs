using BandBookerData.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BandBooker.Pages
{
    public class BandEditorCode : ComponentBase
    {
        [Inject]
        private IJSRuntime Js { get; set; }

        protected List<Musician> AllMusicians { get; set; }
        protected List<Musician> SelectedMusicians { get; set; }

        [Parameter]
        public EventCallback<string> CancelPressed { get; set; }

        [Parameter]
        public EventCallback<Band> BandAdded { get; set; }

        [Parameter]
        public EventCallback<Band> BandUpdated { get; set; }

        protected Band Band { get; set; } = new Band();
        protected bool showBandPanel = false;
        protected bool adding = false;
        protected string submitButtonText = "";

        public void Hide()
        {
            showBandPanel = false;
        }

        public async Task NewBand(List<Musician> musicians)
        {
            this.Band = new Band();
            AllMusicians = musicians;
            SelectedMusicians = new List<Musician>();
            adding = true;
            submitButtonText = "Add";
            showBandPanel = true;
            await Js.InvokeVoidAsync("SetFocus", "band.name");
        }

        public async Task EditBand(List<Musician> musicians, Band band)
        {
            this.Band = band;
            AllMusicians = musicians;
            SelectedMusicians = new List<Musician>();
            foreach (var bm in band.BandMusician)
            {
                SelectedMusicians.Add(bm.Musician);
            }
            adding = false;
            submitButtonText = "Update";
            showBandPanel = true;
            await Js.InvokeVoidAsync("SetFocus", "band.name");
        }

        protected async Task SubmitButtonPressed()
        {
            // Clear band's musician and add the selected musicians
            this.Band.BandMusician.Clear();
            foreach (var musician in SelectedMusicians)
            {
                this.Band.BandMusician.Add(
                new BandMusician
                {
                    Band = this.Band,
                    BandId = this.Band.BandId,
                    Musician = musician,
                    MusicianId = musician.MusicianId
                });
            }

            if (adding)
            {
                await BandAdded.InvokeAsync(this.Band);
            }
            else
            {
                await BandUpdated.InvokeAsync(this.Band);
            }
        }

        protected async Task CancelButtonPressed()
        {
            showBandPanel = false;
            await CancelPressed.InvokeAsync("");
        }
    }
}

using BandBooker.Data;
using BandBookerData.Models;
using Blazor.FileReader;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.IO;
using System.Threading.Tasks;

namespace BandBooker.Pages
{
    public class VenueEditorCode : ComponentBase
    {
        [Inject] 
        private IFileReaderService FileReaderService { get; set; }
        
        [Inject] 
        private IJSRuntime Js { get; set; }

        [Parameter]
        public EventCallback<string> CancelPressed { get; set; }

        [Parameter]
        public EventCallback<Venue> VenueAdded { get; set; }

        [Parameter]
        public EventCallback<Venue> VenueUpdated { get; set; }

        protected Venue Venue { get; set; } = new Venue();
        protected bool showVenuePanel = false;
        protected ElementReference inputTypeFileElement;
        protected string submitButtonText = "";
        private bool adding = false;

        public void Hide()
        {
            showVenuePanel = false;
        }

        public async Task ReadFile()
        {
            string filename = "wwwroot/images/";

            foreach (var file in await
               FileReaderService.CreateReference(inputTypeFileElement).EnumerateFilesAsync())
            {
                // Read into buffer and act (uses less memory)
                using Stream stream = await file.OpenReadAsync();
                var buffer = new byte[stream.Length];
                await stream.ReadAsync(buffer, 0, buffer.Length);

                var info = await file.ReadFileInfoAsync();
                filename += info.Name;

                using (Stream outfile = System.IO.File.OpenWrite(filename))
                {
                    outfile.Write(buffer, 0, buffer.Length);
                }
                ImageResizer.ResizeAndSaveImage(filename, 200);
                Venue.PhotoUrl = "/images/" + info.Name;
            }
        }

        public async Task NewVenue()
        {
            this.Venue = new Venue();
            adding = true;
            submitButtonText = "Add";
            showVenuePanel = true;
            await Js.InvokeVoidAsync("SetFocus", "venue.name");
        }

        public async Task EditVenue(Venue venue)
        {
            adding = false;
            submitButtonText = "Update";
            this.Venue = venue;
            showVenuePanel = true;
            await Js.InvokeVoidAsync("SetFocus", "venue.name");
        }

        protected async Task SubmitButtonPressed()
        {
            if (adding)
            {
                await VenueAdded.InvokeAsync(this.Venue);
            }
            else
            {
                await VenueUpdated.InvokeAsync(this.Venue);
            }
        }

        protected async Task CancelButtonPressed()
        {
            showVenuePanel = false;
            await CancelPressed.InvokeAsync("");
        }
    }
}

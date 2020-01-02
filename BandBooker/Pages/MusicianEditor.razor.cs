using BandBooker.Data;
using BandBookerData.Models;
using Blazor.FileReader;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BandBooker.Pages
{
    public class MusicianEditorCode : ComponentBase
    {
        [Inject] 
        private IFileReaderService FileReaderService { get; set; }
        
        [Inject] 
        private IJSRuntime Js { get; set; }

        [Parameter]
        public EventCallback<string> CancelPressed { get; set; }

        [Parameter]
        public EventCallback<Musician> MusicianAdded { get; set; }

        [Parameter]
        public EventCallback<Musician> MusicianUpdated { get; set; }

        [Parameter]
        public EventCallback<string> BioUpdated { get; set; }

        protected List<Instrument> AllInstruments { get; set; }
        protected List<Instrument> SelectedInstruments { get; set; } = new List<Instrument>();

        protected Musician Musician { get; set; } = new Musician();
        protected  bool showMusicianPanel = false;
        protected  ElementReference inputTypeFileElement;
        protected  bool adding = false;
        protected  string submitButtonText = "";

        public void Hide()
        {
            showMusicianPanel = false;
        }

        public async Task ReadFile()
        {
            string filename = "wwwroot/images/";

            foreach (var file in await FileReaderService.CreateReference(inputTypeFileElement).EnumerateFilesAsync())
            {
                // Read into buffer and act (uses less memory)
                using Stream stream = await file.OpenReadAsync();
                var buffer = new byte[stream.Length];
                await stream.ReadAsync(buffer, 0, buffer.Length);

                var info = await file.ReadFileInfoAsync();
                var extension = Path.GetExtension(filename + info.Name);
                var name = $@"{Guid.NewGuid()}" + extension;
                filename += name;

                using (Stream outfile = System.IO.File.OpenWrite(filename))
                {
                    outfile.Write(buffer, 0, buffer.Length);
                }
                ImageResizer.ResizeAndSaveImage(filename, 200);
                Musician.PhotoUrl = "/images/" + name;
            }
        }

        public async Task NewMusician(List<Instrument> instruments)
        {
            this.Musician = new Musician();
            AllInstruments = instruments;
            SelectedInstruments = new List<Instrument>();
            adding = true;
            submitButtonText = "Add";
            showMusicianPanel = true;
            await Js.InvokeVoidAsync("SetFocus", "musician.name");
        }

        public async Task EditMusician(List<Instrument> instruments, Musician musician)
        {
            AllInstruments = instruments;
            adding = false;
            SelectedInstruments = new List<Instrument>();
            SelectedInstruments.AddRange(musician.MusicianInstrument.Select(mi => mi.Instrument));
            submitButtonText = "Update";
            this.Musician = musician;
            showMusicianPanel = true;
            await Js.InvokeVoidAsync("SetFocus", "musician.name");
        }

        public void UpdateMusicianBio(int musicianId, string bio)
        {
            if (Musician != null && Musician.MusicianId == musicianId)
            {
                Musician.Bio = bio;
                StateHasChanged();
            }
        }

        protected async Task SubmitButtonPressed()
        {
            // Clear musician's instruments and add the selected instruments
            this.Musician.MusicianInstrument.Clear();
            foreach (var instrument in SelectedInstruments)
            {
                this.Musician.MusicianInstrument.Add(
                    new MusicianInstrument
                    {
                        Instrument = instrument,
                        InstrumentId = instrument.InstrumentId,
                        Musician = this.Musician,
                        MusicianId = this.Musician.MusicianId
                    });
            }

            if (adding)
            {
                await MusicianAdded.InvokeAsync(this.Musician);
            }
            else
            {
                await MusicianUpdated.InvokeAsync(this.Musician);
            }
        }

        protected async Task CancelButtonPressed()
        {
            showMusicianPanel = false;
            await CancelPressed.InvokeAsync("");
        }

        public async Task SyncMusicianBio(ChangeEventArgs e)
        {
            try
            {
                await BioUpdated.InvokeAsync(e.Value.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}

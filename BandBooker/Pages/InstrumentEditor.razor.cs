using BandBookerData.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace BandBooker.Pages
{
    public class InstrumentEditorCode : ComponentBase
    {
        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Parameter]
        public EventCallback<Instrument> AddInstrumentClicked { get; set; }

        [Parameter]
        public EventCallback<string> UpdateInstrumentClicked { get; set; }
        
        [Parameter]
        public EventCallback<string> CancelClicked { get; set; }

        [Parameter]
        public Instrument Instrument { get; set; }

        protected const string InstrumentNameElementId = "InstrumentNameId";
        protected bool showInstrumentPanel;
        protected bool addMode = false;
        protected string submitButtonText;
        
        public async Task Initialize(bool isAddMode)
        {
            showInstrumentPanel = true;
            if (isAddMode)
            {
                SetAddMode();
            }
            else
            {
                SetEditMode();
            }
            await JsRuntime.InvokeVoidAsync("SetFocus", InstrumentNameElementId);
        }

        public void Hide()
        {
            showInstrumentPanel = false;
        }

        private void SetAddMode()
        {
            addMode = true;
            submitButtonText = "Add";
        }

        private void SetEditMode()
        {
            addMode = false;
            submitButtonText = "Edit";
        }

        protected async Task SubmitButtonClicked()
        {
            if (addMode)
            {
                await AddInstrumentClicked.InvokeAsync(Instrument);
            }
            else
            {
                await UpdateInstrumentClicked.InvokeAsync(string.Empty);
            }
        }

        protected async Task CancelButtonClicked()
        {
            showInstrumentPanel = false;
            await CancelClicked.InvokeAsync("");
        }
    }
}

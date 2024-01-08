using Microsoft.AspNetCore.Components;

namespace Blazor.Client.Services
{
    public class MyStateService
    {
        public bool ShowData { get; private set; }
        public String text { get; set; }
        // Event to notify subscribers of state changes.
        public event Action<ComponentBase, String>? ShowDataChanged;

        // Public method to update the state.
        public void UpdateShowData(ComponentBase source, String responseText)
        {
            text = responseText;
            NotifyShowDataChanged(source, responseText);
        }

        // Private method to trigger the event.
        private void NotifyShowDataChanged(ComponentBase source, String responseText)
        {
            ShowDataChanged?.Invoke(source, responseText);
        }
    }
}

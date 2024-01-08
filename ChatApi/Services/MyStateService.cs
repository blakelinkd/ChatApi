using Microsoft.AspNetCore.Components;

namespace Blazor.Client.Services
{
    public class MyStateService
    {
        public bool ShowData { get; private set; }
        public event Action<ComponentBase, bool>? ShowDataChanged;

        private void NotifyShowDataChanged(ComponentBase source, bool value)
        {
            ShowData = value;
            ShowDataChanged?.Invoke(source, value);
        }

    }
}
using Microsoft.AspNetCore.Components;
using Blazor.Client.Models;
using System.Text.Json;

namespace Blazor.Client.Services
{
    public class MyStateService
    {
        public List<ChatMessage> Messages = new List<ChatMessage>();

        public void UpdateChatMessages(ComponentBase source, List<ChatMessage> responseText)
        {
            try
            {
                Messages = responseText;
            }
            catch (JsonException e)
            {
                // Log the exception or handle it as needed
                Messages = new List<ChatMessage>(); // Fallback to empty array in case of deserialization failure
            }

            NotifyChatMessagesChanged(source);
        }

        // Private method to trigger the event.
        private void NotifyChatMessagesChanged(ComponentBase source)
        {
            ChatMessagesChanged?.Invoke(source, Messages);
        }

        // Event to notify subscribers of chat message changes.
        public event Action<ComponentBase, List<ChatMessage>?>? ChatMessagesChanged;
    }
}

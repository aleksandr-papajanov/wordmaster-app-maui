using WordMaster.Common;
using WordMasterApp.Components;
using WordMasterApp.EntityViewModels;
using WordMasterApp.Helpers;

namespace WordMasterApp.Features.MessageContainer.Messages
{
    public class ErrorMessage : MessageViewModel
    {
        public ErrorMessage(string message, string title = "Error")
        {
            Title = title;
            Message = message;
            Actions.Add(new MessageAction { Text = "OK", Value = true });
        }
    }

    public class NotificationMessage : MessageViewModel
    {
        public NotificationMessage(string message, string title = "Notification")
        {
            Title = title;
            Message = message;
            Actions.Add(new MessageAction { Text = "OK", Value = true });
        }
    }
    
    public class ConfirmBasicWordCompletionMessage : MessageViewModel
    {
        public string Text { get; set; } = string.Empty;
        public string Translation { get; set; } = string.Empty;
        public string Definition { get; set; } = string.Empty;
        public double Progress { get; set; } = 0.0;


        public ConfirmBasicWordCompletionMessage(string text, string translation, string definition, WordUsageFrequenсy frequency)
        {
            Title = "Do you want to add this word to a current deck?";
            Text = text;
            Translation = translation;
            Definition = definition;
            Progress = (double)frequency / Enum.GetValues(typeof(WordUsageFrequenсy)).Length;

            Actions.Add(new MessageAction { Text = "Cancel", Value = ConfirmWordCompletionMessageResult.Cancel });
            Actions.Add(new MessageAction { Text = "Try again", Value = ConfirmWordCompletionMessageResult.TryAgain });
            Actions.Add(new MessageAction { Text = "Yes", Value = ConfirmWordCompletionMessageResult.Yes });
        }
    }

    enum ConfirmWordCompletionMessageResult
    {
        Cancel,
        TryAgain,
        Yes
    }
}

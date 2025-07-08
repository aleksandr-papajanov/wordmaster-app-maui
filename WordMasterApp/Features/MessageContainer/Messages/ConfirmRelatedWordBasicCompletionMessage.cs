using WordMaster.Common;

namespace WordMasterApp.Features.MessageContainer.Messages
{
    public class ConfirmRelatedWordBasicCompletionMessage : MessageViewModel
    {
        public string Text { get; set; } = string.Empty;
        public string Translation { get; set; } = string.Empty;
        public string Definition { get; set; } = string.Empty;
        public double Progress { get; set; } = 0.0;

        public ConfirmRelatedWordBasicCompletionMessage(string text, string translation, string definition, WordUsageFrequenсy frequency)
        {
            Title = "Do you want to add this word to a current deck?";
            Text = text;
            Translation = translation;
            Definition = definition;
            Progress = (double)frequency / Enum.GetValues(typeof(WordUsageFrequenсy)).Length;

            Actions.Add(new MessageAction { Text = "Cancel", Value = ConfirmRelatedWordCompletionMessageResult.Cancel });
            Actions.Add(new MessageAction { Text = "Try again", Value = ConfirmRelatedWordCompletionMessageResult.TryAgain });
            Actions.Add(new MessageAction { Text = "Save", Value = ConfirmRelatedWordCompletionMessageResult.Save });
            Actions.Add(new MessageAction { Text = "Add to deck", Value = ConfirmRelatedWordCompletionMessageResult.AddToDeck });
        }
    }

    public enum ConfirmRelatedWordCompletionMessageResult
    {
        Cancel,
        TryAgain,
        Save,
        AddToDeck
    }
}

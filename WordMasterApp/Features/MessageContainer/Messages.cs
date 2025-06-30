using WordMaster.Common;
using WordMasterApp.Components;
using WordMasterApp.Helpers;

namespace WordMasterApp.Features.MessageContainer
{
    public class NotificationMessage : MessageModel
    {
        public NotificationMessage(string message, string title = "Notification")
        {
            Title = title;
            Message = message;
            PrimaryColor = ThemeExtentions.GetColor("Primary");

            Actions.Add(new MessageAction { Text = "OK", Value = true });
        }
    }

    public class ConfirmationMessage : MessageModel
    {
        public ConfirmationMessage(string message, string title = "Confirmation")
        {
            Title = title;
            Message = message;
            PrimaryColor = ThemeExtentions.GetColor("PrimaryVariant");

            Actions.Add(new MessageAction { Text = "No", Value = false });
            Actions.Add(new MessageAction { Text = "Yes", Value = true });
        }
    }
    
    public class ConfirmWordCompletionMessage : ConfirmationMessage
    {
        public ConfirmWordCompletionMessage(string word, string translation, string definition, WordUsageFrequenсy frequenсy)
            : base($"", "Do you want to complete the word with this suggestion?")
        {
            Actions.Clear();
            Actions.Add(new MessageAction { Text = "Cancel", Value = ConfirmWordCompletionMessageResult.Cancel });
            Actions.Add(new MessageAction { Text = "Try again", Value = ConfirmWordCompletionMessageResult.TryAgain });
            Actions.Add(new MessageAction { Text = "Yes", Value = ConfirmWordCompletionMessageResult.Yes });

            ExtraContent = new StackLayout
            {
                Spacing = 10,
                Children = {
                    new Label { Text = word, Style = Application.Current?.Resources["LabelH2"] as Style },
                    new Label { Text = translation, Style = Application.Current?.Resources["LabelRegular"] as Style },
                    new Label { Text = definition, Style = Application.Current?.Resources["LabelCaption"] as Style },
                    new Label { Text = "Usage frequency:", Style = Application.Current?.Resources["LabelSubcaption"] as Style },
                    new SegmentedProgressBar
                    { 
                        StartColor = ThemeExtentions.GetColor("Error"),
                        EndColor = ThemeExtentions.GetColor("SecondaryVariant"),
                        BackgroundColor = PrimaryColor,
                        Segments = 5,
                        Progress = (double)frequenсy / Enum.GetValues(typeof(WordUsageFrequenсy)).Length,
                    }
                }
            };
        }
    }

    enum ConfirmWordCompletionMessageResult
    {
        Cancel,
        TryAgain,
        Yes
    }

    public class ErrorMessage : MessageModel
    {
        public ErrorMessage(string message, string title = "Error")
        {
            PrimaryColor = ThemeExtentions.GetColor("Error");

            Title = title;
            Message = message;

            Actions.Add(new MessageAction { Text = "OK", Value = true });
        }
    }
}

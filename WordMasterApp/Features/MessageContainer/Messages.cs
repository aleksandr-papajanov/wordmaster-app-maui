namespace WordMasterApp.Features.MessageContainer
{
    public class NotificationMessage : MessageModel
    {
        public NotificationMessage(string message, string title = "Notification")
        {
            Title = title;
            Message = message;

            Actions.Add(new MessageAction { Text = "OK", Value = true });
        }
    }

    public class ConfirmationMessage : MessageModel
    {
        public ConfirmationMessage(string message, string title = "Confirmation")
        {
            Title = title;
            Message = message;

            Actions.Add(new MessageAction { Text = "No", Value = false });
            Actions.Add(new MessageAction { Text = "Yes", Value = true });
        }
    }
    
    public class ConfirmWordCompletionMessage : ConfirmationMessage
    {
        public ConfirmWordCompletionMessage(string word, string translation, string definition)
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
                    new Label { Margin = 0, Padding = 0, Text = word, FontAttributes = FontAttributes.Bold, FontSize = 25 },
                    new Label { Margin = 0, Padding = 0, Text = translation, FontSize = 17 },
                    new Label { Margin = 0, Padding = 0, Text = definition, FontSize = 12 }
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
            Title = title;
            Message = message;

            Actions.Add(new MessageAction { Text = "OK", Value = true });
        }
    }
}

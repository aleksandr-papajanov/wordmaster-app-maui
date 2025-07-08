using DynamicData;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace WordMasterApp.Features.MessageContainer.Messages
{
    public class ConfirmRelatedWordUsagesCompletionMessage : MessageViewModel
    {
        public string Word { get; set; } = string.Empty;
        public ObservableCollection<Sentence> Sentences { get; set; } = new();


        public ConfirmRelatedWordUsagesCompletionMessage(string word, Dictionary<string, string> sentences)
        {
            Title = "Do you want to add this word to a current deck?";
            Word = word;
            AddSentencesFromDictionary(sentences);

            Actions.Add(new MessageAction { Text = "Cancel", Value = ConfirmRelatedWordUsagesCompletionMessageResult.Cancel });
            Actions.Add(new MessageAction { Text = "Try again", Value = ConfirmRelatedWordUsagesCompletionMessageResult.TryAgain });
            Actions.Add(new MessageAction { Text = "Add to deck", Value = ConfirmRelatedWordUsagesCompletionMessageResult.AddToDeck });
        }


        public Dictionary<string, string> GetSelectedSentencesAsDictionary()
        {
            return Sentences
                .Where(s => s.IsSelected)
                .ToDictionary(s => s.Text, s => s.Translation);
        }

        public void AddSentencesFromDictionary(Dictionary<string, string> sentences)
        {
            foreach (var pair in sentences)
            {
                Sentences.Add(new Sentence
                {
                    Text = pair.Key,
                    Translation = pair.Value
                });
            }
        }

        public void RemoveUnselectedSentences()
        {
            foreach (var sentence in Sentences.ToList())
            {
                if (!sentence.IsSelected)
                {
                    Sentences.Remove(sentence);
                }
            }
        }
    }

    public class Sentence : ReactiveObject
    {
        public string Text { get; set; } = string.Empty;
        public string Translation { get; set; } = string.Empty;

        private bool _isSelected = false;
        public bool IsSelected
        {
            get => _isSelected;
            set => this.RaiseAndSetIfChanged(ref _isSelected, value);
        }

        public ICommand SelectCommand { get; }

        public Sentence()
        {
            SelectCommand = ReactiveCommand.Create(() =>
            {
                IsSelected = !IsSelected;
            });
        }
    }

    public enum ConfirmRelatedWordUsagesCompletionMessageResult
    {
        Cancel,
        TryAgain,
        AddToDeck
    }
}

using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordMaster.Data.DTOs
{
    public class WordDTO : ReactiveObject, IDisblayable
    {
        public Guid Id { get; set; }

        private string _text = string.Empty;
        public string Text
        {
            get => _text;
            set => this.RaiseAndSetIfChanged(ref _text, value);
        }

        private string _translation = string.Empty;
        public string Translation
        {
            get => _translation;
            set => this.RaiseAndSetIfChanged(ref _translation, value);
        }

        public string? Definition { get; set; }
    }
}

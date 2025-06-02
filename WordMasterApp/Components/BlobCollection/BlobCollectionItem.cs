using CommunityToolkit.Mvvm.ComponentModel;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordMaster.Data.DTOs;

namespace WordMasterApp.Components
{
    public partial class BlobCollectionItem<T> : ReactiveObject
        where T : IDisblayable
    {
        private string _text = string.Empty;
        public string Text
        {
            get => _text;
            set => this.RaiseAndSetIfChanged(ref _text, value);
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => this.RaiseAndSetIfChanged(ref _isSelected, value);
        }

        public Guid Id { get; }
        public IDisblayable OriginalItem { get; }

        public BlobCollectionItem(T item)
        {
            OriginalItem = item;
            Id = item.Id;
            Text = item.Text;
        }
    }
}

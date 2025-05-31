using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordMaster.Data.DTOs;

namespace WordMasterApp.Components
{
    public partial class BlobCollectionItem<T> : ObservableObject
        where T : IDisblayable
    {
        [ObservableProperty]
        private string _text;

        [ObservableProperty]
        private bool _isSelected;

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

using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordMaster.Data.DTOs;

namespace WordMasterApp.Components
{
    public class BlobCollectionViewModel : ReactiveObject
    {
        private Guid? _selectedItemId;
        public Guid? SelectedItemId
        {
            get => _selectedItemId;
            set => this.RaiseAndSetIfChanged(ref _selectedItemId, value);
        }

        public ReadOnlyObservableCollection<IDisblayable> Items { get; set; } = null!;

        public IObservable<IChangeSet<IDisblayable>> Connect() => Items.ToObservableChangeSet();
    }
}

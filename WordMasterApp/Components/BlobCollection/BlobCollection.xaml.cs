using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using WordMaster.Data.DTOs;

namespace WordMasterApp.Components;

public partial class BlobCollection : ContentView
{
    // Observable collection of BlobCollectionItems for UI binding
    private ObservableCollection<BlobCollectionItem<IDisblayable>> _blobItems = new();
    public ObservableCollection<BlobCollectionItem<IDisblayable>> BlobItems
    {
        get => _blobItems;
        set
        {
            if (_blobItems != value)
            {
                _blobItems = value;
                OnPropertyChanged(nameof(BlobItems));
            }
        }
    }

    // ItemsSource bindable property
    public static readonly BindableProperty ItemsSourceProperty =
        BindableProperty.Create(
            nameof(ItemsSource),
            typeof(ObservableCollection<IDisblayable>),
            typeof(BlobCollection),
            default,
            propertyChanged: OnItemsSourceChanged);

    public ObservableCollection<IDisblayable> ItemsSource
    {
        get => (ObservableCollection<IDisblayable>)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not BlobCollection control)
            return;

        if (newValue is not ObservableCollection<IDisblayable> newValueCollection)
            return;

        control.BlobItems.Clear();
        foreach (var item in newValueCollection)
        {
            control.BlobItems.Add(new BlobCollectionItem<IDisblayable>(item));
        }

        newValueCollection.CollectionChanged += control.OnSourceCollectionChanged;
    }

    public void OnSourceCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (sender is not ObservableCollection<IDisblayable> source)
            return;


        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            foreach (IDisblayable item in e.NewItems!)
            {
                var blob = new BlobCollectionItem<IDisblayable>(item);
                blob.IsSelected = item.Id == SelectedItem?.Id;
                BlobItems.Add(blob);
            }
        }

        if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            foreach (IDisblayable item in e.OldItems!)
            {
                var blob = BlobItems.FirstOrDefault(x => x.OriginalItem == item);

                if (blob != null)
                {
                    if (blob.OriginalItem == SelectedItem)
                    {
                        SelectedItem = null;
                    }

                    BlobItems.Remove(blob);
                }
            }

        }

        if (e.Action == NotifyCollectionChangedAction.Reset)
        {
            BlobItems.Clear();

            foreach (IDisblayable item in source)
            {
                var blob = new BlobCollectionItem<IDisblayable>(item);
                blob.IsSelected = item.Id == SelectedItem?.Id;
                BlobItems.Add(blob);
            }
        }

        if (e.Action == NotifyCollectionChangedAction.Replace)
        {
            foreach (IDisblayable oldItem in e.OldItems!)
            {
                var blob = BlobItems.FirstOrDefault(x => x.OriginalItem == oldItem);

                if (blob != null)
                {
                    if (blob.OriginalItem == SelectedItem)
                    {
                        SelectedItem = null;
                    }

                    BlobItems.Remove(blob);
                }
            }

            foreach (IDisblayable item in e.NewItems!)
            {
                var blob = new BlobCollectionItem<IDisblayable>(item);
                blob.IsSelected = item.Id == SelectedItem?.Id;
                BlobItems.Add(blob);
            }
        }

        if (e.Action == NotifyCollectionChangedAction.Move)
        {
        }
    }

    // SelectedItem bindable property
    public static readonly BindableProperty SelectedItemProperty =
        BindableProperty.Create(
            nameof(SelectedItem),
            typeof(IDisblayable),
            typeof(BlobCollection),
            default,
            propertyChanged: OnSelectedItemChanged);

    public IDisblayable? SelectedItem
    {
        get => (IDisblayable)GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    private static void OnSelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not BlobCollection control)
            return;

        if (control.ItemsSource is null)
            return;

        foreach (var item in control.BlobItems)
        {
            item.IsSelected = newValue is IDisblayable original && item.Id == original.Id;
        }
    }

    // Command for selecting a blob item
    public ICommand SelectCommand { get; }

    public BlobCollection()
    {
        InitializeComponent();
        SelectCommand = new Command<BlobCollectionItem<IDisblayable>>(OnSelect);
    }

    private void OnSelect(BlobCollectionItem<IDisblayable> item)
    {
        SelectedItem = item.OriginalItem;
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using GalleryApp.Models;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GalleryApp.ViewModels
{
    public class GalleryViewModel : ViewModel
    {
        private readonly IPhotoImporter photoImporter;
        private readonly ILocalStorage localStorage;

        public GalleryViewModel(IPhotoImporter photoImporter, ILocalStorage localStorage)
        {
            this.photoImporter = photoImporter;
            this.localStorage = localStorage;

            Task.Run(Initialize);
        }

        public ObservableCollection<Photo> Photos { get; set; }

        private async Task Initialize()
        {
            IsBusy = true;

            Photos = await photoImporter.Get(0,20);

            RaisePropertyChanged(nameof(Photos));

            Photos.CollectionChanged += Photos_CollectionChanged;

            await Task.Delay(3000);

            IsBusy = false;
        }

        private void Photos_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if(e.NewItems != null && e.NewItems.Count > 0)
            {
                IsBusy = false;

                Photos.CollectionChanged -= Photos_CollectionChanged;
            }
        }

        public ICommand AddFavorites => new Command<List<Photo>>((photos) =>
        {
            foreach (var photo in photos)
            {
                localStorage.Store(photo.Filename);
            }

            MessagingCenter.Send(this, "FavoritesAdded");
        });

        private int currentStartIndex = 0;
        public ICommand LoadMore => new Command(async() =>
        {
            currentStartIndex += 20;
            itemsAdded = 0;
            var collection = await photoImporter.Get(currentStartIndex, 20);
            collection.CollectionChanged += Collection_CollectionChanged;
        });

        private int itemsAdded;
        private void Collection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs args)
        {
            foreach(Photo photo in args.NewItems)
            {
                itemsAdded++;
                Photos.Add(photo);
            }

            if(itemsAdded == 20)
            {
                var collection = (ObservableCollection<Photo>)sender;
                collection.CollectionChanged -= Collection_CollectionChanged;
            }
        }
    }
}

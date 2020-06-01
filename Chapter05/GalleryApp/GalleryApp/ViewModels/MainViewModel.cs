using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GalleryApp.Models;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GalleryApp.ViewModels
{
    public class MainViewModel : ViewModel
    {
        private readonly IPhotoImporter photoImporter;
        private readonly ILocalStorage localStorage;

        public MainViewModel(IPhotoImporter photoImporter, ILocalStorage localStorage)
        {
            this.photoImporter = photoImporter;
            this.localStorage = localStorage;
            Task.Run(Initialize);
        }

        public ObservableCollection<Photo> Recent { get; set; }
        public ObservableCollection<Photo> Favorites { get; set; }

        public async Task Initialize()
        {
            var photos = await photoImporter.Get(0, 20, Quality.Low);

            Recent = photos;

            RaisePropertyChanged(nameof(Recent));

            await LoadFavorites();

            MessagingCenter.Subscribe<GalleryViewModel>(this, "FavoritesAdded", (sender) =>
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await LoadFavorites();
                });
            });
        }

        private async Task LoadFavorites()
        {
            var filenames = await localStorage.Get();

            var favorites = await photoImporter.Get(filenames, Quality.Low);

            Favorites = favorites;

            RaisePropertyChanged(nameof(Favorites));
        }
    }
}

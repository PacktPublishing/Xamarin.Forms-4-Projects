using System;
using System.Collections.Generic;
using System.Linq;
using GalleryApp.Models;
using GalleryApp.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GalleryApp.Views
{
    public partial class GalleryView : ContentPage
    {
       

        public GalleryView()
        {
            InitializeComponent();

            BindingContext = Resolver.Resolve<GalleryViewModel>();
        }

        private void SelectToolBarItem_Clicked(object sender, EventArgs e)
        {
            if (!Photos.SelectedItems.Any())
            {
                DisplayAlert("No photos", "No photos selected", "OK");
                return;
            }

            var viewModel = (GalleryViewModel)BindingContext;

            viewModel.AddFavorites.Execute(Photos.SelectedItems.Select(x => (Photo)x).ToList());

            DisplayAlert("Added", "Selected photos has been added to favorites", "OK");
        }
    }
}

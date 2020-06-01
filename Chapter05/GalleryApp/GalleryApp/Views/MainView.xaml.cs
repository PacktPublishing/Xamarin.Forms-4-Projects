using System;
using System.Collections.Generic;
using GalleryApp.ViewModels;
using Xamarin.Forms;

namespace GalleryApp.Views
{
    public partial class MainView : ContentPage
    {
        public MainView()
        {
            InitializeComponent();

            BindingContext = Resolver.Resolve<MainViewModel>();
        }
    }
}

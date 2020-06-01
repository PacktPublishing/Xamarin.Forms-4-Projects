using System;
using System.Collections.Generic;
using News.Views;
using Xamarin.Forms;

namespace News
{
    public partial class MainShell
    {
        public MainShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("articleview", typeof(ArticleView));
        }
    }
}

﻿using WordMasterApp.Features;

namespace WordMasterApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(WordDetailsPage), typeof(WordDetailsPage));
        }
    }
}

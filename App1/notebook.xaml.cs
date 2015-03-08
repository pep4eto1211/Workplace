﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Syncfsion.UI;
using Syncfusion.DocIO;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Text;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace App1
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class notebook : App1.Common.LayoutAwarePage
    {
        public notebook()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

        private async void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFolder notebooksFolder = await folder.CreateFolderAsync("workplaceNotebooks", CreationCollisionOption.OpenIfExists);
            IReadOnlyList<StorageFile> filesInNotebooksFolder = await notebooksFolder.GetFilesAsync();
            foreach (StorageFile singleFile in filesInNotebooksFolder)
            {
                //Creating button for each book in the folder
                Button btu = new Button();
                btu.Content = singleFile.DisplayName;
                //Styling the button :P
                btu.Height = 400;
                btu.Width = 400;
                btu.VerticalContentAlignment = Windows.UI.Xaml.VerticalAlignment.Bottom;
                btu.HorizontalContentAlignment = Windows.UI.Xaml.HorizontalAlignment.Right;
                btu.FontSize = 40;
                btu.Padding = new Thickness(0, 0, 10, 17);
                btu.FontWeight = FontWeights.Light;
                btu.Tapped += (object senderr,TappedRoutedEventArgs ee) =>
                {
                    object notebookName = singleFile.DisplayName;
                    Frame.Navigate(typeof(singleNotebook), notebookName);
                };
                btu.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch;
                btu.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Stretch;
                btu.Margin = new Thickness(50, 0, 0, 0);
                btu.Style = Application.Current.Resources["NotebookButton"] as Style;
                notebooksStackPanel.Children.Add(btu);
            }
        }
    }
}

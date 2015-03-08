using System;
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
using Windows.Storage;
using Windows.UI.Popups;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace App1
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class profilePage : App1.Common.LayoutAwarePage
    {
        public profilePage()
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
            userNameTextBlock.Text = await FileIO.ReadTextAsync(await folder.GetFileAsync("userName.workplaceData"));
            fullNameTextBlock.Text = await FileIO.ReadTextAsync(await folder.GetFileAsync("fullName.workplaceData"));
            gradeTextBlock.Text = await FileIO.ReadTextAsync(await folder.GetFileAsync("grade.workplaceData"));
            if (await FileIO.ReadTextAsync(await folder.GetFileAsync("homeworkNumber.workplaceData")) == "1")
            {
                homeworkNumberTextBlock.Text = await FileIO.ReadTextAsync(await folder.GetFileAsync("homeworkNumber.workplaceData")) + " предмет";
            }
            else
            {
                homeworkNumberTextBlock.Text = await FileIO.ReadTextAsync(await folder.GetFileAsync("homeworkNumber.workplaceData")) + " предмета";
            }
            StorageFolder booksFolder = await folder.CreateFolderAsync("workplaceBooks", CreationCollisionOption.OpenIfExists);
            IReadOnlyList<StorageFile> allBooks = await booksFolder.GetFilesAsync();
            booksNumberTextBlock.Text = allBooks.Count.ToString();
        }

        private void Button_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(settings));
        }

        private void Button_Tapped_2(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(homework));
        }

        private void Button_Tapped_3(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(books));
        }

        private void Button_Tapped_4(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(gradeManagement));
        }
    }
}

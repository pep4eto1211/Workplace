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
    public sealed partial class sh : App1.Common.LayoutAwarePage
    {
        public sh()
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
            StorageFolder shFolder = await folder.GetFolderAsync("shFolder");
            string tommorrow = DateTime.Now.AddDays(1).DayOfWeek.ToString();
            StorageFile tommorrowSh = await shFolder.CreateFileAsync(tommorrow + ".workplaceData", CreationCollisionOption.OpenIfExists);
            string rawSh = await FileIO.ReadTextAsync(tommorrowSh);
            int toDoForTommorow = 0;
            string[] shArray = rawSh.Split(',');
            StorageFile toDoList = await folder.CreateFileAsync("hsList.workplaceData", CreationCollisionOption.OpenIfExists);
            string rawToDo = await FileIO.ReadTextAsync(toDoList);
            string[] toDoArray = rawToDo.Split(',');
            foreach (string singleShSubject in shArray)
            {
                foreach (string singleToDo in toDoArray)
                {
                    if (singleToDo == singleShSubject && singleToDo != "" && singleShSubject != "")
                    {
                        toDoForTommorow++;
                    }
                }
            }
            homeworkNotification.Text = toDoForTommorow.ToString();
        }

        private void Button_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(todaySh));
        }

        private void Button_Tapped_2(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(tomorrowSh));
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void Button_Tapped_3(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(weeklySh));
        }
    }
}

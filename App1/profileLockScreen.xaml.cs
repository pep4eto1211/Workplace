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
using Windows.Storage.Streams;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.UI.Popups;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace App1
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class profileLockScreen : App1.Common.LayoutAwarePage
    {
        public profileLockScreen()
        {
            this.InitializeComponent();
            string houer = DateTime.Now.Hour.ToString();
            string minute = DateTime.Now.Minute.ToString();
            string second = DateTime.Now.Second.ToString();
            string day = DateTime.Now.Day.ToString();
            string month = DateTime.Now.Month.ToString();
            string year = DateTime.Now.Year.ToString();
            date.Text = day + "." + month + "." + year;
            pageTitle.Text = houer + ":" + minute + ":" + second;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += timer_Tick;
            timer.Start();

        }

        void timer_Tick(object sender, object e)
        {
            string houer = DateTime.Now.Hour.ToString();
            string minute = DateTime.Now.Minute.ToString();
            string second = DateTime.Now.Second.ToString();
            pageTitle.Text = houer + ":" + minute + ":" + second;
            string day = DateTime.Now.Day.ToString();
            string month = DateTime.Now.Month.ToString();
            string year = DateTime.Now.Year.ToString();
            date.Text = day + "." + month + "." + year;
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
            StorageFile homeworkNumber = await folder.GetFileAsync("homeworkNumber.workplaceData");
            if (await FileIO.ReadTextAsync(homeworkNumber) == "0")
            {
                homeworkNotification.Text = "Нямате домашно";
            }
            else
            {
                if (await FileIO.ReadTextAsync(homeworkNumber) == "1")
                {
                    homeworkNotification.Text = "Имате домашно по " + await FileIO.ReadTextAsync(homeworkNumber) + " предмет";
                }
                else
                {
                    homeworkNotification.Text = "Имате домашно по " + await FileIO.ReadTextAsync(homeworkNumber) + " предмета";
                }
            }
            StorageFile userName = await folder.GetFileAsync("userName.workplaceData");
            theUserName.Text = await FileIO.ReadTextAsync(userName);
        }

        private void Button_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            bigUnlockButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            inProgressLogin.Visibility = Windows.UI.Xaml.Visibility.Visible;
            unlockGrid.Visibility = Windows.UI.Xaml.Visibility.Visible;
            passwordTextBox.Focus(Windows.UI.Xaml.FocusState.Programmatic);
        }

        private async void userEnterButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile userPass = await folder.GetFileAsync("userPass.workplaceData");
            if(await FileIO.ReadTextAsync(userPass) == hashPass(passwordTextBox.Password))
            {
                StorageFile profileLock = await folder.GetFileAsync("profileLock.workplaceData");
                await FileIO.WriteTextAsync(profileLock, "no");
                Frame.Navigate(typeof(MainPage));
            }
            else
            {
                MessageDialog mes = new MessageDialog("Въвели сте грешна парола", "Възникна грешка");
                mes.Commands.Add(new UICommand("OK"));
                await mes.ShowAsync();
                passwordTextBox.Password = "";
            }
        }

        public String hashPass(String pass)
        {
            // Convert the message string to binary data.
            IBuffer buffUtf8Msg = CryptographicBuffer.ConvertStringToBinary(pass, BinaryStringEncoding.Utf8);

            // Create a HashAlgorithmProvider object.
            HashAlgorithmProvider objAlgProv = HashAlgorithmProvider.OpenAlgorithm("SHA512");

            // Demonstrate how to retrieve the name of the hashing algorithm.
            String strAlgNameUsed = objAlgProv.AlgorithmName;

            // Hash the message.
            IBuffer buffHash = objAlgProv.HashData(buffUtf8Msg);

            // Verify that the hash length equals the length specified for the algorithm.
            if (buffHash.Length != objAlgProv.HashLength)
            {
                throw new Exception("There was an error creating the hash");
            }

            // Convert the hash to a string (for display).
            String strHashBase64 = CryptographicBuffer.EncodeToBase64String(buffHash);

            // Return the encoded string
            return strHashBase64;
        }

        private void cancelButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            bigUnlockButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
            inProgressLogin.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            unlockGrid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void passwordTextBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                userEnterButton_Tapped(e, new TappedRoutedEventArgs());
            }
        }

        private void pageRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Windows.UI.ViewManagement.ApplicationView.Value == Windows.UI.ViewManagement.ApplicationViewState.Snapped)
            {
                Frame.Navigate(typeof(snappedViewStart));
            }
        }
    }
}

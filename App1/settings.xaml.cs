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
using Windows.Storage.Streams;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace App1
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class settings : App1.Common.LayoutAwarePage
    {
        public settings()
        {
            this.InitializeComponent();
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
            StorageFile userName = await folder.GetFileAsync("userName.workplaceData");
            userNameTextBox.Text = await FileIO.ReadTextAsync(userName);
            string fullName = await FileIO.ReadTextAsync(await folder.GetFileAsync("fullName.workplaceData"));
            string[] names = new string[2];
            names = fullName.Split(' ');
            name.Text = names[0];
            lastName.Text = names[1];
        }

        private void Button_Tapped_2(object sender, TappedRoutedEventArgs e)
        {
            rectBackgroundHide.Visibility = Windows.UI.Xaml.Visibility.Visible;
            rectBackgroundHideRow.Visibility = Windows.UI.Xaml.Visibility.Visible;
            newPassPopup.IsOpen = true;
        }

        private void Button_Tapped_3(object sender, TappedRoutedEventArgs e)
        {
            rectBackgroundHide.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            rectBackgroundHideRow.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            newPassPopup.IsOpen = false;
        }

        private async void Button_Tapped_4(object sender, TappedRoutedEventArgs e)
        {
            if (oldPass.Password != "" && newPass.Password != "" && newPassConf.Password != "") 
            {
                if (newPass.Password == newPassConf.Password)
                {
                    StorageFolder folder = ApplicationData.Current.LocalFolder;
                    if (await FileIO.ReadTextAsync(await folder.GetFileAsync("userPass.workplaceData")) == hashPass(oldPass.Password))
                    {
                        await FileIO.WriteTextAsync(await folder.GetFileAsync("userPass.workplaceData"), hashPass(newPass.Password));
                        MessageDialog changeOk = new MessageDialog("Паролата ви е променена успешно, вече може да използвате новата парола.", "Промяната успешна");
                        changeOk.Commands.Add(new UICommand("OK"));
                        await changeOk.ShowAsync();
                        rectBackgroundHide.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                        rectBackgroundHideRow.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                        newPassPopup.IsOpen = false;
                    }
                    else
                    {
                        MessageDialog oldPassError = new MessageDialog("Въвели сте грешна парола", "Възникна грешка");
                        oldPassError.Commands.Add(new UICommand("OK"));
                        oldPass.Password = "";
                        await oldPassError.ShowAsync();
                    }
                }
                else
                {
                    MessageDialog oldPassError = new MessageDialog("Новите пароли не съвпадат", "Възникна грешка");
                    oldPassError.Commands.Add(new UICommand("OK"));
                    newPass.Password = "";
                    newPassConf.Password = "";
                    await oldPassError.ShowAsync();
                }
            }
            else
            {
                MessageDialog oldPassError = new MessageDialog("Имате празни полета", "Възникна грешка");
                oldPassError.Commands.Add(new UICommand("OK"));
                await oldPassError.ShowAsync();
            }
        }

        private async void Button_Tapped_5(object sender, TappedRoutedEventArgs e)
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            if (userNameTextBox.Text != "" && name.Text != "" && lastName.Text != "")
            {
                await FileIO.WriteTextAsync(await folder.GetFileAsync("userName.workplaceData"), userNameTextBox.Text);
                await FileIO.WriteTextAsync(await folder.GetFileAsync("fullName.workplaceData"), name.Text + " " + lastName.Text);
                MessageDialog changeOk = new MessageDialog("Данните са променени успешно", "Промяната успешна");
                changeOk.Commands.Add(new UICommand("OK"));
                await changeOk.ShowAsync();
            }
            else
            {
                MessageDialog oldPassError = new MessageDialog("Имате празни полета", "Възникна грешка");
                oldPassError.Commands.Add(new UICommand("OK"));
                await oldPassError.ShowAsync();
            }
        }

        private void pageRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Windows.UI.ViewManagement.ApplicationView.Value == Windows.UI.ViewManagement.ApplicationViewState.Snapped)
            {
                Frame.Navigate(typeof(settingsSnapped));
            }
        }

        private void GoBackTwo(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void Button_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            rectBackgroundHide.Visibility = Windows.UI.Xaml.Visibility.Visible;
            rectBackgroundHideRow.Visibility = Windows.UI.Xaml.Visibility.Visible;
            delProfilePopup.IsOpen = true;
            topBar.IsOpen = false;
        }

        private async void button4_Tapped(object sender, TappedRoutedEventArgs e)
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            if (await FileIO.ReadTextAsync(await folder.GetFileAsync("userPass.workplaceData")) == hashPass(delProfilePass.Password))
            {
                rectBackgroundHide.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                rectBackgroundHideRow.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                delProfilePopup.IsOpen = false;
                MessageDialog finishDialog = new MessageDialog("Искате ли да запазите учебниците при изтриването на профила?", "Изтриване на профила");
                finishDialog.Commands.Add(new UICommand("Да", async (UICEH) =>
                {
                    IReadOnlyList<StorageFile> allFiles = await folder.GetFilesAsync();
                    foreach (StorageFile singleFile in allFiles)
                    {
                        await singleFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                    }
                    IReadOnlyList<StorageFolder> allFolders = await folder.GetFoldersAsync();
                    foreach (StorageFolder singleFolder in allFolders)
                    {
                        if (singleFolder.DisplayName != "workplaceBooks")
                        {
                            await singleFolder.DeleteAsync(StorageDeleteOption.PermanentDelete);
                        }
                    }
                    Frame.Navigate(typeof(MainPage));
                }));
                finishDialog.Commands.Add(new UICommand("Не", async (UICEH) =>
                {
                    IReadOnlyList<StorageFile> allFiles = await folder.GetFilesAsync();
                    foreach (StorageFile singleFile in allFiles)
                    {
                        await singleFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                    }
                    IReadOnlyList<StorageFolder> allFolders = await folder.GetFoldersAsync();
                    foreach (StorageFolder singleFolder in allFolders)
                    {
                        await singleFolder.DeleteAsync(StorageDeleteOption.PermanentDelete);
                    }
                    Frame.Navigate(typeof(MainPage));
                }));
                finishDialog.Commands.Add(new UICommand("Отказ"));
                await finishDialog.ShowAsync();
            }
            else
            {
                MessageDialog oldPassError = new MessageDialog("Въвели сте грешна парола", "Възникна грешка");
                oldPassError.Commands.Add(new UICommand("OK"));
                delProfilePass.Password = "";
                await oldPassError.ShowAsync();
            }
        }

        private void button5_Tapped(object sender, TappedRoutedEventArgs e)
        {
            rectBackgroundHide.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            rectBackgroundHideRow.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            delProfilePopup.IsOpen = false;
        }
    }
}
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
    public sealed partial class gradeManagement : App1.Common.LayoutAwarePage
    {
        public gradeManagement()
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

        private void Button_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            rectBackgroundHide.Visibility = Windows.UI.Xaml.Visibility.Visible;
            rectBackgroundHideRow.Visibility = Windows.UI.Xaml.Visibility.Visible;
            delProfilePopup.IsOpen = true;
        }

        private async void button4_Tapped(object sender, TappedRoutedEventArgs e)
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            if (await FileIO.ReadTextAsync(await folder.GetFileAsync("userPass.workplaceData")) == hashPass(delProfilePass.Password))
            {
                rectBackgroundHide.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                rectBackgroundHideRow.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                delProfilePopup.IsOpen = false;
                MessageDialog finishDialog = new MessageDialog("Искате ли да запазите учебниците при преминаването в по-горен клас?", "Завършване на клас");
                finishDialog.Commands.Add(new UICommand("Да", async (UICEH) =>
                {
                    string grade = await FileIO.ReadTextAsync(await folder.GetFileAsync("grade.workplaceData"));
                    int newGradeInt = int.Parse(grade);
                    newGradeInt++;
                    string newGrade = newGradeInt.ToString();
                    await FileIO.WriteTextAsync(await folder.GetFileAsync("grade.workplaceData"), newGrade);
                    await FileIO.WriteTextAsync(await folder.CreateFileAsync("homeworkNumber.workplaceData", CreationCollisionOption.OpenIfExists), "0");
                    await FileIO.WriteTextAsync(await folder.CreateFileAsync("hsList.workplaceData", CreationCollisionOption.OpenIfExists), "");
                    await FileIO.WriteTextAsync(await folder.CreateFileAsync("profileLock.workplaceData", CreationCollisionOption.OpenIfExists), "no");
                    await FileIO.WriteTextAsync(await folder.CreateFileAsync("subjectsList.workplaceData", CreationCollisionOption.OpenIfExists), "");
                    await FileIO.WriteTextAsync(await folder.CreateFileAsync("workplaceSubjects.workplaceData", CreationCollisionOption.OpenIfExists), "");
                    StorageFolder shFolder = await folder.GetFolderAsync("shFolder");
                    IReadOnlyList<StorageFile> shFiles = await shFolder.GetFilesAsync();
                    foreach (StorageFile singleFile in shFiles)
                    {
                        await singleFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                    }
                    StorageFolder homeworkFolder = await folder.GetFolderAsync("workplaceHomework");
                    IReadOnlyList<StorageFile> homeworkFiles = await homeworkFolder.GetFilesAsync();
                    foreach (StorageFile singleFile in homeworkFiles)
                    {
                        await singleFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                    }
                    StorageFolder notebooksFolder = await folder.GetFolderAsync("workplaceNotebooks");
                    IReadOnlyList<StorageFile> notebookFiles = await notebooksFolder.GetFilesAsync();
                    foreach (StorageFile singleFile in notebookFiles)
                    {
                        await singleFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                    }
                    Frame.Navigate(typeof(MainPage));
                }));
                finishDialog.Commands.Add(new UICommand("Не", async (UICEH) =>
                {
                    string grade = await FileIO.ReadTextAsync(await folder.GetFileAsync("grade.workplaceData"));
                    int newGradeInt = int.Parse(grade);
                    newGradeInt++;
                    string newGrade = newGradeInt.ToString();
                    await FileIO.WriteTextAsync(await folder.GetFileAsync("grade.workplaceData"), newGrade);
                    await FileIO.WriteTextAsync(await folder.CreateFileAsync("homeworkNumber.workplaceData", CreationCollisionOption.OpenIfExists), "0");
                    await FileIO.WriteTextAsync(await folder.CreateFileAsync("hsList.workplaceData", CreationCollisionOption.OpenIfExists), "");
                    await FileIO.WriteTextAsync(await folder.CreateFileAsync("profileLock.workplaceData", CreationCollisionOption.OpenIfExists), "no");
                    await FileIO.WriteTextAsync(await folder.CreateFileAsync("subjectsList.workplaceData", CreationCollisionOption.OpenIfExists), "");
                    await FileIO.WriteTextAsync(await folder.CreateFileAsync("workplaceSubjects.workplaceData", CreationCollisionOption.OpenIfExists), "");
                    StorageFolder shFolder = await folder.GetFolderAsync("shFolder");
                    IReadOnlyList<StorageFile> shFiles = await shFolder.GetFilesAsync();
                    foreach (StorageFile singleFile in shFiles)
                    {
                        await singleFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                    }
                    StorageFolder homeworkFolder = await folder.GetFolderAsync("workplaceHomework");
                    IReadOnlyList<StorageFile> homeworkFiles = await homeworkFolder.GetFilesAsync();
                    foreach (StorageFile singleFile in homeworkFiles)
                    {
                        await singleFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                    }
                    StorageFolder notebooksFolder = await folder.GetFolderAsync("workplaceNotebooks");
                    IReadOnlyList<StorageFile> notebookFiles = await notebooksFolder.GetFilesAsync();
                    foreach (StorageFile singleFile in notebookFiles)
                    {
                        await singleFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                    }
                    StorageFolder booksFolder = await folder.GetFolderAsync("workplaceBooks");
                    IReadOnlyList<StorageFile> booksFiles = await booksFolder.GetFilesAsync();
                    foreach (StorageFile singleFile in booksFiles)
                    {
                        await singleFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
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

        private void button2_Tapped(object sender, TappedRoutedEventArgs e)
        {
            rectBackgroundHide.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            rectBackgroundHideRow.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            changeGradePopup.IsOpen = false;
        }

        private void Button_Tapped_2(object sender, TappedRoutedEventArgs e)
        {
            rectBackgroundHide.Visibility = Windows.UI.Xaml.Visibility.Visible;
            rectBackgroundHideRow.Visibility = Windows.UI.Xaml.Visibility.Visible;
            changeGradePopup.IsOpen = true;
        }

        private async void button1_Tapped(object sender, TappedRoutedEventArgs e)
        {   int number;
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            if (await FileIO.ReadTextAsync(await folder.GetFileAsync("userPass.workplaceData")) == hashPass(changeGradePass.Password) && int.TryParse(newGradeBox.Text, out number))
            {
                rectBackgroundHide.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                rectBackgroundHideRow.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                changeGradePopup.IsOpen = false;
                MessageDialog confChange = new MessageDialog("Сигурни ли сте, че искате да промените класът си? Това няма да изтрие или промени нищо от вашите данни освен класът", "Потвърдете смяна на клас");
                confChange.Commands.Add(new UICommand("Да", async (UICEH) =>
                {
                    await FileIO.WriteTextAsync(await folder.GetFileAsync("grade.workplaceData"), newGradeBox.Text);
                    Frame.Navigate(typeof(MainPage));
                }));
                confChange.Commands.Add(new UICommand("Не"));
                await confChange.ShowAsync();
            }
            else
            {
                MessageDialog oldPassError = new MessageDialog("Въвели сте грешни данни", "Възникна грешка");
                oldPassError.Commands.Add(new UICommand("OK"));
                changeGradePass.Password = "";
                newGradeBox.Text = "";
                await oldPassError.ShowAsync();
            }

        }

    }
}

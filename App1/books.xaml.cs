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
using Windows.Storage.Pickers;
using Windows.UI.Text;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace App1
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class books : App1.Common.LayoutAwarePage
    {
        public books()
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
            //Populating the page with the books from ApllicationData.Current.LocalFolder/workplaceBooks
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFolder booksFolder =  await folder.CreateFolderAsync("workplaceBooks", CreationCollisionOption.OpenIfExists);
            IReadOnlyList<StorageFile> filesInBooksFolder = await booksFolder.GetFilesAsync();
            int allBooksNumber = filesInBooksFolder.Count;
            foreach (StorageFile singleFile in filesInBooksFolder)
            {
                //Creating button for each book in the folder
                Button btu = new Button();
                btu.Content = singleFile.DisplayName;
                //Creating the Tapped event
                btu.Tapped += async (object senderr, TappedRoutedEventArgs ee) =>
                {
                    if (singleFile.FileType == ".docx")
                    {
                        object toRead = singleFile.DisplayName;
                        Frame.Navigate(typeof(bookReader), toRead);   
                    }
                    else
                    {
                        await Windows.System.Launcher.LaunchFileAsync(singleFile);
                    }
                };
                //Styling the button :P
                btu.Height = 400;
                btu.Width = 400;
                btu.VerticalContentAlignment = Windows.UI.Xaml.VerticalAlignment.Bottom;
                btu.HorizontalContentAlignment = Windows.UI.Xaml.HorizontalAlignment.Right;
                btu.FontSize = 40;
                btu.Padding = new Thickness(0, 0, 10, 17);
                btu.FontWeight = FontWeights.Light;
                btu.IsDoubleTapEnabled = false;
                btu.RightTapped += async (object senderr, RightTappedRoutedEventArgs ee) =>
                {
                    PopupMenu menu = new PopupMenu();
                    menu.Commands.Add(new UICommand("Премахнете учебник '" + singleFile.DisplayName + "'", async (UICommandInvokedHandler) =>
                        {
                            MessageDialog deleteConf = new MessageDialog("Сигурен ли сте, че искате да премахнете учебник '" + singleFile.DisplayName + "'?", "Потвърдете изтриване на учебник");
                            deleteConf.Commands.Add(new UICommand("Да", async (action) =>
                            {
                                await singleFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                                booksStackPanel.Children.Clear();
                                pageRoot_Loaded(sender, e);
                            }));
                            deleteConf.Commands.Add(new UICommand("Не"));
                            await deleteConf.ShowAsync();
                        }));
                    await menu.ShowAsync(ee.GetPosition(this));
                };
                btu.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch;
                btu.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Stretch;
                btu.Margin = new Thickness(50, 0, 0, 0);
                btu.Style = Application.Current.Resources["BookButton"] as Style;
                booksStackPanel.Children.Add(btu);
            }
        }
        private void Button_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            if (Windows.UI.ViewManagement.ApplicationView.Value != Windows.UI.ViewManagement.ApplicationViewState.Snapped || Windows.UI.ViewManagement.ApplicationView.TryUnsnap() == true)
            {
                //Shows the select book name dialog
                if (Windows.UI.ViewManagement.ApplicationView.Value == Windows.UI.ViewManagement.ApplicationViewState.Filled)
                {
                    Frame.Navigate(typeof(addNewBookFilled));
                }
                else
                {
                    bookNamePopup.IsOpen = true; 
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //Close the select book name dialog
            bookNamePopup.IsOpen = false;
        }

        private async void Button_Tapped_2(object sender, TappedRoutedEventArgs e)
        {
            if (newBookName.Text != "")
            {
                try
                {
                    //Trying to add new book file to the books folder
                    Windows.Storage.Pickers.FileOpenPicker openPicker = new Windows.Storage.Pickers.FileOpenPicker();
                    openPicker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
                    openPicker.FileTypeFilter.Add(".docx");
                    openPicker.FileTypeFilter.Add(".pdf");
                    openPicker.CommitButtonText = "Добавяне";
                    StorageFile fileToCopy = await openPicker.PickSingleFileAsync();
                    if (fileToCopy != null)
                    {
                        //Repopulating the page with the new book
                        StorageFolder mainFolder = ApplicationData.Current.LocalFolder;
                        StorageFolder folderToCopyIn = await mainFolder.GetFolderAsync("workplaceBooks");
                        await fileToCopy.CopyAsync(folderToCopyIn, newBookName.Text + fileToCopy.FileType, NameCollisionOption.FailIfExists);
                        IReadOnlyList<StorageFile> filesInBooksFolder = await folderToCopyIn.GetFilesAsync();
                        int allBooksNumber = filesInBooksFolder.Count;
                        booksStackPanel.Children.Clear();
                        foreach (StorageFile singleFile in filesInBooksFolder)
                        {
                            Button btu = new Button();
                            btu.Content = singleFile.DisplayName;
                            btu.Tapped += async (object senderr, TappedRoutedEventArgs ee) =>
                            {
                                if (singleFile.FileType == ".docx")
                                {
                                    object toRead = singleFile.DisplayName;
                                    Frame.Navigate(typeof(bookReader), toRead);
                                }
                                else
                                {
                                    await Windows.System.Launcher.LaunchFileAsync(singleFile);
                                }
                            };
                            btu.Height = 400;
                            btu.Width = 400;
                            btu.VerticalContentAlignment = Windows.UI.Xaml.VerticalAlignment.Bottom;
                            btu.HorizontalContentAlignment = Windows.UI.Xaml.HorizontalAlignment.Right;
                            btu.FontSize = 40;
                            btu.Padding = new Thickness(0, 0, 10, 17);
                            btu.FontWeight = FontWeights.Light;
                            btu.RightTapped += async (object senderr, RightTappedRoutedEventArgs ee) =>
                            {
                                PopupMenu menu = new PopupMenu();
                                menu.Commands.Add(new UICommand("Премахнете учебник '" + singleFile.DisplayName + "'", async (UICommandInvokedHandler) =>
                                {
                                    MessageDialog deleteConf = new MessageDialog("Сигурен ли сте, че искате да премахнете учебник '" + singleFile.DisplayName + "'?", "Потвърдете изтриване на учебник");
                                    deleteConf.Commands.Add(new UICommand("Да", async (action) =>
                                    {
                                        await singleFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                                        booksStackPanel.Children.Clear();
                                        pageRoot_Loaded(sender, e);
                                    }));
                                    deleteConf.Commands.Add(new UICommand("Не"));
                                    await deleteConf.ShowAsync();
                                }));
                                await menu.ShowAsync(ee.GetPosition(this));
                            };
                            btu.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch;
                            btu.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Stretch;
                            btu.Margin = new Thickness(50, 0, 0, 0);
                            btu.Style = Application.Current.Resources["BookButton"] as Style;
                            booksStackPanel.Children.Add(btu);
                        }
                        //Message that adding is OK
                        MessageDialog deleteDialog = new MessageDialog("Учебникът е добавен успешно към работното ви място, вече може да изтриете сваленият файл.", "Добавяне успешно");
                        deleteDialog.Commands.Add(new UICommand("OK"));
                        await deleteDialog.ShowAsync();
                    }
                    bookNamePopup.IsOpen = false;
                    newBookName.Text = "";
                }
                catch (Exception er)
                {
                    //If there is a book with this name already
                    MessageDialog errorMessage = new MessageDialog(er.Message, "Възникна грешка");
                    errorMessage.Commands.Add(new UICommand("OK"));
                    errorMessage.ShowAsync();
                    newBookName.Text = "";
                }
            }
            else
            {
                MessageDialog emptyNameerror = new MessageDialog("Моля въведете име за учебника", "Възникна грешка");
                emptyNameerror.Commands.Add(new UICommand("OK"));
                await emptyNameerror.ShowAsync();
            }
        }

        private async void Button_Tapped_3(object sender, TappedRoutedEventArgs e)
        {
            endDeleteButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
            pageTitle.Text = "Изтриване на учебници";
            MessageDialog mes = new MessageDialog("За да изтриете учебник го докоснете или кликнете с мишката на него", "Инструкции");
            mes.Commands.Add(new UICommand("Всичко е ясно"));
            await mes.ShowAsync();
            //Populating the page with the books from ApllicationData.Current.LocalFolder/workplaceBooks
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFolder booksFolder = await folder.CreateFolderAsync("workplaceBooks", CreationCollisionOption.OpenIfExists);
            IReadOnlyList<StorageFile> filesInBooksFolder = await booksFolder.GetFilesAsync();
            int allBooksNumber = filesInBooksFolder.Count;
            booksStackPanel.Children.Clear();
            foreach (StorageFile singleFile in filesInBooksFolder)
            {
                //Creating button for each book in the folder
                Button btu = new Button();
                btu.Content = singleFile.DisplayName;
                //Creating the Tapped event
                btu.Tapped += async (object senderr, TappedRoutedEventArgs ee) =>
                {
                    MessageDialog deleteConf = new MessageDialog("Сигурен ли сте, че искате да премахнете учебник '" + singleFile.DisplayName + "'?", "Потвърдете изтриване на учебник");
                    deleteConf.Commands.Add( new UICommand("Да", async (UICommandInvokedHandler) => 
                    {
                       await singleFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                       repopulateForDeleteing();
                    }));
                    deleteConf.Commands.Add(new UICommand("Не", (UICommandInvokedHandler) =>
                    { }));
                    await deleteConf.ShowAsync();
                };
                //Styling the button :P
                btu.Height = 400;
                btu.Width = 400;
                btu.VerticalContentAlignment = Windows.UI.Xaml.VerticalAlignment.Bottom;
                btu.HorizontalContentAlignment = Windows.UI.Xaml.HorizontalAlignment.Right;
                btu.FontSize = 40;
                btu.Padding = new Thickness(0, 0, 10, 17);
                btu.FontWeight = FontWeights.Light;
                btu.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch;
                btu.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Stretch;
                btu.Margin = new Thickness(50, 0, 0, 0);
                btu.Style = Application.Current.Resources["BookButton"] as Style;
                booksStackPanel.Children.Add(btu);
            }
        }
        private async void repopulateForDeleteing()
        {
            //Populating the page with the books from ApllicationData.Current.LocalFolder/workplaceBooks
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFolder booksFolder = await folder.CreateFolderAsync("workplaceBooks", CreationCollisionOption.OpenIfExists);
            IReadOnlyList<StorageFile> filesInBooksFolder = await booksFolder.GetFilesAsync();
            int allBooksNumber = filesInBooksFolder.Count;
            booksStackPanel.Children.Clear();
            foreach (StorageFile singleFile in filesInBooksFolder)
            {
                //Creating button for each book in the folder
                Button btu = new Button();
                btu.Content = singleFile.DisplayName;
                //Creating the Tapped event
                btu.Tapped += async (object senderer, TappedRoutedEventArgs ehe) =>
                {
                    MessageDialog deleteConf = new MessageDialog("Сигурен ли сте, че искате да премахнете учебник '" + singleFile.DisplayName + "'", "Потвърдете изтриване на учебник");
                    deleteConf.Commands.Add(new UICommand("Да", async (UICommandInvokedHandler) =>
                    {
                        await singleFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                        repopulateForDeleteing();
                    }));
                    deleteConf.Commands.Add(new UICommand("Не", (UICommandInvokedHandler) =>
                    { }));
                    await deleteConf.ShowAsync();
                };
                //Styling the button :P
                btu.Height = 400;
                btu.Width = 400;
                btu.VerticalContentAlignment = Windows.UI.Xaml.VerticalAlignment.Bottom;
                btu.HorizontalContentAlignment = Windows.UI.Xaml.HorizontalAlignment.Right;
                btu.FontSize = 40;
                btu.Padding = new Thickness(0, 0, 10, 17);
                btu.FontWeight = FontWeights.Light;
                btu.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch;
                btu.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Stretch;
                btu.Margin = new Thickness(50, 0, 0, 0);
                btu.Style = Application.Current.Resources["BookButton"] as Style;
                booksStackPanel.Children.Add(btu);
            }
        }

        private void endDeleteButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            booksStackPanel.Children.Clear();
            pageRoot_Loaded(sender, e);
            endDeleteButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            pageTitle.Text = "Учебници";
        }

        private void GoBackTwo(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void pageRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Windows.UI.ViewManagement.ApplicationView.Value == Windows.UI.ViewManagement.ApplicationViewState.Filled && bookNamePopup.IsOpen == true)
            {
                Frame.Navigate(typeof(addNewBookFilled));
            }
            else
            {
                if (Windows.UI.ViewManagement.ApplicationView.Value == Windows.UI.ViewManagement.ApplicationViewState.Snapped)
                {
                    Frame.Navigate(typeof(addNewBookFilled));
                }
            }
        }

        private void Image_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            topAppBar.IsOpen = true;
        }
    }
}

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
using Windows.UI.ViewManagement;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace App1
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class subjects : App1.Common.LayoutAwarePage
    {
        public subjects()
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
            subjectsStackPanel.Children.Clear();
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile subjectsFile = await folder.CreateFileAsync("workplaceSubjects.workplaceData", CreationCollisionOption.OpenIfExists);
            if (await FileIO.ReadTextAsync(subjectsFile) == "")
            {
                TextBlock noSubjectsText = new TextBlock();
                noSubjectsText.Text = "Нямате добавени предмети";
                noSubjectsText.Style = Application.Current.Resources["PageHeaderTextStyle"] as Style;
                noSubjectsText.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center;
                noSubjectsText.Margin = new Thickness(0, 200, 0, 0);
                Button addSubjectsButton = new Button();
                addSubjectsButton.Content = "Добавяне на предмети";
                addSubjectsButton.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center;
                addSubjectsButton.Margin = new Thickness(0, 20, 0, 0);
                addSubjectsButton.FontSize = 30;
                addSubjectsButton.Tapped += (object senderr, TappedRoutedEventArgs ee) =>
                {
                    hideRectOne.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    hideRectTwo.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    addSubjectPopup.IsOpen = true;
                };
                subjectsStackPanel.Children.Add(noSubjectsText);
                subjectsStackPanel.Children.Add(addSubjectsButton);
            }
            else
            {
                string[] subjectsArray = new string[100];
                string rawSubjects = await FileIO.ReadTextAsync(subjectsFile);
                subjectsArray = rawSubjects.Split(',');
                foreach (string singleSubject in subjectsArray)
                {
                    Button btu = new Button();
                    btu.Content = singleSubject;
                    StorageFile toDoList = await folder.CreateFileAsync("hsList.workplaceData", CreationCollisionOption.OpenIfExists);
                    string rawToDo = await FileIO.ReadTextAsync(toDoList);
                    string[] toDoArray = rawToDo.Split(',');
                    foreach (string singleToDo in toDoArray)
                    {
                        if (singleToDo == singleSubject)
                        {
                            btu.Content += " (ДР)";
                        }
                    }
                    btu.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch;
                    btu.FontWeight = FontWeights.Light;
                    btu.FontSize = 30;
                    btu.BorderThickness = new Thickness(1);
                    btu.Margin = new Thickness(120, 20, 0, 0);
                    btu.HorizontalContentAlignment = Windows.UI.Xaml.HorizontalAlignment.Left;
                    btu.Tapped += async (action, ee) =>
                    {
                        MessageDialog whichBook = new MessageDialog("Изберете коя тетрадка искате да отворите", "Избор на тетрадка");
                        whichBook.Commands.Add(new UICommand("За домашна работа", (UICEH) =>
                            {
                                object passParameter = singleSubject;
                                Frame.Navigate(typeof(singleHomework), passParameter);
                            }));
                        whichBook.Commands.Add(new UICommand("За работа в клас", (UICEH) =>
                        {
                            object passParameter = singleSubject;
                            Frame.Navigate(typeof(singleNotebook), passParameter);
                        }));
                        whichBook.Commands.Add(new UICommand("Отказ"));
                        await whichBook.ShowAsync();
                    };
                    btu.RightTapped += async (action, aa) =>
                    {
                        PopupMenu deletePopup = new PopupMenu();
                        deletePopup.Commands.Add(new UICommand("Премахнете предмет '" + singleSubject + "'", async (UICIH) =>
                        {
                            MessageDialog deleteConf = new MessageDialog("Сигурен ли сте, че искате да премахнете предмета '" + singleSubject + "' и всички негови тетрадки?", "Потвърдете изтриване на предмет");
                            deleteConf.Commands.Add(new UICommand("Да", async (actionn) =>
                            {
                                string[] subjectsArrayDelete = new string[100];
                                StorageFile subjectsFileDelete = await folder.GetFileAsync("workplaceSubjects.workplaceData");
                                string rawSubjectsDelete = await FileIO.ReadTextAsync(subjectsFileDelete);
                                subjectsArrayDelete = rawSubjectsDelete.Split(',');
                                string newRawSubjects = "";
                                foreach(string singleSubjectDelete in subjectsArrayDelete)
                                {
                                    if (singleSubjectDelete != singleSubject)
                                    {
                                        if (newRawSubjects == "")
                                        {
                                            newRawSubjects = singleSubjectDelete;
                                        }
                                        else
                                        {
                                            newRawSubjects += "," + singleSubjectDelete;
                                        }
                                    }
                                }
                                await FileIO.WriteTextAsync(subjectsFile, newRawSubjects);
                                StorageFolder notebooksFolder = await folder.GetFolderAsync("workplaceNotebooks");
                                StorageFile notebookFile = await notebooksFolder.GetFileAsync(singleSubject + ".rtf");
                                await notebookFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                                StorageFolder homeworkFolder = await folder.GetFolderAsync("workplaceHomework");
                                StorageFile homeworkFile = await homeworkFolder.GetFileAsync(singleSubject + ".rtf");
                                await homeworkFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                                subjectsStackPanel.Children.Clear();
                                pageRoot_Loaded(sender, e);
                            }));
                            deleteConf.Commands.Add(new UICommand("Не"));
                            await deleteConf.ShowAsync();
                        }));
                        deletePopup.Commands.Add(new UICommand("Запазване на тетрадките по '" + singleSubject + "'", async (UICEH) =>
                        {
                            if (Windows.UI.ViewManagement.ApplicationView.Value != Windows.UI.ViewManagement.ApplicationViewState.Snapped || Windows.UI.ViewManagement.ApplicationView.TryUnsnap() == true)
                            {
                                bool one = false, two = false;
                                FileSavePicker savePicker = new FileSavePicker();
                                savePicker.DefaultFileExtension = ".rtf";
                                savePicker.FileTypeChoices.Add(".rtf File", new List<string> { ".rtf" });
                                savePicker.SuggestedFileName = singleSubject;
                                savePicker.CommitButtonText = "Запази";
                                StorageFolder notebooksFolder = await folder.GetFolderAsync("workplaceNotebooks");
                                StorageFile notebookFile = await notebooksFolder.GetFileAsync(singleSubject + ".rtf");
                                savePicker.SuggestedStartLocation = PickerLocationId.Desktop;
                                StorageFile newFile = await savePicker.PickSaveFileAsync();
                                if (newFile != null)
                                {
                                    await notebookFile.CopyAndReplaceAsync(newFile);
                                    one = true;
                                }
                                FileSavePicker savePickerHomework = new FileSavePicker();
                                savePickerHomework.DefaultFileExtension = ".rtf";
                                savePickerHomework.FileTypeChoices.Add(".rtf File", new List<string> { ".rtf" });
                                savePickerHomework.SuggestedFileName = singleSubject + "Домашно";
                                savePickerHomework.CommitButtonText = "Запази";
                                StorageFolder homeworkFolder = await folder.GetFolderAsync("workplaceHomework");
                                StorageFile homeworkFile = await homeworkFolder.GetFileAsync(singleSubject + ".rtf");
                                savePicker.SuggestedStartLocation = PickerLocationId.Desktop;
                                StorageFile newFileHomework = await savePickerHomework.PickSaveFileAsync();
                                if (newFileHomework != null)
                                {
                                    await homeworkFile.CopyAndReplaceAsync(newFileHomework);
                                    two = true;
                                }
                                if (one && two)
                                {
                                    MessageDialog success = new MessageDialog("Тетрадките по " + singleSubject + " са запаметени успешно", "Запаметяване успешно");
                                    success.Commands.Add(new UICommand("OK"));
                                    await success.ShowAsync();
                                }
                            }
                        }));
                        await deletePopup.ShowAsync(aa.GetPosition(this));
                        topBar.IsOpen = false;
                    };
                    subjectsStackPanel.Children.Add(btu);
                }
            }
        }

        private void Button_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            addSubjectPopup.IsOpen = false;
            hideRectOne.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            hideRectTwo.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            subjectsStackPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
            newSubjectNameTextBox.Text = "";
        }

        private void Button_Tapped_2(object sender, TappedRoutedEventArgs e)
        {
            hideRectOne.Visibility = Windows.UI.Xaml.Visibility.Visible;
            hideRectTwo.Visibility = Windows.UI.Xaml.Visibility.Visible;
            subjectsStackPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            addSubjectPopup.IsOpen = true;
            topBar.IsOpen = false;
        }

        private async void Button_Tapped_3(object sender, TappedRoutedEventArgs e)
        {
            if (newSubjectNameTextBox.Text != "")
            {
                try
                {
                    StorageFolder folder = ApplicationData.Current.LocalFolder;
                    StorageFile subjectsFile = await folder.GetFileAsync("workplaceSubjects.workplaceData");
                    StorageFolder notebooksFolder = await folder.CreateFolderAsync("workplaceNotebooks", CreationCollisionOption.OpenIfExists);
                    StorageFolder homeworkFolder = await folder.CreateFolderAsync("workplaceHomework", CreationCollisionOption.OpenIfExists);
                    await notebooksFolder.CreateFileAsync(newSubjectNameTextBox.Text + ".rtf", CreationCollisionOption.FailIfExists);
                    await homeworkFolder.CreateFileAsync(newSubjectNameTextBox.Text + ".rtf", CreationCollisionOption.FailIfExists);
                    string rawSubjects = await FileIO.ReadTextAsync(subjectsFile);
                    if (rawSubjects == "")
                    {
                        rawSubjects = newSubjectNameTextBox.Text;
                    }
                    else
                    {
                        rawSubjects += "," + newSubjectNameTextBox.Text;
                    }
                    await FileIO.WriteTextAsync(subjectsFile, rawSubjects);
                    MessageDialog subjectSuc = new MessageDialog("Предметът е добавен успешно към вашето работно място.", "Добавяне успешно");
                    subjectSuc.Commands.Add(new UICommand("OK"));
                    await subjectSuc.ShowAsync();
                    addSubjectPopup.IsOpen = false;
                    hideRectOne.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    hideRectTwo.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    subjectsStackPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    newSubjectNameTextBox.Text = "";
                    pageRoot_Loaded(sender, e);
                }
                catch (Exception)
                {
                    MessageDialog creationError = new MessageDialog("Вече сте добавили този предмет", "Възникна грешка");
                    creationError.Commands.Add(new UICommand("OK"));
                    creationError.ShowAsync();
                    newSubjectNameTextBox.Text = "";
                }
            }
            else
            {
                MessageDialog creationError = new MessageDialog("Моля въведете име за предмета", "Възникна грешка");
                creationError.Commands.Add(new UICommand("OK"));
                await creationError.ShowAsync();
                newSubjectNameTextBox.Text = "";
            }
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}

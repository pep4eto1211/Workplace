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
using Windows.UI.Notifications;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace App1
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class singleHomework : App1.Common.LayoutAwarePage
    {
        public singleHomework()
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
        protected async override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            pageTitle.Text = "Домашна тетрадка по " + navigationParameter;
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFolder notebooksFolder = await folder.CreateFolderAsync("workplaceHomework", CreationCollisionOption.OpenIfExists);
            StorageFile notebookFile = await notebooksFolder.GetFileAsync(navigationParameter + ".rtf");
            notebookName = navigationParameter.ToString();
            IRandomAccessStream rtfReadStream = await notebookFile.OpenAsync(FileAccessMode.Read);
            notebookField.Document.LoadFromStream(Windows.UI.Text.TextSetOptions.FormatRtf, rtfReadStream);
            rtfReadStream.Dispose();
            StorageFile todoList = await folder.GetFileAsync("hsList.workplaceData");
            string rawToDo = await FileIO.ReadTextAsync(todoList);
            string[] todoArray = rawToDo.Split(',');
            bool isIn = false;
            foreach (string singlesubject in todoArray)
            {
                if (singlesubject == navigationParameter.ToString())
                {
                    isIn = true;
                }
            }
            if (!isIn)
            {
                toolboxBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        public string notebookName = "";

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected async override void SaveState(Dictionary<String, Object> pageState)
        {
            if (madeChanges)
            {
                 try
            {
                StorageFolder folder = ApplicationData.Current.LocalFolder;
                StorageFolder notebooksFolder = await folder.CreateFolderAsync("workplaceHomework", CreationCollisionOption.OpenIfExists);
                StorageFile notebookFile = await notebooksFolder.GetFileAsync(notebookName + ".rtf");
                IRandomAccessStream rtfReadStream = await notebookFile.OpenAsync(FileAccessMode.ReadWrite);
                notebookField.Document.SaveToStream(Windows.UI.Text.TextGetOptions.FormatRtf, rtfReadStream);
                var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);
                var elements = toastXml.GetElementsByTagName("text");
                elements[0].AppendChild(toastXml.CreateTextNode("Промените които направихте по домашното бяха запазени успешно"));
                var toast = new ToastNotification(toastXml);
                ToastNotificationManager.CreateToastNotifier().Show(toast);
                await rtfReadStream.FlushAsync();
                rtfReadStream.Dispose();
                madeChanges = false;
            }
            catch (Exception sa)
            {
               var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);
                    var elements = toastXml.GetElementsByTagName("text");
                    elements[0].AppendChild(toastXml.CreateTextNode("Домашното е в процес на обработка и промените не бяха запазени"));
                    var toast = new ToastNotification(toastXml);
                    ToastNotificationManager.CreateToastNotifier().Show(toast);
            }
            }
        }

        public bool madeChanges = false;

        private void Button_Tapped_2(object sender, TappedRoutedEventArgs e)
        {
            notebookField.Document.GetRange(notebookField.Document.Selection.StartPosition, notebookField.Document.Selection.EndPosition).CharacterFormat.Bold = Windows.UI.Text.FormatEffect.On;
            madeChanges = true;
        }

        private void Button_Tapped_3(object sender, TappedRoutedEventArgs e)
        {
            notebookField.Document.GetRange(notebookField.Document.Selection.StartPosition, notebookField.Document.Selection.EndPosition).CharacterFormat.FontStyle = Windows.UI.Text.FontStyle.Italic;
            madeChanges = true;
        }

        private void Button_Tapped_4(object sender, TappedRoutedEventArgs e)
        {
            notebookField.Document.GetRange(notebookField.Document.Selection.StartPosition, notebookField.Document.Selection.EndPosition).CharacterFormat.Underline = Windows.UI.Text.UnderlineType.Single;
            madeChanges = true;
        }

        private void Button_Tapped_5(object sender, TappedRoutedEventArgs e)
        {
            notebookField.Document.GetRange(notebookField.Document.Selection.StartPosition, notebookField.Document.Selection.EndPosition).CharacterFormat.ForegroundColor = Windows.UI.Colors.Red;
            madeChanges = true;
        }

        private void Button_Tapped_6(object sender, TappedRoutedEventArgs e)
        {
            notebookField.Document.GetRange(notebookField.Document.Selection.StartPosition, notebookField.Document.Selection.EndPosition).CharacterFormat.Size = 25;
            notebookField.Document.GetRange(notebookField.Document.Selection.StartPosition, notebookField.Document.Selection.EndPosition).CharacterFormat.Bold = Windows.UI.Text.FormatEffect.On;
            madeChanges = true;
        }

        private async void saveChanges(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                StorageFolder folder = ApplicationData.Current.LocalFolder;
                StorageFolder notebooksFolder = await folder.CreateFolderAsync("workplaceHomework", CreationCollisionOption.OpenIfExists);
                StorageFile notebookFile = await notebooksFolder.GetFileAsync(notebookName + ".rtf");
                IRandomAccessStream rtfReadStream = await notebookFile.OpenAsync(FileAccessMode.ReadWrite);
                notebookField.Document.SaveToStream(Windows.UI.Text.TextGetOptions.FormatRtf, rtfReadStream);
                MessageDialog saveSuccess = new MessageDialog("Промените по домашното са запаметени успешно", "Запаметяване успешно");
                saveSuccess.Commands.Add(new UICommand("OK"));
                await saveSuccess.ShowAsync();
                await rtfReadStream.FlushAsync();
                rtfReadStream.Dispose();
                madeChanges = false;
            }
            catch (Exception sa)
            {
                MessageDialog saveSuccess = new MessageDialog("Домашното е в процес на обработка, моля опитайте след няколко секунди", "Възникна грешка");
                saveSuccess.Commands.Add(new UICommand("OK"));
                saveSuccess.ShowAsync();
            }
        }

        private void notebookField_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            madeChanges = true;
        }

        private async void Button_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            if (madeChanges)
            {
                MessageDialog ask = new MessageDialog("Искате ли да продължите без да запазите последните промени?", "Имате незапазени промени");
                ask.Commands.Add(new UICommand("Да", async (action) =>
                {
                    StorageFolder folder = ApplicationData.Current.LocalFolder;
                    StorageFile homeworkNumberFile = await folder.GetFileAsync("homeworkNumber.workplaceData");
                    string numberToDo = await FileIO.ReadTextAsync(homeworkNumberFile);
                    int number = int.Parse(numberToDo);
                    number--;
                    await FileIO.WriteTextAsync(homeworkNumberFile, number.ToString());
                    StorageFile todoList = await folder.GetFileAsync("hsList.workplaceData");
                    string rawToDo = await FileIO.ReadTextAsync(todoList);
                    string[] toDoarray = rawToDo.Split(',');
                    string newToDo = "";
                    foreach (string singleSubject in toDoarray)
                    {
                        if (singleSubject != notebookName)
                        {
                            if (newToDo == "")
                            {
                                newToDo = singleSubject;
                            }
                            else
                            {
                                newToDo += "," + singleSubject;
                            }
                        }
                    }
                    await FileIO.WriteTextAsync(todoList, newToDo);
                    Frame.Navigate(typeof(homework));
                }));
                ask.Commands.Add(new UICommand("Не"));
                await ask.ShowAsync();
            }
            else
            {
                StorageFolder folder = ApplicationData.Current.LocalFolder;
                StorageFile homeworkNumberFile = await folder.GetFileAsync("homeworkNumber.workplaceData");
                string numberToDo = await FileIO.ReadTextAsync(homeworkNumberFile);
                int number = int.Parse(numberToDo);
                number--;
                await FileIO.WriteTextAsync(homeworkNumberFile, number.ToString());
                string newNumber = number.ToString();
                StorageFile todoList = await folder.GetFileAsync("hsList.workplaceData");
                string rawToDo = await FileIO.ReadTextAsync(todoList);
                string[] toDoarray = rawToDo.Split(',');
                string newToDo = "";
                foreach (string singleSubject in toDoarray)
                {
                    if (singleSubject != notebookName)
                    {
                        if (newToDo == "")
                        {
                            newToDo = singleSubject;
                        }
                        else
                        {
                            newToDo += "," + singleSubject;
                        }
                    }
                }
                await FileIO.WriteTextAsync(todoList, newToDo);

                var tileContent = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquareText02);
                var tileLines = tileContent.SelectNodes("tile/visual/binding/text");
                tileLines[0].InnerText = "Имате";
                if (newNumber == "1")
                {
                    tileLines[1].InnerText = "домашно по " + newNumber + " предмет";
                }
                else
                {

                    tileLines[1].InnerText = "домашно по " + newNumber + " предмета";
                }

                var tileContentWide = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWideBlockAndText02);

                var tileLinesWide = tileContentWide.SelectNodes("tile/visual/binding/text");
                tileLinesWide[0].InnerText = "Имате незавършена домашна работа по:";
                tileLinesWide[1].InnerText = newNumber;
                if (newNumber == "1")
                {
                    tileLinesWide[2].InnerText = "предмет";
                }
                else
                {

                    tileLinesWide[2].InnerText = "предмета";
                }

                var node = tileContent.ImportNode(tileContentWide.GetElementsByTagName("binding").Item(0), true);
                tileContent.GetElementsByTagName("visual").Item(0).AppendChild(node);

                var notificationWide = new TileNotification(tileContent);
                var updaterWide = TileUpdateManager.CreateTileUpdaterForApplication();
                updaterWide.Update(notificationWide);

                MessageDialog success = new MessageDialog("Успешно завършихте това домашно", "Завършване успешно");
                success.Commands.Add(new UICommand("OK"));
                await success.ShowAsync();
                Frame.Navigate(typeof(homework));
            }
            
        }

    }
}

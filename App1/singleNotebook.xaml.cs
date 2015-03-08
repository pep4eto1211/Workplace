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
using Windows.UI.Popups;
using Windows.UI.Notifications;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace App1
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class singleNotebook : App1.Common.LayoutAwarePage
    {
        public singleNotebook()
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
        /// 

        public string notebookName = "";
        public bool madeChanges = false;

        protected async override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            pageTitle.Text = "Тетрадка по " + navigationParameter.ToString();
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFolder notebooksFolder = await folder.CreateFolderAsync("workplaceNotebooks", CreationCollisionOption.OpenIfExists);
            StorageFile notebookFile = await notebooksFolder.GetFileAsync(navigationParameter + ".rtf");
            notebookName = navigationParameter.ToString();
            IRandomAccessStream rtfReadStream = await notebookFile.OpenAsync(FileAccessMode.Read);
            notebookField.Document.LoadFromStream(Windows.UI.Text.TextSetOptions.FormatRtf, rtfReadStream);
            rtfReadStream.Dispose();
        }

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
                    StorageFolder notebooksFolder = await folder.CreateFolderAsync("workplaceNotebooks", CreationCollisionOption.OpenIfExists);
                    StorageFile notebookFile = await notebooksFolder.GetFileAsync(notebookName + ".rtf");
                    IRandomAccessStream rtfReadStream = await notebookFile.OpenAsync(FileAccessMode.ReadWrite);
                    notebookField.Document.SaveToStream(Windows.UI.Text.TextGetOptions.FormatRtf, rtfReadStream);
                    var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);
                    var elements = toastXml.GetElementsByTagName("text");
                    elements[0].AppendChild(toastXml.CreateTextNode("Промените които направихте по тетрадката бяха запазени успешно"));
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
                    elements[0].AppendChild(toastXml.CreateTextNode("Тетрадката е в процес на обработка и промените не бяха запазени"));
                    var toast = new ToastNotification(toastXml);
                    ToastNotificationManager.CreateToastNotifier().Show(toast);
                }
            }
        }

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

        private void GoBack(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private async void saveChanges(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                StorageFolder folder = ApplicationData.Current.LocalFolder;
                StorageFolder notebooksFolder = await folder.CreateFolderAsync("workplaceNotebooks", CreationCollisionOption.OpenIfExists);
                StorageFile notebookFile = await notebooksFolder.GetFileAsync(notebookName + ".rtf");
                IRandomAccessStream rtfReadStream = await notebookFile.OpenAsync(FileAccessMode.ReadWrite);
                notebookField.Document.SaveToStream(Windows.UI.Text.TextGetOptions.FormatRtf, rtfReadStream);
                MessageDialog saveSuccess = new MessageDialog("Промените по тетрадката са запаметени успешно", "Запаметяване успешно");
                saveSuccess.Commands.Add(new UICommand("OK"));
                await saveSuccess.ShowAsync();
                await rtfReadStream.FlushAsync();
                rtfReadStream.Dispose();
                madeChanges = false;
            }
            catch(Exception sa)
            {
                MessageDialog saveSuccess = new MessageDialog("Тетрадката е в процес на обработка, моля опитайте след няколко секунди", "Възникна грешка");
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
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile homeworkNumberFile = await folder.GetFileAsync("homeworkNumber.workplaceData");
            StorageFile homeworksubjectsListFile = await folder.CreateFileAsync("hsList.workplaceData", CreationCollisionOption.OpenIfExists);
            string[] toDoArray = new string[100];
            string rawToDo = await FileIO.ReadTextAsync(homeworksubjectsListFile);
            toDoArray = rawToDo.Split(',');
            bool isThere = false;
            foreach (string singleSubject in toDoArray)
            {
                if (singleSubject == notebookName)
                {
                    isThere = true;
                }
            }
            if (!isThere)
            {
                string newNumber = await FileIO.ReadTextAsync(homeworkNumberFile);
                int newNumberInt = int.Parse(newNumber);
                newNumberInt++;
                newNumber = newNumberInt.ToString();
                await FileIO.WriteTextAsync(homeworkNumberFile, newNumber);
                if (rawToDo == "")
                {
                    rawToDo = notebookName;
                }
                else
                {
                    rawToDo += "," + notebookName;
                }
                await FileIO.WriteTextAsync(homeworksubjectsListFile, rawToDo);
                MessageDialog okMes = new MessageDialog("Домашното е добавено успешно към списъка с домашни", "Домашно добавено");
                okMes.Commands.Add(new UICommand("OK"));
                await okMes.ShowAsync();

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

            }
            else
            {
                MessageDialog okMes = new MessageDialog("Вече сте отбелязали домашно по този предмет", "Възникна грешка");
                okMes.Commands.Add(new UICommand("OK"));
                await okMes.ShowAsync();
            }
            
        }
    }
}

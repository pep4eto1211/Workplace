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
using Windows.UI.Popups;
using Windows.Storage;
using Windows.UI.Notifications;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace App1
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : App1.Common.LayoutAwarePage
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        public string massUserName = "";

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
            Frame.Navigate(typeof(books));
        }

        private void Button_Tapped_2(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(notebook));
        }

        private async void userGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            PopupMenu popup = new PopupMenu();
            popup.Commands.Add(new UICommand("Настройки", new UICommandInvokedHandler(goToSettings)));
            popup.Commands.Add(new UICommand("Преглед на профила", new UICommandInvokedHandler(goToViewProfile)));
            popup.Commands.Add(new UICommand("Управление на класовете", new UICommandInvokedHandler(goToGradeManagement)));
            popup.Commands.Add(new UICommand("Заключи профил", new UICommandInvokedHandler(lockProfile)));
            Point popupShowPoint = e.GetPosition(this);
            await popup.ShowAsync(new Point(popupShowPoint.X, popupShowPoint.Y));
        }

        private void goToGradeManagement(IUICommand command)
        {
            Frame.Navigate(typeof(gradeManagement));
        }

        private void goToViewProfile(IUICommand command)
        {
            Frame.Navigate(typeof(profilePage));
        }

        private void goToSettings(object sender)
        {
            Frame.Navigate(typeof(settings));
        }

        private async void lockProfile(object sender)
        {
            MessageDialog confirmLock = new MessageDialog("Сигурни ли сте, че искате да заключите профила си?", "Потвърдете заключване на профил");
            confirmLock.Commands.Add(new UICommand("Да", async (UICommandInvokedHandler) =>
                {
                    StorageFolder folder = ApplicationData.Current.LocalFolder;
                    StorageFile profileLock = await folder.GetFileAsync("profileLock.workplaceData");
                    await FileIO.WriteTextAsync(profileLock, "yes");
                    Frame.Navigate(typeof(profileLockScreen));
                }));
            confirmLock.Commands.Add(new UICommand("Не"));
            await confirmLock.ShowAsync();
        }

        private void buttonTwo_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(homework));
        }

        private void Button_Tapped_3(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(sh));
        }

        private void Button_Tapped_4(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(subjects));
        }

        private async void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile userFile = await folder.CreateFileAsync("userName.workplaceData", CreationCollisionOption.OpenIfExists);
            StorageFile homeworkNumber = await folder.CreateFileAsync("homeworkNumber.workplaceData", CreationCollisionOption.OpenIfExists);
            StorageFolder shFolder = await folder.CreateFolderAsync("shFolder", CreationCollisionOption.OpenIfExists);
            string homeworkNumberString = await FileIO.ReadTextAsync(homeworkNumber);
            if (homeworkNumberString == "")
            {
                await FileIO.WriteTextAsync(homeworkNumber, "0");
                homeworkNotification.Text = await FileIO.ReadTextAsync(homeworkNumber);
            }
            else
            {
                homeworkNotification.Text = await FileIO.ReadTextAsync(homeworkNumber);
            }
            string userData = await FileIO.ReadTextAsync(userFile);
            if (userData == "")
            {
                Frame.Navigate(typeof(Login));
            }
            else
            {
                massUserName = userData;
                userNameBlock.Text = massUserName;
            }
            StorageFile profileLocked = await folder.CreateFileAsync("profileLock.workplaceData", CreationCollisionOption.OpenIfExists);
            if (await FileIO.ReadTextAsync(profileLocked) == "" || await FileIO.ReadTextAsync(profileLocked) == "no")
            {
                await FileIO.WriteTextAsync(profileLocked, "no");
            }
            else
            {
                Frame.Navigate(typeof(profileLockScreen));
            }
            var tileContent = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquareText02);
            var tileLines = tileContent.SelectNodes("tile/visual/binding/text");
            tileLines[0].InnerText = "Имате";
            if (homeworkNotification.Text == "1")
            {
                tileLines[1].InnerText = "домашно по " + homeworkNotification.Text + " предмет";
            }
            else
            {

                tileLines[1].InnerText = "домашно по " + homeworkNotification.Text + " предмета";
            }

            var tileContentWide = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWideBlockAndText02);
            
            var tileLinesWide = tileContentWide.SelectNodes("tile/visual/binding/text");
            tileLinesWide[0].InnerText = "Имате незавършена домашна работа по:";
            tileLinesWide[1].InnerText = homeworkNotification.Text;
            if (homeworkNotification.Text == "1")
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

        private void userNameBlock_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
        }

        private void userNameBlock_PointerExited(object sender, PointerRoutedEventArgs e)
        {
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

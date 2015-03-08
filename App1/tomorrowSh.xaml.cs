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
using Windows.UI.Text;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace App1
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class tomorrowSh : App1.Common.LayoutAwarePage
    {
        public tomorrowSh()
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

        public string selectedSubjectToInteract = "";

        private async void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            shStackPanel.Children.Clear();
            pageTitle.Text = "Разписание за " + getWeekNameOnBulgarian();
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFolder shFolder = await folder.GetFolderAsync("shFolder");
            string todayDayName = DateTime.Now.AddDays(1).DayOfWeek.ToString();
            StorageFile todaySh = await shFolder.CreateFileAsync(todayDayName + ".workplaceData", CreationCollisionOption.OpenIfExists);
            if (await FileIO.ReadTextAsync(todaySh) == "")
            {
                TextBlock noShText = new TextBlock();
                noShText.Text = "Нямате разписание за този ден";
                noShText.Style = Application.Current.Resources["PageHeaderTextStyle"] as Style;
                noShText.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center;
                noShText.Margin = new Thickness(0, 200, 0, 0);
                shStackPanel.Children.Add(noShText);
            }
            else
            {
                string rawSh = await FileIO.ReadTextAsync(todaySh);
                string[] shArray = rawSh.Split(',');
                int order = 1;
                foreach (string singleShSubject in shArray)
                {
                    Button btu = new Button();
                    btu.Content = order.ToString() + "." + singleShSubject;
                    order++;
                    StorageFile toDoList = await folder.GetFileAsync("hsList.workplaceData");
                    string rawToDo = await FileIO.ReadTextAsync(toDoList);
                    string[] toDoArray = rawToDo.Split(',');
                    foreach (string singleToDo in toDoArray)
                    {
                        if (singleToDo == singleShSubject)
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
                            object passParameter = singleShSubject;
                            Frame.Navigate(typeof(singleHomework), passParameter);
                        }));
                        whichBook.Commands.Add(new UICommand("За работа в клас", (UICEH) =>
                        {
                            object passParameter = singleShSubject;
                            Frame.Navigate(typeof(singleNotebook), passParameter);
                        }));
                        whichBook.Commands.Add(new UICommand("Отказ"));
                        await whichBook.ShowAsync();
                    };
                    btu.RightTapped += async (action, ee) =>
                    {
                        PopupMenu subjectMenu = new PopupMenu();
                        subjectMenu.Commands.Add(new UICommand("Промяна на предмет", async (UICEH) =>
                        {
                            StorageFile subjects = await folder.CreateFileAsync("workplaceSubjects.workplaceData", CreationCollisionOption.OpenIfExists);
                            string[] subjectsList = new string[100];
                            string rawSubjects = await FileIO.ReadTextAsync(subjects);
                            if (rawSubjects != "")
                            {
                                subjectsList = rawSubjects.Split(',');
                                subjectsComboBoxEdit.Items.Clear();
                                foreach (var singleSubject in subjectsList)
                                {
                                    object comboBoxItem = singleSubject;
                                    subjectsComboBoxEdit.Items.Add(comboBoxItem);
                                }
                                editSubjectPopup.IsOpen = true;
                                hideRectOne.Visibility = Windows.UI.Xaml.Visibility.Visible;
                                hideRectTwo.Visibility = Windows.UI.Xaml.Visibility.Visible;
                                shStackPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                                selectedSubjectToInteract = btu.Content.ToString()[0].ToString();
                                topBar.IsOpen = false;
                            }
                            else
                            {
                                MessageDialog noSubjectsDialog = new MessageDialog("Първо трябва да добавите предмети през меню 'Предмети' на началният екран", "Възникна грешка");
                                noSubjectsDialog.Commands.Add(new UICommand("OK"));
                                await noSubjectsDialog.ShowAsync();
                            }
                        }));
                        subjectMenu.Commands.Add(new UICommand("Премахване на предмет", async (UICEH) =>
                        {
                            MessageDialog confDel = new MessageDialog("Сигурни ли сте, че искате да премахнете този предмет", "Потвърдете премахване");
                            confDel.Commands.Add(new UICommand("Да", async (UICIHT) =>
                            {
                                string rawTodaySh = await FileIO.ReadTextAsync(todaySh);
                                string[] shArrayTwo = rawTodaySh.Split(',');
                                int orderTwo = 1;
                                string newRawSh = "";
                                foreach (string singleShSubjectTwo in shArrayTwo)
                                {
                                    if (orderTwo.ToString() != btu.Content.ToString()[0].ToString())
                                    {
                                        if (newRawSh == "")
                                        {
                                            newRawSh = singleShSubjectTwo;
                                        }
                                        else
                                        {
                                            newRawSh += "," + singleShSubjectTwo;
                                        }
                                    }
                                    orderTwo++;
                                }
                                await FileIO.WriteTextAsync(todaySh, newRawSh);
                                pageRoot_Loaded(sender, e);
                            }));
                            confDel.Commands.Add(new UICommand("Не"));
                            await confDel.ShowAsync();
                        }));
                        await subjectMenu.ShowAsync(ee.GetPosition(this));
                    };
                    shStackPanel.Children.Add(btu);
                }
            }
        }

        string getWeekNameOnBulgarian()
        {
            string day = DateTime.Now.AddDays(1).DayOfWeek.ToString();
            string toReturn = "";
            switch (day)
            {
                case "Monday":
                    toReturn = "понеделник";
                    break;
                case "Tuesday":
                    toReturn = "вторник";
                    break;
                case "Wednesday":
                    toReturn = "сряда";
                    break;
                case "Thursday":
                    toReturn = "четвъртък";
                    break;
                case "Friday":
                    toReturn = "петък";
                    break;
                case "Saturday":
                    toReturn = "събота";
                    break;
                case "Sunday":
                    toReturn = "неделя";
                    break;
            }
            return toReturn;
        }

        private async void Button_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile subjects = await folder.CreateFileAsync("workplaceSubjects.workplaceData", CreationCollisionOption.OpenIfExists);
            string[] subjectsList = new string[100];
            string rawSubjects = await FileIO.ReadTextAsync(subjects);
            if (rawSubjects != "")
            {
                subjectsList = rawSubjects.Split(',');
                subjectsComboBox.Items.Clear();
                foreach (var singleSubject in subjectsList)
                {
                    object comboBoxItem = singleSubject;
                    subjectsComboBox.Items.Add(comboBoxItem);
                }
                editShPopup.IsOpen = true;
                hideRectOne.Visibility = Windows.UI.Xaml.Visibility.Visible;
                hideRectTwo.Visibility = Windows.UI.Xaml.Visibility.Visible;
                topBar.IsOpen = false;
            }
            else
            {
                MessageDialog noSubjectsDialog = new MessageDialog("Първо трябва да добавите предмети през меню 'Предмети' на началният екран", "Възникна грешка");
                noSubjectsDialog.Commands.Add(new UICommand("OK"));
                await noSubjectsDialog.ShowAsync();
            }
        }

        private async void Button_Tapped_3(object sender, TappedRoutedEventArgs e)
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFolder shFolder = await folder.GetFolderAsync("shFolder");
            string todayDayName = DateTime.Now.AddDays(1).DayOfWeek.ToString();
            StorageFile todaySh = await shFolder.CreateFileAsync(todayDayName + ".workplaceData", CreationCollisionOption.OpenIfExists);
            string rawTodaySh = await FileIO.ReadTextAsync(todaySh);
            if (subjectsComboBox.SelectedItem != null)
            {
                if (rawTodaySh == "")
                {
                    rawTodaySh = subjectsComboBox.SelectedItem.ToString();
                }
                else
                {
                    rawTodaySh += "," + subjectsComboBox.SelectedItem.ToString();
                }
                await FileIO.WriteTextAsync(todaySh, rawTodaySh);
                editShPopup.IsOpen = false;
                hideRectOne.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                hideRectTwo.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                pageRoot_Loaded(sender, e);
            }
            else
            {
                MessageDialog addError = new MessageDialog("Моля първо изберете предмет", "Възникна грешка");
                addError.Commands.Add(new UICommand("OK"));
                await addError.ShowAsync();
            }
        }

        private void Button_Tapped_2(object sender, TappedRoutedEventArgs e)
        {
            editShPopup.IsOpen = false;
            hideRectOne.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            hideRectTwo.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }
        private async void Button_Tapped_4(object sender, TappedRoutedEventArgs e)
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFolder shFolder = await folder.GetFolderAsync("shFolder");
            string todayDayName = DateTime.Now.AddDays(1).DayOfWeek.ToString();
            StorageFile todaySh = await shFolder.CreateFileAsync(todayDayName + ".workplaceData", CreationCollisionOption.OpenIfExists);
            string rawTodaySh = await FileIO.ReadTextAsync(todaySh);
            if (subjectsComboBoxEdit.SelectedItem != null)
            {
                string[] shArray = rawTodaySh.Split(',');
                int order = 1;
                string newRawSh = "";
                foreach (string singleShSubject in shArray)
                {
                    if (order.ToString() == selectedSubjectToInteract)
                    {
                        if (newRawSh == "")
                        {
                            newRawSh = subjectsComboBoxEdit.SelectedItem.ToString();
                        }
                        else
                        {
                            newRawSh += "," + subjectsComboBoxEdit.SelectedItem.ToString();
                        }
                    }
                    else
                    {
                        if (newRawSh == "")
                        {
                            newRawSh = singleShSubject;
                        }
                        else
                        {
                            newRawSh += "," + singleShSubject;
                        }
                    }
                    order++;
                }
                await FileIO.WriteTextAsync(todaySh, newRawSh);
                editSubjectPopup.IsOpen = false;
                hideRectOne.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                hideRectTwo.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                shStackPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
                pageRoot_Loaded(sender, e);
            }
            else
            {
                MessageDialog addError = new MessageDialog("Моля първо изберете предмет", "Възникна грешка");
                addError.Commands.Add(new UICommand("OK"));
                await addError.ShowAsync();
            }
        }

        private void Button_Tapped_5(object sender, TappedRoutedEventArgs e)
        {
            editSubjectPopup.IsOpen = false;
            hideRectOne.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            hideRectTwo.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            shStackPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }
    }
}

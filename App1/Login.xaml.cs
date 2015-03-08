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
    public sealed partial class Login : App1.Common.LayoutAwarePage
    {
        public Login()
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

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            int passwordErrors = 0;
            int emptyErrors = 0;
            int typeErrors = 0;
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            if (userNameTextBox.Text != "")
            {
            }  
            else
            {
                emptyErrors++;
            }
            if (passwordTextBox.Password != "")
            {
                if (passwordTextBox.Password == confPass.Password)
                {
                   
                }
                else
                {
                    passwordErrors++;
                }   
            }
            else
            {
                emptyErrors++;
            }
            if (name.Text == "")
            {
                emptyErrors++;
            }
            if (lastName.Text == "")
            {
                emptyErrors++;
            }
            if (grade.Text == "")
            {
                emptyErrors++;
            }
            else
            {
                int number;
                if (!int.TryParse(grade.Text, out number))
                {
                    typeErrors++;
                }
                else
                {
                    if (int.Parse(grade.Text)<1 || int.Parse(grade.Text)>12)
                    {
                        typeErrors++;
                    }
                }
            }
            if (emptyErrors > 0 || passwordErrors > 0 || typeErrors > 0)
            {
                string errorFinal = "";
                if (emptyErrors > 0)
                {
                    errorFinal += "Имате празни полета." + Environment.NewLine;
                }
                if (passwordErrors > 0)
                {
                    errorFinal += "Паролите не съвпадат" + Environment.NewLine;
                }
                if (typeErrors > 0)
                {
                    errorFinal += "Класът е невалиден" + Environment.NewLine;
                }
                MessageDialog mes = new MessageDialog(errorFinal);
                mes.Title = "Създаване на профил неуспешно!";
                await mes.ShowAsync();
            }
            else
            {
                StorageFile newUser = await folder.GetFileAsync("userName.workplaceData");
                await FileIO.WriteTextAsync(newUser, userNameTextBox.Text);
                StorageFile newPass = await folder.CreateFileAsync("userPass.workplaceData");
                await FileIO.WriteTextAsync(newPass, hashPass(passwordTextBox.Password));
                string fullName = name.Text + " " + lastName.Text;
                StorageFile newName = await folder.CreateFileAsync("fullName.workplaceData");
                await FileIO.WriteTextAsync(newName, fullName);
                StorageFile newGrade = await folder.CreateFileAsync("grade.workplaceData");
                await FileIO.WriteTextAsync(newGrade, grade.Text);
                StorageFolder booksFolder = await folder.CreateFolderAsync("workplaceBooks", CreationCollisionOption.OpenIfExists);
                StorageFile subjectsFile = await folder.CreateFileAsync("subjectsList.workplaceData");
                Frame.Navigate(typeof(MainPage));
            }
        }
    }
}

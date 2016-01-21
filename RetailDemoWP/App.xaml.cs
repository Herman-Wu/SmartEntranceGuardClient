using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.PushNotifications;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Windows.System.Profile;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.WindowsAzure.Messaging;
using System.Diagnostics;
using RetailDemoWP.Models;

namespace RetailDemoWP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        private static string SID = "3bb90e";
        private static string SToken = "8ba4a62a";
        private static string NotiHubName = "ccretaildemohub";
        private static string NotiHubConnStr = "Endpoint=sb://ccretaildemo.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=IOTdjYNxwNPWv3NMgvLfke6p2QlB0pJPy7Qs+Iuttrc=";
        private static string ManageWebURL = @"http://retaildemomanagement.azurewebsites.net";
        private string DeviceID = "TestDID";
        private string UID = "Test01";
        private static string UName = "Test01";
        private static string UGender = "male";
        private static string UAge = "25";
        private static string Tag4 = "N/A";
        private static string AIKey = "6bf0979d-125b-4e9e-913f-37de51fa9a5c";
        private RetailVisiter CurrentVisiter;
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            CurrentVisiter = new RetailVisiter();
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
                Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
                Microsoft.ApplicationInsights.WindowsCollectors.Session);
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(MainPage), e.Arguments);
            }
            // Ensure the current window is active
            Window.Current.Activate();
            //Get DeviceID
            DeviceID= GetDeviceID();
            InitNotificationsAsync();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }


        private async void InitNotificationsAsync()
        {
            PushChannel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
            
            PushChannel.PushNotificationReceived += Channel_PushNotificationReceived;

            var hub = new NotificationHub(NotiHubName, NotiHubConnStr);
            // need set tag to identify user.
            var result = await hub.RegisterNativeAsync(PushChannel.Uri, new List<string> { DeviceID, "miniConnect", "Jason" });

            // Displays the registration ID so you know it was successful
            if (result.RegistrationId != null)
            {

                var pushStr = string.Format("{0}/adduser.aspx?sid={1}&token={2}&did={3}&name={4}&pk=NONE&type=wp&tag1={5}&tag2={6}&tag3={7}&tag4={8}&ntag=200",
                    ManageWebURL,SID, SToken, DeviceID, UName, UID,UGender, UAge,Tag4 );
           
                var client = new HttpClient();
                var response = await client.GetAsync(pushStr);

               
                //var dialog = new MessageDialog("Registration successful: " + result.RegistrationId+","+ response);
                var dialog = new MessageDialog("IoT Server Regist result: " + response);
                dialog.Commands.Add(new UICommand("OK"));
               
                var senMsg = string.Format("http://retaildemomanagement.azurewebsites.net/pushcamp.aspx?sid={0}&token={1}&rid=r2&did={2}", SID, SToken, DeviceID);
                var sendresponse = await client.GetAsync(senMsg);
                //var senddialog = new MessageDialog("IoT Server Send Message result: " + sendresponse);
                //dialog.Commands.Add(new UICommand("OK"));
                await dialog.ShowAsync();

                //Debug.WriteLine("DEBICE_ID:"+ GetDeviceID());

            }






        }

        private string GetDeviceID()
        {
            HardwareToken token = HardwareIdentification.GetPackageSpecificToken(null);
            IBuffer hardwareId = token.Id;

            HashAlgorithmProvider hasher = HashAlgorithmProvider.OpenAlgorithm("MD5");
            IBuffer hashed = hasher.HashData(hardwareId);

            string hashedString = CryptographicBuffer.EncodeToHexString(hashed);
            return hashedString;
        }

        private void Channel_PushNotificationReceived(PushNotificationChannel sender, PushNotificationReceivedEventArgs args)
        {
            switch (args.NotificationType)
            {
                case PushNotificationType.Toast:
                    ToastNotificationManager.CreateToastNotifier("App").Show(args.ToastNotification);
                    IXmlNode launch = args.ToastNotification.Content.GetElementsByTagName("toast").FirstOrDefault().Attributes.GetNamedItem("launch");
                    if (launch != null)
                    {
                        // do what?
                    }
                    break;
                case PushNotificationType.Tile:
                    TileUpdateManager.CreateTileUpdaterForApplication("App").Update(args.TileNotification);
                    break;
                case PushNotificationType.Raw:
                    break;
                default:
                    break;
            }
        }

        public static PushNotificationChannel PushChannel;
    }
}

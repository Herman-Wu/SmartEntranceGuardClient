using Microsoft.WindowsAzure.Messaging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Networking.PushNotifications;
using Windows.UI.Notifications;
using Windows.UI.Popups;

namespace RetailDemoWP.Services
{
    public class DemoPushNotificationService
    {
        private static string SID = "3bb90e";
        private static string SToken = "8ba4a62a";
        private static string NotiHubName = "ccretaildemohub";
        private static string NotiHubConnStr = "Endpoint=sb://ccretaildemo.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=IOTdjYNxwNPWv3NMgvLfke6p2QlB0pJPy7Qs+Iuttrc=";
        private static string ManageWebURL = @"http://retaildemomanagement.azurewebsites.net";
        private static string pushurl = "{0}/adduser.aspx?sid={1}&token={2}&did={3}&name={4}&pk=NONE&type=wp&tag1={5}&tag2={6}&tag3={7}&tag4={8}&ntag=200";
        private static string sendMsgUrl = "http://retaildemomanagement.azurewebsites.net/pushcamp.aspx?sid={0}&token={1}&rid=r2&did={2}";
        public string DeviceID = "TestDID";
        public string UID = "Test02";
        public string UName = "Test02";
        public string UGender = "male";
        public string UAge = "25";
        public string Tag4 = "N/A";
        private static string AIKey = "6bf0979d-125b-4e9e-913f-37de51fa9a5c";
        public static PushNotificationChannel PushChannel;


        public async void InitNotificationsAsync()
        {
            //建立推播管道
            PushChannel = 
                await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();

            PushChannel.PushNotificationReceived += Channel_PushNotificationReceived;

            //NotificationHub名稱 及ConnectionString
            var hub = new NotificationHub(NotiHubName, NotiHubConnStr);

            ////註冊Tags
            var result = await hub.RegisterNativeAsync(PushChannel.Uri, new List<string> { DeviceID, SToken, "Jason" });

            //成功時顯示註冊ID
            //if (result.RegistrationId != null)
            //{
            //    var pushStr = string.Format(pushurl, ManageWebURL, SID, SToken, DeviceID, UName, UID, UGender, UAge, Tag4);
            //    var client = new HttpClient();
            //    var response = await client.GetAsync(pushStr);
            //    var senMsg = string.Format(sendMsgUrl, SID, SToken, DeviceID);
            //    var sendresponse = await client.GetAsync(senMsg);              
            //}
        }
        private static async void SendNotificationAsync()
        {
             
            //NotificationHubClient hub = NotificationHubClient
            //    .CreateClientFromConnectionString("<connection string with full access>", "<hub name>");
            //var toast = @"<toast><visual><binding template=""ToastText01""><text id=""1"">Hello from a .NET App!</text></binding></visual></toast>";
            //await hub.SendWindowsNativeNotificationAsync(toast);
        }

        public void Channel_PushNotificationReceived(PushNotificationChannel sender, PushNotificationReceivedEventArgs args)
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


    }
}

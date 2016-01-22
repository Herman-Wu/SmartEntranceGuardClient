using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using RetailDemoWP.Services;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace RetailDemoWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProductHighLight : Page
    {
        private string DeviceID;
        public ProductHighLight()
        {
            this.InitializeComponent();
            PushNotificationService src = new PushNotificationService();
            UAgetxt.Text = src.UAge = App.CurrentVisiter.Age;
            UGendertxt.Text= src.UGender = App.CurrentVisiter.Gender;
            src.InitNotificationsAsync();
            
        }
    }
}

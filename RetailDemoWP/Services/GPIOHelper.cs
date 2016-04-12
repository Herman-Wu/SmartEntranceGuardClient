using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace RetailDemoWP.Services
{
    public class GPIOHelper
    {
        private const int LED_PIN = 5;
        private string GpioStatus;
        private GpioPin pin;
        private GpioPinValue pinValue;
        public void InitGPIO()
        {

            //以下兩種判斷裝置方式暫時不Work, 因為相關API在Windows Desktop 上也有
            //if (ApiInformation.IsTypePresent(typeof(Windows.Devices.Gpio.GpioController)))
            //if (ApiInformation.IsApiContractPresent("Windows.Devices.DevicesLowLevelContract", 1))

            //判斷是否在IoT 裝置上執行
            if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily.ToString() == "Windows.IoT")
            {
                var gpio = GpioController.GetDefault();

                // 確認是否有GPI Contorller
                if (gpio == null)
                {
                    pin = null;
                    GpioStatus= "此裝置上沒有 GPIO controller ";
                    return;
                }

                pin = gpio.OpenPin(LED_PIN);
                pinValue = GpioPinValue.High;
                pin.Write(pinValue); //傳送高電位
                //設定Pin腳為輸出
                pin.SetDriveMode(GpioPinDriveMode.Output);

                GpioStatus = "GPIO pin 成功設置";
            }
            else
            {
                GpioStatus = "此裝置無GPIO ";
            }

        }

        public void TurnlightOn()
        {
            if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily.ToString() == "Windows.IoT")
            {                
                pinValue = GpioPinValue.Low;
                pin.Write(pinValue);
            }
        }
        public void TurnlightOff()
        {
            if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily.ToString() == "Windows.IoT")
            {
                pinValue = GpioPinValue.High;
                pin.Write(pinValue);
            }
        }
    }
}

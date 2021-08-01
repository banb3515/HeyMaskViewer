using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;

namespace HeyMaskViewer
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// 클라이언트의 요청을 수신받을 Port 번호입니다.
        /// </summary>
        public static int ReceivePort { get; set; }

        /// <summary>
        /// 저장 공간을 관리할 용량 % 입니다.
        /// </summary>
        public static double DMPercent { get; set; }

        /// <summary>
        /// 각 프레임에 보여질 클라이언트의 IPv4 주소입니다.
        /// </summary>
        public static Dictionary<string, string> ViewAddress { get; set; }

        /// <summary>
        /// 클라이언트에게 요청을 수신받는 Receiver입니다.
        /// </summary>
        public static Receiver Receiver { get; set; }

        /// <summary>
        /// 자동으로 용량을 관리하는 DriveManager입니다.
        /// </summary>
        public static DriveManager DriveManager { get; set; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ReceivePort = Convert.ToInt32(HeyMaskViewer.Properties.Settings.Default["Port"]);
            DMPercent = Convert.ToDouble(HeyMaskViewer.Properties.Settings.Default["DMPercent"]);

            var views = new string[4];
            views[0] = HeyMaskViewer.Properties.Settings.Default["View1"].ToString();
            views[1] = HeyMaskViewer.Properties.Settings.Default["View2"].ToString();
            views[2] = HeyMaskViewer.Properties.Settings.Default["View3"].ToString();
            views[3] = HeyMaskViewer.Properties.Settings.Default["View4"].ToString();

            ViewAddress = new Dictionary<string, string>
            {
                { "View1", views[0] == null ? string.Empty : views[0] },
                { "View2", views[1] == null ? string.Empty : views[1] },
                { "View3", views[2] == null ? string.Empty : views[2] },
                { "View4", views[3] == null ? string.Empty : views[3] },
            };

            Trace.Log("HeyMaskViewer 실행");
        }
    }
}

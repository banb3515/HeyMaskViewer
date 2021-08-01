using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace HeyMaskViewer
{
    /// <summary>
    /// SettingPopup.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingPopup : UserControl
    {
        /// <summary>
        /// 적용된 설정을 UI에 표시하기 위한 이벤트입니다.
        /// </summary>
        public event EventHandler Save;

        /// <summary>
        /// 설정창을 닫기 위한 이벤트입니다.
        /// </summary>
        public event EventHandler Close;

        public SettingPopup()
        {
            InitializeComponent();

            Init();
        }

        /// <summary>
        /// UI를 초기화합니다.
        /// </summary>
        private void Init()
        {
            try
            {
                ReceivePort.Text = App.ReceivePort.ToString();
                DMPercent.Text = App.DMPercent.ToString();
                View1Address.Text = App.ViewAddress["View1"];
                View2Address.Text = App.ViewAddress["View2"];
                View3Address.Text = App.ViewAddress["View3"];
                View4Address.Text = App.ViewAddress["View4"];
            }
            catch (Exception ex)
            {
                Trace.Exception(ex);
            }
        }

        /// <summary>
        /// 저장 버튼을 클릭 시 유효성을 확인하고 설정을 저장 후 적용합니다.
        /// </summary>
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int port;
                double dmPercent;
                var regex = new Regex(@"^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$");

                // 올바른 Port 번호인지 확인
                if (!int.TryParse(ReceivePort.Text, out port))
                {
                    MessageBox.Show("올바른 Port 형식이 아닙니다.", "설정", MessageBoxButton.OK);
                    ReceivePort.Focus();
                    ReceivePort.SelectAll();
                    return;
                }
                else if (!double.TryParse(DMPercent.Text, out dmPercent) || double.Parse(DMPercent.Text) > 1.0 || double.Parse(DMPercent.Text) < 0.0)
                { 
                    MessageBox.Show("올바른 관리 용량 % 형식이 아닙니다.\nex) 80% = 0.8", "설정", MessageBoxButton.OK);
                    DMPercent.Focus();
                    DMPercent.SelectAll();
                    return;
                }
                // IPv4 형식이 맞는지 확인
                else if (!string.IsNullOrWhiteSpace(View1Address.Text) && !regex.IsMatch(View1Address.Text.Trim(), 0))
                {
                    MessageBox.Show("올바른 IPv4 주소 형식이 아닙니다.", "설정", MessageBoxButton.OK);
                    View1Address.Focus();
                    View1Address.SelectAll();
                    return;
                }
                else if (!string.IsNullOrWhiteSpace(View2Address.Text) && !regex.IsMatch(View2Address.Text.Trim(), 0))
                {
                    MessageBox.Show("올바른 IPv4 주소 형식이 아닙니다.", "설정", MessageBoxButton.OK);
                    View2Address.Focus();
                    View2Address.SelectAll();
                    return;
                }
                else if (!string.IsNullOrWhiteSpace(View3Address.Text) && !regex.IsMatch(View3Address.Text.Trim(), 0))
                {
                    MessageBox.Show("올바른 IPv4 주소 형식이 아닙니다.", "설정", MessageBoxButton.OK);
                    View3Address.Focus();
                    View3Address.SelectAll();
                    return;
                }
                else if (!string.IsNullOrWhiteSpace(View4Address.Text) && !regex.IsMatch(View4Address.Text.Trim(), 0))
                {
                    MessageBox.Show("올바른 IPv4 주소 형식이 아닙니다.", "설정", MessageBoxButton.OK);
                    View4Address.Focus();
                    View4Address.SelectAll();
                    return;
                }

                // 설정 저장
                Properties.Settings.Default["View1"] = View1Address.Text;
                Properties.Settings.Default["View2"] = View2Address.Text;
                Properties.Settings.Default["View3"] = View3Address.Text;
                Properties.Settings.Default["View4"] = View4Address.Text;
                Properties.Settings.Default.Save();

                // 설정 적용
                var views = new string[4];
                views[0] = Properties.Settings.Default["View1"].ToString();
                views[1] = Properties.Settings.Default["View2"].ToString();
                views[2] = Properties.Settings.Default["View3"].ToString();
                views[3] = Properties.Settings.Default["View4"].ToString();

                App.ViewAddress = new Dictionary<string, string>
                {
                    { "View1", views[0] == null ? string.Empty : views[0] },
                    { "View2", views[1] == null ? string.Empty : views[1] },
                    { "View3", views[2] == null ? string.Empty : views[2] },
                    { "View4", views[3] == null ? string.Empty : views[3] },
                };

                if (App.ReceivePort != port)
                {
                    App.ReceivePort = port;
                    App.Receiver.ChangePort(App.ReceivePort);
                }
                if (App.DMPercent != dmPercent)
                {
                    App.DMPercent = dmPercent;
                    App.DriveManager.DMPercent = App.DMPercent;
                }

                if (Save != null)
                    Save(this, null);
            }
            catch (Exception ex)
            {
                Trace.Exception(ex);
            }
        }

        /// <summary>
        /// 닫기 버튼을 클릭 시 설정창을 닫습니다.
        /// </summary>
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Close != null)
                    Close(this, null);
            }
            catch (Exception ex)
            {
                Trace.Exception(ex);
            }
        }
    }
}

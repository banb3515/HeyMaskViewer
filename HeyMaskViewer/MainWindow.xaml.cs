using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HeyMaskViewer
{
    /// <summary>
    /// 보여질 모드입니다.
    /// </summary>
    public enum ViewMode
    {
        View4 = 4, // Default
        View1 = 1
    }

    public partial class MainWindow : Window
    {
        /// <summary>
        /// 현재 보여지고 있는 모드입니다.
        /// </summary>
        private ViewMode CurrentViewMode = ViewMode.View4;

        /// <summary>
        /// 현재 보여지고 있는 뷰입니다. (ViewMode = View1)
        /// </summary>
        private string CurrentView;

        public MainWindow()
        {
            InitializeComponent();

            Init();
        }

        /// <summary>
        /// UI 및 기능을 초기화합니다.
        /// </summary>
        private void Init()
        {
            try
            {
                #region UI
                // 모니터 해상도에 맞게 해상도 조절
                Width = SystemParameters.PrimaryScreenWidth;
                Height = SystemParameters.PrimaryScreenHeight;
                Left = 0;
                Top = 0;

                // 뷰 - 타이틀 초기화
                ReloadUI();

                // 뷰 - 프레임 초기화
                View1Frame.Source = BitmapToImageSource(Properties.Resources.Empty);
                View2Frame.Source = BitmapToImageSource(Properties.Resources.Empty);
                View3Frame.Source = BitmapToImageSource(Properties.Resources.Empty);
                View4Frame.Source = BitmapToImageSource(Properties.Resources.Empty);
                #endregion

                // Receiver 생성
                App.Receiver = new Receiver(App.ReceivePort);
                App.Receiver.Receive += Receiver_Receive;
                App.Receiver.Start();

                // DriveManager 생성
                App.DriveManager = new DriveManager(TimeSpan.FromSeconds(1), App.DMPercent);
                App.DriveManager.Start();
            }
            catch (Exception ex)
            {
                Trace.Exception(ex);
            }
        }

        /// <summary>
        /// Bitmap을 ImageSource로 변환합니다.
        /// </summary>
        /// <param name="bitmap">변활한 Bitmap 입니다.</param>
        /// <returns>ImageSource로 변환된 Bitmap입니다.</returns>
        private ImageSource BitmapToImageSource(Bitmap bitmap)
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    bitmap.Save(ms, ImageFormat.Png);
                    ms.Position = 0;
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = ms;
                    image.EndInit();
                    return image;
                }
            }
            catch (Exception ex)
            {
                Trace.Exception(ex);
                return null;
            }
        }

        /// <summary>
        /// UI를 새로고침합니다.
        /// </summary>
        private void ReloadUI()
        {
            View1Title.Text = "[View1] " + (App.ViewAddress["View1"] == string.Empty ? "설정되지 않음" : App.ViewAddress["View1"]);
            View2Title.Text = "[View2] " + (App.ViewAddress["View2"] == string.Empty ? "설정되지 않음" : App.ViewAddress["View2"]);
            View3Title.Text = "[View3] " + (App.ViewAddress["View3"] == string.Empty ? "설정되지 않음" : App.ViewAddress["View3"]);
            View4Title.Text = "[View4] " + (App.ViewAddress["View4"] == string.Empty ? "설정되지 않음" : App.ViewAddress["View4"]);
        }

        /// <summary>
        /// 창이 닫힐 때 프로그램을 종료합니다.
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        /// <summary>
        /// Receiver에서 클라이언트에게 프레임을 수신받았을 경우 프레임을 뷰에 표시합니다.
        /// </summary>
        private void Receiver_Receive(object sender, EventArgs e)
        {
            try
            {
                ReceiverArgs args = (ReceiverArgs)e;

                switch (args.ClientInfo.View)
                {
                    case "View1":
                        View1Frame.Source = args.Source;
                        break;
                    case "View2":
                        View2Frame.Source = args.Source;
                        break;
                    case "View3":
                        View3Frame.Source = args.Source;
                        break;
                    case "View4":
                        View4Frame.Source = args.Source;
                        break;
                }
            }
            catch (Exception ex)
            {
                Trace.Exception(ex);
            }
        }

        /// <summary>
        /// 뷰를 클릭 시 실행되는 이벤트입니다.
        /// ViewMode = View4, 뷰를 더블 클릭 시 View1 모드로 변경합니다.
        /// ViewMode = View1, 뷰를 더블 클릭 시 View4 모드로 변경합니다.
        /// </summary>
        private void View_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                // 더블 클릭이 아닐 시 반환
                if (e.ClickCount != 2) return;

                var view = sender as Border;

                switch (CurrentViewMode)
                {
                    case ViewMode.View4:
                        switch (view.Tag.ToString())
                        {
                            case "View1":
                                CurrentView = "View1";
                                ViewGrid.ColumnDefinitions[1].Width = new GridLength(0);
                                ViewGrid.RowDefinitions[1].Height = new GridLength(0);
                                break;
                            case "View2":
                                CurrentView = "View2";
                                ViewGrid.ColumnDefinitions[0].Width = new GridLength(0);
                                ViewGrid.RowDefinitions[1].Height = new GridLength(0);
                                break;
                            case "View3":
                                CurrentView = "View3";
                                ViewGrid.ColumnDefinitions[1].Width = new GridLength(0);
                                ViewGrid.RowDefinitions[0].Height = new GridLength(0);
                                break;
                            case "View4":
                                CurrentView = "View4";
                                ViewGrid.ColumnDefinitions[0].Width = new GridLength(0);
                                ViewGrid.RowDefinitions[0].Height = new GridLength(0);
                                break;
                        }
                        CurrentViewMode = ViewMode.View1;
                        break;
                    case ViewMode.View1:
                        switch (CurrentView)
                        {
                            case "View1":
                                CurrentView = string.Empty;
                                ViewGrid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
                                ViewGrid.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Star);
                                break;
                            case "View2":
                                CurrentView = string.Empty;
                                ViewGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
                                ViewGrid.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Star);
                                break;
                            case "View3":
                                CurrentView = string.Empty;
                                ViewGrid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
                                ViewGrid.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Star);
                                break;
                            case "View4":
                                CurrentView = string.Empty;
                                ViewGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
                                ViewGrid.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Star);
                                break;
                        }
                        CurrentViewMode = ViewMode.View4;
                        break;
                }
            }
            catch (Exception ex)
            {
                Trace.Exception(ex);
            }
        }

        /// <summary>
        /// 설정 버튼을 클릭 시 설정 팝업을 띄웁니다.
        /// </summary>
        private void Setting_Click(object sender, RoutedEventArgs e)
        {
            var popup = new SettingPopup();
            popup.Save += SettingPopup_Save;
            popup.Close += Popup_Close;

            PopupGrid.Visibility = Visibility.Visible;
            PopupGrid.Children.Add(popup);
        }

        /// <summary>
        /// 종료 버튼을 클릭 시 프로그램을 종료합니다.
        /// </summary>
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            var popup = new MessagePopup(380, 190, "프로그램 종료", "Hey! Mask! Viewer를 종료합니다.", BitmapToImageSource(Properties.Resources.Yes), "종료", BitmapToImageSource(Properties.Resources.Close), "취소");
            popup.ClickYes += (s, a) =>
            {
                Trace.Log("HeyMaskViewer 종료");
                Environment.Exit(0);
            };
            popup.ClickNo += Popup_Close;

            PopupGrid.Visibility = Visibility.Visible;
            PopupGrid.Children.Add(popup);
        }

        /// <summary>
        /// 변경된 설정을 적용합니다.
        /// </summary>
        private void SettingPopup_Save(object sender, EventArgs e)
        {
            ReloadUI();

            var popup = new MessagePopup(380, 190, "설정", "설정이 저장되었습니다.", BitmapToImageSource(Properties.Resources.Yes), "확인");
            popup.ClickYes += Popup_Close;
            PopupGrid.Children.Add(popup);
        }

        /// <summary>
        /// 열린 팝업창에서 닫기 이벤트 호출 시 팝업을 닫습니다.
        /// </summary>
        private void Popup_Close(object sender, EventArgs e)
        {
            PopupGrid.Children.RemoveAt(PopupGrid.Children.Count - 1);

            if (PopupGrid.Children.Count == 1)
                PopupGrid.Visibility = Visibility.Collapsed;
        }
    }
}

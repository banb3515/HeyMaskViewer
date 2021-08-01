using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace HeyMaskViewer
{
    /// <summary>
    /// MessagePopup.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MessagePopup : UserControl
    {
        /// <summary>
        /// Yes 버튼을 클릭 시 실행될 이벤트입니다.
        /// </summary>
        public event EventHandler ClickYes;

        /// <summary>
        /// No 버튼을 클릭 시 실행될 이벤트입니다.
        /// </summary>
        public event EventHandler ClickNo;

        /// <summary>
        /// 메시지 팝업을 생성합니다.
        /// </summary>
        /// <param name="width">팝업의 너비입니다.</param>
        /// <param name="height">팝업의 높이입니다.</param>
        /// <param name="title">팝업의 제목입니다.</param>
        /// <param name="content">팝업의 내용입니다.</param>
        /// <param name="yesButton">Yes 버튼의 아이콘입니다.</param>
        /// <param name="yesText">Yes 버튼의 텍스트입니다.</param>
        /// <param name="noButton">No 버튼의 아이콘입니다.</param>
        /// <param name="noText">No 버튼의 텍스트입니다.</param>
        public MessagePopup(int width, int height, string title, string content, ImageSource yesButton, string yesText, ImageSource noButton = null, string noText = null)
        {
            InitializeComponent();

            Width = width;
            Height = height;
            PopupTitle.Text = title;
            PopupContent.Text = content;
            YesButton.Source = yesButton;
            YesText.Text = yesText;
            if (noButton != null) NoButton.Source = noButton;
            if (noText != null) NoText.Text = noText;

            if (noButton == null && noText == null)
            {
                NoButtonParent.Visibility = System.Windows.Visibility.Collapsed;
                NoButton.Visibility = System.Windows.Visibility.Collapsed;
                NoText.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Yes 버튼을 클릭 시 실행되는 이벤트입니다.
        /// </summary>
        private void Yes_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (ClickYes != null)
                ClickYes(this, null);
        }

        /// <summary>
        /// No 버튼을 클릭 시 실행되는 이벤트입니다.
        /// </summary>
        private void No_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (ClickNo != null)
                ClickNo(this, null);
        }
    }
}

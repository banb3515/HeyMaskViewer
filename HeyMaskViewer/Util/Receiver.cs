using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace HeyMaskViewer
{
    /// <summary>
    /// 클라이언트의 정보입니다.
    /// </summary>
    public struct ClientInfo
    {
        /// <summary>
        /// 클라이언트의 정보를 초기화합니다.
        /// </summary>
        /// <param name="view">보여질 뷰의 이름입니다.</param>
        /// <param name="address">클라이언트의 IPv4 주소입니다.</param>
        public ClientInfo(string view, string address)
        {
            View = view;
            Address = address;
            TotalBytes = 0;
        }

        /// <summary>
        /// 보여질 뷰의 이름입니다.
        /// </summary>
        public string View { get; }

        /// <summary>
        /// 클라이언트의 IPv4 주소입니다.
        /// </summary>
        public string Address { get; }

        /// <summary>
        /// 수신받은 Bytes의 길이입니다.
        /// </summary>
        public int TotalBytes { get; set; }
    }

    /// <summary>
    /// Receiver 이벤트 매개변수입니다.
    /// </summary>
    public class ReceiverArgs : EventArgs
    {
        /// <summary>
        /// Receiver 이벤트 매개변수를 초기화합니다.
        /// </summary>
        /// <param name="clientInfo">클라이언트의 정보입니다.</param>
        /// <param name="source">Bytes에서 변환된 이미지 소스입니다.</param>
        public ReceiverArgs(ClientInfo clientInfo, BitmapImage source)
        {
            ClientInfo = clientInfo;
            Source = source;
        }

        /// <summary>
        /// 클라이언트 정보입니다.
        /// </summary>
        public ClientInfo ClientInfo { get; }

        /// <summary>
        /// Bytes에서 변환된 이미지 소스입니다.
        /// </summary>
        public BitmapImage Source { get; }
    }

    /// <summary>
    /// TCP 프로토콜을 이용하여 클라이언트들에게 프레임을 수신받습니다.
    /// </summary>
    public class Receiver
    {
        /// <summary>
        /// 프레임을 UI에 표시하기 위한 이벤트입니다.
        /// </summary>
        public event EventHandler Receive;

        /// <summary>
        /// 클라리언트의 연결 요청을 받는 스레드입니다.
        /// </summary>
        private Thread thread;

        private TcpListener listener;

        private bool IsListen = false;

        private string address;

        private int port;

        /// <summary>
        /// Receiver를 초기화합니다.
        /// </summary>
        /// <param name="port">수신받을 Port 번호입니다.</param>
        public Receiver(int port)
        {
            var addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            foreach (IPAddress addr in addressList)
            {
                if (addr.AddressFamily == AddressFamily.InterNetwork)
                    address = addr.ToString();
            }

            this.port = port;
            listener = new TcpListener(IPAddress.Parse(address), port);
        }

        /// <summary>
        /// 클라이언트와 수신을 시작합니다.
        /// </summary>
        public void Start()
        {
            IsListen = true;
            listener.Start();

            thread = new Thread(Listen);
            thread.Start();
        }

        /// <summary>
        /// 클라이언트와 수신을 종료합니다.
        /// </summary>
        public void Stop()
        {
            IsListen = false;
            listener.Stop();
        }

        /// <summary>
        /// 포트를 변경합니다.
        /// </summary>
        /// <param name="port">변경할 Port 번호입니다.</param>
        public void ChangePort(int port)
        {
            Stop();
            this.port = port;
            listener = new TcpListener(IPAddress.Any, port);
            Start();
        }

        /// <summary>
        /// 지속적으로 클라이언트의 연결 요청을 받습니다.
        /// </summary>
        private void Listen()
        {
            Trace.Log($"Receiver 대기 중: {address}:{port}");

            while (true)
            {
                if (!IsListen) break;

                if (listener.Pending())
                {
                    var client = listener.AcceptTcpClient();
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => Process(client)));
                }
            }
        }

        /// <summary>
        /// 클라이언트에게 받은 프레임을 처리합니다.
        /// </summary>
        /// <param name="client">연결된 클라이언트입니다.</param>
        private void Process(TcpClient client)
        {
            try
            {
                // 연결된 클라이언트의 IPv4 주소
                var address = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();

                // 허용된 클라이언트인지 확인
                if (!App.ViewAddress.ContainsValue(address)) return;

                // 보여질 뷰의 이름
                var view = App.ViewAddress.FirstOrDefault(x => x.Value == address).Key;

                // 클라이언트 정보 초기화
                var info = new ClientInfo(view, address);

                // 버퍼 조각 초기화, 최대 크기: 1MB (1024KB)
                var fragment = new byte[1024 * 1024];
                byte[] buffer;

                // 프레임이 나눠져서 수신되었을 경우 모든 프레임을 수신받을 때 까지 버퍼 조각을 계속 읽기
                using (var ms = new MemoryStream())
                using (var stream = client.GetStream())
                {
                    int recvBytes;
                    while ((recvBytes = stream.Read(fragment, 0, fragment.Length)) > 0)
                    {
                        Array.Resize(ref fragment, recvBytes);
                        ms.Write(fragment, 0, fragment.Length);
                    }

                    // 프레임 스트림을 Bytes로 변환
                    buffer = ms.ToArray();
                    info.TotalBytes = buffer.Length;
                }

                // 정상적으로 프레임을 수신받았을 경우 파일로 저장 후 UI에 표시
                if (info.TotalBytes > 0)
                {
                    // 파일 경로, 파일 이름, 날짜 포맷 형식 지정
                    var now = DateTime.Now;
                    var msec = (long)now.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
                    var path = $"Images\\{info.View}\\{now:yyyy}Y\\{now:MM}M\\{now:dd}D\\";
                    var filename = $"{msec}.jpg";

                    // 이미지 디렉토리가 존재하지 않으면 생성
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    // 파일 생성
                    File.WriteAllBytes(Path.Combine(path, filename), buffer);

                    // 전송한 클라이언트의 IPv4 주소, 수신받은 Bytes의 길이 출력
                    Trace.Log($"[{info.Address}] 수신: {info.TotalBytes} Bytes");
                    // 이벤트가 설정되어있을 경우 UI 표시
                    if (Receive != null)
                    {
                        // Bytes를 이미지 소스로 변환
                        using (var ms = new MemoryStream(buffer))
                        {
                            var source = new BitmapImage();
                            source.BeginInit();
                            source.CacheOption = BitmapCacheOption.OnLoad;
                            source.StreamSource = ms;
                            source.EndInit();

                            Receive(this, new ReceiverArgs(info, source));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.Exception(ex);
            }

            client.Close();
        }
    }
}

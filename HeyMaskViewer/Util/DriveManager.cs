using System;
using System.IO;
using System.Linq;
using System.Windows.Threading;

namespace HeyMaskViewer
{
    public class DriveManager
    {
        /// <summary>
        /// 설정된 시간만큼 반복해서 처리하기 위한 타이머입니다.
        /// </summary>
        private DispatcherTimer timer;

        /// <summary>
        /// 관리할 용량 % 입니다.
        /// </summary>
        public double DMPercent { get; set; }

        /// <summary>
        /// 드라이브의 용량이 설정된 % 이상일 경우 여유 공간만큼 이미지 파일을 자동으로 삭제합니다.
        /// </summary>
        /// <param name="processTime">처리 반복 시간입니다.</param>
        /// <param name="dmPercent">관리할 용량 % 입니다.</param>
        public DriveManager(TimeSpan processTime, double dmPercent)
        {
            DMPercent = dmPercent;

            timer = new DispatcherTimer(DispatcherPriority.Background);
            timer.Interval = processTime;
            timer.Tick += Process;
        }

        /// <summary>
        /// 드라이브 관리를 시작합니다.
        /// </summary>
        public void Start()
        {
            timer.Start();
        }

        /// <summary>
        /// 드라이브 관리를 종료합니다.
        /// </summary>
        public void Stop()
        {
            timer.Stop();
        }

        /// <summary>
        /// 설정된 타이머 틱마다 드라이브의 용량을 확인하고 이미지 파일을 삭제합니다.
        /// </summary>
        /// <param name="sender">DispatcherTimer</param>
        /// <param name="e">DispatcherTimer Event Arguments</param>
        private void Process(object sender, EventArgs e)
        {
            try
            {
                // 이미지 디렉토리가 존재하지 않으면 처리 안함
                if (!Directory.Exists("Images")) return;

                // 프로그램이 실행중인 루트 디렉토리의 경로
                string rootDir = Environment.CurrentDirectory.Split('\\')[0];
                var driveInfo = new DriveInfo(rootDir);

                int count = 0;

                // 용량이 설정된 % 이상일 경우 이미지 파일 삭제
                while ((driveInfo.TotalSize - driveInfo.AvailableFreeSpace) >= (driveInfo.TotalSize * DMPercent))
                {
                    var imagesDir = new DirectoryInfo("Images");
                    
                    // View1
                    foreach (var viewDir in imagesDir.GetDirectories())
                    {
                        // 2021Y
                        foreach (var yearDir in viewDir.GetDirectories())
                        {
                            // 07M
                            foreach (var monthDir in yearDir.GetDirectories())
                            {
                                // 05D
                                foreach (var dayDir in monthDir.GetDirectories())
                                {
                                    // Image Files
                                    var files = dayDir.GetFiles();
                                    if (files.Length > 0)
                                        files[0].Delete();

                                    if (TryRemoveDirectory(dayDir)) break;
                                }

                                if (TryRemoveDirectory(monthDir)) break;
                            }

                            if (TryRemoveDirectory(yearDir)) break;
                        }

                        if (TryRemoveDirectory(viewDir)) break;
                    }

                    if (TryRemoveDirectory(imagesDir)) break;
                }

                if (count > 0)
                    Trace.Log($"{count}개의 이미지 파일 삭제");
            }
            catch (Exception ex)
            {
                Trace.Exception(ex);
            }
        }

        /// <summary>
        /// 디렉터리가 비어있는지 확인하고 디렉터리가 비어있으면 삭제합니다.
        /// </summary>
        /// <param name="dir">확인할 디렉터리입니다.</param>
        /// <returns></returns>
        private bool TryRemoveDirectory(DirectoryInfo dir)
        {
            var size = dir.EnumerateFiles("*", SearchOption.AllDirectories).Sum(fi => fi.Length);
            if (size == 0)
            {
                dir.Delete(true);
                return true;
            }
            else return false;
        }
    }
}

using System;
using System.IO;

namespace HeyMaskViewer
{
    /// <summary>
    /// 로그 또는 예외를 출력합니다.
    /// </summary>
    public class Trace
    {
        /// <summary>
        /// 로그를 출력합니다.
        /// </summary>
        /// <param name="message">출력 할 내용입니다.</param>
        public static void Log(string message)
        {
            // 파일 경로, 파일 이름, 날짜 포맷 형식 지정
            var now = DateTime.Now;
            var path = $"Trace\\Logs\\{now:yyyy}Y\\{now:MM}M\\";
            var filename = $"{now:dd}D.log";
            var log = $"[{now:yyyy-MM-dd} {now:HH:mm:ss}] {message}\n";

            Console.Write(log);

            // 로그 디렉토리가 존재하지 않으면 생성
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            // 로그 파일에 로그 기록
            File.AppendAllText(Path.Combine(path, filename), log);
        }

        /// <summary>
        /// 예외를 출력합니다.
        /// </summary>
        /// <param name="ex">예외 정보가 담긴 객체입니다.</param>
        public static void Exception(Exception ex)
        {
            // 파일 경로, 파일 이름, 날짜 포맷 형식 지정
            var now = DateTime.Now;
            var path = $"Trace\\Exceptions\\{now:yyyy}Y\\{now:MM}M\\";
            var filename = $"{now:dd}D.log";
            var log = $"[{now:yyyy-MM-dd} {now:HH:mm:ss}] {ex.Message} ->\n @ {ex.StackTrace}\n";

            Console.Write(log);

            // 예외 디렉토리가 존재하지 않으면 생성
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            // 예외 파일에 예외 기록
            File.AppendAllText(Path.Combine(path, filename), log);
        }
    }
}

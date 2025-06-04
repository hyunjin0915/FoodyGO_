using System;

namespace FoodyGo.Services.GPS
{
    /// <summary>
    /// gps 위치 제공자 인터페이스
    /// 이 인터페이스 구현한 클래스는 위치 제공 기능을 가져야 함
    /// </summary>
    public interface ILocationProvider
    {
        /// <summary>
        /// 위도
        /// </summary>
        double latitude { get; }

        /// <summary>
        /// 경도
        /// </summary>
        double longitude { get; }

        /// <summary>
        /// 고도
        /// </summary>
        double altitude { get; }

        /// <summary>
        /// latitude, longitude, altitude, horizontalAccuracy, timestamp
        /// </summary>
        event Action<double, double, double, float, double> onLocationUpdated;

        /// <summary>
        /// 위치 데이터 갱신 시작
        /// </summary>
        void StartService();

        /// <summary>
        /// 위치 데이터 갱신 종료
        /// </summary>
        void StopService();
    }
}
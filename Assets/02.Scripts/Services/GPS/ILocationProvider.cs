using System;

namespace FoodyGo.Services.GPS
{
    /// <summary>
    /// gps ��ġ ������ �������̽�
    /// �� �������̽� ������ Ŭ������ ��ġ ���� ����� ������ ��
    /// </summary>
    public interface ILocationProvider
    {
        /// <summary>
        /// ����
        /// </summary>
        double latitude { get; }

        /// <summary>
        /// �浵
        /// </summary>
        double longitude { get; }

        /// <summary>
        /// ��
        /// </summary>
        double altitude { get; }

        /// <summary>
        /// latitude, longitude, altitude, horizontalAccuracy, timestamp
        /// </summary>
        event Action<double, double, double, float, double> onLocationUpdated;

        /// <summary>
        /// ��ġ ������ ���� ����
        /// </summary>
        void StartService();

        /// <summary>
        /// ��ġ ������ ���� ����
        /// </summary>
        void StopService();
    }
}
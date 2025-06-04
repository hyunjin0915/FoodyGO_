using System;
using UnityEngine;

namespace FoodyGo.Services.GPS
{
    /// <summary>
    /// 실제 디바이스의 GPS 데이터를 사용할 클래스
    /// 나중에 실제 API 연결해야 구현 가능
    /// </summary>
    public class DeviceLocationProvider : MonoBehaviour, ILocationProvider
    {
        public double latitude => throw new NotImplementedException();

        public double longitude => throw new NotImplementedException();

        public double altitude => throw new NotImplementedException();

        public event Action<double, double, double, float, double> onLocationUpdated;

        public void StartService()
        {
            throw new NotImplementedException();
        }

        public void StopService()
        {
            throw new NotImplementedException();
        }
    }
}

using FoodyGo.Mapping;
using System;
using UnityEngine;

namespace FoodyGo.Services.GPS
{
    /// <summary>
    /// GPS 데이터를 통합적으로 관리하는 중앙 컨트롤러 
    /// </summary>
    public class GPSLocationService : MonoBehaviour
    {
        public bool isReady { get; private set; }

        [Header("Map Tile Parameters")]
        [Tooltip("맵 타일 스케일")]
        [field : SerializeField] public int _mapTileScale { get; private set; } = 1;

        [Tooltip("맵 타일 크기(픽셀)")]
        [field : SerializeField]  public int _mapTileSizePixels { get; private set; } = 640;

        [Tooltip("맵 타일 Zoom 레벨(1~20)")]
        [Range(1,20)]
        [field: SerializeField]  public int _mapTileZoomLevel { get; private set; } = 15;

        private ILocationProvider _locationProvider;

        [Header("Simulation Settings (Editor Only)")]
        [SerializeField] bool _isSimulation;
        [SerializeField] Transform _simulationTarget;
        [SerializeField] MapLocation _simulationStartLocation = new MapLocation(37.4946, 127.0276056);

        public double latitude { get; private set; }
        public double longitude { get; private set; }
        public double altitude { get; private set; }
        public float accuracy { get; private set; }
        public double timeStamp { get; private set; }

        public event Action OnMapRedraw;

        public MapLocation mapCenter;
        public Vector3 mapWorldCenter;
        public Vector2 mapScale;
        public MapEnvelope mapEnvelope;


        private void Awake()
        {
#if UNITY_EDITOR
            SimulatedLocationProvider simulatedLocationProvider = gameObject.AddComponent<SimulatedLocationProvider>();
            simulatedLocationProvider.target = _simulationTarget;
            simulatedLocationProvider.startLocation = _simulationStartLocation;
            _locationProvider = simulatedLocationProvider;
            isReady = true;
#else
            _locationProvider = gameObject.AddComponent<DeviceLocationProvider>();
#endif
        }

        private void OnEnable()
        {
            _locationProvider.onLocationUpdated += OnLocationUpdated;
            _locationProvider.StartService();
        }

        private void OnDisable()
        {
            _locationProvider.onLocationUpdated -= OnLocationUpdated;
            _locationProvider.StopService();
        }

        private void OnLocationUpdated(double newLatitude, double newLongitude, double newAltitude, float newAccuracy, double newTimeStamp)
        {
            latitude = newLatitude;
            longitude = newLongitude;
            altitude = newAltitude;
            accuracy = newAccuracy;
            timeStamp = newTimeStamp;

            if (mapEnvelope.Contains(new MapLocation(latitude, longitude))== false) //경계를 넘어가면 다시 중심을 잡아서 계산하도록 
            {
                CenterMap();
            }

            OnMapRedraw?.Invoke();
        }


        private void CenterMap() //gps 좌표계 -> 월드 좌표계 
        {
            mapCenter.latitude = latitude;
            mapCenter.longitude = longitude;
            mapWorldCenter.x = GoogleMapUtils.LonToX(mapCenter.longitude);
            mapWorldCenter.y = GoogleMapUtils.LatToY(mapCenter.latitude);

            mapScale.x = (float)GoogleMapUtils.CalculateScaleX(latitude, _mapTileSizePixels, _mapTileScale, _mapTileZoomLevel);
            mapScale.y = (float)GoogleMapUtils.CalculateScaleY(longitude, _mapTileSizePixels, _mapTileScale, _mapTileZoomLevel);

            var lon1 = GoogleMapUtils.AdjustLonByPixels(longitude, -_mapTileSizePixels / 2, _mapTileZoomLevel);
            var lat1 = GoogleMapUtils.AdjustLatByPixels(latitude, _mapTileSizePixels / 2, _mapTileZoomLevel);

            var lon2 = GoogleMapUtils.AdjustLonByPixels(longitude, _mapTileSizePixels / 2, _mapTileZoomLevel);
            var lat2 = GoogleMapUtils.AdjustLatByPixels(latitude, -_mapTileSizePixels / 2, _mapTileZoomLevel);

            mapEnvelope = new MapEnvelope((float)lon1, (float)lat1, (float)lon2, (float)lat2);

            OnMapRedraw?.Invoke();
        }
    }
}

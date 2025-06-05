using FoodyGo.Services.GoogleMaps;
using FoodyGo.Services.GPS;
using UnityEngine;

namespace FoodyGo.Mapping
{
    public class GoogleMapTile : MonoBehaviour
    {
        [Header("Map Settings")]
        [Tooltip("줌 레벨")]
        [Range(1, 15)]
        public int zoomLevel = 15;

        [Range(64, 1024)]
        [Tooltip("맵 텍스쳐 사이즈")]
        public int size = 640;

        [Tooltip("월드맵 원점")]
        public MapLocation worldCenterLocation;

        [Header("Tile Settings")]
        [Tooltip("타일링을 위한 오프셋")]
        public Vector2 tileOffSet;

        [Tooltip("오프셋 적용한 맵의 중심 위치")]
        public MapLocation tileCenterLocation;

        [Header("Map Services")]
        public GoogleStaticMapService googleStaticMapService;

        [Header("GPS Services")]
        public GPSLocationService gpsLocationService
        {
            get => _gpsLocationService;
            set
            {
                if(value != null)
                {
                    if(_gpsLocationService != null)
                    {
                        _gpsLocationService.OnMapRedraw -= RefreshMapTile;
                    }
                    value.OnMapRedraw += RefreshMapTile;
                }
                _gpsLocationService = value;
            }
        }

        private Renderer _renderer;
        private GPSLocationService _gpsLocationService;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
        }
        /*private void OnEnable()
        {
            gpsLocationService.OnMapRedraw += RefreshMapTile;
        } 
        private void OnDisable()
        {
            gpsLocationService.OnMapRedraw -= RefreshMapTile;
        }*/
        private void Start()
        {
            //RefreshMapTile();
        }

        public void RefreshMapTile()
        {
            //오프셋에 따른 중심 위치 계산
            tileCenterLocation.latitude = GoogleMapUtils.AdjustLatByPixels(
                worldCenterLocation.latitude, (int)(size * tileOffSet.y), zoomLevel);
            tileCenterLocation.longitude = GoogleMapUtils.AdjustLonByPixels(
                worldCenterLocation.longitude, (int)(size * tileOffSet.x), zoomLevel);

            googleStaticMapService.LoadMap(tileCenterLocation.latitude,
                tileCenterLocation.longitude,
                zoomLevel,
                new Vector2(size, size),
                OnMapLoaded);

            
        }


        private void OnMapLoaded(Texture2D texture)
        {
            if (_renderer.material.mainTexture != null) 
            {
                Destroy(_renderer.material.mainTexture); //기존에 있으면 없애서 메모리 확보 
            }
            _renderer.material.mainTexture = texture;
        }

    }
}

﻿using FoodyGo.Services.GoogleMaps;
using FoodyGo.Services.GPS;
using System.Collections;
using UnityEngine;

namespace FoodyGo.Mapping
{
    /// <summary>
    /// MapTile 생성, 갱신, 제거 등의 관리
    /// GPS 데이터가 범위를 벗어날 때 타일맵 확장
    /// </summary>
    public class GoogleMapTileManager : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] GoogleStaticMapService _googleStaticMapService;
        [SerializeField] GPSLocationService _gpsLocationService;
        [SerializeField] GoogleMapTile _mapTilePrefab;
        [SerializeField] Transform _mapTilesParent;

        [Header("Debug")]
        Vector2Int _currentCenterTile;

        [Header("Managed mapTiles")]
        GoogleMapTile[,] _mapTiles = new GoogleMapTile[3, 3];
        readonly int[] TILE_OFFSETS = { -1, 0, 1 };

        IEnumerator Start()
        {
            yield return new WaitUntil(() => _gpsLocationService.isReady);
            InitializeTiles();
        }

        /// <summary>
        /// 현재 GPS 기반으로 중심 타일 인덱스 계산
        /// 3X3 배열로 MapTile들 생성 
        /// </summary>
        void InitializeTiles()
        {
            
            _currentCenterTile = CalcTileCoordinate(_gpsLocationService.mapCenter);
            CreateTiles(_currentCenterTile);
        }

        void CreateTiles(Vector2Int center)
        {
            //중심 인덱스 기준으로 모든 방향 타일들 인덱스 계산 
            for (int i = 0; i < TILE_OFFSETS.Length; i++)
            {
                for (int j = 0; j < TILE_OFFSETS.Length; j++)
                {
                    Vector2Int coord = new Vector2Int(center.x + TILE_OFFSETS[i],
                                                        center.y + TILE_OFFSETS[j]);
                    GoogleMapTile tile =  Instantiate(_mapTilePrefab, _mapTilesParent);
                    tile.tileOffSet = new Vector2Int(i - 1, j - 1);
                    tile.googleStaticMapService = _googleStaticMapService;
                    tile.zoomLevel = _gpsLocationService._mapTileZoomLevel;
                    tile.gpsLocationService = _gpsLocationService;
                    tile.name = $"MapTile_{coord.x}_{coord.y}";
                    tile.transform.position = CalcWorldPosition(coord);
                    tile.RefreshMapTile();
                }
            }
        }

        /// <summary>
        /// 타일 인덱스로 게임월드 포지션 산출
        /// </summary>
        /// <param name="coord">타일 인덱스</param>
        /// <returns></returns>
        Vector3 CalcWorldPosition(Vector2Int coord)
        {
            float spacing = 10f;
            //float spacing = CalcWorldPositionSpacing(_gpsLocationService._mapTileZoomLevel); //나중에 변수로 바꾸기 
            return new Vector3(-coord.x * spacing, 0f, coord.y * spacing);
        }


        float CalcWorldPositionSpacing(int zoomLevel)
        {
            float delta = zoomLevel - 30f;
            return 30f * Mathf.Pow(0.5f, delta);
        }

        /// <summary>
        /// 특정 위도, 경도에 해당하는 MapTile 의 인덱스를 계산
        /// </summary>
        /// <param name="center">MapTile을 그릴 위도 경도 중심</param>
        /// <returns></returns>
        Vector2Int CalcTileCoordinate(MapLocation center)
        {
            //메르카토르 픽셀 좌표(zoom=21)
            int pixelX21 = GoogleMapUtils.LonToX(center.longitude);
            int pixelY21 = GoogleMapUtils.LatToY(center.latitude);

            // GoogleMap Zoomlevel 1당 2배씩 값이 작아지기 때문에(공식 문서)
            // Zoom Level 차이 만큼 오른쪽으로 Bit shift 하면 원하는 픽셀 값ㅇ르 구할 수 있음
            int shift = 21 - _gpsLocationService._mapTileZoomLevel;
            int pixelX = pixelX21 >> shift;
            int pixelY = pixelY21 >> shift;

            //MapTIle 당 픽셀수로 나누면 인덱스 구할 수 잇음
            return new Vector2Int(Mathf.RoundToInt(pixelX / (float)_gpsLocationService._mapTileSizePixels),
                Mathf.RoundToInt(pixelY / (float)_gpsLocationService._mapTileSizePixels));
        }
    }
}

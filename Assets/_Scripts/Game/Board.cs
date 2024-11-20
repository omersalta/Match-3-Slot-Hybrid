using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Game
{
    public class Board : MonoBehaviour
    {
        private Transform _tilePrefab;
        public int RowCount { get; private set; }
        public int ColumnCount { get; private set; }

        private List<List<ITile>> _tiles;
        public List<List<ITile>> Tiles => _tiles;
        
        protected RandomDropCreator randomDropCreator;

        public void Initialize(Transform tilePrefab, int row, int column)
        {
            RowCount = row;
            ColumnCount = column;
            _tilePrefab = tilePrefab;
            _tiles = new List<List<ITile>>();
            GenerateBoard();
            Reset();
        }

        public void Reset()
        {
            randomDropCreator = new RandomDropCreator(GameManager.Instance.DropTypes);
            
            //kill Drops
            foreach (List<ITile> columnTiles in _tiles)
            {
                foreach (ITile tile in columnTiles)
                { 
                    tile.GetDrop()?.Kill();
                }
            }
            //spawn Drops
            foreach (List<ITile> columnTiles in _tiles)
            {
                foreach (ITile tile in columnTiles)
                {
                    randomDropCreator.SpawnDrop(tile);
                }
            }
        }
        
        private void GenerateBoard()
        {
            //spawn Tiles
            for (int j = 0; j < ColumnCount; j++)
            {
                _tiles.Add(new List<ITile>());
                for (int i = 0; i < RowCount; i++)
                {
                    _tiles[j].Add(Tile.SpawnTile(_tilePrefab).Initialize(j, i, transform));
                }
            }
            
        }
        
        public ITile GetTile(int x, int y)
        {
            if (x < 0 || y < 0 || x >= ColumnCount || y >= RowCount)
            {
                return null;
            }
    
            return _tiles[x][y];
        }
        
        public ITile GetTile(Vector2Int pos)
        {
            return GetTile(pos.x, pos.y);
        }

        public ITile GetNeighborTile(ITile tile, Vector2Int offset)
        {
            return GetTile(tile.Pos + offset);
        }

    }
}
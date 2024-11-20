namespace _Scripts.Game
{
    using System.Collections.Generic;
    using UnityEngine;

    public class MatchChecker
    {
        private readonly int _rowCount;
        private readonly int _columnCount;
        private readonly SlotMachineBoard _slotMachineBoard;

        public MatchChecker(SlotMachineBoard slotMachineBoard, int rowCount, int columnCount)
        {
            _slotMachineBoard = slotMachineBoard;
            _rowCount = rowCount;
            _columnCount = columnCount;
        }
        
        public bool CheckMatch(ITile tile, Axis axis, ISet<ITile> visited = null)
        {
            if (tile.GetDrop() == null)
                return false;
            if(visited == null)
                visited = new HashSet<ITile>();
            int matchCount = CheckMatchRecursive(tile, axis, tile.GetDrop().GetColor(), visited);
            return matchCount >= 3;
        }

        private int CheckMatchRecursive(ITile tile, Axis axis, dropColors color, ISet<ITile> matchedTiles)
        {
            ITile Neighbor (ITile tile, Vector2Int offset) => _slotMachineBoard.GetNeighborTile(tile, offset);
            
            if (tile == null)
                return 0;
            
            if (matchedTiles.Contains(tile))
                return 0;
            
            if (tile.GetDrop()?.GetColor() != color)
                return 0;
            
            matchedTiles.Add(tile);
            
            int matchCount = 1;
            if (axis == Axis.all)
            {

                ISet<ITile> horizontalMatchedTiles = new HashSet<ITile>();
                ISet<ITile> verticalMatchedTiles = new HashSet<ITile>();
                int verticalMatchCount = 0;
                int horizontalMatchCount = 0;

                horizontalMatchCount += CheckMatchRecursive(Neighbor(tile,Vector2Int.right), Axis.horizontal, color, horizontalMatchedTiles); // right
                horizontalMatchCount += CheckMatchRecursive(Neighbor(tile,Vector2Int.left), Axis.horizontal, color, horizontalMatchedTiles); // left
                
                verticalMatchCount += CheckMatchRecursive(Neighbor(tile,Vector2Int.up), Axis.vertical, color, verticalMatchedTiles); // up
                verticalMatchCount += CheckMatchRecursive(Neighbor(tile,Vector2Int.down), Axis.vertical, color, verticalMatchedTiles); // down
                
                if (horizontalMatchedTiles.Count > 2) {
                    matchedTiles.UnionWith(horizontalMatchedTiles);
                    matchCount += horizontalMatchCount;
                }
                if (verticalMatchedTiles.Count > 2)
                {
                    matchedTiles.UnionWith(verticalMatchedTiles);
                    matchCount += verticalMatchCount;
                }
            }
            else if (axis == Axis.LeftAndVertical)
            {
                matchCount += CheckMatchRecursive(Neighbor(tile,Vector2Int.left), Axis.horizontal, color, matchedTiles);
                matchCount += CheckMatchRecursive(Neighbor(tile,Vector2Int.up), Axis.vertical, color, matchedTiles);
                matchCount += CheckMatchRecursive(Neighbor(tile,Vector2Int.down), Axis.vertical, color, matchedTiles);
            }
            else if (axis == Axis.horizontal)
            { 
                matchCount += CheckMatchRecursive(Neighbor(tile,Vector2Int.left), Axis.horizontal, color, matchedTiles);
                matchCount += CheckMatchRecursive(Neighbor(tile,Vector2Int.right), Axis.horizontal, color, matchedTiles);
            }
            else if (axis == Axis.vertical)
            {
                matchCount += CheckMatchRecursive(Neighbor(tile,Vector2Int.up), Axis.vertical, color, matchedTiles);
                matchCount += CheckMatchRecursive(Neighbor(tile,Vector2Int.down), Axis.vertical, color, matchedTiles);
            }

            return matchCount;
        }
        
    }

    public enum Axis
    {
        horizontal,
        vertical,
        all,
        LeftAndVertical
    }
}
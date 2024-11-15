namespace _Scripts.Game
{
    using System.Collections.Generic;
    using UnityEngine;

    public class MatchChecker
    {
        private readonly int _rowCount;
        private readonly int _columnCount;
        private readonly Board _board;

        public MatchChecker(Board board, int rowCount, int columnCount)
        {
            _board = board;
            _rowCount = rowCount;
            _columnCount = columnCount;
        }
        
        public bool CheckMatch(ITile tile, Axis axis, ISet<ITile> visited = null)
        {
            if(visited == null)
                visited = new HashSet<ITile>();
            int matchCount = CheckMatchRecursive(tile, axis, tile.GetDrop().GetColor(), visited);
            return matchCount >= 3;
        }

        private int CheckMatchRecursive(ITile tile, Axis axis, dropColors color, ISet<ITile> matchedTiles)
        {
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
                matchCount += CheckMatchRecursive(Neighbor(tile,Vector2Int.right), Axis.right, color, matchedTiles); // right
                matchCount += CheckMatchRecursive(Neighbor(tile,Vector2Int.left), Axis.left, color, matchedTiles); // left
                matchCount += CheckMatchRecursive(Neighbor(tile,Vector2Int.up), Axis.up, color, matchedTiles); // up
                matchCount += CheckMatchRecursive(Neighbor(tile,Vector2Int.down), Axis.down, color, matchedTiles); // down
            }
            else if (axis == Axis.LeftAndVertical)
            {
                matchCount += CheckMatchRecursive(Neighbor(tile,Vector2Int.left), Axis.left, color, matchedTiles);
                matchCount += CheckMatchRecursive(Neighbor(tile,Vector2Int.up), Axis.up, color, matchedTiles);
                matchCount += CheckMatchRecursive(Neighbor(tile,Vector2Int.down), Axis.down, color, matchedTiles);
            }
            else if (axis == Axis.left)
            {
                matchCount += CheckMatchRecursive(Neighbor(tile,Vector2Int.left), Axis.left, color, matchedTiles);
            }
            else if (axis == Axis.right)
            {
                matchCount += CheckMatchRecursive(Neighbor(tile,Vector2Int.right), Axis.right, color, matchedTiles);
            }
            else if (axis == Axis.up)
            {
                matchCount += CheckMatchRecursive(Neighbor(tile,Vector2Int.up), Axis.up, color, matchedTiles);
            }
            else if (axis == Axis.down)
            {
                matchCount += CheckMatchRecursive(Neighbor(tile,Vector2Int.down), Axis.down, color, matchedTiles);
            }

            return matchCount;
            
            ITile Neighbor (ITile tile, Vector2Int offset) => _board.GetNeighborTile(tile, offset);
        }

    }

    public enum Axis
    {
        left,
        right,
        up,
        down,
        all,
        LeftAndVertical
    }
}
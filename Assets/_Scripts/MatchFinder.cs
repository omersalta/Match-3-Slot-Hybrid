namespace _Scripts
{
using System.Collections.Generic;
using UnityEngine;

public class MatchFinder
{
    private Board _board;
    // Assuming tile types are represented as integers (e.g., 0, 1, 2, etc.)
    
    public MatchFinder (Board board)
    {
        _board = board;
    }

    public class Match
    {
        public dropColors color;
        public List<Vector2Int> positions;

        public Match(dropColors color)
        {
            this.color = color;
            positions = new List<Vector2Int>();
        }
    }
    
    bool IsMatchAt(int x, int y,dropColors color)
    {
        return (  y >= 2 && _board.GetTileFromAll(x, y - 1)?.GetDrop().GetColor() == color &&
                _board.GetTileFromAll(x, y - 2)?.GetDrop().GetColor() == color  ) ||
               (  x >= 2 && _board.GetTileFromAll(x - 1, y)?.GetDrop().GetColor() == color &&
                _board.GetTileFromAll(x, y - 1)?.GetDrop().GetColor() == color  );
    }

    public List<Match> FindMatchesAll ()
    {
        List<Match> matches = new List<Match>();
        int numRows = _board.RowCount;
        int numCols = _board.ColumnCount;

        bool[,] visitedHorizontal = new bool[numRows, numCols];
        bool[,] visitedVertical = new bool[numRows, numCols];

        // Check horizontal matches recursively
        for (int i = 0; i < numRows; i++)
        {
            for (int j = 0; j < numCols; j++)
            {
                if (!visitedHorizontal[i, j])
                {
                    List<Vector2Int> matchPositions = new List<Vector2Int>();
                    FindHorizontalMatchAll(_board, i, j, visitedHorizontal, matchPositions);

                    if (matchPositions.Count >= 3)
                    {
                        Match match = new Match(_board.GetTileFromAllWithIndex(i,j).GetDrop().GetColor());
                        match.positions.AddRange(matchPositions);
                        matches.Add(match);
                    }
                }
            }
        }

        // Check vertical matches recursively
        for (int j = 0; j < numCols; j++)
        {
            for (int i = 0; i < numRows; i++)
            {
                if (!visitedVertical[i, j])
                {
                    List<Vector2Int> matchPositions = new List<Vector2Int>();
                    FindVerticalMatchAll(_board, i, j, visitedVertical, matchPositions);

                    if (matchPositions.Count >= 3)
                    {
                        Match match = new Match(_board.GetTileFromAllWithIndex(i,j).GetDrop().GetColor());
                        match.positions.AddRange(matchPositions);
                        matches.Add(match);
                    }
                }
            }
        }

        return matches;
    }

    void FindHorizontalMatchAll(Board board, int i, int j, bool[,] visited, List<Vector2Int> matchPositions)
    {
        int numRows = board.RowCount;
        int numCols = board.ColumnCount;

        visited[i, j] = true;
        matchPositions.Add(new Vector2Int(j, i)); // x = column (j), y = row (i)

        dropColors currentColor = board.GetTileFromAllWithIndex(i, j).GetDrop().GetColor();

        // Move right
        if (j + 1 < numCols && !visited[i, j + 1] && board.GetTileFromAllWithIndex(i, j+1).GetDrop().GetColor() == currentColor)
        {
            FindHorizontalMatchAll(board, i, j + 1, visited, matchPositions);
        }
    }

    void FindVerticalMatchAll(Board board, int i, int j, bool[,] visited, List<Vector2Int> matchPositions)
    {
        int numRows = board.RowCount;
        int numCols = board.ColumnCount;

        visited[i, j] = true;
        matchPositions.Add(new Vector2Int(j, i));

        dropColors currentColor = board.GetTileFromAllWithIndex(i, j).GetDrop().GetColor();

        // Move down
        if (i + 1 < numRows && !visited[i + 1, j] && board.GetTileFromAllWithIndex(i+1, j).GetDrop().GetColor() == currentColor)
        {
            FindVerticalMatchAll(board, i + 1, j, visited, matchPositions);
        }
    }
}

}
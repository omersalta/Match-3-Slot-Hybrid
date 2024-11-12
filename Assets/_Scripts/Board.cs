using System.Collections.Generic;
using System.Linq;
using _Scripts;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private Transform _tilePrefab;
    [SerializeField] private Transform _slotPrefab;
    
    private List<Slot> _slots;
    private MatchFinder _matchFinder;
    
    private bool _isSpinning = false;
    
    public int VisibleRowCount { get; private set; }
    public int RowCount { get; private set; }
    public int ColumnCount { get; private set; }
    public int BoardYOffset { get; private set; }
    
    
    
    public void Initialize(int rowCount = 5, int columnCount = 5, int boardYOffset = 1)
    {
        VisibleRowCount = rowCount;
        RowCount = VisibleRowCount+2;
        ColumnCount = columnCount;
        BoardYOffset = boardYOffset;
        
        _slots = new List<Slot>(ColumnCount);
        _matchFinder = new MatchFinder(this);
        
        //Create Tiles
        for (int j = 0; j < ColumnCount; j++)
        {
            Slot slot = Slot.SpawnSlot(_slotPrefab);
            slot.Initialize(RowCount, j, _tilePrefab, transform);
            _slots.Add(slot);
            slot.OnStopEvent.AddListener(OnSlotStop);
        }
        
        foreach (var slot in _slots)
        {
            foreach (var tile in slot._tiles)
            {
                Drop.SpawnDrop(GameManager.Instance.GetDropSO().PickRandom(), tile);
            }
        }
        
        foreach (var slot in _slots)
        {
            foreach (var tile in slot._tiles)
            {
                FixInvalidateDrop(tile);
            }
        }
    }
    
    public void FixInvalidateDrop (ITile tile)
    {
        Debug.Log(tile.GetCoordination() + "before :" + tile.GetDrop().GetColor());
        while (CheckMatch(tile,Axis.all))
        {
            var createdDrop = GameManager.Instance.GetDropSO().PickRandom();
            tile.GetDrop().ChangeDrop(createdDrop);
        }
        Debug.Log(tile.GetCoordination() + "after :" + tile.GetDrop().GetColor());
    }
    
    public void SpinButtonToggle()
    {
        _isSpinning = !_isSpinning;
        if (_isSpinning)
        {
            RunSpin();
        }
        else
        {
            StopSpin();
        }
    }
    
    public void RunSpin()
    {
        foreach (var slot in _slots)
        {
            slot.RunSlotSpin();
        }
    }
    
    public void StopSpin()
    {
        _slots.FirstOrDefault()?.TryToStop();
    }
    
    void OnSlotStop(int index)
    {
        if (index >= -1 && index < _slots.Count - 1)
        {
            _slots[index + 1]?.TryToStop();
        }
    }
    
    public ITile GetTileFromAllWithIndex(int row, int column)
    {
        return GetTileFromAll(column, row);
    }
    
    public ITile GetTileFromAll(int x, int y)
    {
        ITile tile = null;
        
        if (x < 0 || y < 0 || x >= ColumnCount || y >= RowCount)
        {
            Debug.LogWarning("invalid index :" + x + "," + y );
            return tile;
        }

        tile = _slots[x]._tiles[y];
        
        return tile;
    }
    
    public ITile GetTile(int x, int boardY)
    {
        int y = boardY + BoardYOffset;
        
        if (y < BoardYOffset  || y >= VisibleRowCount + BoardYOffset)
        {
            Debug.LogWarning("invalid index :" + x + "," + y );
            return null;
        }
        return GetTileFromAllWithIndex(x, y);
    }
    
    public bool CheckMatch(ITile tile,Axis axis)
    {
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        int matchCount = CheckMatchRecursive(tile.GetCoordination().x, tile.GetCoordination().y,axis, tile.GetDrop().GetColor(), visited);
        
        return matchCount >= 3;
    }
    
    private int CheckMatchRecursive(int x, int y,Axis axis, dropColors color, HashSet<Vector2Int> visited)
    {
        // Koşullar: Grid sınırları içinde mi ve zaten ziyaret edilmemiş mi?
        if (x < 0 || x >= ColumnCount || y < 0 || y >= RowCount || visited.Contains(new Vector2Int(x, y)))
            return 0;

        // Hedef türle eşleşmiyorsa bu hücreyi dikkate alma
        if (GetTileFromAll(x,y)?.GetDrop()?.GetColor() != color)
            return 0;

        // Bu hücreyi ziyaret edilmiş olarak işaretle
        visited.Add(new Vector2Int(x, y));

        // Eşleşen hücre sayısını 1 başlat ve komşuları kontrol et
        int matchCount = 1;
        if (axis == Axis.all)
        {
            matchCount += CheckMatchRecursive(x + 1, y,Axis.horizontal, color, visited); // right
            matchCount += CheckMatchRecursive(x - 1, y,Axis.horizontal, color, visited); // left
            matchCount += CheckMatchRecursive(x, y + 1,Axis.vertical, color, visited); // up
            matchCount += CheckMatchRecursive(x, y - 1,Axis.vertical, color, visited); // down
        }
        else if (axis == Axis.horizontal)
        {
            matchCount += CheckMatchRecursive(x + 1, y,Axis.horizontal, color, visited); // right
            matchCount += CheckMatchRecursive(x - 1, y,Axis.horizontal, color, visited); // left
        }else if (axis == Axis.vertical)
        {
            matchCount += CheckMatchRecursive(x, y + 1,Axis.vertical, color, visited); // up
            matchCount += CheckMatchRecursive(x, y - 1,Axis.vertical, color, visited); // down
        }else if (axis == Axis.LeftAndDown)
        {
            matchCount += CheckMatchRecursive(x - 1, y,Axis.horizontal, color, visited); // left
            matchCount += CheckMatchRecursive(x, y - 1,Axis.vertical, color, visited); // down
        }
        
        return matchCount;
    }

    public void FindMatchesAll()
    {
        // Find matches
        List<MatchFinder.Match> matches = _matchFinder.FindMatchesAll();

        // Output matches
        foreach (MatchFinder.Match match in matches)
        {
            Debug.Log("Match of type " + match.color + " at positions:");
            foreach (Vector2Int pos in match.positions)
            {
                Debug.Log(pos);
            }
        }
    }

    public enum Axis
    {
        horizontal,
        vertical,
        all,
        LeftAndDown,
    }
    
}

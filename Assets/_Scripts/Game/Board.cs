using System.Collections.Generic;
using System.Linq;
using _Scripts;
using _Scripts.Game;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using Utilities;

public class Board : MonoBehaviour
{
    private List<Slot> _slots;
    private bool _isSpinning = false;
    
    public int RowCount { get; private set; }
    public int ColumnCount { get; private set; }

    public UnityEvent OnSpinStart;
    public UnityEvent OnSpinStop;
    public UnityEvent OnSpinTryToStop;
    private Dictionary<DropSO, int> _dropTypes = new Dictionary<DropSO, int>();
    
    public void Initialize(Transform slotPrefab,Transform tilePrefab, int rowCount = 5, int columnCount = 5)
    {
        RowCount = rowCount;
        ColumnCount = columnCount;
        
        _slots = new List<Slot>(ColumnCount);
        
        InitializeDropTypes(GameManager.Instance.DropTypes);
        
        //Create Tiles
        for (int j = 0; j < ColumnCount; j++)
        {
            Slot slot = Slot.SpawnSlot(slotPrefab);
            slot.Initialize(RowCount, j, tilePrefab, transform);
            _slots.Add(slot);
            slot.OnStopEvent.AddListener(OnSlotStop);
        }
        
        foreach (var slot in _slots)
        {
            foreach (var tile in slot._tiles)
            {
                SpawnDrop(tile);
                Drop drop = tile.GetDrop();
                _dropTypes[drop.DropSO] = _dropTypes.GetValueOrDefault(drop.DropSO, 0) + 1;
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
    
    public void InitializeDropTypes(List<DropSO> allDropTypes)
    {
        foreach (var dropSo in allDropTypes)
        {
            if (!_dropTypes.ContainsKey(dropSo))
                _dropTypes[dropSo] = 0;
        }
    }

    private DropSO GetRandomDropType()
    {
        var incompleteDropTypes = _dropTypes
            .Where(pair => pair.Value < 3)
            .Select(pair => pair.Key)
            .ToList();

        DropSO selectedDropSO;

        if (incompleteDropTypes.Any())  
        {
            selectedDropSO = incompleteDropTypes.PickRandom();
        }
        else
        {
            selectedDropSO = _dropTypes.Keys.ToList().PickRandom();
        }
        
        return selectedDropSO;
    }

    public void SpawnDrop(ITile tile)
    {
        var dropColor = GetRandomDropType();
        
        var drop = Drop.SpawnDrop(dropColor);
        drop.SetTile(tile);
        drop.transform.position = tile.GetPosition();
    }

    public void LogAllTileDrops()
    {
        foreach (var slot in _slots)
        {
            foreach (var tile in slot._tiles)
            {
                Debug.Log(tile.GetCoordination() + ", " + tile.GetDrop()?.DropSO.color);
            }
        }
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
    
    void RunSpin()
    {
        foreach (var slot in _slots)
        {
            slot.RunSlotSpin();
        }
        OnSpinStart?.Invoke();
    }
    
    public void StopSpin()
    {
        _slots.FirstOrDefault()?.TryToStop();
        OnSpinTryToStop?.Invoke();
    }
    
    void OnSlotStop(int index)
    {
        if (index >= -1 && index < _slots.Count - 1)
        {
            _slots[index + 1]?.TryToStop();
        }

        if (index == _slots.LastOrDefault()?.SlotIndex)
        {
            OnSpinStop?.Invoke();
        }
    }
    
    void FixInvalidateDrop (ITile tile)
    {
        while (CheckMatch(tile,Axis.all))
        {
            var createdDrop = GetRandomDropType();
            ChangeDrop(tile.GetDrop(), createdDrop);
        }
    }
    
    void ChangeDrop(Drop drop, DropSO dropSO)
    {
        _dropTypes[drop.DropSO]--;
        drop.ChangeColor(dropSO);
        _dropTypes[dropSO]++;
    }

    public bool CheckMatch(ITile tile,Axis axis)
    {
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        int matchCount = CheckMatchRecursive(tile.GetCoordination().x, tile.GetCoordination().y, axis, tile.GetDrop().GetColor(), visited);
        return matchCount >= 3;
    }
    
    int CheckMatchRecursive(int x, int y, Axis axis, dropColors color, HashSet<Vector2Int> visited)
    {
        // Conditions: Is it within the Grid boundaries and not already visited?
        if (x < 0 || x >= ColumnCount || y < 0 || y >= RowCount || visited.Contains(new Vector2Int(x, y)))
            return 0;

        // Ignore this cell if it does not match the target type
        if (GetTile(x,y)?.GetDrop()?.GetColor() != color)
            return 0;

        // every called tiles added to visited
        visited.Add(new Vector2Int(x, y));

        // Start Recursive function with match count 1 
        int matchCount = 1;
        if (axis == Axis.all)
        {
            matchCount += CheckMatchRecursive(x + 1, y,Axis.right, color, visited); // right
            matchCount += CheckMatchRecursive(x - 1, y,Axis.left, color, visited); // left
            matchCount += CheckMatchRecursive(x, y + 1,Axis.up, color, visited); // up
            matchCount += CheckMatchRecursive(x, y - 1,Axis.down, color, visited); // down
        }
        else if (axis == Axis.LeftAndVertical)
        {
            matchCount += CheckMatchRecursive(x - 1, y,Axis.left, color, visited); // left
            matchCount += CheckMatchRecursive(x, y + 1,Axis.up, color, visited); // up
            matchCount += CheckMatchRecursive(x, y - 1,Axis.down, color, visited); // down
        }
        else if (axis == Axis.left)
        {
            matchCount += CheckMatchRecursive(x - 1, y,Axis.left, color, visited); // left (horizontal)
        }
        else if (axis == Axis.right)
        {
            matchCount += CheckMatchRecursive(x, y + 1,Axis.right, color, visited); // right (horizontal)
        }
        else if (axis == Axis.up)
        {
            matchCount += CheckMatchRecursive(x, y + 1,Axis.up, color, visited); // up (vertical)
        }
        else if (axis == Axis.down)
        {
            matchCount += CheckMatchRecursive(x, y - 1,Axis.down, color, visited); // down (vertical)
        }
        
        return matchCount;
    }
    
    public ITile GetTileFromAllWithIndex(int row, int column)
    {
        return GetTile(column, row);
    }
    
    public ITile GetTile(int x, int y)
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

using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts;
using DG.Tweening;
using UnityEngine;
using Utilities;

public class BoardManager : Singleton<BoardManager>
{
    [SerializeField] private Transform _tilePrefab;
    [SerializeField] private Transform _slotPrefab;
    [SerializeField] private Transform _slotsGradient;
    [SerializeField] private Transform _maskObject;
    [SerializeField] private List<DropSO> _dropList;
    [SerializeField] private Camera _camera;

    private Sequence slotSingleSpinSequence;
    
    private List<Slot> _slots;
    
    const int RowCount = 6;
    const int ColumnCount = 6;
    
    
    const float SpinSpeed = 8.5f;
    private bool _isSpinning = false;
    
    public float speeeeeed { get; private set; }
    
    public List<DropSO> GetDropSO() => _dropList;
    
    void Start()
    {
        _slots = new List<Slot>(ColumnCount);
        
        //Create Tiles and slots
        for (int i = 0; i < ColumnCount; i++)
        {
            _slots.Add(Slot.SpawnSlot(_slotPrefab).Initialize(RowCount,i,_tilePrefab,transform));
        }
        
        //set gradient and camera position 
        Vector3 centerPosition = new Vector2((ColumnCount-1)/2f,(RowCount-1)/2f);
        
        float ratio = ((float)Screen.height / (float)Screen.width);
        _camera.orthographicSize = ColumnCount * ( ratio / 2f);
        _camera.transform.position = new Vector3(centerPosition.x, centerPosition.y,_camera.transform.position.z);
        _slotsGradient.transform.position = centerPosition;
        _maskObject.transform.position = centerPosition;
        _slotsGradient.GetComponent<SpriteRenderer>().size = new Vector2(ColumnCount, RowCount);
        _maskObject.localScale = new Vector3(ColumnCount, RowCount, 0);
        speeeeeed = 0.3f;
    }
    
    public void SpinButtonToggle()
    {
        _isSpinning = !_isSpinning;
        if (_isSpinning)
        {
            RunSingleSpin();
        }
    }

    public void RunSingleSpin()
    {
        slotSingleSpinSequence = DOTween.Sequence();
        slotSingleSpinSequence.OnComplete(() =>
        {
            if(_isSpinning)
                RunSingleSpin();
        });
       
        foreach (var slot in _slots)
        {
            slot.StartSpin(slotSingleSpinSequence);
        }
    }
    
    
    // Update is called once per frame
    void Update()
    {
        
    }
}

public static class EnumerableExtension
{
    public static T PickRandom<T>(this IEnumerable<T> source)
    {
        return source.PickRandom(1).Single();
    }

    public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int count)
    {
        return source.Shuffle().Take(count);
    }

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        return source.OrderBy(x => Guid.NewGuid());
    }
}

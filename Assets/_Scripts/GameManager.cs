using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private Transform _slotsGradient;
    [SerializeField] private Transform _maskObject;
    [SerializeField] private List<DropSO> _dropTypes;
    [SerializeField] private Camera _camera;
    
    [SerializeField] private Board _board;
    public Board Board => _board;
    
    [SerializeField] private int RowCount = 6;
    [SerializeField] private int ColumnCount = 6;
    [SerializeField] private int BoardYOffset = 1;
    
    public float speeeeeed { get; private set; }
    
    public List<DropSO> GetDropSO() => _dropTypes;
    
    void Start()
    {
        //set gradient and camera position 
        Vector3 centerPosition = new Vector2((ColumnCount-BoardYOffset)/2f,(RowCount-BoardYOffset)/2f);
        float ratio = ((float)Screen.height / (float)Screen.width);
        _camera.orthographicSize = ColumnCount * ( ratio / 2f);
        _camera.transform.position = new Vector3(centerPosition.x, centerPosition.y,_camera.transform.position.z);
        _slotsGradient.transform.position = centerPosition;
        _maskObject.transform.position = centerPosition;
        _slotsGradient.GetComponent<SpriteRenderer>().size = new Vector2(ColumnCount, RowCount);
        _maskObject.localScale = new Vector3(ColumnCount, RowCount, 0);
        speeeeeed = 0.03f;
        
        _board.Initialize(RowCount,ColumnCount,BoardYOffset);
        
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

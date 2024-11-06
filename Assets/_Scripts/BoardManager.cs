using System.Collections.Generic;
using System.Linq;
using _Scripts;
using UnityEngine;
using Utilities;

public class BoardManager : Singleton<BoardManager>
{
    [SerializeField] private Transform _tilePrefab;
    [SerializeField] private Transform _slotPrefab;
    [SerializeField] private Transform _slotGradient;
    [SerializeField] private Camera _camera;
    
    private List<Slot> _slots;
    
    const int RowCount = 8;
    const int ColumnCount = 8;
    
    void Start()
    {
        _slots = new List<Slot>(ColumnCount);
        
        //Create Tiles and slots
        for (int x = 0; x < ColumnCount; x++)
        {
            _slots.Add(Instantiate(_slotPrefab,transform).GetComponent<Slot>());
            for (int y = 0; y < RowCount; y++)
            {
                Tile.SpawnTile(_tilePrefab).Initialize(x,y,_slots.LastOrDefault());
            }
        }
        
        //set gradient and camera position 
        Vector3 centerPosition = new Vector2((ColumnCount-1)/2,(RowCount-1)/2);
        
        float ratio = ((float)Screen.height / (float)Screen.width);
        _camera.orthographicSize = ColumnCount * ( ratio / 2);
        _camera.transform.position = new Vector3(centerPosition.x, centerPosition.y,_camera.transform.position.z);
        _slotGradient.transform.position = centerPosition;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

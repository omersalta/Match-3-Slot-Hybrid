using _Scripts;
using DG.Tweening;
using UnityEngine;

public class Tile : MonoBehaviour, ITile
{
    [SerializeField] private Slot _slot;
    
    private SpriteRenderer _renderer;
    private Drop _currentDrop;
    
    public Vector2Int coordinate { get; private set; }
    
    public Tile Initialize(int x, int y, Transform parent)
    {
        coordinate = new Vector2Int(x, y);
        transform.position = new Vector3(x, y-1, 0);
        transform.parent = parent;
        
        
        Drop.SpawnDrop(BoardManager.Instance.GetDropSO().PickRandom(), this);
        return this;
    }

    public void Interact(Tile tile)
    {
        //todo make swap
    }

    public void SetDrop(Drop drop)
    {
        _currentDrop = drop;
        drop.transform.parent = transform;
        //drop.transform.localPosition = Vector3.zero;
    }

    public Drop GetDrop()
    {
        return _currentDrop;
    }
    
    public bool HasDrop()
    {
        return GetDrop() != null;
    }

    public void ClearDrop()
    {
        _currentDrop = null;
    }
    
    public Vector3 GetPosition()
    {
        return transform.position;
    }
    
    public static Tile SpawnTile(Transform prefab)
    {
        Tile tile = Instantiate(prefab).GetComponent<Tile>();
        return tile;
    }
}
using _Scripts;
using UnityEngine;

public class Tile : MonoBehaviour, ITile
{
    [SerializeField] protected Slot _slot;

    private SpriteRenderer _renderer;
    protected Drop _drop;
    
    public Vector2Int coordinate { get; private set; }

    public void Initialize(int x, int y, Slot slot)
    {
        coordinate = new Vector2Int(x, y);
        transform.position = new Vector3(x, y, 0);
        _slot = slot;
        _slot.AddTile(this);
        transform.parent = slot.transform;
    }

    public void Interact(Tile tile)
    {
        //todo make swap
    }

    public void SetDrop(Drop drop)
    {
        _drop = drop;
        drop.transform.parent = _slot.transform;
        drop.transform.localPosition = Vector3.zero;
    }

    public Drop GetDrop()
    {
        return _drop;
    }
    
    public bool HasDrop()
    {
        return GetDrop() != null;
    }

    public void ClearDrop()
    {
        _drop = null;
    }
    
    public static Tile SpawnTile(Transform prefab)
    {
        Tile tile = Instantiate(prefab).GetComponent<Tile>();
        return tile;
    }
}
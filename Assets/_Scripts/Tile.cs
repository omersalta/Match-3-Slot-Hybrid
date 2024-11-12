using UnityEngine;

public class Tile : MonoBehaviour, ITile
{
    private SpriteRenderer _renderer;
    private Drop _currentDrop;

    private Vector2Int coordination;
    
    public Tile Initialize(int x, int y, Transform parent)
    {
        coordination = new Vector2Int(x, y);
        transform.position = new Vector3(x, y-1, 0);
        transform.parent = parent;
        
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
    public Vector2Int GetCoordination()
    {
        return coordination;
    }

    public static Tile SpawnTile(Transform prefab)
    {
        Tile tile = Instantiate(prefab).GetComponent<Tile>();
        return tile;
    }
}
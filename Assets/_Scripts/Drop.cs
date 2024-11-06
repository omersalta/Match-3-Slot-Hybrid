using System.Collections;
using System.Collections.Generic;

using UnityEngine;
public class Drop : MonoBehaviour
{
    [SerializeField] private DropSO _dropSo;

    private ITile _tile;

    public void SetTile(ITile tile)
    {
        if (tile.HasDrop())
        {
            Debug.LogError("Tile has already drop");
        }
        _tile?.ClearDrop();
        _tile = tile;
        _tile.SetDrop(this);
    }

    public static Drop SpawnDrop(DropSO DropSO, ITile tile)
    {
        Drop drop = Instantiate(DropSO.prefab).GetComponent<Drop>();
        drop.SetTile(tile);
        return drop;
    }

}






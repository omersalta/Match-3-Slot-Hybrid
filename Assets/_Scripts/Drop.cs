using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
public class Drop : MonoBehaviour
{
    [SerializeField] private DropSO _dropSo;
    [SerializeField] private GameObject _dropVisual;

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

    public void Move(ITile target,Sequence sequence, bool isHiddenMove = false)
    {
        _tile = null;
        if (isHiddenMove)
        {
            _dropVisual.GetComponent<SpriteRenderer>().enabled = false;
        }

        sequence.Join(
            transform.DOMove(target.GetPosition(), BoardManager.Instance.speeeeeed).OnComplete(() => {
                SetTile(target);
                _dropVisual.GetComponent<SpriteRenderer>().enabled = true;
            }).SetEase(Ease.Linear)
        );
    }

    public static Drop SpawnDrop(DropSO DropSO, ITile tile)
    {
        Drop drop = Instantiate(DropSO.prefab).GetComponent<Drop>();
        drop._dropVisual.GetComponent<SpriteRenderer>().sprite = DropSO.sprite;
        drop.SetTile(tile);
        drop.transform.position = tile.GetPosition();
        return drop;
    }

}






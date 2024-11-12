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
            ShowSprite(false);
        }

        sequence.Join(
            transform.DOMove(target.GetPosition(), GameManager.Instance.speeeeeed).OnComplete(() => {
                SetTile(target);
                ShowSprite();
            }).SetEase(Ease.Linear)
        );
    }

    public void ChangeDrop(DropSO DropSO)
    {
        _dropVisual.GetComponent<SpriteRenderer>().sprite = DropSO.sprite;
        _dropSo = DropSO;
    }

    public static Drop SpawnDrop(DropSO DropSO, ITile tile)
    {
        Drop drop = Instantiate(DropSO.prefab).GetComponent<Drop>();
        drop._dropVisual.GetComponent<SpriteRenderer>().sprite = DropSO.sprite;
        drop._dropSo = DropSO;
        drop.SetTile(tile);
        
        drop.transform.position = tile.GetPosition();
        return drop;
    }

    public void ShowSprite(bool show = true)
    {
        _dropVisual.GetComponent<SpriteRenderer>().enabled = show;
    }

    public dropColors getColor()
    {
        return _dropSo.color;
    }

    
}






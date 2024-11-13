using DG.Tweening;
using UnityEngine;

public class Drop : MonoBehaviour
{
    [SerializeField] private DropSO _dropSo;
    public DropSO DropSO => _dropSo;
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
            transform.DOMove(target.GetPosition(), GameManager.Instance.SingleDropTime).OnComplete(() => {
                SetTile(target);
                ShowSprite();
            }).SetEase(Ease.Linear)
        );
    }

    public void ChangeColor(DropSO dropSO)
    {
        _dropVisual.GetComponent<SpriteRenderer>().sprite = dropSO.sprite;
        _dropSo = dropSO;
    }

    public static Drop SpawnDrop(DropSO DropSO)
    {
        Drop drop = Instantiate(DropSO.prefab).GetComponent<Drop>();
        drop._dropVisual.GetComponent<SpriteRenderer>().sprite = DropSO.sprite;
        drop._dropSo = DropSO;
        return drop;
    }

    public void ShowSprite(bool show = true)
    {
        _dropVisual.GetComponent<SpriteRenderer>().enabled = show;
    }

    public dropColors GetColor()
    {
        return _dropSo.color;
    }
    
}






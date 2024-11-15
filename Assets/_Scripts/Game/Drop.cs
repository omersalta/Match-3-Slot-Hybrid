using _Scripts.Game;
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
        if (tile == null)
        {
            _tile = null;
            return;
        }
        
        if (tile.HasDrop())
        {
            Debug.LogWarning("Tile has already drop");
        }
        
        _tile = tile;
        _tile.ClearDrop();
        _tile.SetDrop(this);
    }

    public void Move(ITile target,Sequence sequence, float duration = 1f, bool isHiddenMove = false)
    {
        SetTile(null);
        
        if (isHiddenMove)
        {
            ShowSprite(false);
        }
        
        sequence.Join(
            transform.DOMove(target.GetTransformPos(), duration).OnComplete(() => {
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
    
    public dropColors GetColor()
    {
        return _dropSo.color;
    }

    public void Explode()
    {
        _tile?.ClearDrop();
        SetTile(null);
        ShowSprite(false);
        //todo explode animation
    }

    public void Kill()
    {
        Destroy(gameObject);
    } 
    
    //Private Methods....................................
    private void ShowSprite(bool show = true)
    {
        _dropVisual.GetComponent<SpriteRenderer>().enabled = show;
    }
    
}






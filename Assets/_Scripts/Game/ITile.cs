using DG.Tweening;
using UnityEngine;

namespace _Scripts.Game
{
    public interface ITile
    {
        public void SetDrop(Drop drop);
    
        public bool HasDrop();

        public Drop GetDrop();
    
        public void ClearDrop();
    
        public Vector3 GetTransformPos();

        public Vector2Int Pos { get; }

        public void Swap(ITile targetTile, Sequence sequence);

    }
}
using System.Collections.Generic;
using System.Linq;
using _Scripts.Game;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace _Scripts
{
    public class Slot : MonoBehaviour,ISlot
    {
        private int _slotIndex = -1;
        public int SlotIndex => _slotIndex;
        public List<ITile> Tiles { get; private set; }
        public UnityEvent<int> OnStopEvent { get; private set; } = new UnityEvent<int>();

        private int _rowCount;
        
        private Sequence _slotSingleSpinSequence;
        private bool _isSpinning = false;
        private int _spinMinStopTile = 0;
        readonly int  MinSpinStopTileOffset = 4;
        
        private Board _board;
        
        public void Initialize(Board board, int rowCount, int columnIndex, Transform tilePrefab, Transform parentTransform)
        {
            _slotIndex = columnIndex;
            _rowCount = rowCount;
            transform.localPosition += new Vector3(columnIndex, 0, 0);
            _board = board;
            
            Tiles = new List<ITile>();
            
            for (int i = 0; i < _rowCount; i++)
            {
                Tiles.Add(Tile.SpawnTile(tilePrefab).Initialize(_slotIndex, i, transform));
            }
            
            transform.parent = parentTransform;
        }
      
        public void StartSpin()
        {
            _isSpinning = true;
            SingleSpinRecursion();
        }
        
        public void TryToStop()
        {
            _isSpinning = false;
            _spinMinStopTile = MinSpinStopTileOffset;
        }
        
        public static Slot SpawnSlot(Transform prefab)
        {
            Slot slot = Instantiate(prefab).GetComponent<Slot>();
            return slot;
        }
        
        
        //Private Methods........
        
        private void SingleSpinRecursion()
        {
            _slotSingleSpinSequence = DOTween.Sequence();
            _slotSingleSpinSequence.OnComplete(() =>
            {
                
                if (_isSpinning)
                {
                    SingleSpinRecursion();
                }
                else
                {
                    bool canStop = _spinMinStopTile <= 0 && _board.CanStop(this);
                    
                    if (canStop)
                    {
                        StopSpin();
                    }
                    else
                    {
                        SingleSpinRecursion();
                        _spinMinStopTile--;
                    }
                }
                SpinMove(_slotSingleSpinSequence);
            });
        }

        private void SpinMove(Sequence sequenceTween)
        {
            bool isHidden = false;
            
            for (int i = 0; i < Tiles.Count; i++)
            {
                ITile nextTile = null;
                ITile tile = Tiles[i];
                isHidden = false;

                if (i == 0) //last item
                {
                    nextTile = Tiles.LastOrDefault();
                    isHidden = true;
                }
                else
                {
                    nextTile = Tiles[i - 1];
                }
                tile.GetDrop().Move(nextTile, sequenceTween,GameManager.Instance.SingleDropTime, isHidden);
                tile.ClearDrop();
            }
        }

        private void StopSpin()
        {
            Debug.Log(_slotIndex + " Slot is stoped");
            OnStopEvent?.Invoke(_slotIndex);
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace _Scripts
{
    public class Slot : MonoBehaviour
    {
        private int _slotIndex = -1;
        public int SlotIndex => _slotIndex;
        private int _rowCount;
        
        public List<ITile> _tiles { get; private set; }
        
        private Sequence _slotSingleSpinSequence;
        private bool _isSpinning = false;
        private int _spinMinStopTile = 0;
        readonly int  MinSpinStopTileOffset = 4;
        
        public UnityEvent<int> OnStopEvent;

        public Slot Initialize(int rowCount,int slotIndex,Transform _tilePrefab,Transform parent)
        {
            _slotIndex = slotIndex;
            _rowCount = rowCount;
            transform.localPosition += new Vector3(slotIndex, 0, 0);
            
            
            _tiles = new List<ITile>();
            
            for (int i = 0; i < _rowCount; i++)
            {
                _tiles.Add(Tile.SpawnTile(_tilePrefab).Initialize(_slotIndex, i, transform));
            }
            
            transform.parent = parent;
            return this;
        }

        public void SingleSpin()
        {
            _slotSingleSpinSequence = DOTween.Sequence();
            _slotSingleSpinSequence.OnComplete(() =>
            {
                
                if (_isSpinning)
                {
                    SingleSpin();
                }
                else
                {
                    bool canStop = _spinMinStopTile <= 0 && !_tiles.Any(tile => GameManager.Instance.Board.CheckMatch(tile, Board.Axis.LeftAndVertical));
                    
                    if (canStop)
                    {
                        Debug.Log(_slotIndex + " Slot is stoped");
                        OnStopEvent?.Invoke(_slotIndex);
                    }
                    else
                    {
                        SingleSpin();
                        _spinMinStopTile--;
                    }
                }
            });
            
            
            bool isHidden = false;
            
            for (int i = 0; i < _tiles.Count; i++)
            {
                ITile nextTile = null;
                ITile tile = _tiles[i];
                isHidden = false;

                if (i == 0) //last item
                {
                    nextTile = _tiles.LastOrDefault();
                    isHidden = true;
                    tile.GetDrop().ChangeDrop();
                }
                else
                {
                    nextTile = _tiles[i - 1];
                }
                tile.GetDrop().Move(nextTile, _slotSingleSpinSequence, isHidden);
                tile.ClearDrop();
            }
        }
        
        public void RunSlotSpin()
        {
            _isSpinning = true;
            SingleSpin();
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
    }
}
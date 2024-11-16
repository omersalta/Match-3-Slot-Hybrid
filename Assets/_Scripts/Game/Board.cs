using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace _Scripts.Game
{
    public class Board : MonoBehaviour
    {
        protected List<ISlot> _slots;
        protected bool _isSpinning = false;

        public int RowCount { get; private set; }
        public int ColumnCount { get; private set; }

        public UnityEvent OnSpinStart;
        public UnityEvent OnSpinStop;
        public UnityEvent OnSpinTryToStop;

        protected RandomDropCreator _randomDropCreator;
        
        public void Initialize(Transform slotPrefab, Transform tilePrefab, int rowCount = 5, int columnCount = 5)
        {
            RowCount = rowCount;
            ColumnCount = columnCount;
            
            _slots = new List<ISlot>(ColumnCount);
            
            //spawn slots (slots spawn own tiles)
            for (int j = 0; j < ColumnCount; j++)
            {
                ISlot slot = Slot.SpawnSlot(slotPrefab);
                slot.Initialize(this,RowCount, j, tilePrefab, transform);
                _slots.Add(slot);
                slot.OnStopEvent.AddListener(OnSlotStop);
            }
            
            GenerateBoard();
        }

        protected void Reset()
        {
            _isSpinning = false;
            GenerateBoard();
        }

        private void GenerateBoard()
        {
            _randomDropCreator = new RandomDropCreator(GameManager.Instance.DropTypes);

            foreach (var slot in _slots)
            {
                foreach (var tile in slot.Tiles)
                { 
                    tile.GetDrop()?.Kill();
                }
            }
            
            //spawn Drops
            foreach (var slot in _slots)
            {
                foreach (var tile in slot.Tiles)
                {
                    _randomDropCreator.SpawnDrop(tile);
                }
            }
        }

        public void ChangeDrop(ITile tile)
        {
            _randomDropCreator.ChangeDrop(tile.GetDrop());
        }

        public virtual bool CanStop(ISlot slot)
        {
            return true;
        }
        
        public ITile GetTile(int x, int y)
        {
            if (x < 0 || y < 0 || x >= ColumnCount || y >= RowCount)
            {
                return null;
            }
    
            return _slots[x].Tiles[y];
        }
        
        public ITile GetTile(Vector2Int pos)
        {
            return GetTile(pos.x, pos.y);
        }

        public ITile GetNeighborTile(ITile tile, Vector2Int offset)
        {
            return GetTile(tile.Pos + offset);
        }

        public void SpinButtonToggle()
        {
            _isSpinning = !_isSpinning;
            if (_isSpinning)
            {
                Spin();
            }
            else
            {
                TryStopSpin();
            }
        }

        private void Spin()
        {
            foreach (var slot in _slots)
            {
                slot.StartSpin();
            }
            OnSpinStart?.Invoke();
        }

        private void TryStopSpin()
        {
            _slots.FirstOrDefault()?.TryToStop();
            OnSpinTryToStop?.Invoke();
        }

        private void OnSlotStop(int index)
        {
            if (index >= 0 && index < _slots.Count - 1)
            {
                _slots[index + 1]?.TryToStop();
            }

            if (index == _slots.Count - 1)
            {
                OnSpinStop?.Invoke();
            }
        }
    }
}



    


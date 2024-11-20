using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace _Scripts.Game
{
    public class SlotMachineBoard : Board
    {
        protected List<ISlot> _slots;
        protected bool _isSpinning = false;
        
        public UnityEvent OnSpinStart;
        public UnityEvent OnSpinStop;
        public UnityEvent OnSpinTryToStop;
        
        public void Initialize(Transform slotPrefab, Transform tilePrefab, int row, int column)
        {
            base.Initialize(tilePrefab,row,column);
            _slots = new List<ISlot>(ColumnCount);
            
            for (int j = 0; j < ColumnCount; j++)
            {
                Slot slot = Slot.SpawnSlot(slotPrefab);
                slot.Initialize(this, j, Tiles[j], transform);
                _slots.Add(slot);
                slot.OnStopEvent.AddListener(OnSlotStop);
            }
        }

        protected new void Reset()
        {
            base.Reset();
            _isSpinning = false;
        }


        public void ChangeDrop(ITile tile)
        {
            randomDropCreator.ChangeDrop(tile.GetDrop());
        }

        public virtual bool CanStop(ISlot slot)
        {
            return true;
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



    


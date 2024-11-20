using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace _Scripts.Game
{
    public interface ISlot
    {
        List<ITile> Tiles { get; }
        UnityEvent<int> OnStopEvent { get; }
    
        void Initialize(SlotMachineBoard slotMachineBoard, int rowCount, int columnIndex, Transform tilePrefab, Transform parentTransform);
        void Initialize(SlotMachineBoard slotMachineBoard, int columnIndex, List<ITile> slotsTiles,  Transform parentTransform);
        void StartSpin();
        void TryToStop();
    }

}
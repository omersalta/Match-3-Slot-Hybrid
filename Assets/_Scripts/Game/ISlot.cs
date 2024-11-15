﻿using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace _Scripts.Game
{
    public interface ISlot
    {
        int SlotIndex { get; }
        List<ITile> Tiles { get; }
        UnityEvent<int> OnStopEvent { get; }
    
        void Initialize(Board board, int rowCount, int columnIndex, Transform tilePrefab, Transform parentTransform);
        void StartSpin();
        void TryToStop();
    }

}
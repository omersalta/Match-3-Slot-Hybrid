﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace _Scripts.Game
{
    public class Match3SlotMachineBoard : SlotMachineBoard
    {
        protected MatchChecker _matchChecker;
        protected SwipeManager _swipeManager;
        public UnityEvent OnFirstMatch3SwipeStart;
        
        public new void Initialize(Transform slotPrefab, Transform tilePrefab, int row, int column)
        {
            base.Initialize(slotPrefab, tilePrefab, row, column);
            _matchChecker = new MatchChecker(this);
            _swipeManager = new SwipeManager(this, GameManager.Instance.InputHandler, OnFirstMatch3SwipeStart);
            FixBoard();
        }

        private void Start()
        {
            OnSpinStop.AddListener(_swipeManager.CanMakeSwipe);
            OnSpinStart.AddListener(_swipeManager.CanNotMakeSwipe);
        }

        public new void Reset()
        {
            base.Reset();
            FixBoard();
            _swipeManager.Reset();
            _isSpinning = false;
        }

        void FixBoard()
        {
            foreach (var slot in _slots)
            {
                foreach (ITile tile in slot.Tiles)
                {
                    FixMatchedDrop(tile);
                }
            }
        }
        
        void FixMatchedDrop (ITile tile)
        {
            while (_matchChecker.CheckMatch(tile,Axis.all))
            {
                randomDropCreator.ChangeDrop(tile.GetDrop());
            }
        }
        
        public override bool CanStop(ISlot slot)
        {
            foreach (ITile tile in slot.Tiles)
            {
                if (_matchChecker.CheckMatch(tile, Axis.LeftAndVertical))
                    return false;
            }
            return true;
        }
        
        public void TryToExplode (ITile sourceTile, ITile targetTile)
        {
            ISet<ITile> explosionTiles = new HashSet<ITile>();
            var sourceExplosions = GetExplosionSet(sourceTile);
            var targetExplosions = GetExplosionSet(targetTile);
            
            if (sourceExplosions?.Count > 0)
                explosionTiles.UnionWith(sourceExplosions);
            if (targetExplosions?.Count > 0)
                explosionTiles.UnionWith(targetExplosions);
            
            if (explosionTiles.Count <= 0)
            {
                return;
            }
            else
            {
                foreach (ITile tile in explosionTiles)
                {
                    Explode(tile);
                }
                CheckIfValidMovesLeft();
            }
            
        }

        private ISet<ITile> GetExplosionSet(ITile tile)
        {
            ISet<ITile> explosions = new HashSet<ITile>();
            if (_matchChecker.CheckMatch(tile, Axis.all, explosions))
                return explosions;
            return null;
        }

        
        private void Explode(ITile tile)
        {
            randomDropCreator.RemoveDrop(tile.GetDrop().DropSO);
            tile.GetDrop().Explode();
        }
        
        private void CheckIfValidMovesLeft()
        {
            if (!randomDropCreator.CheckIfEnoughNumOfDropsFromAnyColor())
            {
                GameManager.Instance.OnGameWin();
                _swipeManager.CanNotMakeSwipe();
                Debug.Log("No moves left");
            }
        }

    }
}
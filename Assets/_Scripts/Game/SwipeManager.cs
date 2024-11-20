using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace _Scripts.Game
{
    public class SwipeManager
    {
        private List<KeyValuePair<ITile, ITile>> _swipeActions = new List<KeyValuePair<ITile, ITile>>();
        private Sequence _swipeAnimationSequence;
        private bool _canSwipe = false;
        private bool _isFirstSwipeDone = false;
        private UnityEvent _onFirstMatch3SwipeStart;

        private Match3SlotMachineBoard _board;

        public SwipeManager (Match3SlotMachineBoard board,InputHandler inputHandler, UnityEvent onFirstMatch3SwipeStart)
        {
            _onFirstMatch3SwipeStart = onFirstMatch3SwipeStart;
            inputHandler.onSwipeDown.AddListener(Swipe);
            inputHandler.onSwipeUp.AddListener(Swipe);
            inputHandler.onSwipeRight.AddListener(Swipe);
            inputHandler.onSwipeLeft.AddListener(Swipe);
            _board = board;
            Reset();
        }

        public void Reset()
        {
            _swipeActions.Clear();
            _isFirstSwipeDone = false;
        }
        
        public void CanMakeSwipe()
        {
            _canSwipe = true;
        }

        public void CanNotMakeSwipe()
        {
            _canSwipe = false;
        }
        
        private void Swipe(Vector2 pos, Vector2 dir)
        {
            Debug.Log("Swipe : " + dir);
            if (!_canSwipe) return;
            
            Vector3 wPos = Camera.main.ScreenToWorldPoint(new Vector3(pos.x, pos.y, Camera.main.nearClipPlane));

            ITile sourceTile = _board.GetTile(Mathf.RoundToInt(wPos.x), Mathf.RoundToInt(wPos.y));
            if (sourceTile == null || sourceTile.GetDrop() == null) return;

            ITile targetTile = _board.GetTile(Mathf.RoundToInt(wPos.x + dir.x), Mathf.RoundToInt(wPos.y + dir.y));
            if (targetTile == null || targetTile.GetDrop() == null) return;
            
            if (_isFirstSwipeDone == false)
            {
                _isFirstSwipeDone = true;
                _onFirstMatch3SwipeStart?.Invoke();
            }
            
            var action = new KeyValuePair<ITile, ITile>(sourceTile, targetTile);
            _swipeActions.Add(action);

            if (_swipeActions.Count == 1) // If this is the first in queue
            {
                ProcessNextAction();
            }
        }
        
        private void ProcessNextAction()
        {
            if (_swipeActions.Count == 0) return;

            var swipeAction = _swipeActions.First();

            ITile sourceTile = swipeAction.Key;
            ITile targetTile = swipeAction.Value;
            
            _swipeAnimationSequence = DOTween.Sequence();
            _swipeAnimationSequence.OnComplete(() =>
            {
                _board.TryToExplode(sourceTile, targetTile);
                _swipeActions.RemoveAt(0);
                
                if (_swipeActions.Count > 0)
                {
                    ProcessNextAction();
                }
                
            });
            
            sourceTile.Swap(targetTile, _swipeAnimationSequence);
            if (_swipeAnimationSequence.active)
                DOTween.Kill(_swipeAnimationSequence);
        }
        
    }
}
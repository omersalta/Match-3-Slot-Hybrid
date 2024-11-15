using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace _Scripts.Game
{
    public class Match3Board : Board
    {
        private List<KeyValuePair<ITile, ITile>> _swipeActions = new List<KeyValuePair<ITile, ITile>>();
        private Sequence _swipeAnimationSequence;
        private bool _isAnimating = false;
        private bool _canSwipe = false;
        protected MatchChecker _matchChecker;

        public void Initialize(Transform slotPrefab, Transform tilePrefab, int rowCount = 5, int columnCount = 5)
        {
            base.Initialize(slotPrefab, tilePrefab, rowCount, columnCount);
            _matchChecker = new MatchChecker(this, rowCount, columnCount);
        }

        private void Start()
        {
            OnSpinStop.AddListener(CanMakeSwipe);
            OnSpinStart.AddListener(CanNotMakeSwipe);
        }

        public void Reset()
        {
            _isAnimating = false;
            _canSwipe = false;
            _swipeActions.Clear();
        }

        public void Swipe(Vector2 pos, Vector2 dir)
        {
            if(!_canSwipe) return;
            
            // Screen position to world position.
            Vector3 wPos = Camera.main.ScreenToWorldPoint(new Vector3(pos.x, pos.y, Camera.main.nearClipPlane));
            
            ITile sourceTile = GetTile(Mathf.RoundToInt(wPos.x), Mathf.RoundToInt(wPos.y));
            if (sourceTile == null || sourceTile.GetDrop() == null) return; 

            ITile targetTile = GetTile(Mathf.RoundToInt(wPos.x + dir.x), Mathf.RoundToInt(wPos.y + dir.y));
            if (targetTile == null || targetTile.GetDrop() == null) return; 

            var action = new KeyValuePair<ITile, ITile>(sourceTile, targetTile);
            _swipeActions.Add(action);

            if (_swipeActions.Count == 1) // If there is no animation, we start the new process
            {
                ProcessNextAction();
            }
        }
        
        public override bool CanStop(ISlot slot)
        {
            return !slot.Tiles.Any(tile => _matchChecker.CheckMatch(tile, Axis.LeftAndVertical));
        }
        
        
        //Private Methods....................................
        private void ProcessNextAction()
        {
            if (_swipeActions.Count == 0) return;

            var swipeAction = _swipeActions.First();
            _swipeActions.RemoveAt(0); // We remove it from the list because we have started processing

            ITile sourceTile = swipeAction.Key;
            ITile targetTile = swipeAction.Value;
            
            if (sourceTile == null || targetTile == null || sourceTile.GetDrop() == null || targetTile.GetDrop() == null)
            {
                ProcessNextAction(); // Processing the next Action
                return;
            }

            _isAnimating = true;
            _swipeAnimationSequence = DOTween.Sequence();
            _swipeAnimationSequence.OnComplete(() =>
            {
                _isAnimating = false;

                // If there is a new transaction, process the next one
                if (_swipeActions.Count > 0)
                {
                    ProcessNextAction();
                }
            });

            // Moving and clearing Drop objects
            sourceTile.GetDrop()?.Move(targetTile, _swipeAnimationSequence);
            sourceTile.ClearDrop();
            targetTile.GetDrop()?.Move(sourceTile, _swipeAnimationSequence);
            targetTile.ClearDrop();
        }

        private void ExplodeDrops(HashSet<ITile> tiles)
        {
            foreach (ITile tile in tiles)
            {
                tile.GetDrop()?.Explode();
            }
            // TODO: Boş hücrelere yukarıdan drop düşürme işlemi
        }

        private void CanMakeSwipe()
        {
            _canSwipe = true;
        }
        
        private void CanNotMakeSwipe()
        {
            _canSwipe = false;
        }
        
    }
}

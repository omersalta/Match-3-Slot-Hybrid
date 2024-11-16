using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace _Scripts.Game
{
    public class Match3Board : Board
    {
        private List<KeyValuePair<ITile, ITile>> _swipeActions = new List<KeyValuePair<ITile, ITile>>();
        private Sequence _swipeAnimationSequence;
        private bool _isAnimating = false;
        private bool _canSwipe = false;
        private bool _isFirstSwipeDone = false;
        protected MatchChecker _matchChecker;
        public UnityEvent OnMatch3Start;
        
        public new void Initialize(Transform slotPrefab, Transform tilePrefab, int rowCount = 5, int columnCount = 5)
        {
            base.Initialize(slotPrefab, tilePrefab, rowCount, columnCount);
            _matchChecker = new MatchChecker(this, rowCount, columnCount);
            FixBoard();
        }

        private void Start()
        {
            OnSpinStop.AddListener(CanMakeSwipe);
            OnSpinStart.AddListener(CanNotMakeSwipe);
        }

        public new void Reset()
        {
            base.Reset();
            FixBoard();
            _isAnimating = false;
            _canSwipe = false;
            _isFirstSwipeDone = false;
            _swipeActions.Clear();
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
                _randomDropCreator.ChangeDrop(tile.GetDrop());
            }
        }

        public void Swipe(Vector2 pos, Vector2 dir)
        {
            if (!_canSwipe) return;
            
            Vector3 wPos = Camera.main.ScreenToWorldPoint(new Vector3(pos.x, pos.y, Camera.main.nearClipPlane));

            ITile sourceTile = GetTile(Mathf.RoundToInt(wPos.x), Mathf.RoundToInt(wPos.y));
            if (sourceTile == null && sourceTile.GetDrop() == null) return;

            ITile targetTile = GetTile(Mathf.RoundToInt(wPos.x + dir.x), Mathf.RoundToInt(wPos.y + dir.y));
            if (targetTile == null) return;
            
            if (_isFirstSwipeDone == false)
            {
                _isFirstSwipeDone = true;
                OnMatch3Start?.Invoke();
            }
            
            var action = new KeyValuePair<ITile, ITile>(sourceTile, targetTile);
            _swipeActions.Add(action);

            if (_swipeActions.Count == 1) // If this is the first in queue
            {
                ProcessNextAction();
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
        
        private void ProcessNextAction()
        {
            if (_swipeActions.Count == 0) return;

            var swipeAction = _swipeActions.First();

            ITile sourceTile = swipeAction.Key;
            ITile targetTile = swipeAction.Value;
            
            _isAnimating = true;
            
            _swipeAnimationSequence = DOTween.Sequence();
            _swipeAnimationSequence.OnComplete(() =>
            {
                ISet<ITile> explosions = GetExplosionSet(sourceTile, targetTile);
                _swipeActions.RemoveAt(0);

                if (explosions.Count > 0)
                {
                    foreach (ITile tile in explosions)
                    {
                        _randomDropCreator.RemoveDrop(tile.GetDrop().DropSO);
                        tile.GetDrop().Explode();
                    }
                }
                else
                {
                    _isAnimating = false;
                    
                }
                
                if (!CheckIfValidMovesLeft())
                {
                    // todo no moves left pop-up
                    GameManager.Instance.OnGameWin();
                    CanNotMakeSwipe();
                    Debug.Log("No moves left");
                    _swipeActions.Clear();
                }
                
                if (_swipeActions.Count > 0)
                {
                    ProcessNextAction();
                }
                
            });
            
            sourceTile.Swap(targetTile, _swipeAnimationSequence);
            if (_swipeAnimationSequence.active)
                DOTween.Kill(_swipeAnimationSequence);
        }

        private ISet<ITile> GetExplosionSet (ITile sourceTile, ITile targetTile)
        {
            ISet<ITile> explosions = new HashSet<ITile>();
            var sourceExplosions = ExplodeDrops(sourceTile);
            var targetExplosions = ExplodeDrops(targetTile);
            
            if (sourceExplosions?.Count > 0)
                explosions.UnionWith(sourceExplosions);
            if (targetExplosions?.Count > 0)
                explosions.UnionWith(targetExplosions);
            return explosions;
        }

        private ISet<ITile> ExplodeDrops(ITile tile)
        {
            ISet<ITile> explosions = new HashSet<ITile>();
            if (_matchChecker.CheckMatch(tile, Axis.all, explosions))
                return explosions;
            return null;
        }

        private void CanMakeSwipe()
        {
            _canSwipe = true;
        }

        private void CanNotMakeSwipe()
        {
            _canSwipe = false;
        }

        private bool CheckIfValidMovesLeft()
        {
             return _randomDropCreator.CheckIfEnoughNumOfDropsFromAnyColor();
        }

    }
}
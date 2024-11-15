using System;
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
        private Sequence _processActionSequence;
        private bool _isAnimating = false;
        private bool _canSwipe = false;
        protected MatchChecker _matchChecker;

        public void Initialize(Transform slotPrefab, Transform tilePrefab, int rowCount = 5, int columnCount = 5)
        {
            base.Initialize(slotPrefab, tilePrefab, rowCount, columnCount);
            _matchChecker = new MatchChecker(this, rowCount, columnCount);
            
            foreach (var slot in _slots)
            {
                foreach (ITile tile in slot.Tiles)
                {
                    FixInvalidateDrop(tile);
                }
            }
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
        
        void FixInvalidateDrop (ITile tile)
        {
            while (_matchChecker.CheckMatch(tile,Axis.all))
            {
                _randomDropCreator.ChangeDrop(tile.GetDrop());
            }
        }

        public void Swipe(Vector2 pos, Vector2 dir)
        {
            if (!_canSwipe) return;

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
            foreach (ITile tile in slot.Tiles)
            {
                if (_matchChecker.CheckMatch(tile, Axis.LeftAndVertical))
                    return false;
            }
            return true;
        }


        //Private Methods....................................
        private void ProcessNextAction()
        {
            if (_swipeActions.Count == 0) return;

            var swipeAction = _swipeActions.First();
            _swipeActions.RemoveAt(0); // İşlem başladığı için listeden kaldırıyoruz.

            ITile sourceTile = swipeAction.Key;
            ITile targetTile = swipeAction.Value;

            if (sourceTile == null || targetTile == null || sourceTile.GetDrop() == null || targetTile.GetDrop() == null)
            {
                ProcessNextAction(); // Sonraki işlemi başlat
                return;
            }

            _isAnimating = true;

            _processActionSequence = DOTween.Sequence();
            _swipeAnimationSequence = DOTween.Sequence();

            // İlk swipe animasyonu tamamlandığında tetiklenir
            _swipeAnimationSequence.OnComplete(() =>
            {
                // Eşleşme kontrolü
                ISet<ITile> explosions = GetExplosionSet(sourceTile, targetTile);

                if (explosions.Count > 0)
                {
                    foreach (ITile tile in explosions)
                    {
                        tile.GetDrop().Explode();
                    }
                    /*// Patlamalar gibi animasyonları tetikle
                    TriggerExplosions(() =>
                    {
                        // Patlama tamamlandığında
                        _isAnimating = false;

                        // Sıradaki işlemi başlat
                        if (_swipeActions.Count > 0)
                        {
                            ProcessNextAction();
                        }
                    });*/
                }
                else
                {
                    // Eğer eşleşme yoksa sıradaki işlemi başlat
                    _isAnimating = false;
                    if (_swipeActions.Count > 0)
                    {
                        ProcessNextAction();
                    }
                }
            });

            // Drop objelerini hareket ettir
            sourceTile.Swap(targetTile, _swipeAnimationSequence);
        }

        // Patlamalar için bir animasyon işlemi
        private void TriggerExplosions(UnityAction onComplete)
        {
            var explosionSequence = DOTween.Sequence();

            // Burada patlama animasyonlarını ekle
            explosionSequence.AppendInterval(0.5f); // Örneğin, 0.5 saniyelik bir patlama animasyonu

            // Animasyon tamamlandığında callback'i çağır
            explosionSequence.OnComplete(() => { onComplete?.Invoke(); });
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

    }
}
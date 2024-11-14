using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace _Scripts.Game
{
    public class Match3Manager : Board
    {
        private List<KeyValuePair<ITile, ITile>> _swipeActions = new List<KeyValuePair<ITile, ITile>>();
        private Sequence _swipeAnimationSequence;
        
        public void Swipe(Vector2 pos, Vector2 dir)
        {
        
            Vector3 wPos = Camera.main.ScreenToWorldPoint(new Vector3(pos.x, pos.y, Camera.main.nearClipPlane));
            Debug.Log("pos:" + wPos + ", dir:" + dir);
        
            ITile sourceTile = GetTile(Mathf.RoundToInt(wPos.x), Mathf.RoundToInt(wPos.y));
            if (sourceTile == null)
                return;
        
            ITile targetTile = GetTile(Mathf.RoundToInt(wPos.x + dir.x), Mathf.RoundToInt(wPos.y + dir.y));
            if (targetTile == null)
                return;

            var action = new KeyValuePair<ITile, ITile>(sourceTile, targetTile);
            _swipeActions.Add(action);
        
            if (_swipeActions.Count == 1) // if there is no stacked action
            {
                ProcessNextAction(_swipeActions.First());
            }
        }

        void ProcessNextAction(KeyValuePair<ITile, ITile> swipeAction)
        {
            Debug.Log("swipeAction:" + swipeAction.Key.GetPosition() + ", " + swipeAction.Value.GetPosition());
            
            var sourceTile = swipeAction.Key;
            var targetTile = swipeAction.Value;

            if (sourceTile == null || targetTile == null || sourceTile.GetDrop() == null || targetTile.GetDrop() == null)
            {
                _swipeActions.RemoveAt(0);
            }
            
            _swipeAnimationSequence = DOTween.Sequence();
            _swipeAnimationSequence.OnComplete(() =>
            {
                
                if (_swipeActions.Count > 0)
                {
                    ProcessNextAction(_swipeActions.First());
                }
            });
            
        
            // TODO: get drop could be null
            sourceTile.GetDrop().Move(targetTile,_swipeAnimationSequence,1);
            sourceTile.ClearDrop();
            targetTile.GetDrop().Move(sourceTile,_swipeAnimationSequence,1);
            targetTile.ClearDrop();
        }

        private void ExplodeDrops(HashSet<ITile> tiles)
        {
            foreach (ITile tile in tiles)
            {
                tile.GetDrop().Explode();
            }
            //TODO drop the Drops on empty tiles
        }
    }
}
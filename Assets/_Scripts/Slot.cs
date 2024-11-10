using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace _Scripts
{
    public class Slot : MonoBehaviour
    {

        private int _slotIndex = -1;
        private int _rowCount = 0;
        private int _stripSize = 0;
        private Vector3 _cretionPoint, _endPoint;
        
        private List<ITile> _tiles;

        public Slot Initialize(int rowCount,int slotIndex,Transform _tilePrefab,Transform parent)
        {
            _slotIndex = slotIndex;
            _rowCount = rowCount;
            _stripSize = 2 * _rowCount;
            transform.localPosition += new Vector3(slotIndex, 0, 0);
            
            _tiles = new List<ITile>();
            
            for (int i = 0; i < _stripSize; i++)
            {
                _tiles.Add(Tile.SpawnTile(_tilePrefab).Initialize(_slotIndex, i, transform));
            }
            
            transform.parent = parent;
            return this;
        }
        
        public void StopSpin()
        {
            
        }
        
        public void StartSpin(Sequence sequence)
        {
            bool isHidden = false;
            
            for (int i = 0; i < _tiles.Count; i++)
            {
                ITile nextTile = null;
                ITile tile = _tiles[i];
                isHidden = false;
                
                if (i == 0) //last item
                {
                    nextTile = _tiles.LastOrDefault();
                    isHidden = true;
                }
                else
                {
                    nextTile = _tiles[i-1];
                }
                
                tile.GetDrop().Move(nextTile, sequence,isHidden);
                tile.ClearDrop();
            }
        }

        public void CreateDrop()
        {
            
        }

        public void FillTiles()
        {
            
        }
        
        public static Slot SpawnSlot(Transform prefab)
        {
            Slot slot = Instantiate(prefab).GetComponent<Slot>();
            return slot;
        }
    }
}
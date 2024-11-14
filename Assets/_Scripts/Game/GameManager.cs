using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace _Scripts.Game
{
    public class GameManager : Singleton<GameManager>
    {
    
        [SerializeField] private List<DropSO> _dropTypes;
        public List<DropSO> DropTypes => _dropTypes;
    
    
        [SerializeField] private Transform _tilePrefab;
        [SerializeField] private Transform _slotPrefab;
    
        [SerializeField] private BoardConfigurator _configurator;
        [SerializeField] private Board _board;
        public Board Board => _board;
    
        [SerializeField] private int _rowCount = 6;
        [SerializeField] private int _cColumnCount = 6;
    
        public float SingleDropTime;
    
        void Start()
        {
            //set gradient and camera position 
            _board.Initialize(_slotPrefab, _tilePrefab, _rowCount,_cColumnCount);
            _configurator.Initialize(_cColumnCount, _rowCount);
        }
    
    }
}


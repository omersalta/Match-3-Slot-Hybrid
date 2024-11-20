using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;

namespace _Scripts.Game
{
    public class GameManager : Singleton<GameManager>
    {
    
        [SerializeField] private List<DropSO> _dropTypes;
        public List<DropSO> DropTypes => _dropTypes;
        
        [SerializeField] private Transform _tilePrefab;
        [SerializeField] private Transform _slotPrefab;
        [SerializeField] private InputHandler _inputHandler;
        public InputHandler InputHandler => _inputHandler;
    
        [SerializeField] private BoardConfigurator _configurator;
        [SerializeField] private Match3SlotMachineBoard _slotMachineBoard;
        [SerializeField] private CongratsPopup _congratsPopup;
        [SerializeField] private SpinButton _spinButton;
        
        [SerializeField] private int _rowCount = 6;
        [SerializeField] private int _columnCount = 6;
    
        public float SingleDropTime;
        public float SwapTime;
    
        public new void Awake()
        {
            base.Awake();
            _slotMachineBoard.Initialize(_slotPrefab,_tilePrefab,_rowCount,_columnCount);
            _configurator.Initialize(_columnCount, _rowCount);
            _congratsPopup.Initialize();
        }

        public void OnGameWin()
        {
            _congratsPopup.Congrats();
        }
        
        public void OnGameReset()
        {
            DOVirtual.DelayedCall(0.3f, () =>
            {
                _slotMachineBoard.Reset();
                _congratsPopup.Reset();
                _spinButton.Reset();
            });
        }
    
    }
}


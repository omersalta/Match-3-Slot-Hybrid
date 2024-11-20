using System;
using UnityEngine;

namespace _Scripts
{
    public class BoardConfigurator : MonoBehaviour
    {
        [SerializeField] private Transform _slotsGradient;
        [SerializeField] private Transform _maskObject;
        [SerializeField] private Transform _spinButton;
        [SerializeField] private Transform _CongratsPopup;
        
        public Vector3 CenterPosition {  get; private set; }
        
        public void Initialize(int columnCount,int rowCount)
        {
             CenterPosition = new Vector2((columnCount-1)/2f,(rowCount-1)/2f);
            
             //Set SlotMachineBoard Elements Settings
            _slotsGradient.transform.position = CenterPosition;
            _maskObject.transform.position = CenterPosition;
            _CongratsPopup.transform.position = CenterPosition;
            
            
            _slotsGradient.GetComponent<SpriteRenderer>().size = new Vector2(columnCount, rowCount);
            _maskObject.localScale = new Vector3(columnCount, rowCount, 0);
            
            
            //Set Camera Settings
            float ratio = ((float)Screen.height / (float)Screen.width);
            Camera.main.orthographicSize = columnCount * ( ratio / 2f);
            Camera.main.transform.position = new Vector3(CenterPosition.x, CenterPosition.y,Camera.main.transform.position.z);
            
            _spinButton.transform.position = new Vector3(CenterPosition.y, -1.2f,0);
            
        }
    }
}
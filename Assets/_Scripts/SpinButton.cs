using DG.Tweening;
using UnityEngine;

namespace _Scripts
{
    public class SpinButton : CustomButton
    {
        [SerializeField] private Board _board;
        
        private void Start()
        {
            SetText("Press to Spin");
            SetColor(defaultColor);
            _board.OnSpinStart.AddListener(OnSpinStart);
            _board.OnSpinStop.AddListener(OnSpinStop);
            _board.OnSpinTryToStop.AddListener(OnSpinTryStop);
        }

        private void OnSpinStart ()
        {
            Enable = false;
            defaultColor = Color.grey * 0.6f;
            SetText("wait");
            DOVirtual.DelayedCall(GameManager.Instance.SingleDropTime * 25f, () =>
            {
                SetText("Press to Stop");
                Enable = true;
                SetColor(Color.red*0.9f);
            });
        }

        private void OnSpinStop()
        {
            SetText("Spin");
            Enable = true;
            SetColor(Color.green*0.9f);
        }
        
        private void OnSpinTryStop()
        {
            SetText("Stopped");
            SetColor(Color.yellow*0.7f);
            Enable = false;
        }

        private void OnMatch3()
        {
            SetText("Match-3");
            Enable = false;
            
            SetColor(Color.black);
        }
    }
}
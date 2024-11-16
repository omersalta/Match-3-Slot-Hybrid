using _Scripts.Game;
using DG.Tweening;
using UnityEngine;

namespace _Scripts
{
    public class SpinButton : CustomButton
    {
        [SerializeField] private Match3Board _board;
        
        private void Start()
        {
            SetText("Spin");
            SetColor(defaultColor);
            _board.OnSpinStart.AddListener(OnSpinStart);
            _board.OnSpinStop.AddListener(OnSpinStop);
            _board.OnSpinTryToStop.AddListener(OnSpinTryStop);
            _board.OnFirstMatch3SwipeStart.AddListener(OnMatch3);
        }

        private void OnSpinStart ()
        {
            SetEnable(false);
            defaultColor = Color.grey * 0.6f;
            SetText("wait");
            DOVirtual.DelayedCall(GameManager.Instance.SingleDropTime * 6f, () =>
            {
                SetEnable(true);
                SetText("Press to Stop");
                SetColor(Color.red*0.9f);
            });
        }

        private void OnSpinStop()
        {
            SetText("Spin");
            SetEnable(true);
            SetColor(Color.green*0.9f);
        }
        
        private void OnSpinTryStop()
        {
            SetText("Stopped");
            SetEnable(false);
            SetColor(Color.yellow*0.7f);
        }

        private void OnMatch3()
        {
            SetText("");
            SetEnable(false);
            SetColor(new Color(0,0,0,0));
        }

        public void Reset()
        {
            SetText("Spin");
            SetEnable(true);
            SetColor(Color.green * 0.9f);
        }
    }
}
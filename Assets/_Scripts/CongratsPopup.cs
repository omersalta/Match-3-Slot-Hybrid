using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Game;
using DG.Tweening;
using UnityEngine;

public class CongratsPopup : MonoBehaviour
{
    [SerializeField] private Transform _viusals;
    [SerializeField] private Transform _viusals1;
    [SerializeField] private Transform _viusals2;
    [SerializeField] private SpriteRenderer _darker;
    [SerializeField] private SpriteRenderer _popupBG;
    [SerializeField] private CustomButton _resetButton;
    
    private bool _canReset;
    private Vector3 _popupBGfirstValue;

    public void Initialize()
    {
        Reset();
    }

    public void Reset()
    {
        _canReset = false;
        _popupBGfirstValue = _popupBG.transform.localScale;
        _popupBG.transform.localScale = _popupBGfirstValue / 3f;
        _viusals.gameObject.SetActive(false);
        _viusals1.gameObject.SetActive(false);
        _viusals2.gameObject.SetActive(false);
    }

    public void Congrats()
    {
        _canReset = true;
        _viusals.gameObject.SetActive(true);
        _viusals1.gameObject.SetActive(false);
        _viusals2.gameObject.SetActive(false);
        
        _darker.DOFade(0, 0).OnComplete(() =>
        {
            _darker.DOFade(0.7f, 2).SetEase(Ease.Flash);
        });
        DOVirtual.DelayedCall(0.4f, () =>
        {
            _viusals1.gameObject.SetActive(true);
        }).OnComplete(() =>
        {
            DOVirtual.DelayedCall(1.5f, () =>
            {
                _viusals2.gameObject.SetActive(true);
                _popupBG.gameObject.SetActive(true);
                _popupBG.transform.localScale = _popupBGfirstValue / 3f;
                _popupBG.transform.DOScale(_popupBGfirstValue, 0.5f).SetEase(Ease.OutBounce);
            });
        });
    }
    
    public void RestartGame()
    {
        if (_canReset)
        {
            _canReset = false;
            _resetButton.SetColor(Color.white);
            GameManager.Instance.OnGameReset();
        }
    }
}

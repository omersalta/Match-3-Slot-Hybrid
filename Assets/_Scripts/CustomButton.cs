using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CustomButton : MonoBehaviour
{
    public UnityEvent onClickEvent;
    public bool Enable = true;
    [SerializeField] private TextMeshPro _text;
    
    
    [SerializeField] protected Color defaultColor = Color.green * 0.9f;
    

    void OnMouseDown()
    {
        if (Enable)
        {
            transform.DOScale(transform.localScale / 2, 0.2f).OnComplete(() =>
            {
                transform.DOScale(Vector3.one, 0.2f);
                onClickEvent?.Invoke();
            });
        }
            
    }

    void OnMouseOver()
    {
        GetComponent<Renderer>().material.color = defaultColor*0.85f;
    }

    void OnMouseExit()
    {
        GetComponent<Renderer>().material.color = defaultColor;
    }

    protected void SetColor(Color color)
    {
        GetComponent<Renderer>().material.color = color;
        defaultColor = color;
    }

    public void SetText(string text)
    {
        if(_text)
            _text.text = text;
    }
    
    public string GetText()
    {
        if(_text)
            return _text.text;
        return "";
    }
}
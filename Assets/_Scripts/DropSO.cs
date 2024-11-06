using UnityEngine;

[CreateAssetMenu]
public class DropSO : ScriptableObject
{
    public Transform prefab;
    public Sprite sprite;
    public dropColors color;
}

public enum dropColors
{
    Yellow,
    Blue,
    Green,
    Red,
    Pink,
    Orange,
    Black,
    White
}
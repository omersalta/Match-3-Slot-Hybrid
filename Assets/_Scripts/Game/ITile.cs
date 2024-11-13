using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public interface ITile
{
    public void SetDrop(Drop drop);
    
    public bool HasDrop();

    public Drop GetDrop();
    
    public void ClearDrop();
    
    public Vector3 GetPosition();

    public Vector2Int GetCoordination();

}
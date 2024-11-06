using UnityEngine;

namespace _Scripts
{
    public class Slot : MonoBehaviour
    {
        private int _size = 0;

        public void AddTile(Tile tile)
        {
            _size++;
        }
    }
}
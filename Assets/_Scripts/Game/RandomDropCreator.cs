using System.Collections.Generic;
using System.Linq;
using Utilities;

namespace _Scripts.Game
{
    public class RandomDropCreator
    {
        private Dictionary<DropSO, int> _dropTypes;
        private readonly int _minNumOfDropTypes = 3;

        public RandomDropCreator(List<DropSO> allDropTypes)
        {
            InitializeDropTypes(allDropTypes);
        }
        
        public void SpawnDrop(ITile tile)
        {
            var dropType = GetRandomDropType();
            var drop = Drop.SpawnDrop(dropType);
            drop.SetTile(tile);
            drop.transform.position = tile.GetTransformPos();
            _dropTypes[dropType]++;
        }
        
        public void ChangeDrop(Drop drop)
        {
            if (drop == null || !_dropTypes.ContainsKey(drop.DropSO))
                return;
            
            _dropTypes[drop.DropSO]--;
            DropSO newDropType = GetRandomDropType();
            drop.ChangeColor(newDropType);
            _dropTypes[newDropType]++;
        }
        
        private void InitializeDropTypes(List<DropSO> allDropTypes)
        {
            _dropTypes = new Dictionary<DropSO, int>();
            foreach (var dropSo in allDropTypes)
            {
                if (!_dropTypes.ContainsKey(dropSo))
                    _dropTypes[dropSo] = 0;
            }
        }
        
        private DropSO GetRandomDropType()
        {
            var incompleteDropTypes = _dropTypes
                .Where(pair => pair.Value < _minNumOfDropTypes)
                .Select(pair => pair.Key)
                .ToList();

            DropSO selectedDropSO;
            if (incompleteDropTypes.Any())
            {
                selectedDropSO = incompleteDropTypes.PickRandom();
            }
            else
            {
                selectedDropSO = _dropTypes.Keys.ToList().PickRandom();
            }

            return selectedDropSO;
        }
        
    }

}
using UnityEngine;

namespace MyGameNamespace
{

    [CreateAssetMenu(fileName = "Plant Configs", menuName = "Configs/Plant Level")]
    public class PlantLevelConfigs : ScriptableObject, ILevelPlant
    {
        [field: SerializeField] public int CostToUpgrade { get; set; }
        [field: SerializeField] public int Level { get; set; }
        [field: SerializeField] public int CoinEarn { get; set; }
        [field: SerializeField] public float Duration { get; set; }

        public IPlant ApplyStrategy(IPlant plant)
        {
            plant.CoinEarn = CoinEarn;
            plant.Level = Level;
            plant.Duration = Duration;
            return plant;
        }
    }
}

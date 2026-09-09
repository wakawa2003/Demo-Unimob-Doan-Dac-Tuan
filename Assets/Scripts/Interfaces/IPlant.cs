using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGameNamespace
{
    public interface IPlant
    {
        public Transform transform { get; }
        public void SetLevel(ILevelPlant levelPlant);
        public List<ILevelPlant> LevelPlants { get; }
        public ILevelPlant LevelPlantStrategy { get; set; }
        public int Level { get; set; }
        public int CoinEarn { get; set; }
        public float Duration { get; set; }
    }
}
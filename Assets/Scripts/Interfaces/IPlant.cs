using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGameNamespace
{
    public interface IPlant : IEasyDIDecore<IPlant>
    {


        public ICarryable Carryable { get; }
        public string PlantID { get; set; }
        public string ResourcesID { get; set; }
        public Transform transform { get; }
        public void SetLevel(ILevelPlant levelPlant);
        public List<ILevelPlant> LevelPlants { get; }
        public ILevelPlant LevelPlantStrategy { get; set; }


        public int Level { get; set; }

        public int _CoinEarn { get; set; }
        public int CoinEarn { get => _CoinEarn + (Decore != null ? Decore.CoinEarn : 0); set => _CoinEarn = value; }
        // public int CoinEarn { get; set; }

        public float _Duration { get; set; }
        public float Duration { get => _Duration + (Decore != null ? Decore.Duration : 0); set => _Duration = value; }
        // public float Duration { get; set; }
    }
}
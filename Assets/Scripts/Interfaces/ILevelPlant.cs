using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGameNamespace
{
    public interface ILevelPlant
    {
        public int CostToUpgrade { get; }
        public IPlant ApplyStrategy(IPlant plant);
    }
}
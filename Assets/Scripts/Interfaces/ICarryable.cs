using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGameNamespace
{
    public interface ICarryable
    {
        public void SetSpawn(Transform[] posList);
        public void SetOwner(Transform[] posList, ICarrier owner);


    }
}
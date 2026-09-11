using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyGameNamespace
{
    public interface ICarryable
    {
        public void SetSpawn(Transform[] posList);
        public UniTask SetOwner(Transform[] posList, ICarrier owner, CancellationToken cancellationToken);


    }
}
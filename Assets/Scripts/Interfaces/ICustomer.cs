using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyGameNamespace
{
    public interface ICustomer
    {
        public UniTask MoveTotarget(Transform transformTarget, float time, CancellationToken cancellationToken);


    }
}
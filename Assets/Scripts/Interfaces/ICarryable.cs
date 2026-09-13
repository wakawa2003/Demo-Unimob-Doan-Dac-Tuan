using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyGameNamespace
{
    public interface ICarryable
    {
        component GetComponent<component>();
        ICarrier Owner { get; set; }
        public void Setup(ICarrier owner, Transform[] posList);
        public UniTask SetOwner(Transform[] posList, ICarrier owner, CancellationToken cancellationToken);


    }
}
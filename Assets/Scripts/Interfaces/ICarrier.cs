using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyGameNamespace
{
    public interface ICarrier
    {

        public void TakeOffCarryable();
        public UniTask TakeCarryable(ICarrier fromCarrier, ICarryable carryable, CancellationToken cancellationToken);
        public ICarryable Carryable { get; set; }
    }
}
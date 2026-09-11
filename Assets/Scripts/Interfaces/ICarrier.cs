using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyGameNamespace
{
    public interface ICarrier
    {
        public UniTask GetCarryable(ICarryable carryable, CancellationToken cancellationToken);
        public ICarryable Carryable { get; set; }
    }
}
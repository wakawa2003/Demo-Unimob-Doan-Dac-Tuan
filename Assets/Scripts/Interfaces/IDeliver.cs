using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyGameNamespace
{
    public interface IDeliver
    {
        public UniTask RunToPosition(Vector3 target, Vector3 rotationAtEnd, CancellationToken cancellationToken);
    }
}
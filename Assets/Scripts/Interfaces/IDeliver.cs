using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyGameNamespace
{
    public interface IDeliver : ICarrier
    {
        public void Setup(Vector3 endPos);
        public UniTask RunToPosition(Vector3 target, Vector3 rotationAtEnd, CancellationToken cancellationToken);
    }
}
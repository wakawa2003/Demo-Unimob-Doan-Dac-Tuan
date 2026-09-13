using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyGameNamespace
{
    public interface ICustomer : ICarrier
    {
        public Transform transform { get; }
        public void Setup(Vector3 endPos);
        public UniTask RunToPosition(Vector3 target, Vector3 rotationAtEnd, bool isInstant, CancellationToken cancellationToken);

    }
}
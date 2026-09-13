using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyGameNamespace
{
    public interface ICustomer : ICarrier
    {
        public void Setup(Vector3 endPos);

        public Transform transform { get; }
        public void SetPosition(Vector3 target, Vector3 rotation);
        public UniTask RunToPosition(Vector3 target, Vector3 rotationAtEnd, CancellationToken cancellationToken);

    }
}
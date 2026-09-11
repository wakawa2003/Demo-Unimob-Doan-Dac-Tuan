using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyGameNamespace
{
    public class DeliverController : MonoBehaviour, IDeliver, ICarrier
    {
        [SerializeField] private Transform[] listCarryable;

        public ICarryable Carryable { get; set; }

        public async UniTask GetCarryable(ICarryable carryable, CancellationToken cancellationToken)
        {
            if (Carryable == null)
            {
                this.Carryable = carryable;
                await carryable.SetOwner(listCarryable, this, cancellationToken);
            }
        }
    }
}

using System.Threading;
using Cysharp.Threading.Tasks;

namespace MyGameNamespace
{
    public interface ICarrier
    {

        public void TakeOffCarryableByAnother(ICarrier deliver);
        public UniTask TakeCarryable(ICarrier fromCarrier, ICarryable carryable, CancellationToken cancellationToken);
        public ICarryable Carryable { get; set; }
    }
}
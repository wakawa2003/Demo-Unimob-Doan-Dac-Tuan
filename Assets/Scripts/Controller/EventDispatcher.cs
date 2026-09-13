using UnityEngine;
using UnityEngine.Events;

namespace MyGameNamespace
{
    public class EventDispatcher
    {
        public static UnityEvent<(ICarrier carrier, ICarryable carryable, Transform transformTake)> OnCarryableSpawneed = new();
        public static UnityEvent<ICustomer> OnCustomerReadyToTake = new();
        public static UnityEvent<(ICarryable carryable, IDeliver carrier)> OnDeliverHasCarryable = new();
        public static UnityEvent<(ICustomer customer, ICarrier carrier, ICarryable carryable)> onCustomerTakeCarryable = new();

        public static UnityEvent<ICustomer> OnCustomerInit = new();
        public static UnityEvent<IDeliver> OnDeliverInit = new();
    }
}

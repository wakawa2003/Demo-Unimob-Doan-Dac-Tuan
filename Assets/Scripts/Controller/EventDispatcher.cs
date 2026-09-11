using UnityEngine;
using UnityEngine.Events;

namespace MyGameNamespace
{
    public class EventDispatcher
    {
        public static UnityEvent<(ICarryable carryable, Transform transformTake)> OnCarryableSpawneed = new();
    }
}

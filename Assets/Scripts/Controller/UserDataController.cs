using UnityEngine;
using UnityEngine.Events;

namespace MyGameNamespace
{
    public class UserDataController : MonoBehaviour, IUserData
    {
        [field: SerializeField] public long Coin { get; set; }
        [field: SerializeField] public long Diamond { get; set; }
        [field: SerializeField] public UnityEvent<(long Before, long after)> OnCoinChange { get; set; }
        [field: SerializeField] public UnityEvent<(long Before, long after)> OnDiamondChange { get; set; }
    }
}

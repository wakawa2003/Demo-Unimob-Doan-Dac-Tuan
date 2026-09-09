using UnityEngine;
using UnityEngine.Events;

namespace MyGameNamespace
{
    public class UserDataController : MonoBehaviour, IUserData
    {
        [field: SerializeField] public int Coin { get; set; }
        [field: SerializeField] public int Diamond { get; set; }
        [field: SerializeField] public UnityEvent<(int Before, int after)> OnCoinChange { get; set; }
        [field: SerializeField] public UnityEvent<(int Before, int after)> OnDiamondChange { get; set; }
    }
}

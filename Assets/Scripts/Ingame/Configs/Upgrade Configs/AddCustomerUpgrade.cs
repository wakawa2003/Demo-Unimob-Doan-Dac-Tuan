using System.Threading;
using Cysharp.Threading.Tasks;
using EasyDI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MyGameNamespace
{

    [CreateAssetMenu(fileName = "AddCustomerUpgrade", menuName = "Configs/AddCustomerUpgrade")]
    public class AddCustomerUpgrade : ScriptableObject, IUpgradeStrategy
    {
        [field: SerializeField] public Sprite Avatar { get; set; }
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public string Description { get; set; }
        [field: SerializeField] public long Cost { get; set; }

        [Inject] IUserData userData;

        public bool IsCanBuy()
        {
            var gameControlelr = GameObject.FindAnyObjectByType<GameController>();
            return gameControlelr.IsCanUnlockCustomer();
        }

        [Button]
        public async UniTask Upgrade(CancellationToken cancellationToken)
        {
            var gameControlelr = GameObject.FindAnyObjectByType<GameController>();
            gameControlelr.UnlockMoreCustomer();
            userData.AddCoin(-Cost);
            await UniTask.CompletedTask;
        }

        public void Setup(ContextBase injector)
        {
        }
    }
}

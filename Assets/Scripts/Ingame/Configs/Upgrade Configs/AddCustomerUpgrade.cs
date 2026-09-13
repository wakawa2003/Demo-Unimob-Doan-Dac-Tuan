using System;
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

        private bool IsUpgraded
        {
            get => PlayerPrefs.GetInt("buff_" + Name) == 1;
            set
            {
                PlayerPrefs.SetInt("buff_" + Name, value ? 1 : 0);
                PlayerPrefs.Save();
            }
        }


        [SerializeField] private int AmountCustomer;
        [Inject] IUserData userData;

        public bool IsCanBuy()
        {
            var gameControlelr = GameObject.FindAnyObjectByType<GameController>();
            return !IsUpgraded && gameControlelr.GetNumberUnlockCustomer() >= AmountCustomer;
        }

        [Button]
        public async UniTask Upgrade(CancellationToken cancellationToken)
        {
            for (int i = 0; i < AmountCustomer; i++)
            {
                var gameControlelr = GameObject.FindAnyObjectByType<GameController>();
                gameControlelr.UnlockMoreCustomer();

            }
            IsUpgraded = true;
            userData.AddCoin(-Cost);
            await UniTask.CompletedTask;
        }


    }
}

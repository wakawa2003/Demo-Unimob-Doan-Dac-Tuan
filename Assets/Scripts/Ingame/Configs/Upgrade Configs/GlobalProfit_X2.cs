using System.Threading;
using Cysharp.Threading.Tasks;
using EasyDI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MyGameNamespace
{
    [CreateAssetMenu(fileName = "GlobalProfit_X2", menuName = "Configs/GlobalProfit_X2")]
    public class GlobalProfit_X2 : ScriptableObject, IUpgradeStrategy
    {
        [field: SerializeField] public Sprite Avatar { get; set; }
        [field: SerializeField] public string Name { get; set; } = "Global profit x2";
        [field: SerializeField] public string Description { get; set; } = "x2 global profit";
        [field: SerializeField] public long Cost { get; set; }
        [Inject] protected IUserData userData;
        protected bool IsUpgraded
        {
            get => PlayerPrefs.GetInt("buff_" + Name) == 1;
            set
            {
                PlayerPrefs.SetInt("buff_" + Name, value ? 1 : 0);
                PlayerPrefs.Save();
            }
        }
        public bool IsCanBuy()
        {
            return !IsUpgraded;
        }

        [Button]
        public virtual async UniTask Upgrade(CancellationToken cancellationToken)
        {
            BuffManager.Ins.IsGlobalProfitX2 = true;
            IsUpgraded = true;
            userData.AddCoin(-Cost);
            await UniTask.CompletedTask;
        }
    }
}

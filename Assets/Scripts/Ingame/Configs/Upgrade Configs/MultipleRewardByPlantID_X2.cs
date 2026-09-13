using System.Threading;
using Cysharp.Threading.Tasks;
using EasyDI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MyGameNamespace
{
    [CreateAssetMenu(fileName = "MultipleResourceUpgrade", menuName = "Configs/MultipleResourceUpgrade")]
    public class MultipleRewardByPlantID_X2 : ScriptableObject, IUpgradeStrategy
    {
        [field: SerializeField] public Sprite Avatar { get; set; }
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public string Description { get; set; }
        [field: SerializeField] public long Cost { get; set; }
        [SerializeField] protected string PlantID = "plant_1";
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
            BuffManager.Ins.BuffPlantByPlantID_x2_List.Add(this.PlantID);
            IsUpgraded = true;
            userData.AddCoin(-Cost);
            await UniTask.CompletedTask;
        }

    }
}

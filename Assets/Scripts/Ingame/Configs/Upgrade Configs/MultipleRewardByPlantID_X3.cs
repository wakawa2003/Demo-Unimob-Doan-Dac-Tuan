using System.Threading;
using Cysharp.Threading.Tasks;
using EasyDI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MyGameNamespace
{
    [CreateAssetMenu(fileName = "MultipleRewardByPlantID_X3", menuName = "Configs/MultipleRewardByPlantID_X3")]
    public class MultipleRewardByPlantID_X3 : MultipleRewardByPlantID_X2
    {
        [Button]
        public override async UniTask Upgrade(CancellationToken cancellationToken)
        {
            BuffManager.Ins.BuffPlantByPlantID_x3_List.Add(this.PlantID);
            IsUpgraded = true;
            userData.AddCoin(-Cost);
            await UniTask.CompletedTask;
        }
    }
}

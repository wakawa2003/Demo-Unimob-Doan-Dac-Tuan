using System.Threading;
using Cysharp.Threading.Tasks;
using EasyDI;
using Sirenix.OdinInspector;
using UnityEngine;
namespace MyGameNamespace
{
    [CreateAssetMenu(fileName = "GlobalProfit_x3", menuName = "Configs/GlobalProfit_x3")]
    public class GlobalProfit_x3 : GlobalProfit_X2
    {
        [Button]
        public override async UniTask Upgrade(CancellationToken cancellationToken)
        {
            BuffManager.Ins.IsGlobalProfitX3 = true;
            IsUpgraded = true;
            userData.AddCoin(-Cost);
            await UniTask.CompletedTask;
        }
    }
}

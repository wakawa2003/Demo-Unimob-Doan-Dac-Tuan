using System.Threading;
using Cysharp.Threading.Tasks;
using EasyDI;
using UnityEngine;

namespace MyGameNamespace
{
    public interface IUpgradeStrategy
    {
        public Sprite Avatar { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long Cost { get; set; }
        public UniTask Upgrade(CancellationToken cancellationToken);
        public bool IsCanBuy();

    }
}
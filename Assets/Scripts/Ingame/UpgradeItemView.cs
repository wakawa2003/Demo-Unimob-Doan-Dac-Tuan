using System;
using System.Threading;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MyGameNamespace
{
    public class UpgradeItemView : MonoBehaviour
    {
        [SerializeField] private TMP_Text txtName, txtDetail, txtCost;
        [SerializeField] private Image imgAvatar;
        [SerializeField] private Button btnBuy;
        IUpgradeStrategy upgradeStrategy;

        public void Setup(IUpgradeStrategy upgradeStrategy, Action onBuy)
        {
            if (!upgradeStrategy.IsCanBuy())
                Destroy(gameObject);
            else
            {
                this.upgradeStrategy = upgradeStrategy;
                txtName.text = upgradeStrategy.Name;
                txtDetail.text = upgradeStrategy.Description;
                txtCost.text = GameUtils.FormatNumber(upgradeStrategy.Cost);
                imgAvatar.sprite = upgradeStrategy.Avatar;
                btnBuy.interactable = upgradeStrategy.IsCanBuy();
                btnBuy.OnClickAsObservable().Take(1).Subscribe(delegate
                {
                    upgradeStrategy.Upgrade(CancellationToken.None);
                    Setup(upgradeStrategy, onBuy);
                    onBuy?.Invoke();
                });
            }
        }
    }
}

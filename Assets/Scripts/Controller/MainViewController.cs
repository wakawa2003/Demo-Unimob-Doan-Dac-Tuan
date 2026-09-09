using System;
using EasyDI;
using TMPro;
using UnityEngine;

namespace MyGameNamespace
{
    public class MainViewController : MonoBehaviour
    {

        [Inject] IUserData userData;
        [SerializeField] private TMP_Text txtCoin, txtDiamond;

        private void OnDestroy()
        {
            userData.OnCoinChange.RemoveListener(OnCoinChange);
            userData.OnDiamondChange.RemoveListener(OnDiamondChange);
        }

        void Awake()
        {
            userData.OnCoinChange.AddListener(OnCoinChange);
            userData.OnDiamondChange.AddListener(OnDiamondChange);
        }

        void Start()
        {
            OnCoinChange(new(userData.Coin, userData.Coin));
            OnDiamondChange(new(userData.Diamond, userData.Diamond));
        }

        private void OnDiamondChange((int Before, int after) arg0)
        {
            txtDiamond.text = GameUtils.FormatNumber(arg0.after);
        }

        private void OnCoinChange((int Before, int after) arg0)
        {
            txtCoin.text = GameUtils.FormatNumber(arg0.after);
        }
    }
}

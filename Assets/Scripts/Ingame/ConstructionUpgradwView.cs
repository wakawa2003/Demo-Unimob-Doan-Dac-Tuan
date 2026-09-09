using System;
using System.ComponentModel;
using System.Linq;
using DG.Tweening;
using EasyDI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MyGameNamespace
{
    public class ConstructionUpgradwView : MonoBehaviour
    {
        [Header("UPGRADE VIEW")]
        [SerializeField] private GameObject container;
        [SerializeField] private TMP_Text txtLevel;
        [SerializeField] private TMP_Text txtProductName;
        [SerializeField] private TMP_Text txtCostToUpgrade;
        [SerializeField] private TMP_Text txtReward;
        [SerializeField] private TMP_Text txtDuration;
        [SerializeField] private Slider sliderLevel;
        [SerializeField] private Button btnUpgrade;
        [SerializeField] private GameObject maxUpgrade;
        [Inject] IUserData userData;
        IPlant plant;
        Tween tweenSlider;

        void Awake()
        {
            btnUpgrade.onClick.AddListener(OnclikUpgrade);
        }
        private void OnDestroy()
        {
            btnUpgrade.onClick.RemoveListener(OnclikUpgrade);
        }

        private void OnclikUpgrade()
        {
            var l = plant.LevelPlants;
            var cost = l[plant.Level].CostToUpgrade;
            if (plant.Level < l.Count)
                if (userData.Coin >= cost)
                {
                    plant.SetLevel(l[plant.Level]);
                    userData.AddCoin(-cost);
                    UpdateView(plant);
                }
        }

        public void UpdateView(IPlant plant)
        {
            this.plant = plant;
            txtLevel.text = "Level " + plant.Level;
            var l = plant.LevelPlants;
            sliderLevel.maxValue = l.Count();

            tweenSlider?.Kill();
            tweenSlider = sliderLevel.DOValue(plant.Level, 0.3f);

            if (plant.Level < l.Count)
            {
                txtCostToUpgrade.text = GameUtils.FormatNumber(l[plant.Level].CostToUpgrade);
            }
            txtReward.text = GameUtils.FormatNumber(plant.CoinEarn);
            txtDuration.text = plant.Duration.ToString() + "s";
            btnUpgrade.gameObject.SetActive(plant.Level < l.Count);
            maxUpgrade.gameObject.SetActive(plant.Level >= l.Count);

            container.transform.position = Camera.main.WorldToScreenPoint(plant.transform.position);
        }
    }
}

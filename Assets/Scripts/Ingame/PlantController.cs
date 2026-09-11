using System;
using System.Collections.Generic;
using AYellowpaper;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MyGameNamespace
{
    public class PlantController : MonoBehaviour, IPlant, IPointerClickHandler, ICarrier
    {
        [Header("Config")]
        [SerializeField] private List<InterfaceReference<ILevelPlant>> levelConfigList;

        [Header("Carry Settings")]
        [SerializeField] private GameObject carryable;
        [SerializeField] private Transform[] carryPositionList;

        [Header("STATS")]
        [SerializeField] private TMP_Text txtCoinEarn;
        [SerializeField] private TMP_Text txtDuration;
        [SerializeField] private TMP_Text txtSpeedPerMinute;
        [SerializeField] private Image imgIcon;
        [SerializeField] private GameObject viewStats;
        [SerializeField] private ConstructionUpgradwView constructionUpgradwView;


        public ILevelPlant LevelPlantStrategy { get; set; }
        [field: SerializeField] public int Level { get; set; }
        [field: SerializeField] public int CoinEarn { get; set; }
        [field: SerializeField] public float Duration { get; set; }
        [field: SerializeField] public string ResourcesID { get; set; } = "resource_1";
        public List<ILevelPlant> LevelPlants { get => levelConfigList.ConvertAll(_ => _.Value); }
        public ICarryable Carryable => carryable.GetComponent<ICarryable>();

        void Awake()
        {
            constructionUpgradwView.gameObject.SetActive(false);
            SetLevel(levelConfigList[0].Value);

            viewStats.gameObject.SetActive(true);
            viewStats.transform.localScale = Vector3.zero;
            viewStats.transform.DOScale(Vector3.one, 0.3f).SetDelay(0.5f).SetEase(Ease.OutBack);
        }

        void Start()
        {
            Carryable.SetSpawn(carryPositionList);
        }

        void LateUpdate()
        {
            viewStats.transform.position = Camera.main.WorldToScreenPoint(transform.position);
        }

        public void SetLevel(ILevelPlant levelPlant)
        {
            LevelPlantStrategy = levelPlant;
            LevelPlantStrategy.ApplyStrategy(this);
            txtCoinEarn.text = GameUtils.FormatNumber(CoinEarn);
            txtDuration.text = Duration.ToString() + "s";
            txtSpeedPerMinute.text = GameUtils.FormatNumber((int)(CoinEarn * 60 / Duration)) + "/min";
            var c = GameConfig.Ins.GetResourceConfig(ResourcesID);
            imgIcon.sprite = c.Avatar;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            constructionUpgradwView.gameObject.SetActive(true);
            constructionUpgradwView.UpdateView(this);
        }
    }
}

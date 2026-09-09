using System.Collections.Generic;
using System.Threading.Tasks;
using AYellowpaper;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MyGameNamespace
{
    public class PlantController : MonoBehaviour, IPlant, IPointerClickHandler
    {
        [Header("Config")]
        [SerializeField] private List<InterfaceReference<ILevelPlant>> levelConfigList;

        [Header("STATS")]
        [SerializeField] private TMP_Text txtCoinEarn;
        [SerializeField] private TMP_Text txtDuration;
        [SerializeField] private TMP_Text txtSpeedPerMinute;
        [SerializeField] private GameObject viewStats;
        [SerializeField] private ConstructionUpgradwView constructionUpgradwView;


        public ILevelPlant LevelPlantStrategy { get; set; }
        [field: SerializeField] public int Level { get; set; }
        [field: SerializeField] public int CoinEarn { get; set; }
        [field: SerializeField] public float Duration { get; set; }
        public List<ILevelPlant> LevelPlants { get => levelConfigList.ConvertAll(_ => _.Value); }

        async void Awake()
        {
            constructionUpgradwView.gameObject.SetActive(false);
            viewStats.gameObject.SetActive(false);
            viewStats.transform.localScale = Vector3.zero;
            SetLevel(levelConfigList[0].Value);

            await UniTask.WaitForSeconds(0.5f);

            viewStats.gameObject.SetActive(true);
            viewStats.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
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
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            constructionUpgradwView.gameObject.SetActive(true);
            constructionUpgradwView.UpdateView(this);
        }
    }
}

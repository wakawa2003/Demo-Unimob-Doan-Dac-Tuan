using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace MyGameNamespace
{
    public class BoxController : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private GameObject UIContainer;
        [SerializeField] private GameObject UIPivot;
        [SerializeField] private TMP_Text txtName;
        [SerializeField] private TMP_Text txtCostToBuild;

        public string Name;
        public int CostToUnBox = 40;

        void Awake()
        {
            UIContainer.gameObject.SetActive(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log($"click");

            // Bật UI
            UIContainer.gameObject.SetActive(true);

            // Đặt vị trí UI theo world
            UIPivot.transform.position = Camera.main.WorldToScreenPoint(transform.position) + Vector3.up * 2;

            // Gán text
            txtName.text = Name;
            txtCostToBuild.text = GameUtils.FormatNumber(CostToUnBox).ToString();

            // Reset scale trước khi anim
            UIContainer.transform.localScale = Vector3.zero;

            // Tạo sequence cho cả UI và object
            Sequence seq = DOTween.Sequence();

            // Anim UI scale bật lên
            seq.Join(UIContainer.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack));

            // Anim object 3D (ví dụ rung nhẹ khi click)
            seq.Join(transform.DOShakeScale(0.3f, strength: 0.6f, vibrato: 10, randomness: 90));

            // Có thể thêm hiệu ứng xoay hoặc nhảy lên
            // seq.Join(transform.DOPunchPosition(Vector3.up * 0.5f, 0.3f, 10, 1));

            seq.Play();
        }
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UniState;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.UI;
using EasyDI;
using Cysharp.Threading.Tasks.Linq;

namespace MyGameNamespace
{
    public class BoxController : MonoBehaviour, IPointerClickHandler
    {

        [SerializeField] private Animation animation;
        [SerializeField] private GameObject graphicObject;
        [SerializeField] private GameObject plantObject;

        [Header("Idle")]
        [SerializeField] private GameObject UIContainer;
        [SerializeField] private GameObject UIPivot;
        [SerializeField] private TMP_Text txtName;
        [SerializeField] private TMP_Text txtCostToBuild;
        [SerializeField] private Button btnUnlock;

        [Header("Building")]
        [SerializeField] private float TimeToBuild = 5;
        [SerializeField] private GameObject containerBuildingView;
        [SerializeField] private Slider sliderBuilding;
        [SerializeField] private TMP_Text txtProgress;
        [SerializeField] private GameObject plantBuildCompleteFX;

        IUserData userData;
        public string Name;
        public int CostToUnBox = 40;
        public UnityEvent<PointerEventData> OnPointerClick;
        private IStateMachine _stateMachine;

        [Inject]
        void inject(IUserData userData)
        {
            this.userData = userData;
        }

        async void Awake()
        {
            UIContainer.gameObject.SetActive(false);
            _stateMachine = new StateMachine();
            _stateMachine.SetResolver(new GameUtils.DefaultResolver());

            plantObject.gameObject.SetActive(false);
            // Đăng ký các state
            await _stateMachine.Execute<IdleState, BoxController>(this, destroyCancellationToken);
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            OnPointerClick?.Invoke(eventData);
        }

        public class IdleState : StateBase<BoxController>
        {
            public BoxController boxController;

            public override UniTask Initialize(CancellationToken token)
            {
                Debug.Log($"IdleState");
                boxController = Payload;
                boxController.OnPointerClick.AddListener(OnPointerClick);
                return base.Initialize(token);
            }

            public override UniTask Exit(CancellationToken token)
            {
                boxController.OnPointerClick.RemoveListener(OnPointerClick);
                boxController.UIContainer.gameObject.SetActive(false);
                return base.Exit(token);
            }

            public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
            {
                Debug.Log($"IdleState execute");
                await boxController.btnUnlock.OnClickAsAsyncEnumerable().Where(_ => boxController.CostToUnBox <= boxController.userData.Coin).FirstAsync(cancellationToken: token);
                boxController.userData.AddCoin(-boxController.CostToUnBox);//tru coin sau khi mua
                Sequence seq = DOTween.Sequence();

                // Anim object 3D 
                seq.Join(boxController.graphicObject.transform.DOShakeScale(0.3f, strength: 0.6f, vibrato: 10, randomness: 90));

                return Transition.GoTo<BuildingState, BoxController>(Payload);
            }

            public void OnPointerClick(PointerEventData eventData)
            {
                Debug.Log($"click");

                // Bật UI
                boxController.UIContainer.gameObject.SetActive(true);

                // Đặt vị trí UI theo world
                boxController.UIPivot.transform.position = Camera.main.WorldToScreenPoint(boxController.transform.position) + Vector3.up * 2;

                // Gán text
                boxController.txtName.text = boxController.Name;
                boxController.txtCostToBuild.text = GameUtils.FormatNumber(boxController.CostToUnBox).ToString();

                // Reset scale trước khi anim
                boxController.UIContainer.transform.localScale = Vector3.zero;

                // Tạo sequence cho cả UI và object
                Sequence seq = DOTween.Sequence();

                // Anim UI scale bật lên
                seq.Join(boxController.UIContainer.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack));

                // Anim object 3D (ví dụ rung nhẹ khi click)
                seq.Join(boxController.graphicObject.transform.DOShakeScale(0.3f, strength: 0.6f, vibrato: 10, randomness: 90));

                // Có thể thêm hiệu ứng xoay hoặc nhảy lên
                // seq.Join(transform.DOPunchPosition(Vector3.up * 0.5f, 0.3f, 10, 1));

                seq.Play();
            }
        }

        public class BuildingState : StateBase<BoxController>
        {
            public BoxController boxController;
            Tween tweenBuild;

            public override UniTask Initialize(CancellationToken token)
            {

                boxController = Payload;
                boxController.plantObject.transform.localScale = Vector3.zero;
                return base.Initialize(token);
            }

            public override async UniTask Exit(CancellationToken token)
            {
                boxController.plantBuildCompleteFX.gameObject.SetActive(true);
                boxController.plantObject.gameObject.SetActive(true);
                boxController.containerBuildingView.gameObject.SetActive(false);
                //anim 
                Sequence seq = DOTween.Sequence();
                seq.Join(boxController.graphicObject.transform.DOScale(0f, 0.3f)).SetEase(Ease.InBack);
                seq.Join(boxController.plantObject.transform.DOScale(1f, 0.5f)).SetEase(Ease.OutBack).SetDelay(0.2f);
                await seq.AsyncWaitForCompletion().AsUniTask().AttachExternalCancellation(token);

                await UniTask.WaitForSeconds(1);
                Destroy(boxController.gameObject);
                await base.Exit(token);
            }

            public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
            {
                Debug.Log($"BuildingState");
                boxController.animation.Play("BoxOpen", PlayMode.StopAll);
                boxController.containerBuildingView.gameObject.SetActive(true);

                boxController.sliderBuilding.maxValue = boxController.TimeToBuild;

                tweenBuild?.Kill();
                tweenBuild = DOVirtual.Float(0, boxController.TimeToBuild, boxController.TimeToBuild, t =>
                {
                    float remainingTime = boxController.TimeToBuild - t;
                    boxController.txtProgress.text = remainingTime.ToString("F1") + "s";

                    boxController.sliderBuilding.value = t;
                    boxController.sliderBuilding.transform.position = Camera.main.WorldToScreenPoint(boxController.transform.position) + Vector3.up * 2;
                }).SetEase(Ease.Linear);

                await tweenBuild.AsyncWaitForCompletion().AsUniTask().AttachExternalCancellation(token);

                return await UniTask.FromResult(Transition.GoToExit());

            }
        }

    }
}

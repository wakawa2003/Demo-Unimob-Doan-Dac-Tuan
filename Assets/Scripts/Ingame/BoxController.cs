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
        [SerializeField] private GameObject UIContainer;
        [SerializeField] private GameObject UIPivot;
        [SerializeField] private TMP_Text txtName;
        [SerializeField] private TMP_Text txtCostToBuild;
        [SerializeField] private Button btnUnlock;

        IUserData userData;
        public string Name;
        public int CostToUnBox = 40;
        public UnityEvent<PointerEventData> OnPointerClick;
        private IStateMachine _stateMachine;

        [Inject]
        void inject(IUserData userData)
        {
            this.userData = userData;
            Debug.Log($"userData:{userData.Coin}");
        }

        async void Awake()
        {
            UIContainer.gameObject.SetActive(false);
            _stateMachine = new StateMachine();
            _stateMachine.SetResolver(new GameUtils.DefaultResolver());

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
                return base.Exit(token);
            }

            public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
            {
                Debug.Log($"IdleState execute");
                await boxController.btnUnlock.OnClickAsAsyncEnumerable().Where(_ => boxController.CostToUnBox <= boxController.userData.Coin).FirstAsync(cancellationToken: token);
                boxController.userData.AddCoin(-boxController.CostToUnBox);//tru coin sau khi mua
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
                seq.Join(boxController.transform.DOShakeScale(0.3f, strength: 0.6f, vibrato: 10, randomness: 90));

                // Có thể thêm hiệu ứng xoay hoặc nhảy lên
                // seq.Join(transform.DOPunchPosition(Vector3.up * 0.5f, 0.3f, 10, 1));

                seq.Play();
            }
        }

        public class BuildingState : StateBase<BoxController>
        {
            public BoxController boxController;

            public override UniTask Initialize(CancellationToken token)
            {
                boxController = Payload;
                return base.Initialize(token);
            }

            public override UniTask Exit(CancellationToken token)
            {
                return base.Exit(token);
            }

            public override UniTask<StateTransitionInfo> Execute(CancellationToken token)
            {
                Debug.Log($"BuildingState");
                Destroy(boxController.gameObject);

                return UniTask.FromResult(Transition.GoToExit());

            }
        }

    }
}

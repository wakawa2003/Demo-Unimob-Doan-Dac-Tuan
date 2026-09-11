using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using UniState;
using UnityEngine;
using UnityEngine.AI;

namespace MyGameNamespace
{
    public class DeliverController : MonoBehaviour, IDeliver, ICarrier
    {

        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Transform[] listCarryablePosition;
        [SerializeField] private Animator animator;
        public ICarryable Carryable { get; set; }
        IStateMachine stateMachine = new StateMachine();
        InitState initState;


        void Awake()
        {
            stateMachine.SetResolver(new GameUtils.DefaultResolver());
            stateMachine.Execute<InitState, DeliverController>(this, destroyCancellationToken);
        }

        // void Update()
        // {
        //     // ví dụ: nhấn chuột để di chuyển đến vị trí click
        //     if (Input.GetMouseButtonDown(0))
        //     {
        //         Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //         if (Physics.Raycast(ray, out RaycastHit hit))
        //         {
        //             // agent.SetDestination(hit.point);
        //             RunToPosition(hit.point, Vector3.forward, destroyCancellationToken).Forget();
        //         }
        //     }

        // }


        public async UniTask TakeCarryable(ICarryable carryable, CancellationToken cancellationToken) => initState?.TakeCarryable(carryable, cancellationToken);

        public async UniTask RunToPosition(Vector3 target, Vector3 rotationAtEnd, CancellationToken cancellationToken)
        {
            if (agent.SetDestination(target))
            {
                animator.SetBool("IsMove", true);


            }

            await Observable.EveryUpdate().FirstAsync(_ =>
            {
                // transform.LookAt(agent.nextPosition);
                var posA = new Vector2(transform.position.x, transform.position.z);
                var posB = new Vector2(target.x, target.z);
                return Vector2.Distance(posA, posB) <= 0.1f;
            }, cancellationToken);
            await transform.DORotate(rotationAtEnd, 0.2f).AsyncWaitForCompletion().AsUniTask().AttachExternalCancellation(cancellationToken);
            agent.isStopped = true;
            animator.SetBool("IsMove", false);
            Debug.Log($"chay toi noi");
        }

        public void TakeOffCarryable()
        {
            Debug.Log($"TakeOffCarryable", this);
            Carryable = null;
        }


        /// <summary>
        /// luc chua nhan hang
        /// </summary>
        public class InitState : StateBase<DeliverController>
        {
            public override UniTask Initialize(CancellationToken token)
            {
                Payload.initState = this;



                Debug.Log($"InitState", Payload);
                return base.Initialize(token);
            }

            public override UniTask Exit(CancellationToken token)
            {
                Payload.initState = null;
                return base.Exit(token);
            }
            public async override UniTask<StateTransitionInfo> Execute(CancellationToken token)
            {
                await Observable.EveryUpdate().FirstAsync(_ => Payload.Carryable != null);
                return await UniTask.FromResult(Transition.GoTo<DeliveringState, DeliverController>(Payload));
            }

            public async UniTask TakeCarryable(ICarryable carryable, CancellationToken cancellationToken)
            {
                if (Payload.Carryable == null)
                {
                    Payload.Carryable = carryable;
                    await carryable.SetOwner(Payload.listCarryablePosition, Payload, cancellationToken);
                }
            }
        }


        /// <summary>
        /// sau khi da nhan dc hang
        /// </summary>
        public class DeliveringState : StateBase<DeliverController>
        {
            public override UniTask Initialize(CancellationToken token)
            {
                Debug.Log($"DeliveringState", Payload);
                Payload.animator.SetBool("IsCarryMove", true);
                return base.Initialize(token);
            }


            public override UniTask Exit(CancellationToken token)
            {
                Payload.animator.SetBool("IsCarryMove", false);
                return base.Exit(token);
            }
            public async override UniTask<StateTransitionInfo> Execute(CancellationToken token)
            {
                return await UniTask.FromResult(Transition.GoToExit());
            }
        }
    }
}

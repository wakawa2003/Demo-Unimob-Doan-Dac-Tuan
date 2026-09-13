using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using UniState;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace MyGameNamespace
{
    public class CustomerController : MonoBehaviour, ICustomer
    {
        [SerializeField] private Animator animator;
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Transform[] listCarryablePosition;

        IdleState idleState;
        WaitingState waitingState;
        EndState endState;
        bool isCarry;
        bool isEmpty;

        IStateMachine stateMachine;
        public ICarryable Carryable { get; set; }

        public UnityEvent<Vector3> onGotoPosition = new();
        private Vector3 endPos;

        void Awake()
        {
            stateMachine = new StateMachine();
            stateMachine.SetResolver(new GameUtils.DefaultResolver());
            stateMachine.Execute<IdleState, CustomerController>(this, destroyCancellationToken);
        }

        public void SetPosition(Vector3 target, Vector3 rotation)
        {
            transform.SetPositionAndRotation(target, Quaternion.Euler(rotation));

            onGotoPosition?.Invoke(target);
        }

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
            animator.SetBool("IsMove", false);
            await transform.DORotate(rotationAtEnd, 0.2f).AsyncWaitForCompletion().AsUniTask().AttachExternalCancellation(cancellationToken);
            onGotoPosition?.Invoke(target);
            Debug.Log($"chay toi noi");
        }

        public void TakeOffCarryable()
        {
            throw new NotImplementedException();
        }

        public UniTask TakeCarryable(ICarrier fromCarrier, ICarryable carryable, CancellationToken cancellationToken)
        {
            return waitingState == null ? UniTask.CompletedTask : waitingState.TakeCarryable(fromCarrier, carryable, cancellationToken);
        }

        public void Setup(Vector3 endPos)
        {
            this.endPos = endPos;
        }


        [System.Serializable]
        public class IdleState : StateBase<CustomerController>
        {

            public override UniTask Initialize(CancellationToken token)
            {
                Payload.idleState = this;

                EventDispatcher.OnCustomerInit?.Invoke(Payload);
                return base.Initialize(token);
            }


            public override UniTask Exit(CancellationToken token)
            {
                Payload.idleState = null;
                return base.Exit(token);
            }

            public async override UniTask<StateTransitionInfo> Execute(CancellationToken token)
            {
                await Payload.onGotoPosition.AsObservable().FirstAsync(token);
                return Transition.GoTo<WaitingState, CustomerController>(Payload);

            }


        }

        [System.Serializable]
        public class WaitingState : StateBase<CustomerController>
        {

            public override UniTask Initialize(CancellationToken token)
            {
                Debug.Log($"customer waiting", Payload);
                Payload.waitingState = this;
                EventDispatcher.OnCustomerReadyToTake?.Invoke(Payload);
                return base.Initialize(token);
            }

            public override UniTask Exit(CancellationToken token)
            {
                Payload.waitingState = null;
                return base.Exit(token);
            }

            public async override UniTask<StateTransitionInfo> Execute(CancellationToken token)
            {
                await Observable.EveryUpdate().FirstAsync(_ => Payload.Carryable != null, token);
                return Transition.GoTo<EndState, CustomerController>(Payload);
            }

            public async UniTask TakeCarryable(ICarrier fromCarrier, ICarryable carryable, CancellationToken cancellationToken)
            {
                if (Payload.Carryable == null)
                {
                    await carryable.SetOwner(Payload.listCarryablePosition, Payload, cancellationToken);
                    Payload.Carryable = carryable;
                    EventDispatcher.onCustomerTakeCarryable?.Invoke(new(Payload, fromCarrier, carryable));
                }
            }
        }
        [System.Serializable]
        public class EndState : StateBase<CustomerController>
        {

            public override UniTask Initialize(CancellationToken token)
            {
                Payload.endState = this;
                return base.Initialize(token);
            }

            public override UniTask Exit(CancellationToken token)
            {
                Payload.endState = null;
                return base.Exit(token);
            }

            public async override UniTask<StateTransitionInfo> Execute(CancellationToken token)
            {
                await Payload.RunToPosition(Payload.endPos, Vector3.forward, token);
                Destroy(Payload.gameObject);
                Debug.Log($"Customer hoan thanh!!!!!");
                return Transition.GoToExit();

            }
        }
    }
}

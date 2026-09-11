using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniState;
using UnityEngine;

namespace MyGameNamespace
{
    public class TomatoController : MonoBehaviour, ICarryable
    {

        [SerializeField] private GameObject[] bodyObjectList;
        IStateMachine stateMachine = new StateMachine();
        IdleState idleState;
        CarryingState carryingState;

        void Awake()
        {

            stateMachine.SetResolver(new GameUtils.DefaultResolver());
            stateMachine.Execute<IdleState, TomatoController>(this, destroyCancellationToken);
        }

        public void SetSpawn(Transform[] posList) => idleState.SetSpawn(posList);
        public async UniTask SetOwner(Transform[] posList, ICarrier owner, CancellationToken cancellationToken) => await (idleState == null ? UniTask.CompletedTask : idleState.SetOwner(posList, owner, cancellationToken));

        public class IdleState : StateBase<TomatoController>
        {

            Sequence animMove;
            public override UniTask Initialize(CancellationToken token)
            {
                Debug.Log($"Initialize IdleState");
                Payload.idleState = this;
                return base.Initialize(token);
            }

            public override UniTask Exit(CancellationToken token)
            {
                Debug.Log($"Exit IdleState");
                Payload.idleState = null;
                return base.Exit(token);
            }

            public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
            {
                while (true)
                {
                    await UniTask.NextFrame();
                }

                return await UniTask.FromResult(Transition.GoBack());
            }

            public void SetSpawn(Transform[] posList)
            {
                Debug.Log($"SetSpawn");
                for (int i = 0; i < posList.Length; i++)
                {
                    var item = posList[i];

                    Payload.bodyObjectList[i].transform.SetParent(item, false);
                    Payload.bodyObjectList[i].transform.SetPositionAndRotation(item.position, item.rotation);
                    Payload.bodyObjectList[i].transform.localPosition = Vector3.zero;
                }
            }

            public async UniTask SetOwner(Transform[] listPos, ICarrier owner, CancellationToken cancellationToken)
            {

                Debug.Log($"OnSetOwner");
                animMove = DOTween.Sequence();
                for (int i = 0; i < listPos.Length; i++)
                {
                    animMove.Join(Payload.bodyObjectList[i].transform.DOMove(listPos[i].transform.position, 0.3f));
                    Payload.bodyObjectList[i].transform.SetParent(listPos[i]);
                }
                Payload.transform.SetParent(listPos.First());

                await animMove.AsyncWaitForCompletion().AsUniTask().AttachExternalCancellation(cancellationToken);
            }
        }
        public class CarryingState : StateBase<TomatoController>
        {

            public override UniTask Initialize(CancellationToken token)
            {
                Payload.carryingState = this;
                return base.Initialize(token);
            }

            public override UniTask Exit(CancellationToken token)
            {
                Payload.carryingState = null;
                return base.Exit(token);
            }
            public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
            {
                return await UniTask.FromResult(Transition.GoBack());
            }


        }


    }
}

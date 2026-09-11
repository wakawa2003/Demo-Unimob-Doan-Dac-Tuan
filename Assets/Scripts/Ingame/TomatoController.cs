using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using UniState;
using UnityEngine;
using UnityEngine.Events;

namespace MyGameNamespace
{
    public class TomatoController : MonoBehaviour, ICarryable
    {

        [SerializeField] private GameObject[] bodyObjectList;
        IStateMachine stateMachine = new StateMachine();
        UnityEvent<Transform[]> OnSetSpawn;
        UnityEvent<Transform[], ICarrier> OnSetOwner;

        void Awake()
        {
            OnSetSpawn = new();
            OnSetOwner = new();

            stateMachine.SetResolver(new GameUtils.DefaultResolver());
            stateMachine.Execute<IdleState, TomatoController>(this, destroyCancellationToken);
        }

        public void SetSpawn(Transform[] posList) => OnSetSpawn?.Invoke(posList);
        public void SetOwner(Transform[] posList, ICarrier owner) => OnSetOwner?.Invoke(posList, owner);

        public class IdleState : StateBase<TomatoController>
        {

            Sequence animMove;
            public override UniTask Initialize(CancellationToken token)
            {
                Debug.Log($"Initialize");
                Payload.OnSetSpawn.AddListener(SetSpawn);
                return base.Initialize(token);
            }

            public override UniTask Exit(CancellationToken token)
            {

                Payload.OnSetSpawn.RemoveListener(SetSpawn);
                return base.Exit(token);
            }

            public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
            {
                var a = await Payload.OnSetOwner.AsObservable().FirstAsync();
                Debug.Log($"OnSetOwner");
                animMove = DOTween.Sequence();
                var listPos = a.Arg0;
                for (int i = 0; i < listPos.Length; i++)
                {
                    animMove.Join(Payload.bodyObjectList[i].transform.DOMove(listPos[i].transform.position, 0.3f));
                }
                return await UniTask.FromResult(Transition.GoBack());
            }

            void SetSpawn(Transform[] posList)
            {
                for (int i = 0; i < posList.Length; i++)
                {
                    var item = posList[i];

                    Payload.bodyObjectList[i].transform.SetParent(item, false);
                    Payload.bodyObjectList[i].transform.SetPositionAndRotation(item.position, item.rotation);
                    Payload.bodyObjectList[i].transform.localPosition = Vector3.zero;
                }
            }

        }
        public class CarryingState : StateBase<TomatoController>
        {
            public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
            {
                return await UniTask.FromResult(Transition.GoBack());
            }


        }


    }
}

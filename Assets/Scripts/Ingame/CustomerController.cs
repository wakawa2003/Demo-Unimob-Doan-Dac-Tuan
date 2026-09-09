using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace MyGameNamespace
{
    public class CustomerController : MonoBehaviour, ICustomer
    {
        [SerializeField] private Animator Animator;

        Sequence tweenMove;
        CancellationTokenSource moveCts;
        bool isCarry;
        bool isEmpty;


        public async UniTask MoveTotarget(Transform transformTarget, float time, CancellationToken cancellationToken)
        {
            Animator.SetBool("IsMove", true);
            Animator.SetBool("IsCarryMove", isCarry);
            Animator.SetBool("IsEmpty", isEmpty);


            // Hủy task cũ
            moveCts?.Cancel();
            moveCts?.Dispose();

            moveCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            tweenMove?.Kill();
            tweenMove = DOTween.Sequence();
            tweenMove.Append(transform.DOMove(transformTarget.position, time));
            tweenMove.Append(transform.DORotate(transformTarget.eulerAngles, time == 0 ? 0 : 0.1f));
            tweenMove.Play();

            try
            {
                await tweenMove.AsyncWaitForCompletion()
                               .AsUniTask()
                               .AttachExternalCancellation(moveCts.Token);
            }
            catch (OperationCanceledException)
            {
                tweenMove?.Kill();
            }
            Animator.SetBool("IsMove", false);

        }
    }
}

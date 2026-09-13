using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniState;
using UnityEngine;

namespace MyGameNamespace
{
    public class TomatoController : MonoBehaviour, ICarryable, ICostable
    {

        [SerializeField] private GameObject[] bodyObjectList;
        public ICarrier Owner { get; set; }
        public int Cost { get; set; }
        Sequence animMove;

        public void Setup(int cost)
        {
            Cost = cost;
            animMove = DOTween.Sequence();
            for (int i = 0; i < bodyObjectList.Count(); i++)
            {
                var initScale = bodyObjectList[i].transform.localScale;
                var initLocalPos = bodyObjectList[i].transform.localPosition;

                bodyObjectList[i].transform.localPosition += Vector3.up * 0.7f;
                bodyObjectList[i].transform.localScale = Vector3.zero;
                animMove.Join(bodyObjectList[i].transform.DOLocalMove(initLocalPos, 0.3f));
                animMove.Join(bodyObjectList[i].transform.DOScale(initScale, 0.7f).SetDelay((i) * 0.1f).SetEase(Ease.OutBack));
            }
        }
        public void Setup(ICarrier owner, Transform[] posList)
        {
            // Debug.Log($"SetSpawn");
            Owner = owner;
            for (int i = 0; i < posList.Length; i++)
            {
                var item = posList[i];

                bodyObjectList[i].transform.SetParent(item, false);
                bodyObjectList[i].transform.SetPositionAndRotation(item.position, item.rotation);
                bodyObjectList[i].transform.localPosition = Vector3.zero;
            }
        }

        public async UniTask SetOwner(Transform[] listPos, ICarrier newOwner, CancellationToken cancellationToken)
        {

            Debug.Log($"OnSetOwner");
            if (Owner != null)
                Owner.TakeOffCarryableByAnother(newOwner);
            Owner = newOwner;
            animMove?.Kill(true);
            animMove = DOTween.Sequence();
            for (int i = 0; i < listPos.Length; i++)
            {
                animMove.Join(bodyObjectList[i].transform.DOShakeScale(0.2f, new Vector3(0.2f, 0.4f, 0.2f), 1).SetDelay(i * 0.1f));
                animMove.Join(bodyObjectList[i].transform.DOJump(listPos[i].transform.position, 0.6f, 1, 0.5f).SetDelay((i + 1) * 0.1f));

                // animMove.Join(Payload.bodyObjectList[i].transform.DOMove(listPos[i].transform.position, 0.3f));
                // animMove.Join(Payload.bodyObjectList[i].transform.DOMove(listPos[i].transform.position + Vector3.up * 5, 0.15f).SetLoops(1, LoopType.Yoyo)); 
                bodyObjectList[i].transform.SetParent(listPos[i]);
            }
            transform.SetParent(listPos.First());

            await animMove.AsyncWaitForCompletion().AsUniTask().AttachExternalCancellation(cancellationToken);
        }

    }
}

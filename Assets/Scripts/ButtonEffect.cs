using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace TuanTool
{
    [DisallowMultipleComponent]
    public class ButtonEffect : MonoBehaviour
    {
        [SerializeField] private float strength = 0.2f;
        [SerializeField] private float duration = 0.2f;
        [SerializeField] private Button button;
        Tween tween;

        private void Reset()
        {
            button = GetComponent<Button>();
        }

        private void Awake()
        {
            button.onClick.AddListener(onCLick);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(onCLick);
        }

        private void onCLick()
        {
            tween?.Kill(true);
            tween = transform.DOPunchScale(Vector3.one * strength, duration).SetUpdate(true);
            // SoundController.Ins.PlayOnClickBtn();
        }

    }
}

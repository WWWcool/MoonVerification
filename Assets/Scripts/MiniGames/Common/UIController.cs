using DG.Tweening;
using Moon.Asyncs;
using UnityEngine;
using UnityEngine.UI;

namespace MiniGames.Common
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private Image _bgLayer = null;
        [SerializeField] private Image _progressBarImage = null;
        [SerializeField] private GameObject _progressBar = null;
        [SerializeField] private Vector3 _barTargetPosition;
        [SerializeField] private float _bgAlpha = 0.5f;
        [SerializeField] private float _animTime = 0.5f;
        [SerializeField] private float _barAnimTime = 0.5f;

        private Vector3 _barOriginPosition;
        private int _maxCount = 1;
        private bool _tweenRunning = false;

        public AsyncState ResetProgress(int count)
        {
            _maxCount = count;
            return Planner.Chain()
                    .AddTween(SetValueTo, 0f)
                ;
        }

        public AsyncState IncrementProgress(int count)
        {
            return Planner.Chain()
                    .AddTween(SetValueTo, (float)count / _maxCount)
                ;
        }

        public AsyncState ShowUI()
        {
            _barOriginPosition = _progressBar.transform.localPosition;
            return Planner.Chain()
                    .AddTween(FadeTo, 0f)
                    .AddTween(MoveTo, _progressBar.transform, _barTargetPosition)
                    .AddAwait(AwaitTweenFinished)
                ;
        }

        public AsyncState HideUI()
        {
            return Planner.Chain()
                    .JoinTween(MoveTo, _progressBar.transform, _barOriginPosition)
                    .JoinTween(FadeTo, _bgAlpha)
                    .AddAwait(AwaitTweenFinished)
                ;
        }

        private Tween MoveTo(Transform transform, Vector3 movePosition)
        {
            _tweenRunning = true;
            var tween = transform
                        .DOLocalMove(movePosition, _animTime)
                        .SetEase(Ease.InOutSine);
            tween.onComplete += () => _tweenRunning = false;
            return tween;
        }

        private Tween FadeTo(float alpha)
        {
            _tweenRunning = true;
            var tween = _bgLayer.DOFade(alpha, _animTime);
            tween.onComplete += () => _tweenRunning = false;
            return tween;
        }

        public Tween SetValueTo(float value)
        {
            _tweenRunning = true;
            var tween = _progressBarImage
                    .DOFillAmount(value, _barAnimTime);
            tween.onComplete += () => _tweenRunning = false;
            return tween;
        }

        private void AwaitTweenFinished(AsyncStateInfo state)
        {
            state.IsComplete = !_tweenRunning;
        }
    }
}
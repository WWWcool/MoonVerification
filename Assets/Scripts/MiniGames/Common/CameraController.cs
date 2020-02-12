using DG.Tweening;
using Moon.Asyncs;
using UnityEngine;

namespace MiniGames.Common
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Vector3 _targetPosition;
        [SerializeField] private Vector3 _targetRotation;
        [SerializeField] private float _animTime = 0.3f;

        private Vector3 _originPosition;
        private Vector3 _originRotation;
        private bool _tweenRunning = false;

        public AsyncState MoveToBoard()
        {
            _originPosition = transform.localPosition;
            _originRotation = transform.localEulerAngles;

            return Planner.Chain()
                    .AddTimeout(1f)
                    .AddFunc(() => Planner.Chain()
                        .JoinTween(MoveTo, _targetPosition)
                        .JoinTween(RotateTo, _targetRotation)
                    )
                    .AddAwait(AwaitTweenFinished)
                ;
        }

        public AsyncState MoveBack()
        {
            return Planner.Chain()
                    .JoinTween(MoveTo, _originPosition)
                    .JoinTween(RotateTo, _originRotation)
                    .AddAwait(AwaitTweenFinished)
                ;
        }

        private Tween MoveTo(Vector3 movePosition)
        {
            _tweenRunning = true;
            var tween = transform
                        .DOLocalMove(movePosition, _animTime)
                        .SetEase(Ease.InOutSine);
            tween.onComplete += () => _tweenRunning = false;
            return tween;
        }

        private Tween RotateTo(Vector3 angles)
        {
            _tweenRunning = true;
            var tween = transform
                        .DOLocalRotateQuaternion(Quaternion.Euler(angles), _animTime)
                        .SetEase(Ease.OutExpo);
            tween.onComplete += () => _tweenRunning = false;
            return tween;
        }

        private void AwaitTweenFinished(AsyncStateInfo state)
        {
            state.IsComplete = !_tweenRunning;
        }
    }
}
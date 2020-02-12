using DG.Tweening;
using Moon.Asyncs;
using UnityEngine;
using System;

namespace MiniGames.Memory
{
    public class MemoryCard : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _faceTexture = null;
        [SerializeField] private Animator _animator = null;
        [SerializeField] private Vector3 _targetScale;
        [SerializeField] private float _animTime = 0.3f;

        public Texture texture => _faceTexture.material.mainTexture;

        private Vector3 _originRotation;
        private Vector3 _originScale;
        private Action<MemoryCard> _onCardClick;
        private bool _tryAnim = false;
        private bool _tweenRunning = false;

        public AsyncState Init(Texture2D texture, Action<MemoryCard> onCardClick)
        {
            _faceTexture.material.mainTexture = texture;
            _onCardClick = onCardClick;
            return Planner.Chain()
                    .AddAction(Debug.Log, "[MemoryCard][Init]")
                    .AddFunc(Appear, true)
                ;
        }

        public bool Match(MemoryCard card)
        {
            return texture == card.texture;
        }

        public AsyncState Appear(bool on)
        {
            if (_tryAnim)
            {
                return Planner.Chain()
                        .AddAction(_animator.SetTrigger, on ? "Appear" : "Disappear")
                        .AddAwait((AsyncStateInfo state) => state.IsComplete = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                    ;
            }
            else
            {
                if (on)
                {
                    return Planner.Chain()
                            .AddTween(Scale)
                            .AddAwait(AwaitTweenFinished)
                        ;
                }
                else
                {
                    return Planner.Chain()
                            .AddTween(ReScale)
                            .AddAwait(AwaitTweenFinished)
                        ;
                }
            }
        }

        public AsyncState Flip(bool on)
        {
            if (_tryAnim)
            {
                return Planner.Chain()
                        .AddAction(_animator.SetTrigger, on ? "Open" : "Close")
                        .AddAwait((AsyncStateInfo state) => state.IsComplete = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                    ;
            }
            else
            {
                if (on)
                {
                    return Planner.Chain()
                            .AddTween(Rotate)
                            .AddAwait(AwaitTweenFinished)
                        ;
                }
                else
                {
                    return Planner.Chain()
                            .AddTween(ReRotate)
                            .AddAwait(AwaitTweenFinished)
                        ;
                }
            }
        }

        public AsyncState SetActiveGameObject(bool isActive)
        {
            return Planner.Chain()
                    .AddAction(gameObject.SetActive, isActive)
                ;
        }

        // Internal

        private void OnMouseDown()
        {
            _onCardClick(this);
        }

        private Tween ReRotate()
        {
            return InitTween(transform
                    .DOLocalRotateQuaternion(Quaternion.Euler(_originRotation), _animTime))
                ;
        }

        private Tween Rotate()
        {
            _originRotation = transform.localEulerAngles;
            return InitTween(transform
                    .DOLocalRotateQuaternion(Quaternion.Euler(0f, 0f, 180f), _animTime))
                ;
        }

        private Tween ReScale()
        {
            return InitTween(transform
                    .DOScale(_originScale, _animTime))
                ;
        }

        private Tween Scale()
        {
            _originScale = transform.localScale;
            return InitTween(transform
                   .DOScale(_targetScale, _animTime))
                ;
        }

        private Tween InitTween(Tween tween)
        {
            _tweenRunning = true;
            tween.onComplete += () => _tweenRunning = false;
            return tween;
        }

        private void AwaitTweenFinished(AsyncStateInfo state)
        {
            state.IsComplete = !_tweenRunning;
        }
    }
}

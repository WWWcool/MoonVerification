using Moon.Asyncs;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace MiniGames.Memory
{
    public class MemoryCard : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _faceTexture = null;
        [SerializeField] private Animator _animator = null;
        public Texture texture => _faceTexture.material.mainTexture;

        private Action<MemoryCard> _onCardClick;
        public void Init(Texture2D texture, Action<MemoryCard> onCardClick)
        {
            _faceTexture.material.mainTexture = texture;
            _onCardClick = onCardClick;
        }

        public bool Match(MemoryCard card)
        {
            return texture == card.texture;
        }

        public AsyncState Appear(bool on)
        {
            return Planner.Chain()
                .AddAction(_animator.SetBool, "Appear", on)
                .AddAwait((AsyncStateInfo state) => state.IsComplete = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
        }

        public AsyncState Flip(bool on)
        {
            return Planner.Chain()
                .AddAction(_animator.SetBool, "Open", on)
                .AddAwait((AsyncStateInfo state) => state.IsComplete = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
        }

        public AsyncState SetActiveGameObject(bool isActive)
        {
            return Planner.Chain()
                .AddAction(gameObject.SetActive, isActive);
        }

        // Internal

        private void OnMouseDown()
        {
            _onCardClick(this);
        }

    }
}

using Moon.Asyncs;
using UnityEngine;
using System.Collections.Generic;

namespace MiniGames.Memory
{
    public class MemoryCard : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _faceTexture;

        public void Init(Texture2D texture)
        {
            _faceTexture.material.mainTexture = texture;
        }

        private void OnMouseDown()
        {
            Flip();
        }

        public void Flip()
        {

        }

    }
}

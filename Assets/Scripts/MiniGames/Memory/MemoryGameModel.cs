using UnityEngine;

namespace MiniGames.Memory
{
    [System.Serializable]
    public class RoundParams
    {
        [SerializeField] public int numberOfCardPairs;
        [SerializeField] public SpriteGroup spriteGroup;
    }

    [CreateAssetMenu(menuName = "MiniGames/Memory/MemoryGameModel")]
    public class MemoryGameModel : ScriptableObject
    {
        public RoundParams[] rounds;
        public float faceUpTime = 0.2f;
    }
}
using UnityEngine;

namespace MiniGames.Memory
{
    public enum SpriteGroupType
    {
        Animals,
        Clothes,
        Form,
        Forniture,
        Products,
    }

    [System.Serializable]
    public class SpriteGroup
    {
        [SerializeField] public SpriteGroupType type = SpriteGroupType.Animals;
        [SerializeField] public Texture2D[] sprites;
    }

    [CreateAssetMenu(menuName = "MiniGames/Memory/MemoryGameModel")]
    public class MemoryGameModel : ScriptableObject
    {
        public int numberOfCardPairs;
        public int numberOfRounds;
        public SpriteGroup[] spriteGroups;

        public SpriteGroup[] GetValidSpriteGroups()
        {
            return spriteGroups;
        }
    }
}
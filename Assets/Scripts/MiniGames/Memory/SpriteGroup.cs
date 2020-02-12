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

    [CreateAssetMenu(menuName = "MiniGames/Memory/SpriteGroup")]
    public class SpriteGroup : ScriptableObject
    {
        [SerializeField] public SpriteGroupType type = SpriteGroupType.Animals;
        [SerializeField] public Texture2D[] sprites;
    }
}
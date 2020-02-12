using MiniGames.Common;
using Moon.Asyncs;
using UnityEngine;

namespace MiniGames.Memory
{
    public enum DifficultyType
    {
        Easy = 0,
        Normal,
        Hard,
    }

    [System.Serializable]
    public class DifficultyModel
    {
        [SerializeField] public DifficultyType difficulty = DifficultyType.Normal;
        [SerializeField] public MemoryGameModel model;
    }

    public class MemoryDifficultyController : MonoBehaviour
    {
        public DifficultyModel[] gameModels;

        public MemoryGameModel GetModel(DifficultyType difficulty)
        {
            foreach (var model in gameModels)
            {
                if (model.difficulty == difficulty)
                {
                    return model.model;
                }
            }
            return null;
        }
    }
}
using Moon.Asyncs;
using UnityEngine;
using System.Collections.Generic;

namespace MiniGames.Memory
{
    public class MemoryGameBoard : MonoBehaviour
    {
        [SerializeField] private GameObject _cardPrefab;

        public bool GameEnded => _gameEnded;

        private List<MemoryCard> _pool;
        private bool _gameEnded = false;

        public AsyncState Init(MemoryGameModel gameModel)
        {
            ReinitGame();
            var asyncChain = Planner.Chain();
            asyncChain.AddAction(Debug.Log, "[MemoryGameBoard][Init]");

            var groups = gameModel.GetValidSpriteGroups();

            if (groups.Length > 0)
            {
                asyncChain.AddFunc(InitCards, groups[(int)Random.value], gameModel);
            }
            else
            {
                asyncChain.AddAction(Debug.LogError, "[MemoryGameBoard][Init] can`t get valid sprite group");
            }

            return asyncChain;
        }

        private AsyncState InitCards(SpriteGroup group, MemoryGameModel gameModel)
        {
            var asyncChain = Planner.Chain();
            asyncChain.AddAction(Debug.Log, "[MemoryGameBoard][InitCards]");

            var types = new List<Texture2D>(group.sprites);

            for (var i = 0; i < gameModel.numberOfCardPairs; i++)
            {
                var index = Random.Range(0, types.Count);
                asyncChain.AddFunc(AddCardPair, types[index]);
                types.RemoveAt(index);
            }

            return asyncChain;
        }

        private AsyncState AddCardPair(Texture2D texture)
        {
            var asyncChain = Planner.Chain();
            asyncChain.AddAction(Debug.Log, "[MemoryGameBoard][InitCardPair]");

            for (var i = 0; i < 2; i++)
            {
                var card = Instantiate(_cardPrefab).GetComponent<MemoryCard>();
                asyncChain.AddAction(card.Init, texture);
                _pool.Add(card);
            }

            return asyncChain;
        }

        private void ReinitGame()
        {
            _gameEnded = false;
            _pool = new List<MemoryCard>();
        }
    }
}

using Moon.Asyncs;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace MiniGames.Memory
{
    public class MemoryGameBoard : MonoBehaviour
    {
        [SerializeField] private GameObject _cardPrefab = null;
        [SerializeField] private GridLayoutGroup  _grid = null;
        private List<MemoryCard> _pool;
        private List<MemoryCard> _opened;

        public AsyncState Init(Action<MemoryCard> onCardClick, MemoryGameModel gameModel)
        {
            ReinitGame();
            var asyncChain = Planner.Chain();
            asyncChain.AddAction(Debug.Log, "[MemoryGameBoard][Init]");

            var groups = gameModel.GetValidSpriteGroups();

            if (groups.Length > 0)
            {
                asyncChain.AddFunc(InitCards, groups[(int)UnityEngine.Random.value], onCardClick, gameModel);
            }
            else
            {
                asyncChain.AddAction(Debug.LogError, "[MemoryGameBoard][Init] can`t get valid sprite group");
            }

            return asyncChain;
        }

        public void AddToOpen(MemoryCard card)
        {
            _opened.Add(card);
        }

        public AsyncState ProcessOpened()
        {            
            var asyncChain = Planner.Chain();
            asyncChain.AddAction(Debug.Log, "[MemoryGameBoard][ProcessOpened]");

            if(_opened.Count == 2)
            {
                if(_opened[0].Match(_opened[1]))
                {
                    foreach(var card in _opened)
                    {
                        asyncChain.JoinFunc(card.Appear, false);
                        asyncChain.JoinFunc(card.SetActiveGameObject, false);
                        _pool.Remove(card);
                    }
                }
                else
                {
                    foreach(var card in _opened)
                    {
                        asyncChain.JoinFunc(card.Flip, false);
                    }
                }
                asyncChain.AddAction(_opened.Clear);
            }

            return asyncChain;
        }

        public bool IsEmpty()
        {
            return _pool.Count == 0;
        }

        // Internal

        private AsyncState InitCards(SpriteGroup group, Action<MemoryCard> onCardClick, MemoryGameModel gameModel)
        {
            var asyncChain = Planner.Chain();
            asyncChain.AddAction(Debug.Log, "[MemoryGameBoard][InitCards]");

            var types = new List<Texture2D>(group.sprites);

            for (var i = 0; i < gameModel.numberOfCardPairs; i++)
            {
                var index = UnityEngine.Random.Range(0, types.Count);
                asyncChain.AddFunc(AddCardPair, types[index], onCardClick);
                types.RemoveAt(index);
            }

            return asyncChain;
        }

        private AsyncState AddCardPair(Texture2D texture, Action<MemoryCard> onCardClick)
        {
            var asyncChain = Planner.Chain();
            asyncChain.AddAction(Debug.Log, "[MemoryGameBoard][InitCardPair]");

            for (var i = 0; i < 2; i++)
            {
                var card = Instantiate(_cardPrefab, _grid.transform).GetComponent<MemoryCard>();
                asyncChain.AddAction(card.Init, texture, onCardClick);
                _pool.Add(card);
            }

            return asyncChain;
        }

        private void ReinitGame()
        {
            _pool = new List<MemoryCard>();
            _opened = new List<MemoryCard>();
        }
    }
}

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
        [SerializeField] private GridLayoutGroup _grid = null;
        [SerializeField] private GameObject _shuffleNode = null;
        private List<MemoryCard> _pool;
        private List<MemoryCard> _opened;
        private float _faceUpDelay = 0f;

        public AsyncState Init(Action<MemoryCard> onCardClick, GameParams gameParams)
        {
            ReinitGame();
            var asyncChain = Planner.Chain();
            asyncChain.AddAction(Debug.Log, "[MemoryGameBoard][Init] round: " + gameParams.roundIndex.ToString());

            var group = gameParams.model.rounds[gameParams.roundIndex].spriteGroup;
            _faceUpDelay = gameParams.model.faceUpTime;

            asyncChain.AddFunc(InitCards, group, onCardClick, gameParams);

            return asyncChain;
        }

        public AsyncState Shuffle()
        {
            var asyncChain = Planner.Chain();
            asyncChain.AddAction(Debug.Log, "[MemoryGameBoard][Shuffle]");

            // TODO we can add move anim here

            for (var i = 0; i < _grid.transform.childCount; i++)
            {
                _grid.transform.GetChild(i).transform.SetParent(_shuffleNode.transform);
            }

            while (_shuffleNode.transform.childCount > 0)
            {
                var rnd = UnityEngine.Random.Range(0, _shuffleNode.transform.childCount);
                _shuffleNode.transform.GetChild(rnd).transform.SetParent(_grid.transform);
            }

            return asyncChain;
        }

        public bool IsOpen(MemoryCard card)
        {
            return _opened.Contains(card);
        }

        public void AddToOpen(MemoryCard card)
        {
            _opened.Add(card);
        }

        public AsyncState ProcessOpened()
        {
            var asyncChain = Planner.Chain();
            asyncChain.AddAction(Debug.Log, "[MemoryGameBoard][ProcessOpened]");

            if (_opened.Count == 2)
            {
                asyncChain.AddTimeout(_faceUpDelay);
                if (_opened[0].Match(_opened[1]))
                {
                    asyncChain.AddFunc(RemoveCardPair);
                }
                else
                {
                    asyncChain.AddFunc(() => Planner.Chain()
                        .JoinFunc(_opened[0].Flip, false)
                        .JoinFunc(_opened[1].Flip, false)
                    );
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

        private AsyncState InitCards(SpriteGroup group, Action<MemoryCard> onCardClick, GameParams gameParams)
        {
            var asyncChain = Planner.Chain();
            asyncChain.AddAction(Debug.Log, "[MemoryGameBoard][InitCards]");

            var types = new List<Texture2D>(group.sprites);

            for (var i = 0; i < gameParams.model.rounds[gameParams.roundIndex].numberOfCardPairs; i++)
            {
                var index = UnityEngine.Random.Range(0, types.Count);
                asyncChain.JoinFunc(AddCardPair, types[index], onCardClick);
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
                asyncChain.JoinFunc(card.Init, texture, onCardClick);
                _pool.Add(card);
            }

            return asyncChain;
        }

        private void ReinitGame()
        {
            _pool = new List<MemoryCard>();
            _opened = new List<MemoryCard>();
            foreach (Transform child in _grid.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        private AsyncState RemoveCardPair()
        {
            var asyncChain = Planner.Chain();
            asyncChain.AddAction(Debug.Log, "[MemoryGameBoard][RemoveCardPair]");

            asyncChain.AddFunc(() => Planner.Chain()
                .JoinFunc(_opened[0].Appear, false)
                .JoinFunc(_opened[1].Appear, false)
            );

            asyncChain.AddFunc(() => Planner.Chain()
                .AddAwait(_opened[0].AwaitTweenFinished)
                .AddAwait(_opened[1].AwaitTweenFinished)
            );

            asyncChain.AddAction(() => _pool.Remove(_opened[0]));
            asyncChain.AddAction(() => _pool.Remove(_opened[1]));

            return asyncChain;
        }
    }
}

using Moon.Asyncs;
using UnityEngine;
using System;

namespace MiniGames.Memory
{
    public class MemoryGameController : MonoBehaviour
    {
        [SerializeField] private MemoryGameBoard _board = null;
        private bool _gameEnded = false;
        private bool _initFinished = false;
        private bool _isProcessOnClick = false;

        public AsyncState RunGame(MemoryGameModel gameModel)
        {
            _gameEnded = false;
            _initFinished = false;
            return Planner.Chain()
                    .AddAction(Debug.Log, "[MemoryGameController][RunGame]")
                    .AddFunc<Action<MemoryCard>, MemoryGameModel>(_board.Init, OnCardClick, gameModel)
                    .AddFunc(_board.Shuffle)
                    .AddAction(SetInitFinished)
                    .AddAwait(AwaitGameEnd)
                ;
        }

        private void AwaitGameEnd(AsyncStateInfo state)
        {
            state.IsComplete = _gameEnded;
        }

        private void OnCardClick(MemoryCard card)
        {
            if (!IsAvailableForClick() || _board.IsOpen(card))
            {
                return;
            }

            var asyncChain = Planner.Chain();
            asyncChain.AddAction(Debug.Log, "[MemoryGameController][OnCardClick]");
            asyncChain.AddAction(SetProcessOnClick, true);
            asyncChain.AddFunc(card.Flip, true);
            asyncChain.AddAction(_board.AddToOpen, card);
            asyncChain.AddFunc(_board.ProcessOpened);
            asyncChain.AddAction(CheckBoard);
            asyncChain.AddAction(SetProcessOnClick, false);
        }

        private void CheckBoard()
        {
            Debug.Log("[MemoryGameController][CheckBoard] - " + _board.IsEmpty().ToString());
            _gameEnded = _board.IsEmpty();
        }

        private bool IsAvailableForClick()
        {
            return _initFinished && !_isProcessOnClick;
        }

        private void SetInitFinished()
        {
            _initFinished = true;
        }

        private void SetProcessOnClick(bool running)
        {
            _isProcessOnClick = running;
        }
    }
}

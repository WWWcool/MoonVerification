using Moon.Asyncs;
using UnityEngine;

namespace MiniGames.Memory
{
    public class MemoryGameController : MonoBehaviour
    {
        [SerializeField] private MemoryGameBoard _board;

        public AsyncState RunGame(MemoryGameModel gameModel)
        {
            return Planner.Chain()
                    .AddAction(Debug.Log, "[MemoryGameController][RunGame]")
                    .AddFunc(_board.Init, gameModel)
                    .AddAwait(AwaitFunc)
                ;
        }

        private void AwaitFunc(AsyncStateInfo state)
        {
            state.IsComplete = _board.GameEnded;
        }
    }
}

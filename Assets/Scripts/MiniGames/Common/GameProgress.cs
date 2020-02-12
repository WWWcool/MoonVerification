using Moon.Asyncs;
using UnityEngine;

namespace MiniGames.Common
{
    public class GameProgress : MonoBehaviour
    {
        [SerializeField] private UIController uiController = null;
        private int _count = 0;
        private int _maxCount = 0;

        public AsyncState ResetProgress(int count)
        {
            _count = 0;
            _maxCount = count;
            return Planner.Chain()
                    .AddFunc(uiController.ResetProgress, count)
                ;
        }

        public AsyncState IncrementProgress()
        {
            _count++;
            return Planner.Chain()
                    .AddFunc(uiController.IncrementProgress, _count)
                ;
        }
    }
}
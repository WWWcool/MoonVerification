using MiniGames.Common;
using Moon.Asyncs;
using UnityEngine;

namespace MiniGames.Memory
{
    public class GameParams
    {
        public MemoryGameModel model = null;
        public int roundIndex = 0;
    }

    public class MemoryScenario : GameScenarioBase
    {
        public MemoryGameController controller = null;
        public MemoryDifficultyController difficultyController = null;
        public CameraController cameraController = null;
        public DifficultyType difficulty = DifficultyType.Normal;
        public UIController uiController = null;

        private MemoryGameModel _currentModel = null;
        private GameParams _params = null;

        protected override AsyncState OnExecute()
        {
            return Planner.Chain()
                    .AddFunc(Intro)
                    .AddFunc(GameCircle)
                    .AddFunc(Outro)
                ;
        }

        private AsyncState Intro()
        {
            return Planner.Chain()
                    .AddAction(Debug.Log, "[MemoryScenario][Intro] start intro")
                    .AddFunc(cameraController.MoveToBoard)
                    .JoinFunc(uiController.ShowUI)
                    .AddAction(Debug.Log, "[MemoryScenario][Intro] intro finished")
                ;
        }

        private AsyncState GameCircle()
        {
            _currentModel = difficultyController.GetModel(difficulty);

            var asyncChain = Planner.Chain();
            if (_currentModel != null)
            {
                _params = new GameParams() { model = _currentModel };

                asyncChain.AddAction(Debug.Log, "[MemoryScenario][GameCircle] game started");
                asyncChain.AddFunc(progress.ResetProgress, _currentModel.rounds.Length);

                for (var i = 0; i < _currentModel.rounds.Length; i++)
                {

                    asyncChain.AddAction(Debug.Log, "[MemoryScenario][GameCircle] start round: " + i.ToString());
                    asyncChain
                            .AddFunc(controller.RunGame, _params)
                            .AddFunc(progress.IncrementProgress)
                        ;
                    asyncChain.AddAction(() => _params.roundIndex++);
                }
                asyncChain.AddAction(Debug.Log, "[MemoryScenario][GameCircle] game finished");
            }
            else
            {
                asyncChain.AddAction(Debug.LogError, "[MemoryScenario][GameCircle] can`t find game model for difficulty: " + difficulty.ToString());
            }
            return asyncChain;
        }

        private AsyncState Outro()
        {
            return Planner.Chain()
                    .AddAction(Debug.Log, "[MemoryScenario][Outro] start outro")
                    .AddFunc(cameraController.MoveBack)
                    .JoinFunc(uiController.HideUI)
                    .AddAction(Debug.Log, "[MemoryScenario][Outro] outro finished")
                ;
        }

    }
}
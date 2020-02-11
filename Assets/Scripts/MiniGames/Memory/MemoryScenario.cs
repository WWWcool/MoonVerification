using MiniGames.Common;
using Moon.Asyncs;
using UnityEngine;

namespace MiniGames.Memory
{
    public class MemoryScenario : GameScenarioBase
    {
        public MemoryGameController controller;

        //TODO: select gameModel with difficulty controller class
        public MemoryGameModel defaultGameModel;

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
                    // TODO: intro cut scene. Run camera animation. Await animation finish
                    .AddAction(Debug.Log, "start intro")
                    .AddTimeout(1f)
                    .AddAction(Debug.Log, "intro finished")
                ;
        }

        private AsyncState GameCircle()
        {
            var asyncChain = Planner.Chain();
            asyncChain.AddAction(Debug.Log, "game started");
            for (var i = 0; i < defaultGameModel.numberOfRounds; i++)
            {
                asyncChain
                        .AddFunc(controller.RunGame, defaultGameModel)
                        .AddFunc(progress.IncrementProgress)
                    ;
            }

            asyncChain.AddAction(Debug.Log, "game finished");
            return asyncChain;
        }

        private AsyncState Outro()
        {
            // TODO: outro cut scene. Run camera back to origin position. Await animation finish
            return Planner.Chain()
                    .AddAction(Debug.Log, "start outro")
                    .AddTimeout(1f)
                    .AddAction(Debug.Log, "outro finished")
                ;
        }

    }
}
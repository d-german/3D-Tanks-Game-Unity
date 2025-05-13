using System.Linq;

// For TankManager.m_Instance.activeSelf

namespace Complete
{
    public class ClassicDeathmatchRulesStrategy : IGameRulesStrategy
    {
        private readonly int _numRoundsToWinForGame;

        public ClassicDeathmatchRulesStrategy(int numRoundsToWinForGame)
        {
            _numRoundsToWinForGame = numRoundsToWinForGame;
        }

        public void StartRound()
        {
            // This strategy has no specific state to reset or initialize per round.
            // This empty implementation is a pragmatic choice to fit the single interface,
            // it is a clear Interface Segregation Principle (ISP) violation.
        }

        public bool IsRoundOver(TankManager[] tanks)
        {
            if (tanks == null) return true; // Or specific handling for null
            return tanks.Count(t => t != null && t.m_Instance != null && t.m_Instance.activeSelf) <= 1;
        }

        public TankManager DetermineRoundWinner(TankManager[] tanks)
        {
            return tanks?.FirstOrDefault(t => t != null && t.m_Instance != null && t.m_Instance.activeSelf);
        }

        public TankManager DetermineGameWinner(TankManager[] tanks)
        {
            return tanks?.FirstOrDefault(t => t != null && t.m_Wins == _numRoundsToWinForGame);
        }
    }
}
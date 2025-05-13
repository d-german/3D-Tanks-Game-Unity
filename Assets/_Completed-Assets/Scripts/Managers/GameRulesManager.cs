using System.Linq;

namespace Complete
{
    public class GameRulesManager
    {
        private readonly int _numRoundsToWin;

        public GameRulesManager(int numRoundsToWin)
        {
            _numRoundsToWin = numRoundsToWin;
        }

        public bool IsOnlyOneTankLeft(TankManager[] tanks)
        {
            if (tanks == null) return true;

            return tanks.Count(t => t != null && t.m_Instance != null && t.m_Instance.activeSelf) <= 1;
        }

        public TankManager DetermineRoundWinner(TankManager[] tanks)
        {
            return tanks?.FirstOrDefault(t => t != null && t.m_Instance != null && t.m_Instance.activeSelf);
        }

        public TankManager DetermineGameWinner(TankManager[] tanks)
        {
            return tanks?.FirstOrDefault(t => t != null && t.m_Wins == _numRoundsToWin);
        }
    }
}
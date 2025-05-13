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
            if (tanks == null || tanks.Length == 0) return true;

            int numTanksLeft = 0;

            for (int i = 0; i < tanks.Length; i++)
            {
                if (tanks[i] != null && tanks[i].m_Instance != null && tanks[i].m_Instance.activeSelf)
                    numTanksLeft++;
            }

            return numTanksLeft <= 1;
        }

        public TankManager DetermineRoundWinner(TankManager[] tanks)
        {
            if (tanks == null) return null;

            for (int i = 0; i < tanks.Length; i++)
            {
                if (tanks[i] != null && tanks[i].m_Instance != null && tanks[i].m_Instance.activeSelf)
                    return tanks[i];
            }

            return null;
        }

        public TankManager DetermineGameWinner(TankManager[] tanks)
        {
            if (tanks == null) return null;

            for (int i = 0; i < tanks.Length; i++)
            {
                if (tanks[i] != null && tanks[i].m_Wins == _numRoundsToWin)
                    return tanks[i];
            }

            return null;
        }
    }
}
using System.Linq;
using System;

namespace Complete
{
    public class TimedRoundRulesStrategy : IGameRulesStrategy
    {
        private readonly float _roundDurationSeconds;
        private readonly int _numRoundsToWinForGame;
        private float _roundStartTime;
        private readonly ITimeProvider _timeProvider;

        public TimedRoundRulesStrategy(
            float roundDurationSeconds,
            int numRoundsToWinForGame,
            ITimeProvider timeProvider)
        {
            _roundDurationSeconds = roundDurationSeconds;
            _numRoundsToWinForGame = numRoundsToWinForGame;
            _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
        }

        public void StartRound()
        {
            _roundStartTime = _timeProvider.Time;
        }

        public bool IsRoundOver(TankManager[] tanks)
        {
            var oneTankLeft = (tanks == null) ||
                              (tanks.Count(t => t != null && t.m_Instance != null && t.m_Instance.activeSelf) <= 1);

            var timeExpired = (_timeProvider.Time - _roundStartTime) >= _roundDurationSeconds;

            return oneTankLeft || timeExpired;
        }

        public TankManager DetermineRoundWinner(TankManager[] tanks)
        {
            if (tanks == null || tanks.Length == 0) return null;

            var activeTanks = tanks.Where(t => t != null && t.m_Instance != null && t.m_Instance.activeSelf).ToList();

            if (activeTanks.Count == 0) return null;
            if (activeTanks.Count == 1) return activeTanks[0];

            TankManager winnerByHealth = null;
            var maxHealth = -1f;

            foreach (var tankManager in activeTanks)
            {
                var healthInfo = tankManager.m_HealthProvider;

                if (healthInfo == null) continue;
                if (!(healthInfo.CurrentHealth > maxHealth)) continue;
                maxHealth = healthInfo.CurrentHealth;
                winnerByHealth = tankManager;
            }

            return winnerByHealth;
        }

        public TankManager DetermineGameWinner(TankManager[] tanks)
        {
            return tanks?.FirstOrDefault(t => t != null && t.m_Wins == _numRoundsToWinForGame);
        }
    }
}
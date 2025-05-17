using System.Linq;
using UnityEngine;

namespace Complete
{
    public class TimedRoundRulesStrategy : IGameRulesStrategy
    {
        private readonly float _roundDurationSeconds;
        private readonly int _numRoundsToWinForGame;
        private float _roundStartTime;

        public TimedRoundRulesStrategy(float roundDurationSeconds, int numRoundsToWinForGame)
        {
            _roundDurationSeconds = roundDurationSeconds;
            _numRoundsToWinForGame = numRoundsToWinForGame;
        }

        public void StartRound()
        {
            _roundStartTime = Time.time;
        }

        public bool IsRoundOver(TankManager[] tanks)
        {
            var oneTankLeft = (tanks == null) ||
                              (tanks.Count(t => t != null && t.m_Instance != null && t.m_Instance.activeSelf) <= 1);

            var timeExpired = (Time.time - _roundStartTime) >= _roundDurationSeconds;

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
                var healthComponent = tankManager.m_Health;

                if (healthComponent != null)
                {
                    if (healthComponent.CurrentHealth > maxHealth)
                    {
                        maxHealth = healthComponent.CurrentHealth;
                        winnerByHealth = tankManager;
                    }
                }
            }

            return winnerByHealth;
        }

        public TankManager DetermineGameWinner(TankManager[] tanks)
        {
            return tanks?.FirstOrDefault(t => t != null && t.m_Wins == _numRoundsToWinForGame);
        }
    }
}
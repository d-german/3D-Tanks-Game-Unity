using System.Linq;
using UnityEngine;

namespace Complete
{
    public class TimedRoundRulesStrategy : IGameRulesStrategy
    {
        private readonly float _roundDurationSeconds;
        private readonly int _numRoundsToWinForGame; // Game still ends after X round wins
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

            // If multiple tanks are active, it means time expired. Winner by highest health.
            // (This assumes IsRoundOver was true because time expired, not because of a bug elsewhere)
            TankManager winnerByHealth = null;
            var maxHealth = -1f;

            foreach (var tankManager in activeTanks)
            {
                // Assuming TankManager.m_Instance is the GameObject with TankHealth
                if (tankManager.m_Instance == null) continue;

                var health = tankManager.m_Instance.GetComponent<TankHealth>();
                if (health != null)
                {
                    if (health.m_CurrentHealth > maxHealth)
                    {
                        maxHealth = health.m_CurrentHealth;
                        winnerByHealth = tankManager;
                    }
                    // Note: Simple tie-breaking (first one with max health wins).
                }
                else
                {
                    // This case should ideally not happen if tanks are set up correctly.
                    // Could log a warning if a logger was available.
                    // For now, tanks without health are not considered for health-based win.
                }
            }

            return winnerByHealth;
        }

        public TankManager DetermineGameWinner(TankManager[] tanks)
        {
            // Game win condition for this strategy is still "first to X round wins".
            return tanks?.FirstOrDefault(t => t != null && t.m_Wins == _numRoundsToWinForGame);
        }
    }
}
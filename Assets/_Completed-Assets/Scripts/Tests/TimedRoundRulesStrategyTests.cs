using System.Collections.Generic;
using Complete;
using NUnit.Framework;
using UnityEngine;

namespace _Completed_Assets.Scripts.Tests
{
    public class TimedRoundRulesStrategyTests
    {
        private MockTimeProvider _mockTimeProvider;
        private List<GameObject> _dummyTankInstances;

        private const int DefaultNumRoundsToWin = 3;
        private const float DefaultRoundDuration = 60f;

        [SetUp]
        public void SetUp()
        {
            _mockTimeProvider = new MockTimeProvider();
            _dummyTankInstances = new List<GameObject>();
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var instance in _dummyTankInstances)
            {
                if (instance != null)
                {
                    GameObject.DestroyImmediate(obj: instance);
                }
            }

            _dummyTankInstances.Clear();
        }

        private TankManager CreateTestTank(int playerNumber, bool isActive, float currentHealth, int wins = 0)
        {
            var dummyInstance = new GameObject(name: $"TestTankInstance_{playerNumber}");
            dummyInstance.SetActive(value: isActive);
            _dummyTankInstances.Add(item: dummyInstance);

            return new TankManager
            {
                m_PlayerNumber = playerNumber,
                m_Instance = dummyInstance,
                m_HealthProvider = new MockHealthProvider(initialHealth: currentHealth),
                m_Wins = wins
            };
        }

        [Test]
        public void StartRound_SetsRoundStartTimeFromTimeProvider()
        {
            var strategy = new TimedRoundRulesStrategy(roundDurationSeconds: DefaultRoundDuration, numRoundsToWinForGame: DefaultNumRoundsToWin, timeProvider: _mockTimeProvider);
            _mockTimeProvider.MockedTime = 123.45f;

            strategy.StartRound();

            var tanks = new TankManager[] { CreateTestTank(playerNumber: 1, isActive: true, currentHealth: 100f), CreateTestTank(playerNumber: 2, isActive: true, currentHealth: 100f) };
            var isOver = strategy.IsRoundOver(tanks: tanks);
            Assert.IsFalse(condition: isOver, message: "Round should not be over immediately after starting if time hasn't passed.");
        }

        [Test]
        public void IsRoundOver_ReturnsFalse_WhenTimeNotExpiredAndMultipleTanksActive()
        {
            var strategy = new TimedRoundRulesStrategy(roundDurationSeconds: DefaultRoundDuration, numRoundsToWinForGame: DefaultNumRoundsToWin, timeProvider: _mockTimeProvider);
            strategy.StartRound();
            var tanks = new TankManager[] { CreateTestTank(playerNumber: 1, isActive: true, currentHealth: 100f), CreateTestTank(playerNumber: 2, isActive: true, currentHealth: 100f) };
            _mockTimeProvider.AdvanceTime(amount: DefaultRoundDuration - 1f);

            var isOver = strategy.IsRoundOver(tanks: tanks);

            Assert.IsFalse(condition: isOver);
        }

        [Test]
        public void IsRoundOver_ReturnsTrue_WhenTimeExpired()
        {
            var strategy = new TimedRoundRulesStrategy(roundDurationSeconds: DefaultRoundDuration, numRoundsToWinForGame: DefaultNumRoundsToWin, timeProvider: _mockTimeProvider);
            strategy.StartRound();
            var tanks = new TankManager[] { CreateTestTank(playerNumber: 1, isActive: true, currentHealth: 100f), CreateTestTank(playerNumber: 2, isActive: true, currentHealth: 100f) };
            _mockTimeProvider.AdvanceTime(amount: DefaultRoundDuration);

            var isOver = strategy.IsRoundOver(tanks: tanks);

            Assert.IsTrue(condition: isOver);
        }

        [Test]
        public void IsRoundOver_ReturnsTrue_WhenOneTankLeft_EvenIfTimeNotExpired()
        {
            var strategy = new TimedRoundRulesStrategy(roundDurationSeconds: DefaultRoundDuration, numRoundsToWinForGame: DefaultNumRoundsToWin, timeProvider: _mockTimeProvider);
            strategy.StartRound();
            var tanks = new TankManager[] { CreateTestTank(playerNumber: 1, isActive: true, currentHealth: 100f), CreateTestTank(playerNumber: 2, isActive: false, currentHealth: 0f) };
            _mockTimeProvider.AdvanceTime(amount: 10f);

            var isOver = strategy.IsRoundOver(tanks: tanks);

            Assert.IsTrue(condition: isOver);
        }

        [Test]
        public void DetermineRoundWinner_OneTankActive_ReturnsThatTank()
        {
            var strategy = new TimedRoundRulesStrategy(roundDurationSeconds: DefaultRoundDuration, numRoundsToWinForGame: DefaultNumRoundsToWin, timeProvider: _mockTimeProvider);
            var tank1 = CreateTestTank(playerNumber: 1, isActive: true, currentHealth: 100f);
            var tank2 = CreateTestTank(playerNumber: 2, isActive: false, currentHealth: 0f);
            var tanks = new TankManager[] { tank1, tank2 };

            var winner = strategy.DetermineRoundWinner(tanks: tanks);

            Assert.AreSame(expected: tank1, actual: winner);
        }

        [Test]
        public void DetermineRoundWinner_TimeExpired_MultipleActive_ReturnsHighestHealth()
        {
            var strategy = new TimedRoundRulesStrategy(roundDurationSeconds: DefaultRoundDuration, numRoundsToWinForGame: DefaultNumRoundsToWin, timeProvider: _mockTimeProvider);
            var tank1 = CreateTestTank(playerNumber: 1, isActive: true, currentHealth: 70f);
            var tank2 = CreateTestTank(playerNumber: 2, isActive: true, currentHealth: 100f);
            var tank3 = CreateTestTank(playerNumber: 3, isActive: true, currentHealth: 30f);
            var tanks = new TankManager[] { tank1, tank2, tank3 };

            var winner = strategy.DetermineRoundWinner(tanks: tanks);

            Assert.AreSame(expected: tank2, actual: winner);
        }

        [Test]
        public void DetermineRoundWinner_TimeExpired_EqualHighestHealth_ReturnsFirstInList()
        {
            var strategy = new TimedRoundRulesStrategy(roundDurationSeconds: DefaultRoundDuration, numRoundsToWinForGame: DefaultNumRoundsToWin, timeProvider: _mockTimeProvider);
            var tank1 = CreateTestTank(playerNumber: 1, isActive: true, currentHealth: 100f);
            var tank2 = CreateTestTank(playerNumber: 2, isActive: true, currentHealth: 100f);
            var tank3 = CreateTestTank(playerNumber: 3, isActive: true, currentHealth: 30f);
            var tanks = new TankManager[] { tank1, tank2, tank3 };

            var winner = strategy.DetermineRoundWinner(tanks: tanks);

            Assert.AreSame(expected: tank1, actual: winner, message: "With equal health, the first tank in the list should win.");
        }

        [Test]
        public void DetermineRoundWinner_NoActiveTanks_ReturnsNull()
        {
            var strategy = new TimedRoundRulesStrategy(roundDurationSeconds: DefaultRoundDuration, numRoundsToWinForGame: DefaultNumRoundsToWin, timeProvider: _mockTimeProvider);
            var tanks = new TankManager[] { CreateTestTank(playerNumber: 1, isActive: false, currentHealth: 0f), CreateTestTank(playerNumber: 2, isActive: false, currentHealth: 0f) };

            var winner = strategy.DetermineRoundWinner(tanks: tanks);

            Assert.IsNull(anObject: winner);
        }

        [Test]
        public void DetermineGameWinner_TankHasEnoughWins_ReturnsThatTank()
        {
            var strategy = new TimedRoundRulesStrategy(roundDurationSeconds: DefaultRoundDuration, numRoundsToWinForGame: DefaultNumRoundsToWin, timeProvider: _mockTimeProvider);
            var tank1 = CreateTestTank(playerNumber: 1, isActive: true, currentHealth: 100f, wins: DefaultNumRoundsToWin - 1);
            var tank2 = CreateTestTank(playerNumber: 2, isActive: true, currentHealth: 100f, wins: DefaultNumRoundsToWin);
            var tanks = new TankManager[] { tank1, tank2 };

            var gameWinner = strategy.DetermineGameWinner(tanks: tanks);

            Assert.AreSame(expected: tank2, actual: gameWinner);
        }

        [Test]
        public void DetermineGameWinner_NoTankHasEnoughWins_ReturnsNull()
        {
            var strategy = new TimedRoundRulesStrategy(roundDurationSeconds: DefaultRoundDuration, numRoundsToWinForGame: DefaultNumRoundsToWin, timeProvider: _mockTimeProvider);
            var tanks = new TankManager[]
            {
                CreateTestTank(playerNumber: 1, isActive: true, currentHealth: 100f, wins: DefaultNumRoundsToWin - 1),
                CreateTestTank(playerNumber: 2, isActive: true, currentHealth: 100f, wins: DefaultNumRoundsToWin - 2)
            };

            var gameWinner = strategy.DetermineGameWinner(tanks: tanks);

            Assert.IsNull(anObject: gameWinner);
        }
    }
}
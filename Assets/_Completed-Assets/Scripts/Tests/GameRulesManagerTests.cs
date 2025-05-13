using System;
using NUnit.Framework;
using UnityEngine;
using Complete;

public class GameRulesManagerTests
{
    private const int DefaultNumRoundsToWin = 3;
    private GameRulesManager _rulesManager;

    private GameObject _tank1GameObject;
    private GameObject _tank2GameObject;
    private GameObject _tank3GameObject;

    [SetUp]
    public void SetUp()
    {
        _rulesManager = new GameRulesManager(DefaultNumRoundsToWin);

        _tank1GameObject = new GameObject("TestTank1_Obj");
        _tank2GameObject = new GameObject("TestTank2_Obj");
        _tank3GameObject = new GameObject("TestTank3_Obj");
    }

    [TearDown]
    public void TearDown()
    {
        if (_tank1GameObject != null) GameObject.DestroyImmediate(_tank1GameObject);
        if (_tank2GameObject != null) GameObject.DestroyImmediate(_tank2GameObject);
        if (_tank3GameObject != null) GameObject.DestroyImmediate(_tank3GameObject);
    }

    private static TankManager CreateTestTankManager(GameObject instanceReference, int playerNumber, int wins, bool isActive)
    {
        var tankManager = new TankManager
        {
            m_PlayerNumber = playerNumber,
            m_Wins = wins,
            m_Instance = instanceReference
        };
        if (instanceReference != null)
        {
            instanceReference.SetActive(isActive);
        }
        return tankManager;
    }

    [Test]
    public void IsOnlyOneTankLeft_NullTanksArray_ReturnsTrue()
    {
        Assert.IsTrue(_rulesManager.IsOnlyOneTankLeft(null));
    }

    [Test]
    public void IsOnlyOneTankLeft_EmptyTanksArray_ReturnsTrue()
    {
        Assert.IsTrue(_rulesManager.IsOnlyOneTankLeft(Array.Empty<TankManager>()));
    }

    [Test]
    public void IsOnlyOneTankLeft_NoActiveTanks_ReturnsTrue()
    {
        TankManager[] tanks = {
            CreateTestTankManager(_tank1GameObject, 1, 0, false),
            CreateTestTankManager(_tank2GameObject, 2, 0, false)
        };
        Assert.IsTrue(_rulesManager.IsOnlyOneTankLeft(tanks));
    }

    [Test]
    public void IsOnlyOneTankLeft_OneActiveTank_ReturnsTrue()
    {
        TankManager[] tanks = {
            CreateTestTankManager(_tank1GameObject, 1, 0, true),
            CreateTestTankManager(_tank2GameObject, 2, 0, false)
        };
        Assert.IsTrue(_rulesManager.IsOnlyOneTankLeft(tanks));
    }

    [Test]
    public void IsOnlyOneTankLeft_MultipleActiveTanks_ReturnsFalse()
    {
        TankManager[] tanks = {
            CreateTestTankManager(_tank1GameObject, 1, 0, true),
            CreateTestTankManager(_tank2GameObject, 2, 0, true)
        };
        Assert.IsFalse(_rulesManager.IsOnlyOneTankLeft(tanks));
    }

    [Test]
    public void IsOnlyOneTankLeft_OneTankManagerIsNullInArray_IgnoresNullAndCorrectlyCounts()
    {
        TankManager[] tanks = {
            CreateTestTankManager(_tank1GameObject, 1, 0, true),
            null,
            CreateTestTankManager(_tank2GameObject, 2, 0, true)
        };
        Assert.IsFalse(_rulesManager.IsOnlyOneTankLeft(tanks));
    }

    [Test]
    public void IsOnlyOneTankLeft_OneTankManagerInstanceIsNull_IgnoresAndCorrectlyCounts()
    {
        TankManager[] tanks = {
            CreateTestTankManager(_tank1GameObject, 1, 0, true),
            CreateTestTankManager(null, 2, 0, true), CreateTestTankManager(_tank2GameObject, 3, 0, true)
        };
        Assert.IsFalse(_rulesManager.IsOnlyOneTankLeft(tanks));
    }

    [Test]
    public void DetermineRoundWinner_NullTanksArray_ReturnsNull()
    {
        Assert.IsNull(_rulesManager.DetermineRoundWinner(null));
    }

    [Test]
    public void DetermineRoundWinner_NoActiveTanks_ReturnsNull()
    {
        TankManager[] tanks = {
            CreateTestTankManager(_tank1GameObject, 1, 0, false),
            CreateTestTankManager(_tank2GameObject, 2, 0, false)
        };
        Assert.IsNull(_rulesManager.DetermineRoundWinner(tanks));
    }

    [Test]
    public void DetermineRoundWinner_OneActiveTank_ReturnsThatTank()
    {
        var tank1 = CreateTestTankManager(_tank1GameObject, 1, 0, false);
        var tank2Active = CreateTestTankManager(_tank2GameObject, 2, 1, true);
        TankManager[] tanks = { tank1, tank2Active };

        var winner = _rulesManager.DetermineRoundWinner(tanks);
        Assert.AreSame(tank2Active, winner);
    }

    [Test]
    public void DetermineRoundWinner_MultipleActiveTanks_ReturnsFirstActiveTank()
    {
        var tank1Active = CreateTestTankManager(_tank1GameObject, 1, 0, true);
        var tank2Active = CreateTestTankManager(_tank2GameObject, 2, 1, true);
        TankManager[] tanks = { tank1Active, tank2Active };

        var winner = _rulesManager.DetermineRoundWinner(tanks);
        Assert.AreSame(tank1Active, winner);
    }

    [Test]
    public void DetermineGameWinner_NullTanksArray_ReturnsNull()
    {
        Assert.IsNull(_rulesManager.DetermineGameWinner(null));
    }

    [Test]
    public void DetermineGameWinner_NoTankHasEnoughWins_ReturnsNull()
    {
        TankManager[] tanks = {
            CreateTestTankManager(_tank1GameObject, 1, DefaultNumRoundsToWin - 1, true),
            CreateTestTankManager(_tank2GameObject, 2, 0, true)
        };
        Assert.IsNull(_rulesManager.DetermineGameWinner(tanks));
    }

    [Test]
    public void DetermineGameWinner_OneTankHasEnoughWins_ReturnsThatTank()
    {
        var tank1 = CreateTestTankManager(_tank1GameObject, 1, DefaultNumRoundsToWin -1, true);
        var tank2Winner = CreateTestTankManager(_tank2GameObject, 2, DefaultNumRoundsToWin, true);
        TankManager[] tanks = { tank1, tank2Winner };

        var gameWinner = _rulesManager.DetermineGameWinner(tanks);
        Assert.AreSame(tank2Winner, gameWinner);
    }

    [Test]
    public void DetermineGameWinner_MultipleTanksHaveEnoughWins_ReturnsFirstOneFound()
    {
        var tank1Winner = CreateTestTankManager(_tank1GameObject, 1, DefaultNumRoundsToWin, true);
        var tank2Winner = CreateTestTankManager(_tank2GameObject, 2, DefaultNumRoundsToWin, true);
        TankManager[] tanks = { tank1Winner, tank2Winner };

        var gameWinner = _rulesManager.DetermineGameWinner(tanks);
        Assert.AreSame(tank1Winner, gameWinner);
    }
}
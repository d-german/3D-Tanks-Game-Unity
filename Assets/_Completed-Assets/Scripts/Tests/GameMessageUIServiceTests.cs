using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using Complete;
using UnityEngine.TestTools;

public class GameMessageUIServiceTests
{
    private GameObject _uiHostGameObject;
    private Text _testTextComponent;
    private GameMessageUIService _uiService;

    [SetUp]
    public void SetUp()
    {
        _uiHostGameObject = new GameObject("TestUIHostForTextMessageService");
        _testTextComponent = _uiHostGameObject.AddComponent<Text>();

        _uiService = new GameMessageUIService(_testTextComponent);
    }

    [TearDown]
    public void TearDown()
    {
        if (_uiHostGameObject != null)
        {
            GameObject.DestroyImmediate(_uiHostGameObject);
        }
    }

    [Test]
    public void Constructor_WhenTextComponentIsNull_LogsError()
    {
        LogAssert.Expect(LogType.Error, "GameMessageUIService requires a Text component to be provided.");

        _ = new GameMessageUIService(null);
    }

    [Test]
    public void DisplayRoundStart_SetsCorrectText()
    {
        const int roundNumber = 5;
        var expectedText = "ROUND " + roundNumber;

        _uiService.DisplayRoundStart(roundNumber);

        Assert.AreEqual(expectedText, _testTextComponent.text);
    }

    [Test]
    public void ClearMessage_SetsTextToEmpty()
    {
        _testTextComponent.text = "Some pre-existing text";

        _uiService.ClearMessage();

        Assert.AreEqual(string.Empty, _testTextComponent.text);
    }

    private static TankManager CreateTestTankManager(int playerNumber, Color color, int wins)
    {
        var tankManager = new TankManager
        {
            m_PlayerNumber = playerNumber,
            m_PlayerColor = color,
            m_Wins = wins,
            m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(color) + ">PLAYER " + playerNumber + "</color>"
        };
        return tankManager;
    }

    [Test]
    public void DisplayRoundEndResults_Draw_SetsCorrectMessage()
    {
        TankManager[] tanks = {
            CreateTestTankManager(1, Color.blue, 2),
            CreateTestTankManager(2, Color.red, 1)
        };
        var expectedMessage = "DRAW!\n\n\n\n" +
                              tanks[0].m_ColoredPlayerText + ": " + tanks[0].m_Wins + " WINS\n" +
                              tanks[1].m_ColoredPlayerText + ": " + tanks[1].m_Wins + " WINS\n";
        _uiService.DisplayRoundEndResults(null, null, tanks);

        Assert.AreEqual(expectedMessage, _testTextComponent.text);
    }

    [Test]
    public void DisplayRoundEndResults_RoundWinner_SetsCorrectMessage()
    {
        var tank1 = CreateTestTankManager(1, Color.green, 3);
        var tank2 = CreateTestTankManager(2, Color.yellow, 2);
        TankManager[] tanks = { tank1, tank2 };

        var expectedMessage = tank1.m_ColoredPlayerText + " WINS THE ROUND!\n\n\n\n" +
                              tanks[0].m_ColoredPlayerText + ": " + tanks[0].m_Wins + " WINS\n" +
                              tanks[1].m_ColoredPlayerText + ": " + tanks[1].m_Wins + " WINS\n";
        _uiService.DisplayRoundEndResults(tank1, null, tanks);

        Assert.AreEqual(expectedMessage, _testTextComponent.text);
    }

    [Test]
    public void DisplayRoundEndResults_GameWinner_SetsCorrectMessage()
    {
        var tank1 = CreateTestTankManager(1, Color.cyan, 5);
        var tank2 = CreateTestTankManager(2, Color.magenta, 4);
        TankManager[] tanks = { tank1, tank2 };
        var expectedMessage = tank1.m_ColoredPlayerText + " WINS THE GAME!";

        _uiService.DisplayRoundEndResults(tank1, tank1, tanks);

        Assert.AreEqual(expectedMessage, _testTextComponent.text);
    }
}
using UnityEngine;
using UnityEngine.UI;

namespace Complete
{
    public class GameMessageUIService
    {
        private readonly Text _messageTextComponent;

        // Constructor that takes the Text component to display messages on.
        public GameMessageUIService(Text messageTextComponent)
        {
            _messageTextComponent = messageTextComponent;
            if (_messageTextComponent == null)
            {
                Debug.LogError("GameMessageUIService requires a Text component to be provided.");
            }
        }

        public void DisplayRoundStart(int roundNumber)
        {
            if (_messageTextComponent != null)
            {
                _messageTextComponent.text = "ROUND " + roundNumber;
            }
        }

        public void ClearMessage()
        {
            if (_messageTextComponent != null)
            {
                _messageTextComponent.text = string.Empty;
            }
        }

        public void DisplayRoundEndResults(TankManager roundWinner, TankManager gameWinner, TankManager[] allTanks)
        {
            if (_messageTextComponent == null) return;

            // By default when a round ends there are no winners so the default end message is a draw.
            string message = "DRAW!";

            // If there is a winner then change the message to reflect that.
            if (roundWinner != null)
                message = roundWinner.m_ColoredPlayerText + " WINS THE ROUND!";

            // Add some line breaks after the initial message.
            message += "\n\n\n\n";

            // Go through all the tanks and add each of their scores to the message.
            for (int i = 0; i < allTanks.Length; i++)
            {
                message += allTanks[i].m_ColoredPlayerText + ": " + allTanks[i].m_Wins + " WINS\n";
            }

            // If there is a game winner, change the entire message to reflect that.
            if (gameWinner != null)
                message = gameWinner.m_ColoredPlayerText + " WINS THE GAME!";

            _messageTextComponent.text = message;
        }
    }
}
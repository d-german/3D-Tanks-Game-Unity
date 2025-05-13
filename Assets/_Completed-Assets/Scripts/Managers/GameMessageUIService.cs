using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Complete
{
    public class GameMessageUIService : IGameMessageUIService
    {
        private readonly Text _messageTextComponent;

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

            var message = "DRAW!";

            if (roundWinner != null)
                message = roundWinner.m_ColoredPlayerText + " WINS THE ROUND!";

            message += "\n\n\n\n";

            message = allTanks.Aggregate(message, (current, t) => current + (t.m_ColoredPlayerText + ": " + t.m_Wins + " WINS\n"));

            if (gameWinner != null)
                message = gameWinner.m_ColoredPlayerText + " WINS THE GAME!";

            _messageTextComponent.text = message;
        }
    }
}

// No direct 'using UnityEngine;' needed if ILogger is in a general namespace
// and _wrappedService (IGameMessageUIService) doesn't expose raw Unity types
// that the decorator needs to directly interact with beyond calling its methods.

namespace Complete // Assuming IGameMessageUIService and TankManager are in this namespace
{
}
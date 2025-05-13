namespace Complete
{
    public class LoggingGameMessageUIServiceDecorator : IGameMessageUIService
    {
        private readonly IGameMessageUIService _wrappedService;
        private readonly ILogger _logger;

        public LoggingGameMessageUIServiceDecorator(IGameMessageUIService wrappedService, ILogger logger)
        {
            _wrappedService = wrappedService;
            _logger = logger;

            if (_wrappedService == null)
            {
                if (_logger != null)
                    _logger.LogError("CRITICAL: Wrapped IGameMessageUIService cannot be null in LoggingGameMessageUIServiceDecorator.");
                else
                    UnityEngine.Debug.LogError("CRITICAL: Wrapped IGameMessageUIService AND ILogger are null in LoggingGameMessageUIServiceDecorator.");
            }
            if (_logger == null)
            {
                UnityEngine.Debug.LogError("CRITICAL: ILogger cannot be null in LoggingGameMessageUIServiceDecorator.");
            }
        }

        public void DisplayRoundStart(int roundNumber)
        {
            _logger?.Log($"[DECORATOR] UI: Displaying round start - Round {roundNumber}");
            _wrappedService.DisplayRoundStart(roundNumber);
            _logger?.Log($"[DECORATOR] UI: DisplayRoundStart finished.");
        }

        public void ClearMessage()
        {
            _logger?.Log($"[DECORATOR] UI: Clearing message.");
            _wrappedService.ClearMessage();
            _logger?.Log($"[DECORATOR] UI: ClearMessage finished.");
        }

        public void DisplayRoundEndResults(TankManager roundWinner, TankManager gameWinner, TankManager[] allTanks)
        {
            var roundWinnerName = roundWinner != null ? roundWinner.m_ColoredPlayerText : "None";
            var gameWinnerName = gameWinner != null ? gameWinner.m_ColoredPlayerText : "None";
            var tankCount = allTanks?.Length ?? 0;

            _logger?.Log($"[DECORATOR] UI: Displaying round end results. RoundWinner: {roundWinnerName}, GameWinner: {gameWinnerName}, TankCount: {tankCount}");
            _wrappedService.DisplayRoundEndResults(roundWinner, gameWinner, allTanks);
            _logger?.Log($"[DECORATOR] UI: DisplayRoundEndResults finished.");
        }
    }
}
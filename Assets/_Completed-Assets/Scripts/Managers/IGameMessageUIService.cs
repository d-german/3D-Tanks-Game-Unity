namespace Complete
{
    public interface IGameMessageUIService
    {
        void DisplayRoundStart(int roundNumber);
        void ClearMessage();
        void DisplayRoundEndResults(TankManager roundWinner, TankManager gameWinner, TankManager[] allTanks);
    }
}
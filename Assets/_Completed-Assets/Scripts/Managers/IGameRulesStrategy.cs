namespace Complete
{
    public interface IGameRulesStrategy
    {
        void StartRound();
        bool IsRoundOver(TankManager[] tanks);
        TankManager DetermineRoundWinner(TankManager[] tanks);
        TankManager DetermineGameWinner(TankManager[] tanks);
    }
}
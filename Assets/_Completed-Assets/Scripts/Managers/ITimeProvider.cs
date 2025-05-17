namespace Complete
{
    public interface ITimeProvider
    {
        float Time { get; }
        float DeltaTime { get; }
    }

    public class UnityTimeProvider : ITimeProvider
    {
        public float Time => UnityEngine.Time.time;
        public float DeltaTime => UnityEngine.Time.deltaTime;
    }
}
using Complete;

namespace _Completed_Assets.Scripts.Tests
{
    public class MockTimeProvider : ITimeProvider
    {
        public float MockedTime { get; set; }
        public float MockedDeltaTime { get; set; }

        public float Time => MockedTime;
        public float DeltaTime => MockedDeltaTime;

        public MockTimeProvider(float initialTime = 0f, float initialDeltaTime = 0.016f)
        {
            MockedTime = initialTime;
            MockedDeltaTime = initialDeltaTime;
        }

        public void AdvanceTime(float amount)
        {
            MockedTime += amount;
        }
    }
}
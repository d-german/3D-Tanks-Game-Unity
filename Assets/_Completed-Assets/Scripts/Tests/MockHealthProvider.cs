using Complete;

namespace _Completed_Assets.Scripts.Tests
{
    public class MockHealthProvider : IHealthProvider
    {
        public float CurrentHealth { get; set; }

        public MockHealthProvider(float initialHealth = 100f)
        {
            CurrentHealth = initialHealth;
        }
    }
}
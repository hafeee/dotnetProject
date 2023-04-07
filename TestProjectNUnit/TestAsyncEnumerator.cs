using FlowerSpot.Models;

namespace TestProjectNUnit
{
    internal class TestAsyncEnumerator<T> : IAsyncEnumerator<Sighting>
    {
        private List<Sighting>.Enumerator enumerator;

        public TestAsyncEnumerator(List<Sighting>.Enumerator enumerator)
        {
            this.enumerator = enumerator;
        }

        public Sighting Current => throw new NotImplementedException();

        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }

        public ValueTask<bool> MoveNextAsync()
        {
            throw new NotImplementedException();
        }
    }
}
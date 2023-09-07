namespace TestOps.Service.Subscriber.Tests.TestResources
{
    /// <summary>
    /// Inherit this class for creating any unit test dependencies. Child classes define all Mocks
    /// and test data for individual unit test classes.
    /// </summary>
    public abstract class MockFactoryBase<TTypeUnderTest> where TTypeUnderTest : class
    {
        /// <summary>
        /// Create a mocked instance of your type under test.
        /// </summary>
        public abstract TTypeUnderTest CreateMockInstance();
    }
}

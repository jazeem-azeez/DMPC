using DMC.Logging;
using DMC.RunTime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;

namespace FunctionalityTests.Runtime
{
    [TestClass]
    public class NonLockingRuntimeWrapperTests
    {
        private MockRepository mockRepository;

        private Mock<ICacheLogger> mockCacheLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);

            this.mockCacheLogger = this.mockRepository.Create<ICacheLogger>();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this.mockRepository.VerifyAll();
        }

        private NonLockingRuntimeWrapper CreateNonLockingRuntimeWrapper()
        {
            return new NonLockingRuntimeWrapper(
                this.mockCacheLogger.Object);
        }

        [TestMethod]
        public async Task ThreadSafefWrapperAsync_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var nonLockingRuntimeWrapper = this.CreateNonLockingRuntimeWrapper();
            string operationKey = null;
            Func callback = null;

            // Act
            var result = await nonLockingRuntimeWrapper.ThreadSafefWrapperAsync(
                operationKey,
                callback);

            // Assert
            Assert.Fail();
        }
    }
}

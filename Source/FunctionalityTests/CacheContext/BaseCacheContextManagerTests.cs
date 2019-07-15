using DMC.CacheContext;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace FunctionalityTests.CacheContext
{
    [TestClass]
    public class BaseCacheContextManagerTests
    {
        private MockRepository mockRepository;



        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);


        }

        [TestCleanup]
        public void TestCleanup()
        {
            this.mockRepository.VerifyAll();
        }

        private BaseCacheContextManager CreateManager()
        {
            return new BaseCacheContextManager();
        }

        [TestMethod]
        public void CreateScope_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var manager = this.CreateManager();
            bool CacheEnabled = false;
            Guid trackGuid = default(global::System.Guid);

            // Act
            var result = manager.CreateScope(
                CacheEnabled,
                trackGuid);

            // Assert
            Assert.Fail();
        }
    }
}

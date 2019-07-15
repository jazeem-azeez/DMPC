using DMC;
using DMC.CacheProvider.CacheStores;
using DMC.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace FunctionalityTests.CacheProvider.CacheStores
{
    [TestClass]
    public class InMemStoreTests
    {
        private MockRepository mockRepository;

        private Mock<ICacheConfig> mockCacheConfig;
        private Mock<ICacheLogger> mockCacheLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);

            this.mockCacheConfig = this.mockRepository.Create<ICacheConfig>();
            this.mockCacheLogger = this.mockRepository.Create<ICacheLogger>();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this.mockRepository.VerifyAll();
        }

        private InMemStore CreateInMemStore()
        {
            return new InMemStore(
                this.mockCacheConfig.Object,
                this.mockCacheLogger.Object);
        }

        [TestMethod]
        public void Compact_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var inMemStore = this.CreateInMemStore();

            // Act
            var result = inMemStore.Compact();

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void GetDefaultExpiry_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var inMemStore = this.CreateInMemStore();

            // Act
            var result = inMemStore.GetDefaultExpiry();

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void GetEntry_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var inMemStore = this.CreateInMemStore();
            string key = null;

            // Act
            var result = inMemStore.GetEntry(
                key);

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void RemoveEntries_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var inMemStore = this.CreateInMemStore();
            List keys = null;

            // Act
            var result = inMemStore.RemoveEntries(
                keys);

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void RemoveEntry_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var inMemStore = this.CreateInMemStore();
            string key = null;

            // Act
            var result = inMemStore.RemoveEntry(
                key);

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void SetEntry_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var inMemStore = this.CreateInMemStore();
            string key = null;
            StoreWrapper value = null;

            // Act
            var result = inMemStore.SetEntry(
                key,
                value);

            // Assert
            Assert.Fail();
        }
    }
}

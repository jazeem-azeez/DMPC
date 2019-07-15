using DMC;
using DMC.BackPlane;
using DMC.CacheContext;
using DMC.Implementations;
using DMC.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;

namespace FeatureTests.CacheProvider
{
    [TestClass]
    public class CacheProviderV1Tests
    {
        private MockRepository mockRepository;

        private Mock<IStoreCollectionProvider<T>> mockStoreCollectionProvider;
        private Mock<ICacheConfig> mockCacheConfig;
        private Mock<IBackPlane> mockBackPlane;
        private Mock<ICacheLogger> mockCacheLogger;
        private Mock<IBaseCacheContextManager> mockBaseCacheContextManager;

        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);

            this.mockStoreCollectionProvider = this.mockRepository.Create<IStoreCollectionProvider<T>>();
            this.mockCacheConfig = this.mockRepository.Create<ICacheConfig>();
            this.mockBackPlane = this.mockRepository.Create<IBackPlane>();
            this.mockCacheLogger = this.mockRepository.Create<ICacheLogger>();
            this.mockBaseCacheContextManager = this.mockRepository.Create<IBaseCacheContextManager>();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this.mockRepository.VerifyAll();
        }

        private CacheProviderV1 CreateCacheProviderV1()
        {
            return new CacheProviderV1(
                this.mockStoreCollectionProvider.Object,
                this.mockCacheConfig.Object,
                this.mockBackPlane.Object,
                this.mockCacheLogger.Object,
                this.mockBaseCacheContextManager.Object);
        }

        [TestMethod]
        public void Compact_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var cacheProviderV1 = this.CreateCacheProviderV1();

            // Act
            var result = cacheProviderV1.Compact();

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void ConnectToBackPlane_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var cacheProviderV1 = this.CreateCacheProviderV1();

            // Act
            cacheProviderV1.ConnectToBackPlane();

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void DisconnectFromBackPlane_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var cacheProviderV1 = this.CreateCacheProviderV1();

            // Act
            cacheProviderV1.DisconnectFromBackPlane();

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public async Task DispatchEvent_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var cacheProviderV1 = this.CreateCacheProviderV1();
            EventMessage eventMessage = null;

            // Act
            await cacheProviderV1.DispatchEvent(
                eventMessage);

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void Get_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var cacheProviderV1 = this.CreateCacheProviderV1();
            string key = null;
            bool autoPropogateOrCachingEnabled = false;

            // Act
            var result = cacheProviderV1.Get(
                key,
                autoPropogateOrCachingEnabled);

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void Get_StateUnderTest_ExpectedBehavior1()
        {
            // Arrange
            var cacheProviderV1 = this.CreateCacheProviderV1();
            string key = null;

            // Act
            var result = cacheProviderV1.Get(
                key);

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void Get_StateUnderTest_ExpectedBehavior2()
        {
            // Arrange
            var cacheProviderV1 = this.CreateCacheProviderV1();
            string key = null;
            int level = 0;
            bool autoPropogateOrCachingEnabled = false;

            // Act
            var result = cacheProviderV1.Get(
                key,
                level,
                autoPropogateOrCachingEnabled);

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void GetOrSet_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var cacheProviderV1 = this.CreateCacheProviderV1();
            string key = null;
            Func getItemCallBack = null;
            bool autoPropogateOrCachingEnabled = false;

            // Act
            var result = cacheProviderV1.GetOrSet(
                key,
                getItemCallBack,
                autoPropogateOrCachingEnabled);

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void GetOrSet_StateUnderTest_ExpectedBehavior1()
        {
            // Arrange
            var cacheProviderV1 = this.CreateCacheProviderV1();
            string key = null;
            Func getItemCallBack = null;
            TimeSpan? expiry = null;
            bool autoPropogateOrCachingEnabled = false;

            // Act
            var result = cacheProviderV1.GetOrSet(
                key,
                getItemCallBack,
                expiry,
                autoPropogateOrCachingEnabled);

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public async Task GetOrSetAsync_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var cacheProviderV1 = this.CreateCacheProviderV1();
            string key = null;
            Func getItemCallBack = null;

            // Act
            var result = await cacheProviderV1.GetOrSetAsync(
                key,
                getItemCallBack);

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public async Task GetOrSetAsync_StateUnderTest_ExpectedBehavior1()
        {
            // Arrange
            var cacheProviderV1 = this.CreateCacheProviderV1();
            string key = null;
            Func getItemCallBack = null;
            bool autoPropogateOrCachingEnabled = false;

            // Act
            var result = await cacheProviderV1.GetOrSetAsync(
                key,
                getItemCallBack,
                autoPropogateOrCachingEnabled);

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public async Task GetOrSetAsync_StateUnderTest_ExpectedBehavior2()
        {
            // Arrange
            var cacheProviderV1 = this.CreateCacheProviderV1();
            string key = null;
            Func getItemCallBack = null;
            TimeSpan? expiry = null;
            bool autoPropogateOrCachingEnabled = false;

            // Act
            var result = await cacheProviderV1.GetOrSetAsync(
                key,
                getItemCallBack,
                expiry,
                autoPropogateOrCachingEnabled);

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void Invalidate_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var cacheProviderV1 = this.CreateCacheProviderV1();
            string key = null;

            // Act
            var result = cacheProviderV1.Invalidate(
                key);

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void Invalidate_StateUnderTest_ExpectedBehavior1()
        {
            // Arrange
            var cacheProviderV1 = this.CreateCacheProviderV1();
            List keys = null;

            // Act
            var result = cacheProviderV1.Invalidate(
                keys);

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void Invalidate_StateUnderTest_ExpectedBehavior2()
        {
            // Arrange
            var cacheProviderV1 = this.CreateCacheProviderV1();
            List keys = null;
            List filters = null;

            // Act
            var result = cacheProviderV1.Invalidate(
                keys,
                filters);

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void Invalidate_StateUnderTest_ExpectedBehavior3()
        {
            // Arrange
            var cacheProviderV1 = this.CreateCacheProviderV1();
            List keys = null;
            int level = 0;

            // Act
            var result = cacheProviderV1.Invalidate(
                keys,
                level);

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void Invalidate_StateUnderTest_ExpectedBehavior4()
        {
            // Arrange
            var cacheProviderV1 = this.CreateCacheProviderV1();
            List keys = null;
            List filters = null;
            int level = 0;

            // Act
            var result = cacheProviderV1.Invalidate(
                keys,
                filters,
                level);

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void OnBackPlaneEvent_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var cacheProviderV1 = this.CreateCacheProviderV1();
            EventMessage eventMessage = null;

            // Act
            cacheProviderV1.OnBackPlaneEvent(
                eventMessage);

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void Set_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var cacheProviderV1 = this.CreateCacheProviderV1();
            string key = null;
            T value = default(T);
            TimeSpan? expiry = null;

            // Act
            var result = cacheProviderV1.Set(
                key,
                value,
                expiry);

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void Set_StateUnderTest_ExpectedBehavior1()
        {
            // Arrange
            var cacheProviderV1 = this.CreateCacheProviderV1();
            string key = null;
            T value = default(T);
            TimeSpan? expiry = null;
            int level = 0;
            bool autoPropogateOrCachingEnabled = false;

            // Act
            var result = cacheProviderV1.Set(
                key,
                value,
                expiry,
                level,
                autoPropogateOrCachingEnabled);

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void Update_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var cacheProviderV1 = this.CreateCacheProviderV1();
            string key = null;
            T data = default(T);
            TimeSpan? expiry = null;
            T oldData = default(T);
            bool autoPropogateOrCachingEnabled = false;

            // Act
            var result = cacheProviderV1.Update(
                key,
                data,
                expiry,
                oldData,
                autoPropogateOrCachingEnabled);

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void Update_StateUnderTest_ExpectedBehavior1()
        {
            // Arrange
            var cacheProviderV1 = this.CreateCacheProviderV1();
            List keys = null;
            T data = default(T);
            TimeSpan? expiry = null;
            T oldData = default(T);
            bool autoPropogateOrCachingEnabled = false;

            // Act
            var result = cacheProviderV1.Update(
                keys,
                data,
                expiry,
                oldData,
                autoPropogateOrCachingEnabled);

            // Assert
            Assert.Fail();
        }
    }
}

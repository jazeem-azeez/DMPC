using DMC.Logging.LoggingBinder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Diagnostics.Tracing;

namespace FunctionalityTests.Logging.LoggingBinder
{
    [TestClass]
    public class DynamicLoggingWrapperTests
    {
        private MockRepository mockRepository;

        private Mock<T> mockT;
        private Mock<Func<T, string, Exception, EventLevel, bool>> mockFunc;

        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);

            this.mockT = this.mockRepository.Create<T>();
            this.mockFunc = this.mockRepository.Create<Func<T, string, Exception, EventLevel, bool>>();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this.mockRepository.VerifyAll();
        }

        private DynamicLoggingWrapper CreateDynamicLoggingWrapper()
        {
            return new DynamicLoggingWrapper(
                this.mockT.Object,
                this.mockFunc.Object);
        }

        [TestMethod]
        public void LogAsync_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var dynamicLoggingWrapper = this.CreateDynamicLoggingWrapper();
            string message = null;
            EventLevel level = default(global::System.Diagnostics.Tracing.EventLevel);

            // Act
            dynamicLoggingWrapper.LogAsync(
                message,
                level);

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void Log_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var dynamicLoggingWrapper = this.CreateDynamicLoggingWrapper();
            string message = null;
            Exception exception = null;
            EventLevel level = default(global::System.Diagnostics.Tracing.EventLevel);

            // Act
            dynamicLoggingWrapper.Log(
                message,
                exception,
                level);

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        public void LogException_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var dynamicLoggingWrapper = this.CreateDynamicLoggingWrapper();
            Exception ex = null;

            // Act
            dynamicLoggingWrapper.LogException(
                ex);

            // Assert
            Assert.Fail();
        }
    }
}

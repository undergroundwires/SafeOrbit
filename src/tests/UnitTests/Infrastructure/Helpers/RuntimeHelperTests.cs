using System;
using NUnit.Framework;

namespace SafeOrbit.Helpers
{
    /// <seealso cref="RuntimeHelper"/>
    [TestFixture]
    public class RuntimeHelperTests
    {
        [Test]
        public void ExecuteCodeWithGuaranteedCleanup_GivenActionAndCleanUp_InvokedInOrder()
        {
            // Arrange
            var isActionCalled = false;
            var isCleanupCalled = false;
            void CleanUp()
            {
                if (!isActionCalled)
                    throw new ArgumentException("cleanup called first");
                isCleanupCalled = true;
            }
            void Action()
            {
                if (isCleanupCalled)
                    throw new ArgumentException("cleanup called first");
                isActionCalled = true;
            }
            // Act
            RuntimeHelper.ExecuteCodeWithGuaranteedCleanup(
                Action,
                CleanUp);
            // Assert
            Assert.True(isActionCalled);
            Assert.True(isCleanupCalled);
        }
        
        [Test]
        public void ExecuteCodeWithGuaranteedCleanup_ActionThrows_ExceptionIsThrown()
        {
            // Arrange
            var expected = new ArgumentException("expected exception");
            // Act
            var actual = Assert.Throws<ArgumentException>(() =>
            {
                RuntimeHelper.ExecuteCodeWithGuaranteedCleanup(
                    () => throw expected,
                    () => { });
            });
            // Assert
            Assert.AreEqual(expected.Message, actual.Message);
        }

        [Test]
        public void ExecuteCodeWithGuaranteedCleanup_CleanupThrows_ExceptionIsThrown()
        {
            // Arrange
            var expected = new ArgumentException("expected exception");
            // Act
            var actual = Assert.Throws<ArgumentException>(() =>
            {
                RuntimeHelper.ExecuteCodeWithGuaranteedCleanup(
                    () => { },
                    () => throw expected);
            });
            // Assert
            Assert.AreEqual(expected.Message, actual.Message);
        }

        [Test]
        public void ExecuteCodeWithGuaranteedCleanup_ActionThrows_InvokesCleanup()
        {
            // Arrange
            var isCleanupCalled = false;
            void CleanUp() => isCleanupCalled = true;
            void Action() => throw new ArgumentException("expected exception");
            // Act
            var swallowed = Assert.Throws<ArgumentException>(() =>
            {
                RuntimeHelper.ExecuteCodeWithGuaranteedCleanup(
                    Action,
                    CleanUp);
            });
            // Assert
            Assert.True(isCleanupCalled);
        }

        [Test]
        public void ExecuteCodeWithGuaranteedCleanup_ActionIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                RuntimeHelper.ExecuteCodeWithGuaranteedCleanup(
                    null,
                    () => {  });
            });
        }

        [Test]
        public void ExecuteCodeWithGuaranteedCleanup_CleanUpIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                RuntimeHelper.ExecuteCodeWithGuaranteedCleanup(
                    () => {}, 
                    null);
            });
        }
    }
}
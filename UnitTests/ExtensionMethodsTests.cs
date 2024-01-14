using System.Text;

using Oxygen;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class ExtensionMethodsTests
    {
        [TestMethod]
        public void AppendWithComma_ShouldAppendComma()
        {
            // Arrange
            var sb = new StringBuilder("initial text");

            // Act
            sb.AppendWithComma("new text");

            // Assert
            Assert.AreEqual("initial text, new text", sb.ToString());
        }

        [TestMethod]
        public void AppendWithComma_ShouldNotAppendComma()
        {
            // Arrange
            var sb = new StringBuilder();

            // Act
            sb.AppendWithComma("new text");

            // Assert
            Assert.AreEqual("new text", sb.ToString());
        }
    }
}

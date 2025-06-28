using System.Linq;
using Aspecty.AutoSignals;
using NUnit.Framework;

namespace Aspecty.AutoSignals.Tests.Unit
{
    /// <summary>
    /// Unit tests for the AutoSignal attribute functionality
    /// </summary>
    [TestFixture]
    public class AutoSignalAttributeTests
    {
        [SetUp]
        public void Setup()
        {
            // Test setup
        }

        [Test]
        public void AutoSignalAttribute_WithSignalNameOnly_SetsPropertiesCorrectly()
        {
            // Arrange
            var signalName = "test_signal";

            // Act
            var attribute = new AutoSignalAttribute(signalName);

            // Assert
            Assert.That(attribute.SignalName, Is.EqualTo(signalName));
            Assert.That(attribute.NodePath, Is.EqualTo(""));
            Assert.That(attribute.ConnectionType, Is.EqualTo(SignalConnectionType.Normal));
        }

        [Test]
        public void AutoSignalAttribute_WithSignalNameAndNodePath_SetsPropertiesCorrectly()
        {
            // Arrange
            var signalName = "pressed";
            var nodePath = "UI/Button";

            // Act
            var attribute = new AutoSignalAttribute(signalName, nodePath);

            // Assert
            Assert.That(attribute.SignalName, Is.EqualTo(signalName));
            Assert.That(attribute.NodePath, Is.EqualTo(nodePath));
            Assert.That(attribute.ConnectionType, Is.EqualTo(SignalConnectionType.Normal));
        }

        [Test]
        public void AutoSignalAttribute_WithAllParameters_SetsPropertiesCorrectly()
        {
            // Arrange
            var signalName = "timeout";
            var nodePath = "Timer";
            var connectionType = SignalConnectionType.Deferred;

            // Act
            var attribute = new AutoSignalAttribute(signalName, nodePath, connectionType);

            // Assert
            Assert.That(attribute.SignalName, Is.EqualTo(signalName));
            Assert.That(attribute.NodePath, Is.EqualTo(nodePath));
            Assert.That(attribute.ConnectionType, Is.EqualTo(connectionType));
        }

        [Test]
        public void AutoSignalAttribute_WithEmptySignalName_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.That(
                () => new AutoSignalAttribute(""),
                Throws.TypeOf<System.ArgumentException>()
            );
        }

        [Test]
        public void AutoSignalAttribute_WithNullSignalName_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.That(
                () => new AutoSignalAttribute(null!),
                Throws.TypeOf<System.ArgumentNullException>()
            );
        }

        [Test]
        public void AutoSignalAttribute_WithConstantSignalName_SetsCorrectly()
        {
            // Arrange
            var signalName = ASignalName.Pressed;

            // Act
            var attribute = new AutoSignalAttribute(signalName);

            // Assert
            Assert.That(attribute.SignalName, Is.EqualTo("pressed"));
        }

        // === Input Validation Tests ===

        [Test]
        public void AutoSignalAttribute_WithWhitespaceOnlySignalName_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.That(
                () => new AutoSignalAttribute("   "),
                Throws.TypeOf<System.ArgumentException>()
            );
            Assert.That(
                () => new AutoSignalAttribute("\t"),
                Throws.TypeOf<System.ArgumentException>()
            );
            Assert.That(
                () => new AutoSignalAttribute("\n"),
                Throws.TypeOf<System.ArgumentException>()
            );
            Assert.That(
                () => new AutoSignalAttribute("  \t  \n  "),
                Throws.TypeOf<System.ArgumentException>()
            );
        }

        [Test]
        public void AutoSignalAttribute_WithSignalNameContainingSpaces_PreservesSpaces()
        {
            // Arrange
            var signalName = "signal with spaces";

            // Act
            var attribute = new AutoSignalAttribute(signalName);

            // Assert
            Assert.That(attribute.SignalName, Is.EqualTo(signalName));
        }

        [Test]
        public void AutoSignalAttribute_WithNullNodePath_SetsToEmptyString()
        {
            // Arrange
            var signalName = "test_signal";

            // Act
            var attribute = new AutoSignalAttribute(signalName, null);

            // Assert
            Assert.That(attribute.NodePath, Is.EqualTo(""));
        }

        [Test]
        public void AutoSignalAttribute_WithWhitespaceNodePath_PreservesWhitespace()
        {
            // Arrange
            var signalName = "test_signal";
            var nodePath = "  UI/Button  ";

            // Act
            var attribute = new AutoSignalAttribute(signalName, nodePath);

            // Assert
            Assert.That(attribute.NodePath, Is.EqualTo(nodePath));
        }

        [Test]
        public void AutoSignalAttribute_WithComplexNodePath_SetsCorrectly()
        {
            // Arrange
            var signalName = "pressed";
            var nodePath = "UI/MainMenu/ButtonContainer/StartButton";

            // Act
            var attribute = new AutoSignalAttribute(signalName, nodePath);

            // Assert
            Assert.That(attribute.SignalName, Is.EqualTo(signalName));
            Assert.That(attribute.NodePath, Is.EqualTo(nodePath));
        }

        [Test]
        public void AutoSignalAttribute_WithUnderscoreInSignalName_SetsCorrectly()
        {
            // Arrange
            var signalName = "tree_entered";

            // Act
            var attribute = new AutoSignalAttribute(signalName);

            // Assert
            Assert.That(attribute.SignalName, Is.EqualTo(signalName));
        }

        [Test]
        public void AutoSignalAttribute_WithNumbersInSignalName_SetsCorrectly()
        {
            // Arrange
            var signalName = "button2_pressed";

            // Act
            var attribute = new AutoSignalAttribute(signalName);

            // Assert
            Assert.That(attribute.SignalName, Is.EqualTo(signalName));
        }

        // === Edge Case Tests ===

        [Test]
        public void AutoSignalAttribute_WithVeryLongSignalName_SetsCorrectly()
        {
            // Arrange
            var signalName = new string('a', 1000);

            // Act
            var attribute = new AutoSignalAttribute(signalName);

            // Assert
            Assert.That(attribute.SignalName, Is.EqualTo(signalName));
        }

        [Test]
        public void AutoSignalAttribute_WithVeryLongNodePath_SetsCorrectly()
        {
            // Arrange
            var signalName = "pressed";
            var nodePath = string.Join("/", Enumerable.Repeat("VeryLongNodeName", 50));

            // Act
            var attribute = new AutoSignalAttribute(signalName, nodePath);

            // Assert
            Assert.That(attribute.NodePath, Is.EqualTo(nodePath));
        }

        [Test]
        public void AutoSignalAttribute_WithSingleCharacterSignalName_SetsCorrectly()
        {
            // Arrange
            var signalName = "a";

            // Act
            var attribute = new AutoSignalAttribute(signalName);

            // Assert
            Assert.That(attribute.SignalName, Is.EqualTo(signalName));
        }

        [Test]
        public void AutoSignalAttribute_WithAllConnectionTypes_SetsCorrectly()
        {
            // Test all enum values
            var signalName = "test_signal";
            var connectionTypes = new[]
            {
                SignalConnectionType.Normal,
                SignalConnectionType.Deferred,
                SignalConnectionType.OneShot,
            };

            foreach (var connectionType in connectionTypes)
            {
                // Act
                var attribute = new AutoSignalAttribute(signalName, "", connectionType);

                // Assert
                Assert.That(attribute.ConnectionType, Is.EqualTo(connectionType));
            }
        }

        [Test]
        public void AutoSignalAttribute_WithUnicodeSignalName_SetsCorrectly()
        {
            // Arrange
            var signalName = "测试信号";

            // Act
            var attribute = new AutoSignalAttribute(signalName);

            // Assert
            Assert.That(attribute.SignalName, Is.EqualTo(signalName));
        }

        // === Default Value Tests ===

        [Test]
        public void AutoSignalAttribute_SingleParameterConstructor_UsesDefaultValues()
        {
            // Arrange
            var signalName = "test_signal";

            // Act
            var attribute = new AutoSignalAttribute(signalName);

            // Assert
            Assert.That(attribute.SignalName, Is.EqualTo(signalName));
            Assert.That(attribute.NodePath, Is.EqualTo(""));
            Assert.That(attribute.ConnectionType, Is.EqualTo(SignalConnectionType.Normal));
        }

        [Test]
        public void AutoSignalAttribute_TwoParameterConstructor_UsesDefaultConnectionType()
        {
            // Arrange
            var signalName = "pressed";
            var nodePath = "Button";

            // Act
            var attribute = new AutoSignalAttribute(signalName, nodePath);

            // Assert
            Assert.That(attribute.SignalName, Is.EqualTo(signalName));
            Assert.That(attribute.NodePath, Is.EqualTo(nodePath));
            Assert.That(attribute.ConnectionType, Is.EqualTo(SignalConnectionType.Normal));
        }

        // === Property Behavior Tests ===

        [Test]
        public void AutoSignalAttribute_StringComparison_IsCaseSensitive()
        {
            // Arrange
            var signalName1 = "Pressed";
            var signalName2 = "pressed";

            // Act
            var attribute1 = new AutoSignalAttribute(signalName1);
            var attribute2 = new AutoSignalAttribute(signalName2);

            // Assert
            Assert.That(attribute1.SignalName, Is.Not.EqualTo(attribute2.SignalName));
            Assert.That(attribute1.SignalName, Is.EqualTo("Pressed"));
            Assert.That(attribute2.SignalName, Is.EqualTo("pressed"));
        }

        // === Additional ASignalName Constant Tests ===

        [Test]
        public void AutoSignalAttribute_WithVariousASignalNameConstants_SetsCorrectly()
        {
            // Test multiple constants to ensure they work correctly
            var testCases = new[]
            {
                (ASignalName.Ready, "ready"),
                (ASignalName.TreeEntered, "tree_entered"),
                (ASignalName.ButtonDown, "button_down"),
                (ASignalName.ButtonUp, "button_up"),
                (ASignalName.Toggled, "toggled"),
                (ASignalName.TextChanged, "text_changed"),
                (ASignalName.Timeout, "timeout"),
            };

            foreach (var (constant, expectedValue) in testCases)
            {
                // Act
                var attribute = new AutoSignalAttribute(constant);

                // Assert
                Assert.That(
                    attribute.SignalName,
                    Is.EqualTo(expectedValue),
                    $"Constant {nameof(constant)} should equal '{expectedValue}'"
                );
            }
        }

        [Test]
        public void AutoSignalAttribute_WithSpecialCharactersInNodePath_SetsCorrectly()
        {
            // Arrange
            var signalName = "pressed";
            var nodePath = "UI/Panel@Button#1";

            // Act
            var attribute = new AutoSignalAttribute(signalName, nodePath);

            // Assert
            Assert.That(attribute.NodePath, Is.EqualTo(nodePath));
        }

        // === Edge Case Validation Tests ===

        [Test]
        public void AutoSignalAttribute_PropertiesAreConsistentAfterConstruction()
        {
            // Arrange
            var signalName = "test_signal";
            var nodePath = "TestNode";
            var connectionType = SignalConnectionType.Deferred;

            // Act
            var attribute = new AutoSignalAttribute(signalName, nodePath, connectionType);

            // Assert - Test multiple times to ensure consistency
            for (int i = 0; i < 5; i++)
            {
                Assert.That(attribute.SignalName, Is.EqualTo(signalName));
                Assert.That(attribute.NodePath, Is.EqualTo(nodePath));
                Assert.That(attribute.ConnectionType, Is.EqualTo(connectionType));
            }
        }

        [Test]
        public void AutoSignalAttribute_WithDifferentParameterCombinations_AllWork()
        {
            // Test all constructor overloads work correctly
            var signalName = "test_signal";
            var nodePath = "TestNode";
            var connectionType = SignalConnectionType.OneShot;

            // Single parameter
            var attr1 = new AutoSignalAttribute(signalName);
            Assert.That(attr1.SignalName, Is.EqualTo(signalName));
            Assert.That(attr1.NodePath, Is.EqualTo(""));
            Assert.That(attr1.ConnectionType, Is.EqualTo(SignalConnectionType.Normal));

            // Two parameters
            var attr2 = new AutoSignalAttribute(signalName, nodePath);
            Assert.That(attr2.SignalName, Is.EqualTo(signalName));
            Assert.That(attr2.NodePath, Is.EqualTo(nodePath));
            Assert.That(attr2.ConnectionType, Is.EqualTo(SignalConnectionType.Normal));

            // Three parameters
            var attr3 = new AutoSignalAttribute(signalName, nodePath, connectionType);
            Assert.That(attr3.SignalName, Is.EqualTo(signalName));
            Assert.That(attr3.NodePath, Is.EqualTo(nodePath));
            Assert.That(attr3.ConnectionType, Is.EqualTo(connectionType));
        }
    }
}

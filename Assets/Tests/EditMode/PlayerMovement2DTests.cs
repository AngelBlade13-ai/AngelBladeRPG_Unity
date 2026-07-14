using NUnit.Framework;
using UnityEngine;

namespace AngelBladeRPG.Tests
{
    public class PlayerMovement2DTests
    {
        [Test]
        public void ClampMovementInputKeepsCardinalInputUnchanged()
        {
            Vector2 movement = PlayerMovement2D.ClampMovementInput(Vector2.right);

            Assert.That(movement, Is.EqualTo(Vector2.right));
        }

        [Test]
        public void ClampMovementInputPreventsFasterDiagonalMovement()
        {
            Vector2 movement = PlayerMovement2D.ClampMovementInput(Vector2.one);

            Assert.That(movement.magnitude, Is.EqualTo(1f).Within(0.0001f));
            Assert.That(movement.x, Is.EqualTo(movement.y).Within(0.0001f));
        }

        [Test]
        public void ClampMovementInputPreservesAnalogInputBelowFullSpeed()
        {
            Vector2 input = new Vector2(0.25f, 0.5f);

            Vector2 movement = PlayerMovement2D.ClampMovementInput(input);

            Assert.That(movement, Is.EqualTo(input));
        }
    }
}

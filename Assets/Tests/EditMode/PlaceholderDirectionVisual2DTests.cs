using NUnit.Framework;
using UnityEngine;

namespace AngelBladeRPG.Tests
{
    public class PlaceholderDirectionVisual2DTests
    {
        [TestCase(0f, 1f, 0f)]
        [TestCase(1f, 0f, -90f)]
        [TestCase(0f, -1f, 180f)]
        [TestCase(-1f, 0f, 90f)]
        public void GetFacingAngleReturnsCardinalDirection(
            float x,
            float y,
            float expectedAngle)
        {
            float angle = PlaceholderDirectionVisual2D.GetFacingAngle(
                new Vector2(x, y));

            Assert.That(angle, Is.EqualTo(expectedAngle));
        }

        [Test]
        public void GetFacingAngleUsesDominantDiagonalAxis()
        {
            float angle = PlaceholderDirectionVisual2D.GetFacingAngle(
                new Vector2(0.8f, 0.2f));

            Assert.That(angle, Is.EqualTo(-90f));
        }
    }
}

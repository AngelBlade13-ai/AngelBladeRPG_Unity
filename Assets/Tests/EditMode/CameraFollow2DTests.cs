using NUnit.Framework;
using UnityEngine;

namespace AngelBladeRPG.Tests
{
    public class CameraFollow2DTests
    {
        [Test]
        public void CalculatePositionPreservesCameraDepth()
        {
            Vector3 current = new Vector3(0f, 0f, -10f);
            Vector3 target = new Vector3(4f, 2f, 7f);

            Vector3 position = CameraFollow2D.CalculatePosition(current, target, 0f, 1f);

            Assert.That(position, Is.EqualTo(new Vector3(4f, 2f, -10f)));
        }

        [Test]
        public void CalculatePositionMovesTowardTargetWithoutOvershooting()
        {
            Vector3 current = new Vector3(0f, 0f, -10f);
            Vector3 target = new Vector3(10f, 5f, 0f);

            Vector3 position = CameraFollow2D.CalculatePosition(current, target, 12f, 0.02f);

            Assert.That(position.x, Is.GreaterThan(0f).And.LessThan(10f));
            Assert.That(position.y, Is.GreaterThan(0f).And.LessThan(5f));
            Assert.That(position.z, Is.EqualTo(-10f));
        }
    }
}

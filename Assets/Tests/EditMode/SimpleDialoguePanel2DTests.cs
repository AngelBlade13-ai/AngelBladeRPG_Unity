using NUnit.Framework;
using UnityEngine;

namespace AngelBladeRPG.Tests
{
    public class SimpleDialoguePanel2DTests
    {
        private GameObject source;
        private GameObject viewer;

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(source);
            Object.DestroyImmediate(viewer);
        }

        [Test]
        public void IsOutsideRangeReturnsFalseAtMaximumDistance()
        {
            CreateObjects();
            viewer.transform.position = Vector2.right * 1.5f;

            bool outside = SimpleDialoguePanel2D.IsOutsideRange(
                source.transform,
                viewer.transform,
                1.5f);

            Assert.That(outside, Is.False);
        }

        [Test]
        public void IsOutsideRangeReturnsTrueBeyondMaximumDistance()
        {
            CreateObjects();
            viewer.transform.position = Vector2.right * 1.51f;

            bool outside = SimpleDialoguePanel2D.IsOutsideRange(
                source.transform,
                viewer.transform,
                1.5f);

            Assert.That(outside, Is.True);
        }

        private void CreateObjects()
        {
            source = new GameObject("Message Source");
            viewer = new GameObject("Viewer");
        }
    }
}

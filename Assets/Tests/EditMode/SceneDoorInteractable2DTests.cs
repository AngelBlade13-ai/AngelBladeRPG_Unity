using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class SceneDoorInteractable2DTests
    {
        [TestCase("TownScene", "TownEntrance", true)]
        [TestCase("", "TownEntrance", false)]
        [TestCase("TownScene", "", false)]
        [TestCase("   ", "   ", false)]
        public void HasDestinationValidatesSceneAndSpawn(
            string sceneName,
            string spawnId,
            bool expected)
        {
            bool valid = SceneDoorInteractable2D.HasDestination(
                sceneName,
                spawnId);

            Assert.That(valid, Is.EqualTo(expected));
        }
    }
}

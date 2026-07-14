using NUnit.Framework;
using UnityEngine;

namespace AngelBladeRPG.Tests
{
    public class WorldSceneSpawnController2DTests
    {
        private GameObject controllerObject;
        private GameObject playerObject;
        private GameObject spawnObject;

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(controllerObject);
            Object.DestroyImmediate(playerObject);
            Object.DestroyImmediate(spawnObject);
        }

        [Test]
        public void PlacePlayerAtDefaultSpawnMovesPlayerAndPreservesDepth()
        {
            controllerObject = new GameObject("Spawn Controller");
            playerObject = new GameObject("Player");
            spawnObject = new GameObject("Town Entrance");

            playerObject.transform.position = new Vector3(0f, 0f, -2f);
            spawnObject.transform.position = new Vector3(4f, 6f, 3f);
            PlayerSpawnPoint2D spawnPoint =
                spawnObject.AddComponent<PlayerSpawnPoint2D>();
            WorldSceneSpawnController2D controller =
                controllerObject.AddComponent<WorldSceneSpawnController2D>();

            controller.Configure(playerObject.transform, spawnPoint);

            bool placed = controller.PlacePlayerAtDefaultSpawn();

            Assert.That(placed, Is.True);
            Assert.That(
                playerObject.transform.position,
                Is.EqualTo(new Vector3(4f, 6f, -2f)));
        }

    }
}

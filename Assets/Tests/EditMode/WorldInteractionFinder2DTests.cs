using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace AngelBladeRPG.Tests
{
    public class WorldInteractionFinder2DTests
    {
        private readonly List<GameObject> objects = new List<GameObject>();

        [TearDown]
        public void TearDown()
        {
            foreach (GameObject createdObject in objects)
            {
                Object.DestroyImmediate(createdObject);
            }

            objects.Clear();
        }

        [Test]
        public void FindNearestReturnsClosestAvailableInteractable()
        {
            GameObject interactor = CreateObject("Player", Vector2.zero);
            TestInteractable far = CreateInteractable("Far", Vector2.right * 3f);
            TestInteractable near = CreateInteractable("Near", Vector2.right);
            Collider2D[] candidates =
            {
                far.GetComponent<Collider2D>(),
                near.GetComponent<Collider2D>()
            };

            IWorldInteractable result = WorldInteractionFinder2D.FindNearest(
                candidates,
                Vector2.zero,
                interactor);

            Assert.That(result, Is.SameAs(near));
        }

        [Test]
        public void FindNearestSkipsUnavailableInteractables()
        {
            GameObject interactor = CreateObject("Player", Vector2.zero);
            TestInteractable unavailable =
                CreateInteractable("Unavailable", Vector2.right);
            TestInteractable available =
                CreateInteractable("Available", Vector2.right * 2f);
            unavailable.Available = false;
            Collider2D[] candidates =
            {
                unavailable.GetComponent<Collider2D>(),
                available.GetComponent<Collider2D>()
            };

            IWorldInteractable result = WorldInteractionFinder2D.FindNearest(
                candidates,
                Vector2.zero,
                interactor);

            Assert.That(result, Is.SameAs(available));
        }

        private GameObject CreateObject(string name, Vector2 position)
        {
            GameObject createdObject = new GameObject(name);
            createdObject.transform.position = position;
            objects.Add(createdObject);
            return createdObject;
        }

        private TestInteractable CreateInteractable(string name, Vector2 position)
        {
            GameObject createdObject = CreateObject(name, position);
            createdObject.AddComponent<BoxCollider2D>();
            return createdObject.AddComponent<TestInteractable>();
        }
    }

    public class TestInteractable : MonoBehaviour, IWorldInteractable
    {
        public bool Available { get; set; } = true;

        public bool CanInteract(GameObject interactor)
        {
            return Available;
        }

        public void Interact(GameObject interactor)
        {
        }
    }
}

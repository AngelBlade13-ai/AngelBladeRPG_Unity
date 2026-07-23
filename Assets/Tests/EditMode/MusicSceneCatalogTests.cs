using System.Linq;
using NUnit.Framework;

namespace AngelBladeRPG.Tests
{
    public class MusicSceneCatalogTests
    {
        [Test]
        public void MainGameSceneUsesMainMenuMusic()
        {
            Assert.That(
                MusicSceneCatalog.GetCue("MainGameScene"),
                Is.EqualTo(MusicCue.MainMenu));
        }

        [TestCase("SuncrestAmberRowScene")]
        [TestCase("SuncrestGroveScene")]
        [TestCase("SuncrestGuildHallScene")]
        [TestCase("SuncrestInnScene")]
        [TestCase("SuncrestIronforgeScene")]
        [TestCase("SuncrestShrineScene")]
        [TestCase("SuncrestWatchScene")]
        [TestCase("SuncrestWhisperMarketScene")]
        public void EverySuncrestDistrictUsesSharedTownMusic(string sceneName)
        {
            Assert.That(
                MusicSceneCatalog.GetCue(sceneName),
                Is.EqualTo(MusicCue.Suncrest));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("BattleScene")]
        [TestCase("TownScene")]
        [TestCase("suncrestguildhallscene")]
        public void UnroutedScenesDoNotInheritMusic(string sceneName)
        {
            Assert.That(
                MusicSceneCatalog.GetCue(sceneName),
                Is.EqualTo(MusicCue.None));
        }

        [Test]
        public void SuncrestSceneCatalogContainsEightUniqueNames()
        {
            string[] names =
                MusicSceneCatalog.GetSuncrestSceneNames().ToArray();

            Assert.That(names, Has.Length.EqualTo(8));
            Assert.That(names.Distinct().Count(), Is.EqualTo(8));
        }
    }
}

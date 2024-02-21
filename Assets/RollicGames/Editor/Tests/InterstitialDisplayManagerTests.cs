using NUnit.Framework;
using RollicGames.Advertisements.Ads;

namespace RollicGames.Editor.Tests
{
    public class InterstitialDisplayManagerTests
    {
        private InterstitialDisplayManager _interstitialDisplayManager;

        [SetUp]
        public void SetUp()
        {
            _interstitialDisplayManager = InterstitialDisplayManager.GetInstance();
            _interstitialDisplayManager.Init("");
        }

        [Test]
        public void TestCase1()
        {
            _interstitialDisplayManager.SetRules(30, 0, 0);
            // t = 10
            Assert.That(_interstitialDisplayManager.IsTimerReady(10), Is.EqualTo(true));
            _interstitialDisplayManager._lastTimeAdDisplayed = 10;
            
            // t = 50 : rewarded watched with reset
            _interstitialDisplayManager.AdjustInterstitialDisplayTimer(50, 0);
            Assert.That(_interstitialDisplayManager.IsTimerReady(50), Is.EqualTo(false));
            
            // t = 60
            Assert.That(_interstitialDisplayManager.IsTimerReady(60), Is.EqualTo(false));
            
            // t = 80
            Assert.That(_interstitialDisplayManager.IsTimerReady(80), Is.EqualTo(true));
        }
        
        [Test]
        public void TestCase2()
        {
            _interstitialDisplayManager.SetRules(30, 0, 0);
            // t = 10
            Assert.That(_interstitialDisplayManager.IsTimerReady(10), Is.EqualTo(true));
            _interstitialDisplayManager._lastTimeAdDisplayed = 10;
            
            // t = 31 : rewarded watched with reset
            _interstitialDisplayManager.AdjustInterstitialDisplayTimer(31, 0);
            Assert.That(_interstitialDisplayManager.IsTimerReady(31), Is.EqualTo(false));
            
            // t = 60
            Assert.That(_interstitialDisplayManager.IsTimerReady(60), Is.EqualTo(false));
            
            // t = 62
            Assert.That(_interstitialDisplayManager.IsTimerReady(62), Is.EqualTo(true));
        }
        
        [Test]
        public void TestCase3()
        {
            _interstitialDisplayManager.SetRules(30, 0, 0);
            // t = 10
            Assert.That(_interstitialDisplayManager.IsTimerReady(10), Is.EqualTo(true));
            _interstitialDisplayManager._lastTimeAdDisplayed = 10;
            
            // t = 125 : rewarded watched with reset
            _interstitialDisplayManager.AdjustInterstitialDisplayTimer(125, 0);
            Assert.That(_interstitialDisplayManager.IsTimerReady(125), Is.EqualTo(false));
            
            // t = 130
            Assert.That(_interstitialDisplayManager.IsTimerReady(130), Is.EqualTo(false));
            
            // t = 156
            Assert.That(_interstitialDisplayManager.IsTimerReady(156), Is.EqualTo(true));
        }
        
        [Test]
        public void TestCase4()
        {
            _interstitialDisplayManager.SetRules(30, 0, 0);
            // t = 10
            Assert.That(_interstitialDisplayManager.IsTimerReady(10), Is.EqualTo(true));
            _interstitialDisplayManager._lastTimeAdDisplayed = 10;
            
            // t = 24 
            Assert.That(_interstitialDisplayManager.IsTimerReady(24), Is.EqualTo(false));
            
            // t = 41 
            Assert.That(_interstitialDisplayManager.IsTimerReady(41), Is.EqualTo(true));
            _interstitialDisplayManager._lastTimeAdDisplayed = 41;
            
            // t = 42
            Assert.That(_interstitialDisplayManager.IsTimerReady(42), Is.EqualTo(false));
        }

        [TearDown]
        public void TearDown()
        {
            _interstitialDisplayManager._lastTimeAdDisplayed = 0;
            _interstitialDisplayManager = null;
        }
    }
}
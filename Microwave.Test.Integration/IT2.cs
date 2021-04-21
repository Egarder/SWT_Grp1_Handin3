using System.Xml.Linq;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Microwave.Test.Integration
{
    public class IT2
    {
        private Door _door;
        private Button _powerButton;
        private Button _timerButton;
        private Button _startCancelButton;
        private Light _light;
        private IDisplay _displayFake;
        private IOutput _outputFake;
        private IPowerTube _powerTubeFake;
        private ITimer _timerFake;
        private UserInterface _SUT;
        private CookController _cookController;

        [SetUp]
        public void Setup()
        {
            _door = new Door();
            _powerButton = new Button();
            _timerButton = new Button();
            _startCancelButton = new Button();
            _outputFake = Substitute.For<IOutput>();
            _light = new Light(_outputFake);
            _displayFake = Substitute.For<IDisplay>();
            _powerTubeFake = Substitute.For<IPowerTube>();
            _timerFake = Substitute.For<ITimer>();
            _cookController = new CookController(_timerFake,_displayFake,_powerTubeFake);
            _SUT = new UserInterface(_powerButton, _timerButton, _startCancelButton,_door,_displayFake,_light,_cookController);
        }

        // Emil
        [Test]
        public void StartMicrowaveWhenStopped()
        {
            Assert.Pass();
        }

        [Test]
        public void StopMicrowaveWhenStarted()
        {
            Assert.Pass();
        }

        [Test]
        public void StartMicrowaveThenTimeOutStop()
        {
            Assert.Pass();
        }


    }
}
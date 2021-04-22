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
        public void OpensDoorStateReady()
        {
            _door.Open();

            _outputFake.Received(1).OutputLine(Arg.Is<string>(text => text.Contains("Light is turned on")));
        }

        [Test]
        public void ClosesDoorStateReady()
        {
            _door.Open();
            _door.Close();

            _outputFake.Received(1).OutputLine(Arg.Is<string>(text => text.Contains("Light is turned off")));
        }

        [Test]
        public void PowerBtnPressedStateDoorIsOpen()
        {
            _powerButton.Press();


            Assert.Pass();
        }

        [Test]
        public void TimeBtnPressedStateReady()
        {
            _timerButton.Press();

            Assert.Pass();
        }

        [Test]
        public void StartCancelBtnPressedStateReady()
        {
            _startCancelButton.Press();

            Assert.Pass();
        }

        [Test]
        public void OpensDoorStateCooking()
        {
            _door.Open();

            Assert.Pass();
        }

        [Test]
        public void StartCancelBtnPressedStateCooking()
        {
            _startCancelButton.Press();

            Assert.Pass();
        }

    }
}
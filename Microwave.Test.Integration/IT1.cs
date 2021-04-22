using System.Xml.Linq;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Microwave.Test.Integration
{
    public class IT1
    {
        // Camilla
        private Door _door;
        private Button _powerButton;
        private Button _timeButton;
        private Button _startCancelButton;
        private Light _light;
        private IDisplay _display;
        private IOutput _output;
        //private PowerTube _powerTube;
        //private Timer _timer;
        private UserInterface sut;
        private ICookController _cookController;
        private int fakePowerLevel;
        private int fakeTime;

        [SetUp]
        public void Setup()
        {
            _door = new Door();
            _powerButton = new Button();
            _timeButton = new Button();
            _startCancelButton = new Button();
            _display = Substitute.For<IDisplay>();
            _output = Substitute.For<IOutput>();
            _light = new Light(_output);
            _cookController = Substitute.For<ICookController>();
            sut = new UserInterface(_powerButton, _timeButton, _startCancelButton, _door, _display, _light, _cookController);
        }

        [Test]
        public void Door_OpenCloseDoor_EventRaised()
        {
            //Act
            _door.Open();
            _door.Close();

            //Assert
            //Check on light after door event
            _output.Received(1).OutputLine("Light is turned on");
            _output.Received(1).OutputLine("Light is turned off");
        }
        [Test]
        public void PowerBtn_IsPressed_WattsIsFifty()
        {
            //Arrange
            fakePowerLevel = 50;
            //Act
            _powerButton.Press();
            //Assert
            _display.Received(1).ShowPower(fakePowerLevel);
        }

        [Test]
        public void TimeBtn_IsPressed_TimeIsOne()
        {
            //Arrange
            _powerButton.Press();
            fakeTime = 1;
            //Act
            _timeButton.Press();
            //Assert
            _display.Received(1).ShowTime(fakeTime, 0);
        }

        [Test]
        public void StartCancelBtn_IsPressed_LightOnStartCooking()
        {
            //Arrange
            fakePowerLevel = 50;
            fakeTime = 60;
            _powerButton.Press();
            _timeButton.Press();
            //Act
            _startCancelButton.Press();
            //Assert
            _output.Received(1).OutputLine("Light is turned on");
            _cookController.Received(1).StartCooking(fakePowerLevel, fakeTime);
        }
    }
}
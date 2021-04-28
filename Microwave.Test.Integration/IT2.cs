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
        // Camilla
        private Door _door;
        private Button _powerButton;
        private Button _timeButton;
        private Button _startCancelButton;
        private Light _light;
        private IDisplay _display;
        private IOutput _output;
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
        public void Light_DoorOpen_LightOn()
        {
            //Act
            _door.Open();

            //Assert
            _output.Received(1).OutputLine("Light is turned on");
        }
        [Test]
        public void Light_DoorClose_LightOff()
        {
            //Arrange
            _door.Open();
            //Act
            _door.Close();
            //Assert
            _output.Received(1).OutputLine("Light is turned off");
        }
    }
}
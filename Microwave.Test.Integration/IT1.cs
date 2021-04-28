using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microwave.Test.Integration
{
    class IT1
    {
        private Door _door;
        private Button _powerButton;
        private Button _timeButton;
        private Button _startCancelButton;
        private ILight _light;
        private IDisplay _display;
        private UserInterface sut;
        private int fakePowerLevel;
        private int fakeTime;
        private ICookController _cookController;

        [SetUp]
        public void Setup()
        {
            _door = new Door();
            _powerButton = new Button();
            _timeButton = new Button();
            _startCancelButton = new Button();
            _display = Substitute.For<IDisplay>();
            _light = Substitute.For<ILight>();
            _cookController = Substitute.For<ICookController>();
            sut = new UserInterface(_powerButton, _timeButton, _startCancelButton, _door, _display, _light, _cookController);
        }

        [Test]
        public void Door_DoorOpened_LightOn()
        {
            _door.Open();
            _light.Received(1).TurnOn();
        }

        [Test]
        public void Door_DoorClosed_LightOff()
        {
            //Arrange
            _door.Open();
            //Act
            _door.Close();
            //Assert
            _light.Received(1).TurnOff();
        }

        [Test]
        public void PowerBtn_BtnPressed_ShowPower()
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
            _light.Received(1).TurnOn();
            _cookController.Received(1).StartCooking(fakePowerLevel, fakeTime);
        }
    }
}

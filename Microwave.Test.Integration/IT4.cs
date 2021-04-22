using System;
using System.Threading;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using NSubstitute;
using NUnit.Framework;
using IOutput = Microwave.Classes.Interfaces.IOutput;
using Timer = Microwave.Classes.Boundary.Timer;

namespace Microwave.Test.Integration
{
    // Thomas
    public class IT4
    {
        private IDoor _door;
        private IButton _powerButton;
        private IButton _timeButton;
        private IButton _startCancelButton;
        private ILight _light;
        private IDisplay _display;
        private IOutput _output;
        private IPowerTube _powerTube;
        private ITimer _timer;
        private ICookController _cooker;
        private IUserInterface _sut;

        [SetUp]
        public void Setup()
        {
            _output = Substitute.For<IOutput>();

            _powerButton = new Button();
            _timeButton = new Button();
            _startCancelButton = new Button();
            _door = new Door();
            _light = new Light(_output);

            _timer = new Timer();
            _display = new Display(_output);
            _powerTube = new PowerTube(_output);

            _cooker = new CookController(_timer, _display, _powerTube);

            _sut = new UserInterface(_powerButton, _timeButton, _startCancelButton, _door, _display, _light, _cooker);
        }


        [Test]
        public void Door_DoorOpenAndClose_CorrectOutput()
        {
            // Act
            _door.Open();
            _door.Close();

            // Assert
            _output.Received(1).OutputLine(Arg.Is<string>(text => text.Contains("on")));
            _output.Received(1).OutputLine(Arg.Is<string>(text => text.Contains("off")));
        }


        [Test]
        public void PowerButton_ButtonPressedTwoTimes_CorrectOutput()
        {
            // Act
            _powerButton.Press();
            _powerButton.Press();

            // Assert
            _output.Received(1).OutputLine(Arg.Is<string>(text => text.Contains("100 W")));
        }

        [Test]
        public void PowerButton_ButtonPressedThreeTimes_CorrectOutput()
        {
            // Act
            _powerButton.Press();
            _powerButton.Press();
            _powerButton.Press();

            _output.Received(1).OutputLine(Arg.Is<string>(text => text.Contains("150 W")));
        }

        [Test]
        public void TimerButton_ButtonPressedThreeTimes_CorrectOutput()
        {
            // Arrange
            _powerButton.Press();

            // Act
            _timeButton.Press();
            _timeButton.Press();
            _timeButton.Press();

            // Assert
            _output.Received(1).OutputLine(Arg.Is<string>(text => text.Contains("03:00")));
        }

        [Test]
        public void Timer_TimeSetToOneMinueWaitTwoSeconds_OutPutShows58SecondsRemaining()
        {
            // Arrange
            _powerButton.Press();

            // Act
            _timeButton.Press(); // set time to 1 minute
            _startCancelButton.Press();
            Thread.Sleep(2000); // wait 2 seconds to be certain

            // Assert
            _output.Received(1).OutputLine(Arg.Is<String>(text => text.Contains("00:58")));
        }

        [Test]
        public void MainScenario_CorrectOutput()
        {
            // Act
            _door.Open();
            _door.Close();
            
            _powerButton.Press();
            _powerButton.Press();

            _timeButton.Press();
            _timeButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();


            // Assert
            _output.Received(2).OutputLine(Arg.Is<string>(text => text.Contains("Light is turned on")));
            _output.Received(1).OutputLine(Arg.Is<string>(text => text.Contains("Light is turned off")));

            _output.Received(1).OutputLine(Arg.Is<string>(text => text.Contains("50 W")));
            _output.Received(1).OutputLine(Arg.Is<string>(text => text.Contains("100 W")));

            _output.Received(1).OutputLine(Arg.Is<string>(text => text.Contains("01:00")));
            _output.Received(1).OutputLine(Arg.Is<string>(text => text.Contains("02:00")));
            _output.Received(1).OutputLine(Arg.Is<string>(text => text.Contains("03:00")));
        }

        [Test]
        public void Extension1_DoorOpenedBeforeCookingDone_OutputShowsPowerTubeTurnedOff()
        {
            // Arrange
            _door.Open();
            _door.Close();
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();

            // Act
            _door.Open();

            // Assert
            _output.Received(1).OutputLine(Arg.Is<string>(text => text.Contains("PowerTube turned off")));
        }

        [Test]
        public void Extension2_StartCancelButtonPressedBeforeCookingDone_OutputShowsPowerTubeTurnedOff()
        {
            // Arrange
            _door.Open();
            _door.Close();
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();

            // Act
            _startCancelButton.Press();

            // Assert
            _output.Received(1).OutputLine(Arg.Is<string>(text => text.Contains("PowerTube turned off")));
        }
    }
}
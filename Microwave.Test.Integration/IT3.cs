using System;
using System.Xml.Linq;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Microwave.Test.Integration
{
    public class Tests3
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

        // Thomas
        [Test]
        public void Test1()
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

            _output.Received(2).OutputLine(Arg.Is<string>(text => text.Contains("Light is turned on")));
            _output.Received(1).OutputLine(Arg.Is<string>(text => text.Contains("Light is turned off")));

            _output.Received(1).OutputLine(Arg.Is<string>(text => text.Contains("50 W")));
            _output.Received(1).OutputLine(Arg.Is<string>(text => text.Contains("100 W")));

            _output.Received(1).OutputLine(Arg.Is<string>(text => text.Contains("01:00")));
            _output.Received(1).OutputLine(Arg.Is<string>(text => text.Contains("02:00")));
            _output.Received(1).OutputLine(Arg.Is<string>(text => text.Contains("03:00")));
        }
    }
}
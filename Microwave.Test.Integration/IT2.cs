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



        //User sequences from sequence diagram. 
        [Test]
        public void OpensDoorState_Ready()
        {
            _door.Open();

            _outputFake.Received(1).OutputLine(Arg.Is<string>(text => text.Contains("Light is turned on")));
        }

        [Test]
        public void ClosesDoorState_DoorIsOpen()
        {
            _door.Open();
            _door.Close();

            _outputFake.Received(1).OutputLine(Arg.Is<string>(text => text.Contains("turned off")));
        }

        [Test]
        public void PowerBtnPressedState_Ready()
        {
            _powerButton.Press();

            _outputFake.Received(1).OutputLine(Arg.Is<string>(text => text.Contains("Display shows: ")));
        }

        [Test]
        public void TimeBtnPressedState_SetPower()  /// ============================================================ FAILER
        {
            //_door.Open();
            //_door.Close();
            _powerButton.Press();

            _timerButton.Press();

            _outputFake.Received().OutputLine(Arg.Is<string>(text => text.Contains("Display shows")));
        }

        [Test]
        public void StartCancelBtnPressedState_SetTime() /// ============================================================ FAILER
        {
            _door.Open();
            _door.Close();
            _powerButton.Press();
            _timerButton.Press();

            _startCancelButton.Press();

            _outputFake.Received().OutputLine(Arg.Is<string>(text => text.Contains("Light is turned on")));
            _outputFake.Received().OutputLine(Arg.Is<string>(text => text.Contains("PowerTube works")));
            _outputFake.Received().OutputLine(Arg.Is<string>(text => text.Contains("Display shows")));
            _outputFake.Received().OutputLine(Arg.Is<string>(text => text.Contains("PowerTube turned off")));
            _outputFake.Received().OutputLine(Arg.Is<string>(text => text.Contains("Display cleared")));
            _outputFake.Received().OutputLine(Arg.Is<string>(text => text.Contains("Light is turned off")));

        }

        [Test]
        public void OpensDoorState_Cooking() /// ============================================================ FAILER
        {
            _door.Open();
            _door.Close();
            _powerButton.Press();
            _timerButton.Press();
            _startCancelButton.Press();

            _door.Open();

            _outputFake.Received().OutputLine(Arg.Is<string>(text => text.Contains("PowerTube turned off")));
            _outputFake.Received().OutputLine(Arg.Is<string>(text => text.Contains("Display cleared")));
        }

        [Test]
        public void StartCancelBtnPressedState_Cooking()
        {
            _door.Open();
            _door.Close();
            _powerButton.Press();
            _timerButton.Press();
            _startCancelButton.Press();

            _startCancelButton.Press();

            _outputFake.Received().OutputLine(Arg.Is<string>(text => text.Contains("PowerTube turned off")));
            _outputFake.Received().OutputLine(Arg.Is<string>(text => text.Contains("Display cleared")));
            _outputFake.Received().OutputLine(Arg.Is<string>(text => text.Contains("Light is turned off")));

        }

    }
}
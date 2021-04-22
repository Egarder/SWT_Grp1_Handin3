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
            _cookController.UI = _SUT;

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
        public void PowerBtnPressedOnceState_Ready()
        {
            _powerButton.Press();
            _displayFake.Received(1).ShowPower(50);
        }

        [Test]
        public void PowerBtnPressedTwiceState_Ready() 
        {
            _powerButton.Press();
            _powerButton.Press();
            _displayFake.Received(1).ShowPower(100); 
        }
        [Test]
        public void PowerBtnPressed3State_Ready()
        {
            _powerButton.Press();
            _powerButton.Press();
            _powerButton.Press();
            _displayFake.Received(1).ShowPower(150);
        }
        [Test]
        public void PowerBtnPressed7State_Ready()
        {

            _powerButton.Press();
            _powerButton.Press();
            _powerButton.Press();
            _powerButton.Press();
            _powerButton.Press();
            _powerButton.Press();
            _powerButton.Press();
            _displayFake.Received(1).ShowPower(50);
        }

        [Test]
        public void TimeBtnPressedOnceState_SetPower() 
        {
            _powerButton.Press();

            _timerButton.Press();

            _displayFake.Received(1).ShowTime(1,0);
        }
        [Test]
        public void TimeBtnPressed3State_SetPower() 
        {
            _powerButton.Press();

            _timerButton.Press();
            _timerButton.Press();
            _timerButton.Press();

            _displayFake.Received(1).ShowTime(3, 0);
        }

        [Test]
        public void StartCancelBtnPressedState_SetTime()
        {
            _powerButton.Press();
            _timerButton.Press();

            _startCancelButton.Press();

            _outputFake.Received(1).OutputLine(Arg.Is<string>(text => text.Contains("Light is turned on")));
            _timerFake.Received(1).Start(60);                                                                
            _powerTubeFake.Received(1).TurnOn(50);

            //Fake timer event OnTimerTick. Cookcontroller should call ShowTime at fake display. 
            _timerFake.TimerTick += Raise.Event();
            _displayFake.Received(1).ShowTime(1,0);

            //Fake timer event expired. Cookcontroller should turnoff fake powertube. 
            _timerFake.Expired += Raise.Event();
            _powerTubeFake.Received(1).TurnOff();

            //UI Should afterwards call fake displays Clear()
            _displayFake.Received(1).Clear();

            //UI should turnoff light:
            _outputFake.Received(1).OutputLine(Arg.Is<string>(text => text.Contains("Light is turned off")));

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
        public void StartCancelBtnPressedState_Cooking() /// ============================================================ FAILER
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
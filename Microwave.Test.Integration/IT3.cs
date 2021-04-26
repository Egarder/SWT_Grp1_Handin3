using System.Xml.Linq;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Microwave.Test.Integration
{
    // Emil
    public class IT3
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



        //User sequences from sequence diagram. 
        [Test]
        public void OpensDoorState_Ready_LightOn()
        {
            _door.Open();

            _outputFake.Received(1).OutputLine(Arg.Is<string>(text => text.Contains("Light is turned on")));
        }

        [Test]
        public void ClosesDoorState_DoorIsOpen_LightOff()
        {
            _door.Open();
            _door.Close();

            _outputFake.Received(1).OutputLine(Arg.Is<string>(text => text.Contains("turned off")));
        }
        [Test]
        public void PowerBtnPressedOnceState_Ready_DisplayShowPower()
        {
            _powerButton.Press();
            _displayFake.Received(1).ShowPower(50);
        }

        [Test]
        public void PowerBtnPressedTwiceState_Ready_DisplayShowPower() 
        {
            _powerButton.Press();
            _powerButton.Press();
            _displayFake.Received(1).ShowPower(100); 
        }
        [Test]
        public void PowerBtnPressed3State_Ready_DisplayShowPower()
        {
            _powerButton.Press();
            _powerButton.Press();
            _powerButton.Press();
            _displayFake.Received(1).ShowPower(150);
        }
        [Test]
        public void PowerBtnPressed7State_Ready_DisplayShowPower()
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
        public void TimeBtnPressedOnceState_SetPower_DisplayShowTime() 
        {
            _powerButton.Press();

            _timerButton.Press();

            _displayFake.Received(1).ShowTime(1,0);
        }
        [Test]
        public void TimeBtnPressed3State_SetPower_DisplayShowTime() 
        {
            _powerButton.Press();

            _timerButton.Press();
            _timerButton.Press();
            _timerButton.Press();

            _displayFake.Received(1).ShowTime(3, 0);
        }

        [Test]
        public void StartCancelBtnPressedState_SetTime_LightOn()
        {
            _powerButton.Press();
            _timerButton.Press();
            _startCancelButton.Press();

            _outputFake.Received(1).OutputLine(Arg.Is<string>(text => text.Contains("Light is turned on")));
        }

        [Test]
        public void StartCancelBtnPressedState_SetTime_FakesReceivedCalls()
        {
            _powerButton.Press();
            _timerButton.Press();
            _startCancelButton.Press();

            _timerFake.Received(1).Start(60);
            _powerTubeFake.Received(1).TurnOn(50);
        }

        [Test]
        public void StartCancelBtnPressedState_SetTime_TimerTickExpire_CookControllerCalls()
        {
            _powerButton.Press();
            _timerButton.Press();
            _startCancelButton.Press();

            //Fake timer event OnTimerTick. Cookcontroller should call ShowTime at fake display. 
            _timerFake.TimerTick += Raise.Event();
            _displayFake.Received(1).ShowTime(1, 0);

            //Fake timer event expired. Cookcontroller should turnoff fake powertube. 
            _timerFake.Expired += Raise.Event();
            _powerTubeFake.Received(1).TurnOff();

        }

        [Test]
        public void StartCancelBtnPressedState_SetTime_TimerExpired_DisplayClear_LightTurnOff()
        {
            _powerButton.Press();
            _timerButton.Press();
            _startCancelButton.Press();
            _timerFake.Expired += Raise.Event();

            //UI Should afterwards call fake displays Clear()
            _displayFake.Received(1).Clear();

            //UI should turnoff light:
            _outputFake.Received(1).OutputLine(Arg.Is<string>(text => text.Contains("Light is turned off")));
        }

        [Test]
        public void OpensDoorState_Cooking_FakesReceivedCalls() 
        {
            _powerButton.Press();
            _timerButton.Press();
            _startCancelButton.Press();

            _door.Open();

            _timerFake.Received(1).Stop();
            _powerTubeFake.Received(1).TurnOff();
            _displayFake.Received(1).Clear();
        }

        [Test]
        public void StartCancelBtnPressedState_Cooking_FakesReceivedCalls() 
        {
            _powerButton.Press();
            _timerButton.Press();
            _startCancelButton.Press();

            _startCancelButton.Press();

            _timerFake.Received(1).Stop();
            _powerTubeFake.Received(1).TurnOff();
            _displayFake.Received(1).Clear();
        }

        [Test]
        public void StartCancelBtnPressedState_Cooking_LightOff()
        {
            _powerButton.Press();
            _timerButton.Press();
            _startCancelButton.Press();

            _startCancelButton.Press();

            _outputFake.Received(1).OutputLine(Arg.Is<string>(text => text.Contains("Light is turned off")));
        }

        //======================================================================= Extension tests:

        //[Extension: 1. User pressed start-cancel button during setup]
        [Test]
        public void State_SetPower_Extension1StartCancelBtnPressed()
        {
            _powerButton.Press();
            _powerButton.Press();

            _startCancelButton.Press();

            _displayFake.Received(1).Clear();
        }

        //[Extension 2: The user opens the Door during setup]
        [Test]
        public void State_SetPower_Extension2DoorOpens()
        {
            _powerButton.Press();
            _powerButton.Press();

            _door.Open();
            
            _displayFake.Received(1).Clear();
            _outputFake.Received(1).OutputLine(Arg.Is<string>(text => text.Contains("Light is turned on")));
        }

        //[Extension 3: The user presses the Start-Cancel button during cooking]
        [Test]
        public void State_Cooking_Extension3StartCancelBtn()
        {
            _powerButton.Press();
            _timerButton.Press();
            _startCancelButton.Press();

            _startCancelButton.Press();

            _powerTubeFake.Received(1).TurnOff(); //Powetube turned off
            _displayFake.Received(1).Clear();//Display is blanked
            _outputFake.Received(1).OutputLine(Arg.Is<string>(text => text.Contains("Light is turned off")));//Light goes off
        }

        //[Extension 4: The user opens the Door during cooking]
        [Test]
        public void State_Cooking_Extension4DoorOpens()
        {
            _powerButton.Press();
            _timerButton.Press();
            _startCancelButton.Press();

            _door.Open();

            _powerTubeFake.Received(1).TurnOff(); //Powetube turned off
            _displayFake.Received(1).Clear();//Display is blanked
            _outputFake.Received(1).OutputLine(Arg.Is<string>(text => text.Contains("Light is turned on")));//Light goes off
        }
    }
}
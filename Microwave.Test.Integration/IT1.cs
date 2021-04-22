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
        private IOutput _output;

        [SetUp]
        public void Setup()
        {
            _door = new Door();
            _powerButton = new Button();
            _timeButton = new Button();
            _startCancelButton = new Button();
            _light = Substitute.For<ILight>();
            _output = Substitute.For<IOutput>();

        }

        //[Test]
        //public void Door_DoorOpened_LightOn()
        //{
        //    _door.Open();
        //    _light.Received(1).TurnOn();
        //}
        
    }
}

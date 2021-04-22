using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using NUnit.Framework;
using NSubstitute;

namespace Microwave.Test.Integration
{
    // Simon
    public class IT4
    {
        private UserInterface sut;

        private Button powerButton;
        private Button timeButton;
        private Button startCancelButton;
        private StringWriter sw;
        private Door door;

        private Display display;
        private Light light;

        private CookController cooker;
        private Output output;
        private Timer timer;
        private PowerTube powerTube;
        [SetUp]
        public void Setup()
        {
            powerButton = new Button();
            timeButton = new Button();
            startCancelButton = new Button();
            door = new Door();
            timer = new Timer();
            output = new Output();
            powerTube = new PowerTube(output);
            light = new Light(output);
            display = new Display(output);
            cooker = new CookController(timer, display, powerTube);
            sut = new UserInterface(powerButton, timeButton, startCancelButton, door, display, light, cooker);
            cooker.UI = sut;

            sw = new StringWriter();
            Console.SetOut(sw);
            Console.SetError(sw);
        }

        [Test]
        public void Ready_DoorOpen_LightOnShowsOnOutput()
        {
            // This test that uut has subscribed to door opened, and works correctly
            // simulating the event through NSubstitute
            var sw = new StringWriter();
            Console.SetOut(sw);
            Console.SetError(sw);

            door.Open();

            string result = sw.ToString();

            Assert.That(result,Is.EqualTo("Light is turned on\r\n"));
        }

        [Test]
        public void DoorOpen_DoorClose_LightOff()
        {
            // This test that uut has subscribed to door opened and closed, and works correctly
            // simulating the event through NSubstitute

            door.Open();
            door.Close();

            string result = sw.ToString();

            Assert.That(result, Is.EqualTo("Light is turned on\r\nLight is turned off\r\n"));
        }

        [Test]
        public void Ready_DoorOpenClose_Ready_PowerIs50()
        {
            // This test that uut has subscribed to power button, and works correctly
            // simulating the events through NSubstitute

            door.Open();
            door.Close();

            powerButton.Press();

            string result = sw.ToString();

            Assert.That(result, Is.EqualTo("Light is turned on\r\nLight is turned off\r\nDisplay shows: 50 W\r\n"));
        }

        [Test]
        public void Ready_2PowerButton_PowerIs100()
        {

            powerButton.Press();
            powerButton.Press();

            string result = sw.ToString();

            Assert.That(result, Is.EqualTo("Display shows: 50 W\r\nDisplay shows: 100 W\r\n"));
        }

        [Test]
        public void Ready_14PowerButton_PowerIs700()
        {
            for (int i = 1; i <= 14; i++)
            {
                powerButton.Press();
            }

            string result = sw.ToString();
            Assert.That(result, Is.EqualTo("Display shows: 50 W\r\nDisplay shows: 100 W\r\nDisplay shows: 150 W\r\nDisplay shows: 200 W\r\nDisplay shows: 250 W\r\nDisplay shows: 300 W\r\nDisplay shows: 350 W\r\nDisplay shows: 400 W\r\nDisplay shows: 450 W\r\nDisplay shows: 500 W\r\nDisplay shows: 550 W\r\nDisplay shows: 600 W\r\nDisplay shows: 650 W\r\nDisplay shows: 700 W\r\n"));
        }

        [Test]
        public void Ready_15PowerButton_PowerIs50Again()
        {
            for (int i = 1; i <= 15; i++)
            {
                powerButton.Press();
            }
            string result = sw.ToString();
            Assert.That(result, Is.EqualTo("Display shows: 50 W\r\nDisplay shows: 100 W\r\nDisplay shows: 150 W\r\nDisplay shows: 200 W\r\nDisplay shows: 250 W\r\nDisplay shows: 300 W\r\nDisplay shows: 350 W\r\nDisplay shows: 400 W\r\nDisplay shows: 450 W\r\nDisplay shows: 500 W\r\nDisplay shows: 550 W\r\nDisplay shows: 600 W\r\nDisplay shows: 650 W\r\nDisplay shows: 700 W\r\nDisplay shows: 50 W\r\n"));
        }

        [Test]
        public void SetPower_CancelButton_DisplayCleared()
        {
            // Also checks if TimeButton is subscribed
            powerButton.Press();
            // Now in SetPower

            startCancelButton.Press();

            string result = sw.ToString();

            Assert.That(result, Is.EqualTo("Display shows: 50 W\r\nDisplay cleared\r\n"));
        }

        [Test]
        public void SetPower_DoorOpened_DisplayCleared()
        {
            // Also checks if TimeButton is subscribed
            powerButton.Press();
            // Now in SetPower
            door.Open();

            string result = sw.ToString();

            Assert.That(result, Is.EqualTo("Display shows: 50 W\r\nLight is turned on\r\nDisplay cleared\r\n"));
        }

        [Test]
        public void SetPower_DoorOpened_LightOn()
        {
            // Also checks if TimeButton is subscribed
            powerButton.Press();
            // Now in SetPower
            door.Open();

            string result = sw.ToString();

            Assert.That(result, Is.EqualTo("Display shows: 50 W\r\nLight is turned on\r\nDisplay cleared\r\n"));
        }

        [Test]
        public void SetPower_TimeButton_TimeIs1()
        {
            // Also checks if TimeButton is subscribed
            powerButton.Press();
            // Now in SetPower
            timeButton.Press();

            string result = sw.ToString();

            Assert.That(result, Is.EqualTo("Display shows: 50 W\r\nDisplay shows: 01:00\r\n"));
        }

        [Test]
        public void SetPower_2TimeButton_TimeIs2()
        {
            powerButton.Press();
            // Now in SetPower
            timeButton.Press();
            timeButton.Press();

            string result = sw.ToString();

            Assert.That(result, Is.EqualTo("Display shows: 50 W\r\nDisplay shows: 01:00\r\nDisplay shows: 02:00\r\n"));
        }

        [Test]
        public void SetTime_StartButton_CookerIsCalled()
        {
            powerButton.Press();
            // Now in SetPower
            timeButton.Press();
            // Now in SetTime
            startCancelButton.Press();

            string result = sw.ToString();

            Assert.That(result, Is.EqualTo("Display shows: 50 W\r\nDisplay shows: 01:00\r\nLight is turned on\r\nPowerTube works with 50\r\n"));
        }

        [Test]
        public void SetTime_DoorOpened_DisplayCleared()
        {
            powerButton.Press();
            // Now in SetPower
            timeButton.Press();
            // Now in SetTime
            door.Open();

            string result = sw.ToString();

            Assert.That(result, Is.EqualTo("Display shows: 50 W\r\nDisplay shows: 01:00\r\nLight is turned on\r\nDisplay cleared\r\n"));
        }

        [Test]
        public void SetTime_DoorOpened_LightOn()
        {
            powerButton.Press();
            // Now in SetPower
            timeButton.Press();
            // Now in SetTime
            door.Open();

            string result = sw.ToString();

            Assert.That(result, Is.EqualTo("Display shows: 50 W\r\nDisplay shows: 01:00\r\nLight is turned on\r\nDisplay cleared\r\n"));
        }

        [Test]
        public void Ready_PowerAndTime_CookerIsCalledCorrectly()
        {
            powerButton.Press();
            // Now in SetPower
            powerButton.Press();

            timeButton.Press();
            // Now in SetTime
            timeButton.Press();

            // Should call with correct values
            startCancelButton.Press();

            string result = sw.ToString();

            Assert.That(result, Is.EqualTo("Display shows: 50 W\r\nDisplay shows: 100 W\r\nDisplay shows: 01:00\r\nDisplay shows: 02:00\r\nLight is turned on\r\nPowerTube works with 100\r\n"));
        }

        [Test]
        public void Ready_FullPower_CookerIsCalledCorrectly()
        {
            for (int i = 50; i <= 700; i += 50)
            {
                powerButton.Press();
            }

            timeButton.Press();
            // Now in SetTime
            var sw2 = new StringWriter();
            Console.SetOut(sw2);
            Console.SetError(sw2);

            startCancelButton.Press();
            // Should call with correct values
            string result = sw2.ToString();



            Assert.That(result, Is.EqualTo("Light is turned on\r\nPowerTube works with 700\r\n"));

        }


        [Test]
        public void SetTime_StartButton_LightIsCalled()
        {
            powerButton.Press();
            // Now in SetPower
            timeButton.Press();
            // Now in SetTime
            startCancelButton.Press();
            // Now cooking

            string result = sw.ToString();

            Assert.That(result, Is.EqualTo("Display shows: 50 W\r\nDisplay shows: 01:00\r\nLight is turned on\r\nPowerTube works with 50\r\n"));
        }

        [Test]
        public void Cooking_CookingIsDone_LightOffClearDisplay()
        {
            powerButton.Press();
            // Now in SetPower
            timeButton.Press();
            // Now in SetTime
            startCancelButton.Press();
            // Now in cooking
            sut.CookingIsDone();
            string result = sw.ToString();

            Assert.That(result, Is.EqualTo("Display shows: 50 W\r\nDisplay shows: 01:00\r\nLight is turned on\r\nPowerTube works with 50\r\nDisplay cleared\r\nLight is turned off\r\n"));
        }

        [Test]
        public void Cooking_DoorIsOpened_LightOn()
        {
            powerButton.Press();
            // Now in SetPower
            timeButton.Press();
            // Now in SetTime
            startCancelButton.Press();
            // Now in cooking

            // Open door
            door.Open();
            string result = sw.ToString();

            Assert.That(result, Is.EqualTo("Display shows: 50 W\r\nDisplay shows: 01:00\r\nLight is turned on\r\nPowerTube works with 50\r\nPowerTube turned off\r\nDisplay cleared\r\n"));
        }

        [Test]
        public void Cooking_DoorIsOpened_DisplayCleared()
        {
            powerButton.Press();
            // Now in SetPower
            timeButton.Press();
            // Now in SetTime
            startCancelButton.Press();
            // Now in cooking

            // Open door
            door.Open();

            string result = sw.ToString();

            Assert.That(result, Is.EqualTo("Display shows: 50 W\r\nDisplay shows: 01:00\r\nLight is turned on\r\nPowerTube works with 50\r\nPowerTube turned off\r\nDisplay cleared\r\n"));
        }

        [Test]
        public void Cooking_CancelButton_CookerCalledLightOffDisplayCleared()
        {
            powerButton.Press();
            // Now in SetPower
            timeButton.Press();
            // Now in SetTime
            startCancelButton.Press();
            // Now in cooking

            // Open door
            startCancelButton.Press();

            string result = sw.ToString();

            Assert.That(result, Is.EqualTo("Display shows: 50 W\r\nDisplay shows: 01:00\r\nLight is turned on\r\nPowerTube works with 50\r\nPowerTube turned off\r\nLight is turned off\r\nDisplay cleared\r\n"));
        }

    }

}
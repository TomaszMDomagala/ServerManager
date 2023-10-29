using System.Device.Gpio;
using System.Device.I2c;
using System.Device.Spi;
using System.Threading;
using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.PowerMode;
using Iot.Device.Adc;
using Iot.Device.Arduino;

namespace ServManager.GPIO
{
    public class GPIOController
    {
        public void ReadI2C(int busId, int DeviceAddress)
        {
            var i2cSettings = new I2cConnectionSettings(busId, DeviceAddress);
            using I2cDevice i2cDevice = I2cDevice.Create(i2cSettings);
            using var bme280 = new Bme280(i2cDevice);

            int measurementTime = bme280.GetMeasurementDuration();

            Console.Clear();
            bme280.SetPowerMode(Bmx280PowerMode.Forced);
            Thread.Sleep(measurementTime);

            bme280.TryReadTemperature(out var tempValue);
            bme280.TryReadPressure(out var preValue);
            bme280.TryReadHumidity(out var humValue);
            bme280.TryReadAltitude(out var altValue);

            Console.WriteLine($"Temperature: {tempValue.DegreesCelsius:0.#}\u00B0C");
            Console.WriteLine($"Pressure: {preValue.Hectopascals:#.##} hPa");
            Console.WriteLine($"Relative humidity: {humValue.Percent:#.##}%");
            Console.WriteLine($"Estimated altitude: {altValue.Meters:#} m");
        }

        public void ReadSPI()
        {
            var hardwareSpiSettings = new SpiConnectionSettings(0, 0);

            using SpiDevice spi = SpiDevice.Create(hardwareSpiSettings);
            using var mcp = new Mcp3008(spi);
            while (true)
            {
                Console.Clear();
                double value = mcp.Read(0);
                Console.WriteLine($"{value}");
                Console.WriteLine($"{Math.Round(value/10.23, 1)}%");
                Thread.Sleep(500);
            }
        }

        public void ChangePinOutput(int pin)
        {
            Console.WriteLine("Blinking LED. Press Ctrl+C to end.");
            using var controller = new GpioController();
            controller.OpenPin(pin, PinMode.Output);
            bool ledOn = true;
            while (true)
            {
                controller.Write(pin, ((ledOn) ? PinValue.High : PinValue.Low));
                Thread.Sleep(1000);
                ledOn = !ledOn;
            }
        }
    }


    // https://docs.arduino.cc/hacking/software/FirmataLibrary
    public class Arduino
    {
        private string PortName { get; set; }
        private int BaudRate { get; set; }
        private ArduinoBoard? arduino { get; set; } 

        public Arduino(string PortName, int BaudRate)
        {
            this.PortName = PortName;
            this.BaudRate = BaudRate;
        }

        public void ConnectToArduino()
        {
            arduino = new ArduinoBoard(PortName, BaudRate);
        }

        public void SetPin(int Pin, SupportedMode mode)
        {
            arduino?.SetPinMode(Pin, mode);
        }

        public string GetFirmataVersion()
        {
            if (arduino is not null)
            {
                return arduino.FirmataVersion.ToString();
            }
            return "";
        }

        public string GetFirmwareName()
        {
            if (arduino is not null)
            {
                return arduino.FirmwareName;
            }
            return "";
        }
    }
}
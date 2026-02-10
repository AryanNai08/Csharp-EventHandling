namespace TemperatureEventHandling
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Create publisher object (sensor)
            HeatSensor sensor = new HeatSensor();

            // Create subscriber (thermostat) and pass sensor
            // Constructor will subscribe to events
            Thermostat thermostat = new Thermostat(sensor);

            // Start temperature monitoring
            sensor.Run();

            // Keep console open
            Console.ReadKey();
        }
    }


    // ===================== SUBSCRIBER CLASS =====================
    // This class listens to events raised by HeatSensor
    public class Thermostat
    {
        // Constructor receives sensor and subscribes to its events
        public Thermostat(HeatSensor sensor)
        {
            sensor.WarningReached += OnWarning;
            sensor.EmergencyReached += OnEmergency;
            sensor.TemperatureNormal += OnNormal;
        }

        // Called when warning temperature reached
        private void OnWarning(object sender, TemperatureEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"⚠ {e.Temperature} Warning: Cooling ON");
            Console.ResetColor();
        }

        // Called when emergency temperature reached
        private void OnEmergency(object sender, TemperatureEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("🚨 Emergency: Device Shutdown");
            Console.ResetColor();
        }

        // Called when temperature becomes normal again
        private void OnNormal(object sender, TemperatureEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("ℹ Temperature Normal: Cooling OFF");
            Console.ResetColor();
        }
    }


    // ===================== PUBLISHER CLASS =====================
    // This class generates events when temperature changes
    public class HeatSensor
    {
        // STEP 1: Delegate
        //public delegate void TemperatureChangedHandler(object sender, TemperatureEventArgs e);

        // STEP 2: Events
        //public event TemperatureChangedHandler WarningReached;
        //public event TemperatureChangedHandler EmergencyReached;
        //public event TemperatureChangedHandler TemperatureNormal;


        //above is old/another way to declare events using custom delegate, below is the more common way using built-in EventHandler<T>



        // STEP 1: Declare events using built-in EventHandler<T>
        public event EventHandler<TemperatureEventArgs> WarningReached;
        public event EventHandler<TemperatureEventArgs> EmergencyReached;
        public event EventHandler<TemperatureEventArgs> TemperatureNormal;

        // Temperature thresholds
        private double warningLevel = 27;     // Warning level
        private double emergencyLevel = 75;   // Emergency level
        private bool wasWarning = false;      // Track previous warning state

        // Temperature data simulation
        double[] data = { 16, 17, 16.5, 18, 19, 22, 24, 26.75, 28.7, 27.6, 26, 24, 22, 45, 68, 86, 45 };

        // Method to simulate temperature reading
        public void Run()
        {
            foreach (var temp in data)
            {
                // Display current temperature
                Console.WriteLine($"Temperature: {temp}");

                // Create event args object
                TemperatureEventArgs e = new TemperatureEventArgs
                {
                    Temperature = temp,
                    Time = DateTime.Now
                };

                // STEP 2: Check temperature and raise events

                // Emergency condition
                if (temp >= emergencyLevel)
                {
                    // Raise Emergency event
                    EmergencyReached?.Invoke(this, e);
                }

                // Warning condition
                else if (temp >= warningLevel)
                {
                    wasWarning = true;

                    // Raise warning event
                    WarningReached?.Invoke(this, e);
                }

                // Back to normal after warning
                else if (wasWarning)
                {
                    wasWarning = false;

                    // Raise normal event
                    TemperatureNormal?.Invoke(this, e);
                }

                // Wait 1 second before next reading
                Thread.Sleep(1000);
            }
        }
    }

    // ===================== EVENT ARG CLASS =====================
    // Custom class to pass temperature data with event
    public class TemperatureEventArgs : EventArgs
    {
        public double Temperature { get; set; }  // Current temperature
        public DateTime Time { get; set; }       // Time of reading
    }
}

using System;
using System.Threading;   // Needed for Thread.Sleep

namespace EventHandling
{
    public class VideoEncoder
    {
        // Step 6: Define event
        // Subscribers will attach their methods here
        public event EventHandler VideoEncoded;

        public void Encode(Video video)
        {
            // Step 7: Control came here from Program.cs
            Console.WriteLine($"Video {video.Title} encoding...!");

            // Step 8: Simulate long process (3 sec)
            Thread.Sleep(3000);

            // Step 9: Raise event after encoding complete
            // 🔵 This calls ALL subscribed methods automatically
            VideoEncoded?.Invoke(this, EventArgs.Empty);
        }
    }
}

using System;                     // Allows Console, EventArgs etc.

namespace EventHandling
{
    class Program
    {
        static void Main(string[] args)   // 🔴 PROGRAM STARTS HERE
        {
            // Step 1: Create video object
            var video = new Video() { Title = "Vlog-1" };

            // Step 2: Create publisher (VideoEncoder)
            var videoEncoder = new VideoEncoder();

            // Step 3: Create subscribers
            var mailService = new MailService();
            var messageService = new MessageService();

            // Step 4: Subscribe methods to event
            // When VideoEncoded event happens → call these methods
            videoEncoder.VideoEncoded += mailService.OnVideoEncoded;
            videoEncoder.VideoEncoded += messageService.OnVideoEncoded;

            // Step 5: Start encoding process
            // 🔵 FLOW GOES TO VideoEncoder.cs → Encode() method
            videoEncoder.Encode(video);
        }
    }

    public class MailService
    {
        // Step 10: Called automatically when event fires
        public void OnVideoEncoded(object source, EventArgs e)
        {
            Console.WriteLine("MailService: Sending an email...");
        }
    }

    public class MessageService
    {
        // Step 11: Called automatically after MailService
        public void OnVideoEncoded(object source, EventArgs e)
        {
            Console.WriteLine("MessageService: Sending a message...");
        }
    }
}
using System;
using System.ComponentModel;
using System.IO;
using System.Threading;

namespace EventHandlerListUses
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create publisher object (Download manager)
            DownloadCreatorPub downloadCreatorPub = new DownloadCreatorPub();

            // Create subscriber object and pass publisher to it
            // Subscriber will attach to events inside constructor
            DonwloadListenSub donwloadListenSub = new DonwloadListenSub(downloadCreatorPub);

            // Start download with error URL (will fail)
            downloadCreatorPub.Start(new UrlEventArgs { Url = "http://example.com/error" });

            Console.WriteLine();

            // Start download with valid URL (will succeed)
            downloadCreatorPub.Start(new UrlEventArgs { Url = "http://example.com/main.csv" });
        }
    }

    // ======================= PUBLISHER CLASS =======================
    // This class generates events (start, progress, end)
    public class DownloadCreatorPub
    {
        // Unique keys for each event stored in EventHandlerList
        private static readonly object DownloadStart = new object();
        private static readonly object DownloadProgress = new object();
        private static readonly object DownloadEnd = new object();

        // EventHandlerList stores all events efficiently in one place
        private EventHandlerList events = new EventHandlerList();

        // ---------------- START DOWNLOAD EVENT ----------------
        public event EventHandler<UrlEventArgs> StartDownload
        {
            add
            {
                // Add subscriber method to EventHandlerList
                events.AddHandler(DownloadStart, value);
            }
            remove
            {
                // Remove subscriber method
                events.RemoveHandler(DownloadStart, value);
            }
        }

        // ---------------- PROGRESS DOWNLOAD EVENT ----------------
        public event EventHandler<UrlEventArgs> ProgressDownload
        {
            add
            {
                events.AddHandler(DownloadProgress, value);
            }
            remove
            {
                events.RemoveHandler(DownloadProgress, value);
            }
        }

        // ---------------- END DOWNLOAD EVENT ----------------
        public event EventHandler<UrlEventArgs> EndDownload
        {
            add
            {
                events.AddHandler(DownloadEnd, value);
            }
            remove
            {
                events.RemoveHandler(DownloadEnd, value);
            }
        }

        // Method to raise start event
        protected virtual void OnStartDownload()
        {
            // Get all methods subscribed to StartDownload
            var handler = (EventHandler<UrlEventArgs>)events[DownloadStart];

            // Invoke all subscriber methods if not null
            handler?.Invoke(this, new UrlEventArgs());
        }

        // Method to raise progress event
        protected virtual void OnProgressDownload(UrlEventArgs url)
        {
            var handler = (EventHandler<UrlEventArgs>)events[DownloadProgress];
            handler?.Invoke(this, url);
        }

        // Method to raise end event
        protected virtual void OnEndDownload()
        {
            var handler = (EventHandler<UrlEventArgs>)events[DownloadEnd];
            handler?.Invoke(this, new UrlEventArgs());
        }

        // Main method to start download
        public void Start(UrlEventArgs url)
        {
            Console.WriteLine($"Download Manager Start..!");

            // Raise start event (subscribers will respond)
            OnStartDownload();

            // Simulate delay
            Thread.Sleep(2000);

            // Check if URL contains error
            if (url.Url.Contains("error"))
            {
                Console.WriteLine($"File can't download!");
            }
            else
            {
                // Raise progress event
                OnProgressDownload(url);

                Thread.Sleep(2000);

                // Raise end event
                OnEndDownload();
            }

            Console.WriteLine($"Download Manager Stoped..!");
        }
    }

    // ======================= SUBSCRIBER CLASS =======================
    // This class listens to events from publisher
    public class DonwloadListenSub
    {
        // Constructor receives publisher object
        public DonwloadListenSub(DownloadCreatorPub downloadCreatorPub)
        {
            // Subscribe methods to publisher events
            downloadCreatorPub.StartDownload += DownloadCreatorPub_StartDownload;
            downloadCreatorPub.ProgressDownload += DownloadCreatorPub_ProgressDownload;
            downloadCreatorPub.EndDownload += DownloadCreatorPub_EndDownload;
        }

        // Method runs when StartDownload event fires
        private void DownloadCreatorPub_StartDownload(object? sender, EventArgs e)
        {
            Console.WriteLine($"File downloading....");
        }

        // Method runs during progress event
        private void DownloadCreatorPub_ProgressDownload(object? sender, UrlEventArgs e)
        {
            // Extract file name from URL
            string fileName = Path.GetFileName(e.Url);

            Console.WriteLine($"Donwload progress, File : {fileName}");
        }

        // Method runs when download completes
        private void DownloadCreatorPub_EndDownload(object? sender, EventArgs e)
        {
            Console.WriteLine($"File download completed.");
        }
    }

    // ======================= EVENT ARG CLASSES =======================

    // Event args class for file-based events (not used currently)
    public class FileEventArgs : EventArgs
    {
        public string FileName { get; set; }
    }

    // Event args class for URL-based events
    public class UrlEventArgs : EventArgs
    {
        public string Url { get; set; }
    }
}

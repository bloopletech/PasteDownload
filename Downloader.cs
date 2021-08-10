using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PasteDownload
{
    public class Downloader
    {
        private Download _download;
        private string _basePath;
        private Process _process;
        private TaskCompletionSource<bool> _eventHandled;

        public Downloader(Download download)
        {
            this._download = download;
        }

        // Print a file with any known extension.
        public async Task Download()
        {
            _basePath = Properties.Settings.Default.SaveLocation;

            _eventHandled = new TaskCompletionSource<bool>();

            using(_process = new Process())
            {
                try
                {
                    // Start a process to print a file and raise an event when done.
                    _process.StartInfo.FileName = Properties.Settings.Default.YoutubeDlLocation;
                    _process.StartInfo.Arguments = $"--print-json {EscapeArgument(_download.Url)}";
                    _process.StartInfo.WorkingDirectory = _basePath;
                    _process.StartInfo.UseShellExecute = false;
                    _process.StartInfo.CreateNoWindow = true;
                    _process.StartInfo.RedirectStandardOutput = true;
                    _process.EnableRaisingEvents = true;
                    _process.Exited += new EventHandler(_process_Exited);

                    _process.Start();

                    StreamReader reader = _process.StandardOutput;
#pragma warning disable CS4014
                    Task.Run(() =>
                    {
                        string line = "";
                        bool firstLine = true;
                        using(reader)
                        {
                            while((line = reader.ReadLine()) != null)
                            {
                                if(firstLine)
                                {
                                    firstLine = false;
                                    ParseYoutubeDlOutput(line);
                                }
                                Debug.Print(line);
                            }
                        }
                    });
#pragma warning restore CS4014
                }
                catch(Exception ex)
                {
                    Debug.Print($"An error occurred trying to download \"{_download.Url}\":\n{ex.Message}");
                    return;
                }

                // Wait for Exited events.
                await _eventHandled.Task;
            }
        }

        private void ParseYoutubeDlOutput(string line)
        {
            try
            {
                var json = JObject.Parse(line);
                string filename = json["_filename"].ToObject<string>();

                _download.Path = Path.Combine(_basePath, filename);
            }
            catch(Exception ex)
            {
                Debug.Print($"An error occurred trying to extract the filename from \"{line}\":\n{ex.Message}");
            }
        }

        // Handle Exited event and display process information.
        private void _process_Exited(object sender, System.EventArgs e)
        {
            Debug.Print(
                $"Exit time    : {_process.ExitTime}\n" +
                $"Exit code    : {_process.ExitCode}\n" +
                $"Elapsed time : {Math.Round((_process.ExitTime - _process.StartTime).TotalMilliseconds)}");
            
            if(_process.ExitCode == 0)
            {
                _download.Status = "completed";
            }
            else
            {
                _download.Status = "failed";
            }
            
            _eventHandled.TrySetResult(true);
        }

        private string EscapeArgument(string argument)
        {
            return "\"" + Regex.Replace(argument, @"(\\+)$", @"$1$1") + "\"";
        }
    }
}

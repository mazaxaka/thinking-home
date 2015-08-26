using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkingHome.Core.Plugins;
using ThinkingHome.Plugins.Scripts;

namespace ThinkingHome.Plugins.LocalAudioPlayer
{
    public struct Playlist
    {
        private int nowPlaying;
        private List<string> filePaths;
        private string dirPath;

        public Playlist(string directoryPath)
        {
            dirPath = directoryPath;
            if (System.IO.Directory.Exists(dirPath))
            {
                filePaths = new List<string>(System.IO.Directory.GetFiles(dirPath));
                nowPlaying = 0;
            }
            else
            {
                nowPlaying = 0;
                dirPath = "";
                filePaths = new List<string>();
            }
        }

        public void Refresh()
        {
            if (System.IO.Directory.Exists(dirPath))
            {
                filePaths = new List<string>(System.IO.Directory.GetFiles(dirPath));
                nowPlaying = nowPlaying < filePaths.Capacity ? nowPlaying : 0;
            }
        }

        public string NowPlaying()
        {
            if (nowPlaying == filePaths.Capacity)
                nowPlaying -= filePaths.Capacity;
            else if (nowPlaying == -1)
                nowPlaying += filePaths.Capacity;
            return filePaths[nowPlaying];
        }

        public string NextSong()
        {
            ++nowPlaying;
            return NowPlaying();
        }

        public string PrevSong()
        {
            --nowPlaying;
            return NowPlaying();
        }
    }

    [Plugin]
    public class LocalAudioPlayerPlugin : PluginBase
    {
        private readonly object lockObject = new object();
        private Playlist playlist;
        private BlockAlignReductionStream stream = null;
        //private DirectSoundOut output = null;
        private WaveOut output = null;
        private int deviceNumber = 0;
        private float volumeLevel = 1;
        private System.Threading.Thread volumeChangeThread = null;
        public override void InitPlugin()
        {
            playlist = new Playlist(@"c:\Audio\");
            Logger.Info("Plugin init");
        }

        public override void StartPlugin()
        {
            Logger.Info("Plugin starded");
        }

        public override void StopPlugin()
        {
            output.Dispose();
            output = null;
            Logger.Info("Plugin stopped");

        }

        /*public void volumeChange(int cmd) // 0 == stop || 1 == up || 2 == down
        {
            switch (cmd)
            {
                case 0:
                    if (volumeChangeThread != null)
                        volumeChangeThread.Suspend();
                    volumeChangeThread = null;
                    break;
                case 1:
                    volumeChangeThread = new System.Threading.Thread(() =>
                    {
                        volumeLevel += 0.01F;
                        
                    });
            }
        }*/

        public void PlaySound(string pathToSong)
        {
            Logger.Info("st1");
            lock (lockObject)
            {
                Logger.Info("st2");
                DisposeWave();
                Logger.Info("st3");
                WaveStream pcm = WaveFormatConversionStream.CreatePcmStream(new Mp3FileReader(pathToSong));
                Logger.Info("st3.1 " + pathToSong);
                stream = new BlockAlignReductionStream(pcm);
                Logger.Info("st3.2");
                output = new WaveOut();
                Logger.Info("st4 " + deviceNumber.ToString());
                //output.DeviceNumber = deviceNumber;
                Logger.Info("st5");
                output.Init(stream);
                Logger.Info("st6");
                output.Play();
            }
            Logger.Info("playing " + pathToSong);
            Logger.Info("playing to " + deviceNumber.ToString());
            output.PlaybackStopped += output_PlaybackStopped;
        }

        void output_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            Logger.Info("playback stopped event handler");
            new System.Threading.Thread(() =>
            {
                PlaySound(playlist.NextSong());
            }).Start();
        }

        private void DisposeWave()
        {
            if (output != null)
            {
                if (output.PlaybackState == NAudio.Wave.PlaybackState.Playing) output.Stop();
                output.Dispose();
                output = null;
            }
            if (stream != null)
            {
                stream.Dispose();
                stream = null;
            }
        }


        [ScriptCommand("multimedia")]
        public void AudioCommand(string command)
        {
            Logger.Info(command + " received");
            lock (lockObject)
            {
                switch (command)
                {
                    case "play":
                        PlaySound(playlist.NowPlaying());
                        break;
                    case "pause":
                        output.Pause();
                        break;
                    case "resume":
                        output.Play();
                        break;
                    case "stop":
                        output.PlaybackStopped -= output_PlaybackStopped;
                        output.Stop();
                        break;
                    case "skip":
                        output.PlaybackStopped -= output_PlaybackStopped;
                        PlaySound(playlist.NextSong());
                        break;
                    case "prev":
                        output.PlaybackStopped -= output_PlaybackStopped;
                        PlaySound(playlist.PrevSong());
                        break;
                    case "pausePlayResume":
                        if (output == null || output.PlaybackState == PlaybackState.Stopped)
                        {
                            Logger.Info("stopped to play");
                            PlaySound(playlist.NowPlaying());
                        }
                        else if (output.PlaybackState == PlaybackState.Playing)
                        {
                            Logger.Info("playing to pause");
                            output.Pause();
                        }

                        else if (output.PlaybackState == PlaybackState.Paused)
                        {
                            Logger.Info("paused to resume");
                            output.Play();
                        }
                        break;
                    case "refreshSongList":
                        playlist.Refresh();
                        break;
                    case "getPosition":
                        Logger.Info(stream.Position);
                        break;
                    case "getLength":
                        Logger.Info(stream.Length);
                        break;
                    case "getDeviceInfo":
                        for (int i = 0; i < WaveOut.DeviceCount; ++i)
                            Logger.Info(i + " " + WaveOut.GetCapabilities(i).ProductName);
                        break;
                    case "RollVolumeUp":

                        break;
                    case "RollVolumeDown":
                        break;

                }
            }
            Logger.Info(command + " executed");
        }

        [ScriptCommand("multimediaVolume")]
        public void AudioCommand(string command, string arg = "0.1")
        {
            Logger.Info(command + " " + arg + " recieved");
            float delta = float.Parse(arg);
            //lock (lockObject)
            // {
            switch (command)
            {
                case "set":
                    volumeLevel = delta;
                    break;
                case "up":
                    volumeLevel += delta;
                    break;
                case "down":
                    volumeLevel -= delta;
                    break;

            }
            Logger.Info(command + " approved");
            output.Volume = volumeLevel;
            Logger.Info(command + " executed");
            // }
        }

        [ScriptCommand("multimediaSetDevice")]
        public void SetDevice(string number)
        {
            Logger.Info("multimediaSetDevice recieved");
            deviceNumber = int.Parse(number);
            Logger.Info("multimediaSetDevice " + number + " executed");
        }
    }
}

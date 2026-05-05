#region File Header
/// <summary>
/// File: GifRecorder.cs
/// Description: Records search visualization as GIF animation
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-05-02
/// </summary>
#endregion

#region Namespace Imports
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using ImageMagick;
#endregion

namespace SallamPathFinder4.WinForms.Helpers
{
    public sealed class GifRecorder : IDisposable
    {
        private List<Image> _frames;
        private System.Windows.Forms. Timer _captureTimer;
        private Control _targetControl;
        private string _outputPath;
        private bool _isRecording;
        private int _frameDelayMs;
        private int _frameCount;

        public GifRecorder(int fps = 10)
        {
            _frames = new List<Image>();
            _frameDelayMs = 1000 / fps;
            _isRecording = false;
        }

        public int FrameCount => _frameCount;
        public event Action<int> FrameCaptured;
        public event Action<string> RecordingCompleted;

        public void StartRecording(Control targetControl, string outputPath)
        {
            if (_isRecording) return;
            _targetControl = targetControl;
            _outputPath = outputPath;
            _frames.Clear();
            _isRecording = true;
            _frameCount = 0;

            _captureTimer = new System.Windows.Forms. Timer();
            _captureTimer.Interval = _frameDelayMs;
            _captureTimer.Tick += (s, e) => CaptureFrame();
            _captureTimer.Start();
        }

        public void StopRecording()
        {
            if (!_isRecording) return;
            _captureTimer?.Stop();
            _captureTimer?.Dispose();
            _isRecording = false;
            SaveAsGif();
        }

        public void CaptureFrame()
        {
            if (!_isRecording || _targetControl == null) return;
            try
            {
                using (var bmp = new Bitmap(_targetControl.Width, _targetControl.Height))
                {
                    _targetControl.DrawToBitmap(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
                    _frames.Add(new Bitmap(bmp));
                    _frameCount++;
                    FrameCaptured?.Invoke(_frameCount);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Capture error: {ex.Message}");
            }
        }

        private void SaveAsGif()
        {
            if (_frames.Count == 0) return;
            try
            {
                using (var collection = new MagickImageCollection())
                {
                    foreach (var frame in _frames)
                    {
                        using (var ms = new MemoryStream())
                        {
                            frame.Save(ms, ImageFormat.Png);
                            ms.Position = 0;
                            var img = new MagickImage(ms);
                            img.AnimationDelay = _frameDelayMs / 10;
                            collection.Add(img);
                        }
                    }
                    collection.Optimize();
                    collection.Write(_outputPath, MagickFormat.Gif);
                }
                RecordingCompleted?.Invoke(_outputPath);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GIF save error: {ex.Message}");
                if (_frames.Count > 0)
                {
                    string pngPath = Path.ChangeExtension(_outputPath, ".png");
                    _frames[0].Save(pngPath, ImageFormat.Png);
                }
            }
            finally
            {
                foreach (var frame in _frames) frame.Dispose();
                _frames.Clear();
            }
        }

        public void Dispose() => StopRecording();
    }
}
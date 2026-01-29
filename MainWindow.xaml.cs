using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using AudioRecorder.CoreAudio;
using Path = System.IO.Path;

namespace AudioRecorder
{
    public partial class MainWindow : Window
    {
        private NaudioLoopbackRecorder? _audioRecorder;
        private Thread? _recordingThread;
        private bool _isRecording = false;
        private readonly string _recordingsFolder;
        private DateTime _startTime;
        private string _selectedFilePath = "";
        private string _audioFormat = "wav";
        private bool _isCustomPath = false;

        public MainWindow()
        {
            InitializeComponent();
            _recordingsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Recordings");
            if (!Directory.Exists(_recordingsFolder))
            {
                Directory.CreateDirectory(_recordingsFolder);
            }
            UpdateFilePath();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                DragMove();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isRecording)
            {
                StopRecording();
            }
            Close();
        }

        private void FilePathText_Click(object sender, MouseButtonEventArgs e)
        {
            SelectPath();
        }

        private void UpdateFilePath()
        {
            if (string.IsNullOrEmpty(_selectedFilePath))
            {
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string extension = _audioFormat.ToLower() == "mp3" ? ".mp3" : ".wav";
                _selectedFilePath = Path.Combine(_recordingsFolder, $"recording_{timestamp}{extension}");
            }
            FilePathText.Text = _selectedFilePath;
        }

        private void RecordButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isRecording)
            {
                StopRecording();
            }
            else
            {
                StartRecording();
            }
        }

        private void SelectPath()
        {
            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "WAV音频文件 (*.wav)|*.wav|MP3音频文件 (*.mp3)|*.mp3",
                DefaultExt = ".wav",
                FileName = Path.GetFileName(_selectedFilePath),
                InitialDirectory = Path.GetDirectoryName(_selectedFilePath)
            };

            if (saveDialog.ShowDialog() == true)
            {
                _selectedFilePath = saveDialog.FileName;
                string extension = Path.GetExtension(_selectedFilePath).ToLower();
                _audioFormat = extension == ".mp3" ? "mp3" : "wav";
                _isCustomPath = true;
                FilePathText.Text = _selectedFilePath;
            }
        }

        private void StartRecording()
        {
            try
            {
                if (string.IsNullOrEmpty(_selectedFilePath))
                {
                    UpdateFilePath();
                }

                // 如果文件已存在，自动重命名
                _selectedFilePath = GetUniqueFilePath(_selectedFilePath);
                FilePathText.Text = _selectedFilePath;

                _audioRecorder = new NaudioLoopbackRecorder(_selectedFilePath, _audioFormat);

                if (_audioRecorder.Start())
                {
                    _isRecording = true;
                    _startTime = DateTime.Now;
                    UpdateRecordButtonUI(true);

                    _recordingThread = new Thread(RecordingLoop);
                    _recordingThread.IsBackground = true;
                    _recordingThread.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"错误: {ex.Message}", "录音错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetUniqueFilePath(string originalPath)
        {
            if (!File.Exists(originalPath))
            {
                return originalPath;
            }

            string directory = Path.GetDirectoryName(originalPath) ?? "";
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(originalPath);
            string extension = Path.GetExtension(originalPath);

            int counter = 1;
            string newPath;

            do
            {
                newPath = Path.Combine(directory, $"{fileNameWithoutExt}_{counter}{extension}");
                counter++;
            }
            while (File.Exists(newPath));

            return newPath;
        }

        private void StopRecording()
        {
            _isRecording = false;
            _audioRecorder?.Stop();

            Dispatcher.Invoke(() =>
            {
                UpdateRecordButtonUI(false);
                // 如果不是自定义路径，重置文件路径，下次录音生成新的时间戳文件名
                if (!_isCustomPath)
                {
                    _selectedFilePath = "";
                    UpdateFilePath();
                }
            });
        }

        private void UpdateRecordButtonUI(bool isRecording)
        {
            var template = RecordButton.Template;
            if (template == null) return;

            var buttonEllipse = template.FindName("ButtonEllipse", RecordButton) as System.Windows.Shapes.Ellipse;
            var iconPath = template.FindName("IconPath", RecordButton) as System.Windows.Shapes.Path;
            var timeText = template.FindName("TimeText", RecordButton) as TextBlock;

            if (isRecording)
            {
                if (buttonEllipse != null)
                    buttonEllipse.Fill = new SolidColorBrush(Color.FromRgb(220, 53, 69));
                if (iconPath != null)
                    iconPath.Visibility = Visibility.Collapsed;
                if (timeText != null)
                    timeText.Visibility = Visibility.Visible;
            }
            else
            {
                if (buttonEllipse != null)
                    buttonEllipse.Fill = new SolidColorBrush(Color.FromRgb(0, 120, 212));
                if (iconPath != null)
                    iconPath.Visibility = Visibility.Visible;
                if (timeText != null)
                {
                    timeText.Visibility = Visibility.Collapsed;
                    timeText.Text = "";
                }
            }
        }

        private void RecordingLoop()
        {
            while (_isRecording)
            {
                Thread.Sleep(100);
                TimeSpan elapsed = DateTime.Now - _startTime;
                Dispatcher.Invoke(() =>
                {
                    var template = RecordButton.Template;
                    if (template != null)
                    {
                        var timeText = template.FindName("TimeText", RecordButton) as TextBlock;
                        if (timeText != null)
                        {
                            timeText.Text = elapsed.ToString(@"hh\:mm\:ss");
                        }
                    }
                });
            }
        }
    }
}
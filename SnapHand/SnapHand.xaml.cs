using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WindowsPreview.Kinect;
using LightBuzz.Vitruvius;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SnapHand
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SnapHand : Page
    {
        KinectSensor _sensor;
        MultiSourceFrameReader _reader;
        PlayersController _playersController;
        

        public SnapHand()
        {
            this.InitializeComponent();

            _sensor = KinectSensor.GetDefault();

            if (_sensor != null)
            {
                _sensor.Open();

                _reader = _sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Depth | FrameSourceTypes.Infrared | FrameSourceTypes.Body);
                _reader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;

                _playersController = new PlayersController();
                _playersController.BodyEntered += UserReporter_BodyEntered;
                _playersController.BodyLeft += UserReporter_BodyLeft;
                _playersController.Start();
            }
            viewer.Visualization = Visualization.Color;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            string label = Quit_Button.Content.ToString(); 
           
            if (label.Equals("SAIR")){
                this.Frame.Navigate(typeof(MainPage), null);
            }
        }

        void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            var reference = e.FrameReference.AcquireFrame();

            // Color
            using (var frame = reference.ColorFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    if (viewer.Visualization == Visualization.Color)
                    {
                        viewer.Image = frame.ToBitmap();
                    }
                }
            }

            // Depth
            using (var frame = reference.DepthFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    if (viewer.Visualization == Visualization.Depth)
                    {
                        viewer.Image = frame.ToBitmap();
                    }
                }
            }

            // Infrared
            using (var frame = reference.InfraredFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    if (viewer.Visualization == Visualization.Infrared)
                    {
                        viewer.Image = frame.ToBitmap();
                    }
                }
            }

            // Body
            using (var frame = reference.BodyFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    var bodies = frame.Bodies();

                    _playersController.Update(bodies);

                    foreach (Body body in bodies)
                    {
                        viewer.DrawBody(body);
                    }
                }
            }
        }

        void UserReporter_BodyEntered(object sender, UsersControllerEventArgs e)
        {
            // A new user has entered the scene.
        }

        void UserReporter_BodyLeft(object sender, UsersControllerEventArgs e)
        {
            // A user has left the scene.
            viewer.Clear();
        }

        private void Click1(object sender, RoutedEventArgs e)
        {
            string label = Save_Button.Content.ToString();

            if (label.Equals("GRAVAR"))
            {
                RecordClick();
            }
            else 
            {
                SaveClick();
            }
        }

        private void Click2(object sender, RoutedEventArgs e)
        {
            string label = Quit_Button.Content.ToString();

            if (label.Equals("SAIR"))
            {
                Back_Click(null, null);
            } 
            else
            {
                CancelClick();
            }

        }

        private void RecordClick()
        {
            Save_Button.Content = "EFETIVAR SESSÃO";
            Quit_Button.Content = "CANCELAR SESSÃO";
            WorkText.Text = "Gravando";
        }

        private void SaveClick()
        {
            Save_Button.Content = "GRAVAR";
            Quit_Button.Content = "SAIR";
            WorkText.Text = "";
        }

        private void CancelClick()
        {
            Save_Button.Content = "GRAVAR";
            Quit_Button.Content = "SAIR";
            WorkText.Text = "";
        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }
    }
}

using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmguCv.FaceRecognition
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //declaring global variables
        private Capture capture;        //takes images from camera as image frames
        private bool captureInProgress; // checks if capture is executing


        private void ProcessFrame(object sender, EventArgs arg)
        {
            Image<Bgr, Byte> ImageFrame = capture.QueryFrame().ToImage<Bgr, Byte>();  //line 1
            picCamera.Image = ImageFrame.Bitmap;       //line 2
        }

        private void ReleaseData()
        {
            if (capture != null)
                capture.Dispose();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            #region if capture is not created, create it now
            if (capture == null)
            {
                try
                {
                    capture = new Capture();
                }
                catch (NullReferenceException excpt)
                {
                    MessageBox.Show(excpt.Message);
                }
            }
            #endregion

            if (capture != null)
            {
                if (captureInProgress)
                {  //if camera is getting frames then stop the capture and set button Text
                    // "Start" for resuming capture
                    btnStart.Text = "Start!"; //
                    Application.Idle -= ProcessFrame;
                }
                else
                {
                    //if camera is NOT getting frames then start the capture and set button
                    // Text to "Stop" for pausing capture
                    btnStart.Text = "Stop";
                    Application.Idle += ProcessFrame;
                }

                captureInProgress = !captureInProgress;
            }
        }
    }
}

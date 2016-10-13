using Emgu.CV;
using Emgu.Util;
using Emgu.CV.CvEnum;
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
using FaceDetection;

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
        private Mat myMatImage;
        private CascadeClassifier haar = new CascadeClassifier("haarcascade_frontalface_default.xml");

        Image<Bgr, Byte> myImageFrame;
        
        private void ProcessFrame(object sender, EventArgs arg)
        {
            myImageFrame = capture.QueryFrame().ToImage<Bgr, Byte>();  //line 1
            myMatImage = myImageFrame.Mat;
            picCamera.Image = myImageFrame.Bitmap; //line 2
            
            FaceRecog();
        }

        private void ReleaseData()
        {
            if (capture != null)
                capture.Dispose();
        }

        
        private void FaceRecog()
        {
            bool tryUseCuda = false;
            long detectionTime;
            List<Rectangle> faces = new List<Rectangle>();
            List<Rectangle> eyes = new List<Rectangle>();
            DetectFace.Detect(myMatImage, "haarcascade_frontalface_default.xml", faces, tryUseCuda, out detectionTime);
            foreach (Rectangle face in faces)
                CvInvoke.Rectangle(myMatImage, face, new Bgr(Color.Red).MCvScalar, 2);
            foreach (Rectangle eye in eyes)
                CvInvoke.Rectangle(myMatImage, eye, new Bgr(Color.Blue).MCvScalar, 2);


            //using (var imageFrame = myImageFrame)
            //{
            //    if (imageFrame != null)
            //    {
            //        var grayframe = imageFrame.Convert<Gray, byte>();
            //        var faces = haar.DetectMultiScale(grayframe, 1.1, 10, Size.Empty); //the actual face detection happens here
            //        foreach (var face in faces)
            //        {
            //            imageFrame.Draw(face, new Bgr(Color.BurlyWood), 3); //the detected face(s) is highlighted here using a box that is drawn around it/them

            //        }
            //    }
            //    picCamera.Image = imageFrame.Bitmap;
            //}


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

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
using System.IO;
using Emgu.CV.Face;

namespace EmguCv.FaceRecognition
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            try
            {
                //Load of previus trainned faces and labels for each image
                string Labelsinfo = File.ReadAllText(Application.StartupPath + "/TrainedFaces/TrainedLabels.txt");
                string[] Labels = Labelsinfo.Split('%');
                NumLabels = Convert.ToInt16(Labels[0]);
                ContTrain = NumLabels;
                string LoadFaces;

                for (int tf = 1; tf < NumLabels + 1; tf++)
                {
                    LoadFaces = "face" + tf + ".bmp";
                    trainingImages.Add(new Image<Gray, byte>(Application.StartupPath + "/TrainedFaces/face/" + LoadFaces));
                    labels.Add(Labels[tf]);
                }

            }
            catch (Exception e)
            {

                MessageBox.Show("Nothing in binary database, please add at least a face(Simply train the prototype with the Add Face Button).", "Triained faces load", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        //declaring global variables
        private Capture capture;        //takes images from camera as image frames
        private bool captureInProgress; // checks if capture is executing
        private Mat myMatImage;
        private CascadeClassifier haar = new CascadeClassifier("haarcascade_frontalface_default.xml");
        

        Image<Gray, byte> result, TrainedFace = null;
        List<Image<Gray, byte>> trainingImages = new List<Image<Gray, byte>>();
        List<string> labels = new List<string>();
        List<string> NamePersons = new List<string>();
        int ContTrain, NumLabels, t;
        string name, names = null;

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
            {
                CvInvoke.Rectangle(myMatImage, face, new Bgr(Color.Red).MCvScalar, 2);
                myImageFrame.ROI = face;
                
            }
                
            


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

        private void btnCaptured_Click(object sender, EventArgs e)
        {
           
            
            Image<Gray, Byte> grayed = new Image<Gray, byte>(myImageFrame.Resize(320, 240, Inter.Cubic).Bitmap);
            
            picCaptured.Image = grayed.Bitmap;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                //Trained face counter
                ContTrain++;

                Image<Gray, Byte> grayed = new Image<Gray, byte>(myImageFrame.Resize(320, 240, Inter.Cubic).Bitmap);
                TrainedFace = grayed.Resize(100, 100, Inter.Cubic);
                trainingImages.Add(TrainedFace);
                labels.Add(textBox1.Text);
                picCaptured.Image = TrainedFace.Bitmap;

                File.WriteAllText(Application.StartupPath + "/TrainedFaces/TrainedLabels.txt", trainingImages.ToArray().Length.ToString() + "%");

                //Write the labels of triained faces in a file text for further load
                for (int i = 1; i < trainingImages.ToArray().Length + 1; i++)
                {
                    trainingImages.ToArray()[i - 1].Save(Application.StartupPath + "/TrainedFaces/face/face" + i + ".bmp");
                    File.AppendAllText(Application.StartupPath + "/TrainedFaces/TrainedLabels.txt", labels.ToArray()[i - 1] + "%");
                }

                MessageBox.Show(textBox1.Text + "´s face detected and added :)", "Training OK", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }




        




        



    }
}






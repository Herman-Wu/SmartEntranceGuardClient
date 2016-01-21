using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RetailDemoWP.Services
{
    public class NamedFace : Face
    {
        public NamedFace(Face face)
        {
            FaceId = face.FaceId;
            FaceRectangle = face.FaceRectangle;
            FacialLandmarks = face.FacialLandmarks;
            Attributes = face.Attributes;
        }

        public String Name;
    }
    public class ImageAnalyzer
    {
        private string keyId = "e48f5878946846d2a7ab4452b5b4d324";
        private readonly IFaceServiceClient faceDetector;

        public ImageAnalyzer()
        {
            faceDetector = new FaceServiceClient(keyId);
        }

        public async Task<NamedFace[]> AnalyzeImageUsingHelper(Stream fileStream)
        {

            Face[] faces = await faceDetector.DetectAsync(fileStream, false, true, true, false);

            NamedFace[] namedFaces = new NamedFace[faces.Length];

            //Copy to named faces vector.           
            for (int i = 0; i < faces.Length; i++)
            {
                namedFaces[i] = new NamedFace(faces[i]);
            }
            return namedFaces;
        }


        public async Task TrainFaceDetector()
        {

            string personGroupId = "coworkers";
            try
            {
                await faceDetector.CreatePersonGroupAsync(personGroupId, "MyCoworkers");
            }
            catch (Exception e)
            {
                //if this fails, it is probably that the group was created already.
            }

            // TODO: Get this directory from the config file.
           // string currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            //DirectoryInfo dirInfo = new DirectoryInfo( currentDir + @"\train");
            DirectoryInfo dirInfo = new DirectoryInfo(@"C:\Users\chingch\Pictures\Train");
            foreach (DirectoryInfo personDir in dirInfo.EnumerateDirectories())
            {
                List<Face> friendFaces = new List<Face>();

                if (personDir.Name != "Ching") continue;

                foreach (FileInfo imageFile in personDir.GetFiles("*.*"))
                {
                    using (Stream s = File.OpenRead(imageFile.FullName))
                    {

                        await Task.Delay(3000);
                        Face[] faces;
                        faces = await faceDetector.DetectAsync(s);

                        if (faces.Length == 0)
                        {
                            // No face detected
                            Debug.WriteLine("No face detected in {0}.", imageFile.FullName);
                            continue;
                        }

                        //Assume the image contains only one face
                        friendFaces.Add(faces[0]);
                    }
                }

                var friendFaceIds = friendFaces.Select(face => face.FaceId).ToArray();
                PersonCreationResponse friend = await faceDetector.CreatePersonAsync(personGroupId, friendFaceIds, personDir.Name);
            }

            await faceDetector.TrainPersonGroupAsync(personGroupId);

            TrainingStatus trainingStatus = null;
            while (true)
            {
                trainingStatus = await faceDetector.GetPersonGroupTrainingStatusAsync(personGroupId);

                if (trainingStatus.Status != "running")
                {
                    break;
                }

                await Task.Delay(1000);
            }

        }
    }
}

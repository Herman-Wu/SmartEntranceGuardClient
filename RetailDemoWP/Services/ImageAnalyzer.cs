using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using Microsoft.ProjectOxford.Vision;
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
            FaceLandmarks = face.FaceLandmarks;
            FaceAttributes = face.FaceAttributes;
        }

        public String Name;
    }
    public class ImageAnalyzer
    {
        private string keyId = "14c8ed8ea9b74f0fbab8c89deb7cf5d1";
        private readonly IFaceServiceClient faceDetector;
        private readonly IVisionServiceClient visionDetector;
        private string GroupName = "adb69b6f-3e5d-427d-87ce-a7b4044285ff";

        public ImageAnalyzer()
        {
            faceDetector = new FaceServiceClient(keyId);
            visionDetector = new VisionServiceClient(keyId);
        }

        public async Task<NamedFace[]> AnalyzeImageUsingHelper(Stream fileStream)
        {
            NamedFace[] namedFaces = null;
            try
            {
               // Face[] faces = null;
               
               
                Face[] faces = await faceDetector.DetectAsync(fileStream); //, false, true, new FaceAttributeType[] { FaceAttributeType.Gender, FaceAttributeType.Age, FaceAttributeType.Smile, FaceAttributeType.Glasses }
                namedFaces = new NamedFace[faces.Length];
                Guid[] collectionid = new Guid[faces.Length];
                //Copy to named faces vector.           
                for (int i = 0; i < faces.Length; i++)
                {
                    namedFaces[i] = new NamedFace(faces[i]);
                    collectionid[i] = namedFaces[i].FaceId;
                }

                var faceIds = faces.Select(face => face.FaceId).ToArray();
                string groupname = @"00000000-0000-0000-0000-000000000000";
                var results = await faceDetector.IdentifyAsync(groupname, faces.Select(ff => ff.FaceId).ToArray());
                foreach (var identifyResult in results)
                {
                    Debug.WriteLine("Result of face: {0}", identifyResult.FaceId);
                    if (identifyResult.Candidates.Length == 0)
                    {
                        Debug.WriteLine("No one identified");
                    }
                    else
                    {
                        // Get top 1 among all candidates returned
                        var candidateId = identifyResult.Candidates[0].PersonId;
                        var person = await faceDetector.GetPersonAsync(groupname, candidateId);
                        Debug.WriteLine("Identified as {0}", person.Name);
                    }
                }


            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
            }
            return namedFaces;
        }

        public async Task<Microsoft.ProjectOxford.Vision.Contract.AnalysisResult> AnalyzeVisionUsingHelper(Stream fileStream)
        {
            Microsoft.ProjectOxford.Vision.Contract.AnalysisResult adultResult = null;
            try
            {
                adultResult = await visionDetector.AnalyzeImageAsync(fileStream, new VisualFeature[] { VisualFeature.Adult });
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
            }
            return adultResult;
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
                //PersonCreationResponse
                //PersonCreationResponse friend = await faceDetector.CreatePersonAsync(personGroupId, personDir.Name);
            }

            await faceDetector.TrainPersonGroupAsync(personGroupId);

            TrainingStatus trainingStatus = null;
            while (true)
            {
                trainingStatus = await faceDetector.GetPersonGroupTrainingStatusAsync(personGroupId);

                if (trainingStatus.Status != Status.Running)
                {
                    break;
                }

                await Task.Delay(1000);
            }

        }
    }
}

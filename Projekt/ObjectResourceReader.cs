using GrafikaSzeminarium;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Szeminarium
{
    internal static class ObjectResourceReader
    {
        public static void CreateObjectFromResource(GL Gl, string resourceName, out float[] vertexArray, out float[] colorArray, out uint[] indexArray)
        {
            List<float[]> objVertices = new List<float[]>();
            List<int[]> objFaces = new List<int[]>();
            List<float[]> objNormals = new List<float[]>();
            List<int[]> objFaceNormals = new List<int[]>();

            bool hasNormals = false;

            string fullResourceName = "Projekt.Resources." + resourceName;
            using (var objStream = typeof(ObjectResourceReader).Assembly.GetManifestResourceStream(fullResourceName))
            using (var objReader = new StreamReader(objStream))
            {
                while (!objReader.EndOfStream)
                {
                    var line = objReader.ReadLine();

                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    if (line.StartsWith("#"))
                        continue;

                    var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 0)
                        continue;

                    var lineClassifier = parts[0];
                    var lineData = parts.Skip(1).ToArray();

                    switch (lineClassifier)
                    {
                        case "v":
                            float[] vertex = new float[3];
                            for (int i = 0;i<vertex.Length; ++i)
                                vertex[i] = float.Parse(lineData[i], CultureInfo.InvariantCulture);
                            objVertices.Add(vertex);
                            break;
                        case "vn":
                            hasNormals = true;
                            float[] normal = new float[3];
                            for (int i = 0; i < normal.Length; ++i)
                                normal[i] = float.Parse(lineData[i], CultureInfo.InvariantCulture);
                            objNormals.Add(normal);
                            break;
                        case "f":
                            List<int> face = new List<int>();
                            List<int> faceNormals = new List<int>();
                            for (int i = 0; i < lineData.Length; ++i)
                            {
                                string[] indices = lineData[i].Split('/');

                                if (indices.Length > 0 && !string.IsNullOrEmpty(indices[0]))
                                {
                                    int vertexIndex = int.Parse(indices[0], CultureInfo.InvariantCulture);
                                    face.Add(vertexIndex);

                                    int normalIndex = -1;
                                    if (indices.Length >= 3 && !string.IsNullOrEmpty(indices[2]))
                                    {
                                        normalIndex = int.Parse(indices[2], CultureInfo.InvariantCulture);
                                    }
                                    else if (indices.Length == 2 && !string.IsNullOrEmpty(indices[1]))
                                    {
    
                                        normalIndex = int.Parse(indices[1], CultureInfo.InvariantCulture);
                                    }
                                    faceNormals.Add(normalIndex);

                                }
                               
                            }

                            for (int i = 1; i < face.Count - 1; ++i)
                                {
                                    int[] triangleFace =
                                    [
                                        face[0], 
                                        face[i], 
                                        face[i + 1] 
                                    ];
                                    
                                    int[] triangleNormals =
                                    [
                                        faceNormals[0], 
                                        faceNormals[i], 
                                        faceNormals[i + 1] 
                                    ];
                                    
                                    objFaces.Add(triangleFace);
                                    objFaceNormals.Add(triangleNormals);
                                }

                            objFaces.Add(face.ToArray());
                            objFaceNormals.Add(faceNormals.ToArray());
                            break;
                        default:
                            break;
                    }
                }
            }

            List<ObjVertexTransformationData> vertexTransformations = new List<ObjVertexTransformationData>();
            foreach (var objVertex in objVertices)
            {
                vertexTransformations.Add(new ObjVertexTransformationData(
                    new Vector3D<float>(objVertex[0], objVertex[1], objVertex[2]),
                    Vector3D<float>.Zero,
                    0
                    ));
            }

            if (hasNormals)
            {
                for (int faceIndex = 0; faceIndex < objFaces.Count; faceIndex++)
                {
                    var face = objFaces[faceIndex];
                    var normalIndices = objFaceNormals[faceIndex];

                    for (int i = 0; i < 3; i++)
                    {
                        if (normalIndices[i] > 0)
                        {
                            var vertexIndex = face[i] - 1;
                            var normalIndex = normalIndices[i] - 1;
                            
                            if (normalIndex < objNormals.Count)
                            {
                                var normal = objNormals[normalIndex];
                                vertexTransformations[vertexIndex].UpdateNormalWithContributionFromAFace(
                                    new Vector3D<float>(normal[0], normal[1], normal[2])
                                );
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var objFace in objFaces)
                {
                    var a = vertexTransformations[objFace[0] - 1];
                    var b = vertexTransformations[objFace[1] - 1];
                    var c = vertexTransformations[objFace[2] - 1];

                    var normal = Vector3D.Normalize(Vector3D.Cross(b.Coordinates - a.Coordinates, c.Coordinates - a.Coordinates));

                    a.UpdateNormalWithContributionFromAFace(normal);
                    b.UpdateNormalWithContributionFromAFace(normal);
                    c.UpdateNormalWithContributionFromAFace(normal);
                }
            }

            

            List<float> glVertices = new List<float>();
            List<float> glColors = new List<float>();
            foreach (var vertexTransformation in vertexTransformations)
            {
                glVertices.Add(vertexTransformation.Coordinates.X);
                glVertices.Add(vertexTransformation.Coordinates.Y);
                glVertices.Add(vertexTransformation.Coordinates.Z);

                glVertices.Add(vertexTransformation.Normal.X);
                glVertices.Add(vertexTransformation.Normal.Y);
                glVertices.Add(vertexTransformation.Normal.Z);

                glColors.AddRange([1.0f, 0.0f, 0.0f, 1.0f]);
            }

            List<uint> glIndexArray = new List<uint>();
            foreach (var objFace in objFaces)
            {
                glIndexArray.Add((uint)(objFace[0] - 1));
                glIndexArray.Add((uint)(objFace[1] - 1));
                glIndexArray.Add((uint)(objFace[2] - 1));
            }


            vertexArray = glVertices.ToArray();
            colorArray = glColors.ToArray();
            indexArray = glIndexArray.ToArray();
        }
    }
}
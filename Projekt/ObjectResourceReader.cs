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
    public class MaterialGroup
    {
        public string MaterialName { get; set; }
        public List<int[]> Faces { get; set; } = new List<int[]>();
        public List<int[]> FaceNormals { get; set; } = new List<int[]>();
        public List<int[]> FaceTexCoords { get; set; } = new List<int[]>();
    }

    internal static class ObjectResourceReader
    {
        public static void CreateObjectFromResource(GL Gl, string resourceName, out float[] vertexArray, out float[] colorArray, out uint[] indexArray)
        {
            List<MaterialGroup> materialGroups;
            CreateObjectFromResourceWithMaterials(Gl, resourceName, out vertexArray, out colorArray, out indexArray, out materialGroups);
        }

        public static void CreateObjectFromResourceWithMaterials(GL Gl, string resourceName, out float[] vertexArray, out float[] colorArray, out uint[] indexArray, out List<MaterialGroup> materialGroups)
        {
            List<float[]> objVertices = new List<float[]>();
            List<float[]> objNormals = new List<float[]>();
            List<float[]> objTexCoords = new List<float[]>();

            materialGroups = new List<MaterialGroup>();
            MaterialGroup currentMaterialGroup = new MaterialGroup { MaterialName = "default" };
            materialGroups.Add(currentMaterialGroup);

            bool hasNormals = false;
            bool hasTexCoords = false;

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
                            for (int i = 0; i < vertex.Length; ++i)
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
                        case "vt":
                            hasTexCoords = true;
                            float[] texCoord = new float[2];
                            for (int i = 0; i < Math.Min(lineData.Length, 2); ++i)
                                texCoord[i] = float.Parse(lineData[i], CultureInfo.InvariantCulture);
                            objTexCoords.Add(texCoord);
                            break;
                        case "usemtl":
                            string materialName = lineData.Length > 0 ? lineData[0] : "default";
                            currentMaterialGroup = new MaterialGroup { MaterialName = materialName };
                            materialGroups.Add(currentMaterialGroup);
                            break;
                        case "f":
                            List<int> face = new List<int>();
                            List<int> faceNormals = new List<int>();
                            List<int> faceTexCoords = new List<int>();
                            foreach (string vertex1 in lineData)
                            {
                                string[] indices = vertex1.Split('/');
                                
                                if (indices.Length > 0 && !string.IsNullOrEmpty(indices[0]))
                                    face.Add(int.Parse(indices[0], CultureInfo.InvariantCulture));
                                else
                                    face.Add(1);

                                if (indices.Length > 1 && !string.IsNullOrEmpty(indices[1]))
                                    faceTexCoords.Add(int.Parse(indices[1], CultureInfo.InvariantCulture));
                                else
                                    faceTexCoords.Add(1);

                                if (indices.Length > 2 && !string.IsNullOrEmpty(indices[2]))
                                    faceNormals.Add(int.Parse(indices[2], CultureInfo.InvariantCulture));
                                else
                                    faceNormals.Add(1);
                            }

                            for (int i = 1; i < face.Count - 1; ++i)
                            {
                                currentMaterialGroup.Faces.Add(new int[] { face[0], face[i], face[i + 1] });
                                currentMaterialGroup.FaceTexCoords.Add(new int[] { faceTexCoords[0], faceTexCoords[i], faceTexCoords[i + 1] });
                                currentMaterialGroup.FaceNormals.Add(new int[] { faceNormals[0], faceNormals[i], faceNormals[i + 1] });
                            }
                            break;

                        default:
                            break;
                    }
                }
            }

            List<float> glVertices = new List<float>();
            List<float> glColors = new List<float>();
            List<uint> glIndexArray = new List<uint>();

            uint currentVertexIndex = 0;

            materialGroups = materialGroups.Where(mg => mg.Faces.Count > 0).ToList();

            foreach (var materialGroup in materialGroups)
            {
                var vertexMap = new Dictionary<string, uint>();

                for (int faceIdx = 0; faceIdx < materialGroup.Faces.Count; faceIdx++)
                {
                    var face = materialGroup.Faces[faceIdx];
                    var faceTexCoords = materialGroup.FaceTexCoords[faceIdx];
                    var faceNormals = materialGroup.FaceNormals[faceIdx];

                    for (int i = 0; i < 3; i++)
                    {
                        int vertIdx = face[i] - 1;
                        int texIdx = faceTexCoords[i] - 1;
                        int normalIdx = faceNormals[i] - 1;

                        string vertexKey = $"{vertIdx}/{texIdx}/{normalIdx}";

                        if (!vertexMap.ContainsKey(vertexKey))
                        {
                            var vertex = objVertices[vertIdx];
                            glVertices.Add(vertex[0]);
                            glVertices.Add(vertex[1]);
                            glVertices.Add(vertex[2]);

                            if (hasNormals && normalIdx >= 0 && normalIdx < objNormals.Count)
                            {
                                var normal = objNormals[normalIdx];
                                glVertices.Add(normal[0]);
                                glVertices.Add(normal[1]);
                                glVertices.Add(normal[2]);
                            }
                            else if (!hasNormals)
                            {
                                glVertices.Add(0.0f);
                                glVertices.Add(1.0f);
                                glVertices.Add(0.0f);
                            }
                            else
                            {
                                glVertices.Add(0.0f);
                                glVertices.Add(1.0f);
                                glVertices.Add(0.0f);
                            }


                            if (hasTexCoords && texIdx >= 0 && texIdx < objTexCoords.Count)
                            {
                                var texCoord = objTexCoords[texIdx];
                                glVertices.Add(texCoord[0]);
                                glVertices.Add(1.0f - texCoord[1]);
                            }
                            else
                            {
                                glVertices.Add(0.0f);
                                glVertices.Add(0.0f);
                            }

                            glColors.AddRange(new float[] { 0.5f, 0.5f, 0.5f, 0.5f});

                            vertexMap[vertexKey] = currentVertexIndex;
                            currentVertexIndex++;
                        }

                        glIndexArray.Add(vertexMap[vertexKey]);
                    }
                }
            }

            if (!hasNormals)
            {
                CalculateFaceNormals(glVertices, glIndexArray);
            }

            vertexArray = glVertices.ToArray();
            colorArray = glColors.ToArray();
            indexArray = glIndexArray.ToArray();
        }

        private static void CalculateFaceNormals(List<float> vertices, List<uint> indices)
        {
            int vertexStride = 8; // x,y,z,nx,ny,nz,u,v

            for (int i = 0; i < indices.Count; i += 3)
            {
                uint idx0 = indices[i] * (uint)vertexStride;
                uint idx1 = indices[i + 1] * (uint)vertexStride;
                uint idx2 = indices[i + 2] * (uint)vertexStride;

                var v0 = new Vector3(vertices[(int)idx0], vertices[(int)idx0 + 1], vertices[(int)idx0 + 2]);
                var v1 = new Vector3(vertices[(int)idx1], vertices[(int)idx1 + 1], vertices[(int)idx1 + 2]);
                var v2 = new Vector3(vertices[(int)idx2], vertices[(int)idx2 + 1], vertices[(int)idx2 + 2]);

                var edge1 = v1 - v0;
                var edge2 = v2 - v0;
                var normal = Vector3.Normalize(Vector3.Cross(edge1, edge2));

                vertices[(int)idx0 + 3] = normal.X;
                vertices[(int)idx0 + 4] = normal.Y;
                vertices[(int)idx0 + 5] = normal.Z;

                vertices[(int)idx1 + 3] = normal.X;
                vertices[(int)idx1 + 4] = normal.Y;
                vertices[(int)idx1 + 5] = normal.Z;

                vertices[(int)idx2 + 3] = normal.X;
                vertices[(int)idx2 + 4] = normal.Y;
                vertices[(int)idx2 + 5] = normal.Z;
            }
        }
    }
}
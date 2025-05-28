using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Szeminarium
{
    internal class ObjVertexTransformationData
    {
        public readonly Vector3D<float> Coordinates;

        public Vector3D<float> Normal { get; private set; }

        public Vector2D<float> TextureCoordinates { get; private set; }

        private int aggregatedFaceCount;

        public ObjVertexTransformationData(Vector3D<float> coordinates, Vector3D<float> initialNormal, Vector2D<float> textureCoords, int aggregatedFaceCount)
        {
            this.Coordinates = coordinates;
            this.Normal = Vector3D.Normalize(initialNormal);
            this.TextureCoordinates = textureCoords;
            this.aggregatedFaceCount = aggregatedFaceCount;
        }

        internal void UpdateNormalWithContributionFromAFace(Vector3D<float> normal)
        {
            var newNormalToNormalize = aggregatedFaceCount == 0 ? normal : (aggregatedFaceCount * Normal + normal) / (aggregatedFaceCount + 1);
            var newNormal = Vector3D.Normalize(newNormalToNormalize);
            Normal = newNormal;
            ++aggregatedFaceCount;
        }
        
        internal void SetTextureCoordinates(Vector2D<float> textureCoords)
        {
            TextureCoordinates = textureCoords;
        }
    }
}

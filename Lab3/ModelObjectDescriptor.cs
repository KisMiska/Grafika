using Silk.NET.OpenGL;
using Silk.NET.Vulkan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Silk.NET.Maths;

namespace GrafikaSzeminarium
{
    internal class ModelObjectDescriptor:IDisposable
    {
        private bool disposedValue;

        public uint Vao { get; private set; }
        public uint Vertices { get; private set; }
        public uint Colors { get; private set; }
        public uint Indices { get; private set; }
        public uint IndexArrayLength { get; private set; }

        private GL Gl;


        public float Height {get; private set; } = 2.0f;
        public float Width { get; private set; } = 1.0f;
        public int DeszkakCount { get; private set; } = 18;
        public float DeszkakAngle { get; private set; }
        public float Radius { get; private set; }

        public bool secondBarrel {get ; private set; }

        public unsafe static ModelObjectDescriptor Create(GL Gl, bool secondBarrel)
        {
            const int deszkaCount = 18;
            float deszkaAngle = 20.0f;
            float deszkaAngleRadians = 2 * MathF.PI / deszkaCount;
            float halfAngleRadians = deszkaAngleRadians / 2.0f;
            float radius = 0.5f / MathF.Tan(deszkaAngleRadians / 2);

            ModelObjectDescriptor descriptor = new ModelObjectDescriptor()
            {
                Gl = Gl,
                DeszkakCount = deszkaCount,
                DeszkakAngle = deszkaAngle,
                Radius = radius
            };

            uint vao = Gl.GenVertexArray();
            Gl.BindVertexArray(vao);

            float[] vertexArray = new float[] {
                // Bal alsó csúcs
                -0.5f, -1.0f, radius, 0, 0, 1,
                // Bal felső csúcs
                -0.5f, 1.0f, radius, 0, 0, 1,
                // Jobb felső csúcs
                0.5f, 1.0f, radius, 0, 0, 1,
                // Jobb alsó csúcs
                0.5f, -1.0f, radius, 0, 0, 1
            };

            if(secondBarrel)
            {
                float normalX = MathF.Sin(halfAngleRadians);
                float normalZ = MathF.Cos(halfAngleRadians);

                vertexArray[3] = normalX;
                vertexArray[4] = 0;
                vertexArray[5] = normalZ;

                // Bal felső
                vertexArray[9] = normalX;
                vertexArray[10] = 0;
                vertexArray[11] = normalZ;

                // Jobb felső
                vertexArray[15] = normalX;
                vertexArray[16] = 0;
                vertexArray[17] = normalZ;

                // Jobb alsó
                vertexArray[21] = normalX;
                vertexArray[22] = 0;
                vertexArray[23] = normalZ;

            }

            float[] colorArray = new float[] {
                0.0f, 1.0f, 0.0f, 1.0f,
                0.0f, 1.0f, 0.0f, 1.0f,
                0.0f, 1.0f, 0.0f, 1.0f,
                0.0f, 1.0f, 0.0f, 1.0f,
            };

            uint[] indexArray = new uint[] {
                0, 1, 2,
                0, 2, 3
            };

            uint vertices = Gl.GenBuffer();
            Gl.BindBuffer(GLEnum.ArrayBuffer, vertices);
            Gl.BufferData(GLEnum.ArrayBuffer, (ReadOnlySpan<float>)vertexArray.AsSpan(), GLEnum.StaticDraw);
            // 0 is position
            // 2 is normals
            uint offsetPos = 0;
            uint offsetNormals = offsetPos + 3 * sizeof(float);
            uint vertexSize = offsetNormals + 3 * sizeof(float);
            Gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, vertexSize, (void*)offsetPos);
            Gl.EnableVertexAttribArray(0);
            Gl.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, true, vertexSize, (void*)offsetNormals);
            Gl.EnableVertexAttribArray(2);
            Gl.BindBuffer(GLEnum.ArrayBuffer, 0);

            uint colors = Gl.GenBuffer();
            Gl.BindBuffer(GLEnum.ArrayBuffer, colors);
            Gl.BufferData(GLEnum.ArrayBuffer, (ReadOnlySpan<float>)colorArray.AsSpan(), GLEnum.StaticDraw);
            // 1 is color
            Gl.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 0, null);
            Gl.EnableVertexAttribArray(1);
            Gl.BindBuffer(GLEnum.ArrayBuffer, 0);

            uint indices = Gl.GenBuffer();
            Gl.BindBuffer(GLEnum.ElementArrayBuffer, indices);
            Gl.BufferData(GLEnum.ElementArrayBuffer, (ReadOnlySpan<uint>)indexArray.AsSpan(), GLEnum.StaticDraw);
            Gl.BindBuffer(GLEnum.ElementArrayBuffer, 0);

            descriptor.Vao = vao;
            descriptor.Vertices = vertices;
            descriptor.Colors = colors;
            descriptor.Indices = indices;
            descriptor.IndexArrayLength = (uint)indexArray.Length;

            return descriptor;
        }

        public Matrix4X4<float> GetDeszkaTransformMatrix(int index)
        {
            float angle = index * DeszkakAngle;
            float angleRadians = angle * MathF.PI / 180.0f;
            
            Matrix4X4<float> rotationMatrix = Matrix4X4.CreateRotationY<float>(angleRadians);
            
            return rotationMatrix;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null


                // always unbound the vertex buffer first, so no halfway results are displayed by accident
                Gl.DeleteBuffer(Vertices);
                Gl.DeleteBuffer(Colors);
                Gl.DeleteBuffer(Indices);
                Gl.DeleteVertexArray(Vao);

                disposedValue = true;
            }
        }

        ~ModelObjectDescriptor()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

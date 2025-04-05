using Silk.NET.OpenGL;
using Silk.NET.Vulkan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GrafikaSzeminarium
{
    internal class ModelObjectDescriptor : IDisposable
    {
        private bool disposedValue;

        public uint Vao { get; private set; }
        public uint Vertices { get; private set; }
        public uint Colors { get; private set; }
        public uint Indices { get; private set; }
        public uint IndexArrayLength { get; private set; }

        private GL Gl;

        private const float GAP = 0.05f; //
        private const float SIZE = 0.4f; //

        public static ModelObjectDescriptor[] CreateRubiksCube(GL GL)
        {
            var smallCubes = new ModelObjectDescriptor[27];
            int index = 0;

            var red = new float[] { 1.0f, 0.0f, 0.0f, 1.0f };
            var green = new float[] { 0.0f, 1.0f, 0.0f, 1.0f };
            var blue = new float[] { 0.0f, 0.0f, 1.0f, 1.0f };
            var magenta = new float[] { 1.0f, 0.0f, 1.0f, 1.0f };
            var cyan = new float[] { 0.0f, 1.0f, 1.0f, 1.0f };
            var yellow = new float[] { 1.0f, 1.0f, 0.0f, 1.0f };
            var gray = new float[] { 0.5f, 0.5f, 0.5f, 1.0f };

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    for (int z = -1; z <= 1; z++)
                    {
                        var colors = new float[96]; //24 * 4
                        for (int i = 0; i < 24; i++)
                        {
                            SetVertexColor(colors, i, gray);
                        }

                        if (y == 1)// Top
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                SetVertexColor(colors, 0 + i, red);
                            }
                        }
                        if (y == -1)// Bottom
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                SetVertexColor(colors, 12 + i, green);
                            }
                        }
                        if (x == 1)// Right
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                SetVertexColor(colors, 20 + i, blue);
                            }
                        }
                        if (x == -1)// Left
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                SetVertexColor(colors, 8 + i, magenta);
                            }
                        }
                        if (z == 1)// Front
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                SetVertexColor(colors, 4 + i, cyan);
                            }
                        }
                        if (z == -1)// Back
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                SetVertexColor(colors, 16 + i, yellow);
                            }
                        }

                        smallCubes[index] = CreateCube(GL, colors);
                        index++;

                    }
                }
            }
            return smallCubes;
        }
        private static void SetVertexColor(float[] colors, int vertexIndex, float[] color)
        {
            int baseIndex = vertexIndex * 4;
            colors[baseIndex] = color[0];
            colors[baseIndex + 1] = color[1];
            colors[baseIndex + 2] = color[2];
            colors[baseIndex + 3] = color[3];
        }

        public unsafe static ModelObjectDescriptor CreateCube(GL Gl, float[] colorArray)
        {
            uint vao = Gl.GenVertexArray();
            Gl.BindVertexArray(vao);

            // counter clockwise is front facing
            var vertexArray = new float[] {
                -0.5f, 0.5f, 0.5f,
                0.5f, 0.5f, 0.5f,
                0.5f, 0.5f, -0.5f,
                -0.5f, 0.5f, -0.5f,

                -0.5f, 0.5f, 0.5f,
                -0.5f, -0.5f, 0.5f,
                0.5f, -0.5f, 0.5f,
                0.5f, 0.5f, 0.5f,

                -0.5f, 0.5f, 0.5f,
                -0.5f, 0.5f, -0.5f,
                -0.5f, -0.5f, -0.5f,
                -0.5f, -0.5f, 0.5f,

                -0.5f, -0.5f, 0.5f,
                0.5f, -0.5f, 0.5f,
                0.5f, -0.5f, -0.5f,
                -0.5f, -0.5f, -0.5f,

                0.5f, 0.5f, -0.5f,
                -0.5f, 0.5f, -0.5f,
                -0.5f, -0.5f, -0.5f,
                0.5f, -0.5f, -0.5f,

                0.5f, 0.5f, 0.5f,
                0.5f, 0.5f, -0.5f,
                0.5f, -0.5f, -0.5f,
                0.5f, -0.5f, 0.5f,

            };

            uint[] indexArray = new uint[] {
                0, 1, 2,
                0, 2, 3,

                4, 5, 6,
                4, 6, 7,

                8, 9, 10,
                10, 11, 8,

                12, 14, 13,
                12, 15, 14,

                17, 16, 19,
                17, 19, 18,

                20, 22, 21,
                20, 23, 22
            };

            uint vertices = Gl.GenBuffer();
            Gl.BindBuffer(GLEnum.ArrayBuffer, vertices);
            Gl.BufferData(GLEnum.ArrayBuffer, (ReadOnlySpan<float>)vertexArray.AsSpan(), GLEnum.StaticDraw);
            Gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, null);
            Gl.EnableVertexAttribArray(0);
            Gl.BindBuffer(GLEnum.ArrayBuffer, 0); // ?

            uint colors = Gl.GenBuffer();
            Gl.BindBuffer(GLEnum.ArrayBuffer, colors);
            Gl.BufferData(GLEnum.ArrayBuffer, (ReadOnlySpan<float>)colorArray.AsSpan(), GLEnum.StaticDraw);
            Gl.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 0, null);
            Gl.EnableVertexAttribArray(1);
            Gl.BindBuffer(GLEnum.ArrayBuffer, 0); //?

            uint indices = Gl.GenBuffer();
            Gl.BindBuffer(GLEnum.ElementArrayBuffer, indices);
            Gl.BufferData(GLEnum.ElementArrayBuffer, (ReadOnlySpan<uint>)indexArray.AsSpan(), GLEnum.StaticDraw);
            Gl.BindBuffer(GLEnum.ElementArrayBuffer, 0); //?

            return new ModelObjectDescriptor() { Vao = vao, Vertices = vertices, Colors = colors, Indices = indices, IndexArrayLength = (uint)indexArray.Length, Gl = Gl };

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

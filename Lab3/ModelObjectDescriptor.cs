﻿using Silk.NET.OpenGL;
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

        private readonly static float[][] PredefinedColors = new float[][]
        {
            new float[] { 1.0f, 0.0f, 0.0f, 1.0f },
            new float[] { 0.0f, 1.0f, 0.0f, 1.0f },
            new float[] { 0.0f, 0.0f, 1.0f, 1.0f },
            new float[] { 1.0f, 0.0f, 1.0f, 1.0f },
            new float[] { 0.0f, 1.0f, 1.0f, 1.0f },
            new float[] { 1.0f, 1.0f, 0.0f, 1.0f }
        };

        public readonly static string[] ColorNames = new string[] { "Red", "Green", "Blue", "Magenta", "Cyan", "Yellow" };
        
        public unsafe void UpdateFrontFaceColor(int colorIndex)
        {
            float[] colorArray = new float[] {
               
                1.0f, 0.0f, 0.0f, 1.0f,
                1.0f, 0.0f, 0.0f, 1.0f,
                1.0f, 0.0f, 0.0f, 1.0f,
                1.0f, 0.0f, 0.0f, 1.0f,

                PredefinedColors[colorIndex][0], PredefinedColors[colorIndex][1], PredefinedColors[colorIndex][2], PredefinedColors[colorIndex][3],
                PredefinedColors[colorIndex][0], PredefinedColors[colorIndex][1], PredefinedColors[colorIndex][2], PredefinedColors[colorIndex][3],
                PredefinedColors[colorIndex][0], PredefinedColors[colorIndex][1], PredefinedColors[colorIndex][2], PredefinedColors[colorIndex][3],
                PredefinedColors[colorIndex][0], PredefinedColors[colorIndex][1], PredefinedColors[colorIndex][2], PredefinedColors[colorIndex][3],

                0.0f, 0.0f, 1.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f,

                1.0f, 0.0f, 1.0f, 1.0f,
                1.0f, 0.0f, 1.0f, 1.0f,
                1.0f, 0.0f, 1.0f, 1.0f,
                1.0f, 0.0f, 1.0f, 1.0f,

                0.0f, 1.0f, 1.0f, 1.0f,
                0.0f, 1.0f, 1.0f, 1.0f,
                0.0f, 1.0f, 1.0f, 1.0f,
                0.0f, 1.0f, 1.0f, 1.0f,

                1.0f, 1.0f, 0.0f, 1.0f,
                1.0f, 1.0f, 0.0f, 1.0f,
                1.0f, 1.0f, 0.0f, 1.0f,
                1.0f, 1.0f, 0.0f, 1.0f,
            };

            Gl.DeleteBuffer(Colors);
            
            uint colors = Gl.GenBuffer();
            Gl.BindVertexArray(Vao);
            Gl.BindBuffer(GLEnum.ArrayBuffer, colors);
            Gl.BufferData(GLEnum.ArrayBuffer, (ReadOnlySpan<float>)colorArray.AsSpan(), GLEnum.StaticDraw);
           
            Gl.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 0, null);
            Gl.EnableVertexAttribArray(1);
            Gl.BindBuffer(GLEnum.ArrayBuffer, 0);
            
            Colors = colors;
        }

        public unsafe static ModelObjectDescriptor CreateCube(GL Gl)
        {
            uint vao = Gl.GenVertexArray();
            Gl.BindVertexArray(vao);

            // counter clockwise is front facing
            float[] vertexArray = new float[] {
                // top face
                -0.5f, 0.5f, 0.5f, 0f, 1f, 0f,
                0.5f, 0.5f, 0.5f, 0f, 1f, 0f,
                0.5f, 0.5f, -0.5f, 0f, 1f, 0f,
                -0.5f, 0.5f, -0.5f, 0f, 1f, 0f, 

                // front face
                -0.5f, 0.5f, 0.5f, 0f, 0f, 1f,
                -0.5f, -0.5f, 0.5f, 0f, 0f, 1f,
                0.5f, -0.5f, 0.5f, 0f, 0f, 1f,
                0.5f, 0.5f, 0.5f, 0f, 0f, 1f,

                // left face
                -0.5f, 0.5f, 0.5f, -1f, 0f, 0f,
                -0.5f, 0.5f, -0.5f, -1f, 0f, 0f,
                -0.5f, -0.5f, -0.5f, -1f, 0f, 0f,
                -0.5f, -0.5f, 0.5f, -1f, 0f, 0f,

                // bottom face
                -0.5f, -0.5f, 0.5f, 0f, -1f, 0f,
                0.5f, -0.5f, 0.5f,0f, -1f, 0f,
                0.5f, -0.5f, -0.5f,0f, -1f, 0f,
                -0.5f, -0.5f, -0.5f,0f, -1f, 0f,

                // back face
                0.5f, 0.5f, -0.5f, 0f, 0f, -1f,
                -0.5f, 0.5f, -0.5f,0f, 0f, -1f,
                -0.5f, -0.5f, -0.5f,0f, 0f, -1f,
                0.5f, -0.5f, -0.5f,0f, 0f, -1f,

                // right face
                0.5f, 0.5f, 0.5f, 1f, 0f, 0f,
                0.5f, 0.5f, -0.5f,1f, 0f, 0f,
                0.5f, -0.5f, -0.5f,1f, 0f, 0f,
                0.5f, -0.5f, 0.5f,1f, 0f, 0f,
            };

            float[] colorArray = new float[] {
                1.0f, 0.0f, 0.0f, 1.0f,
                1.0f, 0.0f, 0.0f, 1.0f,
                1.0f, 0.0f, 0.0f, 1.0f,
                1.0f, 0.0f, 0.0f, 1.0f,

                0.0f, 1.0f, 0.0f, 1.0f,
                0.0f, 1.0f, 0.0f, 1.0f,
                0.0f, 1.0f, 0.0f, 1.0f,
                0.0f, 1.0f, 0.0f, 1.0f,

                0.0f, 0.0f, 1.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f,

                1.0f, 0.0f, 1.0f, 1.0f,
                1.0f, 0.0f, 1.0f, 1.0f,
                1.0f, 0.0f, 1.0f, 1.0f,
                1.0f, 0.0f, 1.0f, 1.0f,

                0.0f, 1.0f, 1.0f, 1.0f,
                0.0f, 1.0f, 1.0f, 1.0f,
                0.0f, 1.0f, 1.0f, 1.0f,
                0.0f, 1.0f, 1.0f, 1.0f,

                1.0f, 1.0f, 0.0f, 1.0f,
                1.0f, 1.0f, 0.0f, 1.0f,
                1.0f, 1.0f, 0.0f, 1.0f,
                1.0f, 1.0f, 0.0f, 1.0f,
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

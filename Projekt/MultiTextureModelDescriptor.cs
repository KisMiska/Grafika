using Silk.NET.OpenGL;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using Szeminarium;

namespace GrafikaSzeminarium
{
    public class MaterialRenderInfo
    {
        public uint? TextureId { get; set; }
        public uint StartIndex { get; set; }
        public uint IndexCount { get; set; }
    }

    internal class MultiTextureModelDescriptor : IDisposable
    {
        private bool disposedValue;

        public uint Vao { get; set; }
        public uint Vertices { get; set; }
        public uint Colors { get; set; }
        public uint Indices { get; set; }
        public List<MaterialRenderInfo> Materials { get; set; } = new List<MaterialRenderInfo>();

        public GL Gl;

        public unsafe static MultiTextureModelDescriptor CreateCustomWithMaterials(GL Gl, string modelName)
        {
            float[] vertexArray;
            float[] colorArray;
            uint[] indexArray;
            List<MaterialGroup> materialGroups;

            ObjectResourceReader.CreateObjectFromResourceWithMaterials(Gl, modelName, out vertexArray, out colorArray, out indexArray, out materialGroups);

            // basic model object
            var descriptor = CreateModelObjectFromArrays(Gl, vertexArray, colorArray, indexArray);

            // process material groups
            uint currentIndex = 0;
            foreach (var group in materialGroups)
            {
                var materialInfo = new MaterialRenderInfo
                {
                    StartIndex = currentIndex,
                    IndexCount = (uint)(group.Faces.Count * 3)
                };

                if (!string.IsNullOrEmpty(group.MaterialName) && group.MaterialName != "default")
                {
                    try
                    {
                        var textureImage = ReadTextureImage(group.MaterialName);
                        if (textureImage != null)
                        {
                            materialInfo.TextureId = CreateTexture(Gl, textureImage);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to load texture {group.MaterialName}: {ex.Message}");
                        materialInfo.TextureId = null;
                    }
                }

                descriptor.Materials.Add(materialInfo);
                currentIndex += materialInfo.IndexCount;
            }

            return descriptor;
        }

        private static unsafe MultiTextureModelDescriptor CreateModelObjectFromArrays(GL Gl, float[] vertexArray, float[] colorArray, uint[] indexArray)
        {
            uint vao = Gl.GenVertexArray();
            Gl.BindVertexArray(vao);

            uint vertices = Gl.GenBuffer();
            Gl.BindBuffer(GLEnum.ArrayBuffer, vertices);
            Gl.BufferData(GLEnum.ArrayBuffer, (ReadOnlySpan<float>)vertexArray.AsSpan(), GLEnum.StaticDraw);
            
            // setup vertex
            uint offsetPos = 0;
            uint offsetNormals = offsetPos + 3 * sizeof(float);
            uint offsetTexture = offsetNormals + 3 * sizeof(float);
            uint vertexSize = offsetTexture + 2 * sizeof(float);

            Gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, vertexSize, (void*)offsetPos);
            Gl.EnableVertexAttribArray(0);
            Gl.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, true, vertexSize, (void*)offsetNormals);
            Gl.EnableVertexAttribArray(2);
            Gl.VertexAttribPointer(3, 2, VertexAttribPointerType.Float, false, vertexSize, (void*)offsetTexture);
            Gl.EnableVertexAttribArray(3);
            Gl.BindBuffer(GLEnum.ArrayBuffer, 0);

            uint colors = Gl.GenBuffer();
            Gl.BindBuffer(GLEnum.ArrayBuffer, colors);
            Gl.BufferData(GLEnum.ArrayBuffer, (ReadOnlySpan<float>)colorArray.AsSpan(), GLEnum.StaticDraw);
            Gl.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 0, null);
            Gl.EnableVertexAttribArray(1);
            Gl.BindBuffer(GLEnum.ArrayBuffer, 0);

            uint indices = Gl.GenBuffer();
            Gl.BindBuffer(GLEnum.ElementArrayBuffer, indices);
            Gl.BufferData(GLEnum.ElementArrayBuffer, (ReadOnlySpan<uint>)indexArray.AsSpan(), GLEnum.StaticDraw);
            Gl.BindBuffer(GLEnum.ElementArrayBuffer, 0);

            return new MultiTextureModelDescriptor() 
            { 
                Vao = vao, 
                Vertices = vertices, 
                Colors = colors, 
                Indices = indices, 
                Gl = Gl 
            };
        }

        private static unsafe uint CreateTexture(GL Gl, ImageResult textureImage)
        {
            uint texture = Gl.GenTexture();
            Gl.ActiveTexture(TextureUnit.Texture0);
            Gl.BindTexture(TextureTarget.Texture2D, texture);
            
            Gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)textureImage.Width,
                (uint)textureImage.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, (ReadOnlySpan<byte>)textureImage.Data.AsSpan());
            
            Gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            Gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            Gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            Gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            
            Gl.BindTexture(TextureTarget.Texture2D, 0);
            return texture;
        }

        private static unsafe ImageResult ReadTextureImage(string textureResource)
        {
            try
            {
                using (Stream textureStream = typeof(MultiTextureModelDescriptor).Assembly.GetManifestResourceStream("Projekt.Resources." + textureResource + ".jpg"))
                {
                    if (textureStream == null)
                        return null;
                    return ImageResult.FromStream(textureStream, ColorComponents.RedGreenBlueAlpha);
                }
            }
            catch
            {
                return null;
            }
        }

        public unsafe void Draw(GL Gl, uint shaderProgram, string textureUniformName)
        {
            Gl.BindVertexArray(Vao);
            Gl.BindBuffer(GLEnum.ElementArrayBuffer, Indices);

            int textureLocation = Gl.GetUniformLocation(shaderProgram, textureUniformName);

            foreach (var material in Materials)
            {
                if (material.TextureId.HasValue && textureLocation != -1)
                {
                    Gl.Uniform1(textureLocation, 0);
                    Gl.ActiveTexture(TextureUnit.Texture0);
                    Gl.BindTexture(TextureTarget.Texture2D, material.TextureId.Value);
                }

                Gl.DrawElements(PrimitiveType.Triangles, material.IndexCount, DrawElementsType.UnsignedInt, 
                    (void*)(material.StartIndex * sizeof(uint)));

                if (material.TextureId.HasValue)
                {
                    Gl.BindTexture(TextureTarget.Texture2D, 0);
                }
            }

            Gl.BindBuffer(GLEnum.ElementArrayBuffer, 0);
            Gl.BindVertexArray(0);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                Gl.DeleteBuffer(Vertices);
                Gl.DeleteBuffer(Colors);
                Gl.DeleteBuffer(Indices);
                Gl.DeleteVertexArray(Vao);

                foreach (var material in Materials)
                {
                    if (material.TextureId.HasValue)
                    {
                        Gl.DeleteTexture(material.TextureId.Value);
                    }
                }

                disposedValue = true;
            }
        }

        ~MultiTextureModelDescriptor()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
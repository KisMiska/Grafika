﻿using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;
using System.Dynamic;
using System.Numerics;
using System.Reflection;
using Szeminarium;

namespace GrafikaSzeminarium
{
    internal class Program
    {
        private static IWindow graphicWindow;

        private static GL Gl;

        private static ImGuiController imGuiController;
<<<<<<< HEAD

        private static ModelObjectDescriptor cube;
=======
        private static ModelObjectDescriptor barrel1;
        private static ModelObjectDescriptor barrel2;
>>>>>>> lab3-4bonusz

        private static CameraDescriptor camera = new CameraDescriptor();

        private static CubeArrangementModel cubeArrangementModel = new CubeArrangementModel();

        private const string ModelMatrixVariableName = "uModel";
        private const string NormalMatrixVariableName = "uNormal";
        private const string ViewMatrixVariableName = "uView";
        private const string ProjectionMatrixVariableName = "uProjection";

        private const string LightColorVariableName = "uLightColor";
        private const string LightPositionVariableName = "uLightPos";
        private const string ViewPositionVariableName = "uViewPos";
        private const string ShinenessVariableName = "uShininess";
        private static float shininess = 50;

<<<<<<< HEAD
        private const string AmbientStrengthVariableName = "uAmbientStrength";
        private const string DiffuseStrengthVariableName = "uDiffuseStrength";
        private const string SpecularStrengthVariableName = "uSpecularStrength";

        private static Vector3 ambientStrength = new Vector3(0.1f, 0.1f, 0.1f);
        private static Vector3 diffuseStrength = new Vector3(0.3f, 0.3f, 0.3f);
        private static Vector3 specularStrength = new Vector3(0.6f, 0.6f, 0.6f);


        private static Vector3 backgroundColor = new Vector3(1.0f, 1.0f, 1.0f);

        private static int selectedColorIndex = 1;

        private static uint program;
=======
        private static uint phongProgram;
        private static uint gourardProgram;
        private static bool usePhongShading = true;
>>>>>>> lab3-4bonusz

        static void Main(string[] args)
        {
            WindowOptions windowOptions = WindowOptions.Default;
            windowOptions.Title = "Grafika szeminárium";
            windowOptions.Size = new Silk.NET.Maths.Vector2D<int>(500, 500);

            graphicWindow = Window.Create(windowOptions);

            graphicWindow.Load += GraphicWindow_Load;
            graphicWindow.Update += GraphicWindow_Update;
            graphicWindow.Render += GraphicWindow_Render;
            graphicWindow.Closing += GraphicWindow_Closing;

            graphicWindow.Run();
        }

        private static void GraphicWindow_Closing()
        {
<<<<<<< HEAD
            cube.Dispose();
            Gl.DeleteProgram(program);
=======
            barrel1.Dispose();
            barrel2.Dispose();
            Gl.DeleteProgram(phongProgram);
            Gl.DeleteProgram(gourardProgram);
>>>>>>> lab3-4bonusz
        }

        private static void GraphicWindow_Load()
        {
            Gl = graphicWindow.CreateOpenGL();

            var inputContext = graphicWindow.CreateInput();
            foreach (var keyboard in inputContext.Keyboards)
            {
                keyboard.KeyDown += Keyboard_KeyDown;
            }

            // Handle resizes
            graphicWindow.FramebufferResize += s =>
            {
                // Adjust the viewport to the new window size
                Gl.Viewport(s);
            };

            imGuiController = new ImGuiController(Gl, graphicWindow, inputContext);

            cube = ModelObjectDescriptor.CreateCube(Gl);
            cube.UpdateFrontFaceColor(selectedColorIndex);

            Gl.ClearColor(backgroundColor.X, backgroundColor.Y, backgroundColor.Z, 1.0f);
            
            Gl.Enable(EnableCap.CullFace);
            Gl.CullFace(TriangleFace.Back);

            Gl.Enable(EnableCap.DepthTest);
            Gl.DepthFunc(DepthFunction.Lequal);

            phongProgram = CreateShaderProgram(GetEmbeddedResourceAsString("Shaders.VertexShader.vert"), GetEmbeddedResourceAsString("Shaders.FragmentShader.frag"));

            gourardProgram = CreateShaderProgram(GetEmbeddedResourceAsString("Shaders.GourardVertexShader.vert"), GetEmbeddedResourceAsString("Shaders.GourardFragmentShader.frag"));

        }

        private static uint CreateShaderProgram(string vertexShaderSource, string fragmentShaderSource)
        {
            uint vshader = Gl.CreateShader(ShaderType.VertexShader);
            uint fshader = Gl.CreateShader(ShaderType.FragmentShader);

            Gl.ShaderSource(vshader, vertexShaderSource);
            Gl.CompileShader(vshader);
            Gl.GetShader(vshader, ShaderParameterName.CompileStatus, out int vStatus);
            if (vStatus != (int)GLEnum.True) throw new Exception("Vertex shader failed to compile: " + Gl.GetShaderInfoLog(vshader));

            Gl.ShaderSource(fshader, fragmentShaderSource);
            Gl.CompileShader(fshader);
            Gl.GetShader(fshader, ShaderParameterName.CompileStatus, out int fStatus);
            if (fStatus != (int)GLEnum.True) throw new Exception("Fragment shader failed to compile: " + Gl.GetShaderInfoLog(fshader));

            uint program = Gl.CreateProgram();
            Gl.AttachShader(program, vshader);
            Gl.AttachShader(program, fshader);
            Gl.LinkProgram(program);

            Gl.DetachShader(program, vshader);
            Gl.DetachShader(program, fshader);
            Gl.DeleteShader(vshader);
            Gl.DeleteShader(fshader);

            Gl.GetProgram(program, GLEnum.LinkStatus, out var status);
            if (status == 0)
            {
                Console.WriteLine($"Error linking shader {Gl.GetProgramInfoLog(program)}");
            }

            return program;
        }

        private static string GetEmbeddedResourceAsString(string resourceRelativePath)
        {
            string resourceFullPath = Assembly.GetExecutingAssembly().GetName().Name + "." + resourceRelativePath;

            using (var resStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceFullPath))
            using (var resStreamReader = new StreamReader(resStream))
            {
                var text = resStreamReader.ReadToEnd();
                return text;
            }
        }
        private static void Keyboard_KeyDown(IKeyboard keyboard, Key key, int arg3)
        {
            switch (key)
            {
                case Key.Left:
                    camera.DecreaseZYAngle();
                    break;
                case Key.Right:
                    camera.IncreaseZYAngle();
                    break;
                case Key.Down:
                    camera.IncreaseDistance();
                    break;
                case Key.Up:
                    camera.DecreaseDistance();
                    break;
                case Key.U:
                    camera.IncreaseZXAngle();
                    break;
                case Key.D:
                    camera.DecreaseZXAngle();
                    break;
                case Key.Space:
                    cubeArrangementModel.AnimationEnabled = !cubeArrangementModel.AnimationEnabled;
                    break;
            }
        }

        private static void GraphicWindow_Update(double deltaTime)
        {
            // NO OpenGL
            // make it threadsafe
            cubeArrangementModel.AdvanceTime(deltaTime);

            imGuiController.Update((float)deltaTime);
        }

        private static unsafe void GraphicWindow_Render(double deltaTime)
        {
            Gl.Clear(ClearBufferMask.ColorBufferBit);
            Gl.Clear(ClearBufferMask.DepthBufferBit);

            uint currentProgram = usePhongShading ? phongProgram : gourardProgram;
            Gl.UseProgram(currentProgram);

<<<<<<< HEAD
            SetUniform3(LightColorVariableName, new Vector3(1f, 1f, 1f));
            SetUniform3(LightPositionVariableName, new Vector3(0f, 1.2f, 0f));
            SetUniform3(ViewPositionVariableName, new Vector3(camera.Position.X, camera.Position.Y, camera.Position.Z));
            SetUniform1(ShinenessVariableName, shininess);
=======
            SetUniform3(currentProgram, LightColorVariableName, new Vector3(1f, 1f, 1f));
            SetUniform3(currentProgram, LightPositionVariableName, new Vector3(camera.Position.X /*+ */, camera.Position.Y, camera.Position.Z));
            SetUniform3(currentProgram, ViewPositionVariableName, new Vector3(camera.Position.X, camera.Position.Y, camera.Position.Z));
            SetUniform1(currentProgram, ShinenessVariableName, shininess);
>>>>>>> lab3-4bonusz

            SetUniform3(AmbientStrengthVariableName, ambientStrength);
            SetUniform3(DiffuseStrengthVariableName, diffuseStrength);
            SetUniform3(SpecularStrengthVariableName, specularStrength);

            var viewMatrix = Matrix4X4.CreateLookAt(camera.Position, camera.Target, camera.UpVector);
            SetMatrix(currentProgram, viewMatrix, ViewMatrixVariableName);

            var projectionMatrix = Matrix4X4.CreatePerspectiveFieldOfView<float>((float)(Math.PI / 2), 1024f / 768f, 0.1f, 100f);
            SetMatrix(currentProgram, projectionMatrix, ProjectionMatrixVariableName);

<<<<<<< HEAD

            var modelMatrixCenterCube = Matrix4X4.CreateScale((float)cubeArrangementModel.CenterCubeScale);
            SetModelMatrix(modelMatrixCenterCube);
            DrawModelObject(cube);

            Matrix4X4<float> diamondScale = Matrix4X4.CreateScale(0.25f);
            Matrix4X4<float> rotx = Matrix4X4.CreateRotationX((float)Math.PI / 4f);
            Matrix4X4<float> rotz = Matrix4X4.CreateRotationZ((float)Math.PI / 4f);
            Matrix4X4<float> roty = Matrix4X4.CreateRotationY((float)cubeArrangementModel.DiamondCubeLocalAngle);
            Matrix4X4<float> trans = Matrix4X4.CreateTranslation(1f, 1f, 0f);
            Matrix4X4<float> rotGlobalY = Matrix4X4.CreateRotationY((float)cubeArrangementModel.DiamondCubeGlobalYAngle);
            Matrix4X4<float> dimondCubeModelMatrix = diamondScale * rotx * rotz * roty * trans * rotGlobalY;
            SetModelMatrix(dimondCubeModelMatrix);
            DrawModelObject(cube);
=======
            DrawBarrel(currentProgram, barrel1, 1.5f);
            DrawBarrel(currentProgram, barrel2, -1.5f);
>>>>>>> lab3-4bonusz

            //ImGuiNET.ImGui.ShowDemoWindow();
            ImGuiNET.ImGui.Begin("Lighting", ImGuiNET.ImGuiWindowFlags.AlwaysAutoResize | ImGuiNET.ImGuiWindowFlags.NoCollapse);
            ImGuiNET.ImGui.SliderFloat("Shininess", ref shininess, 5, 100);
<<<<<<< HEAD
            ImGuiNET.ImGui.Text("Illumination Components");
            ImGuiNET.ImGui.SliderFloat3("Ambient", ref ambientStrength, 0.0f, 1.0f);
            ImGuiNET.ImGui.SliderFloat3("Diffuse", ref diffuseStrength, 0.0f, 1.0f);
            ImGuiNET.ImGui.SliderFloat3("Specular", ref specularStrength, 0.0f, 1.0f);
            ImGuiNET.ImGui.Text("Background Color");
            if (ImGuiNET.ImGui.SliderFloat3("RGB", ref backgroundColor, 0.0f, 1.0f))
            {
                Gl.ClearColor(backgroundColor.X, backgroundColor.Y, backgroundColor.Z, 1.0f);
            }
            if (ImGuiNET.ImGui.Combo("Front Face Color", ref selectedColorIndex, ModelObjectDescriptor.ColorNames, ModelObjectDescriptor.ColorNames.Length))
            {
                cube.UpdateFrontFaceColor(selectedColorIndex);
                
            }
=======
            if (ImGuiNET.ImGui.RadioButton("Phong Shading", usePhongShading)) usePhongShading = true;
            ImGuiNET.ImGui.SameLine();
            if (ImGuiNET.ImGui.RadioButton("Gouraud Shading", !usePhongShading)) usePhongShading = false;
>>>>>>> lab3-4bonusz
            ImGuiNET.ImGui.End();
            imGuiController.Render();
        }

<<<<<<< HEAD
        private static unsafe void SetModelMatrix(Matrix4X4<float> modelMatrix)
=======
        private static void DrawBarrel(uint program, ModelObjectDescriptor barrel, float offset)
        {
            for (int i = 0; i < barrel.DeszkakCount; i++)
            {
                Matrix4X4<float> modelMatrix = barrel.GetDeszkaTransformMatrix(i);

                modelMatrix = modelMatrix * Matrix4X4.CreateTranslation(0f, offset, 0f);

                SetModelMatrix(program, modelMatrix);
                DrawModelObject(barrel);
            }
        }
        private static unsafe void SetModelMatrix(uint program, Matrix4X4<float> modelMatrix)
>>>>>>> lab3-4bonusz
        {
            SetMatrix(program, modelMatrix, ModelMatrixVariableName);

            // set also the normal matrix
            int location = Gl.GetUniformLocation(program, NormalMatrixVariableName);
            if (location == -1)
            {
                throw new Exception($"{NormalMatrixVariableName} uniform not found on shader.");
            }

            // G = (M^-1)^T
            var modelMatrixWithoutTranslation = new Matrix4X4<float>(modelMatrix.Row1, modelMatrix.Row2, modelMatrix.Row3, modelMatrix.Row4);
            modelMatrixWithoutTranslation.M41 = 0;
            modelMatrixWithoutTranslation.M42 = 0;
            modelMatrixWithoutTranslation.M43 = 0;
            modelMatrixWithoutTranslation.M44 = 1;

            Matrix4X4<float> modelInvers;
            Matrix4X4.Invert<float>(modelMatrixWithoutTranslation, out modelInvers);
            Matrix3X3<float> normalMatrix = new Matrix3X3<float>(Matrix4X4.Transpose(modelInvers));

            Gl.UniformMatrix3(location, 1, false, (float*)&normalMatrix);
            CheckError();
        }

        private static unsafe void SetUniform1(uint program, string uniformName, float uniformValue)
        {
            int location = Gl.GetUniformLocation(program, uniformName);
            if (location == -1)
            {
                throw new Exception($"{uniformName} uniform not found on shader.");
            }

            Gl.Uniform1(location, uniformValue);
            CheckError();
        }

        private static unsafe void SetUniform3(uint program, string uniformName, Vector3 uniformValue)
        {
            int location = Gl.GetUniformLocation(program, uniformName);
            if (location == -1)
            {
                throw new Exception($"{uniformName} uniform not found on shader.");
            }

            Gl.Uniform3(location, uniformValue);
            CheckError();
        }

        private static unsafe void DrawModelObject(ModelObjectDescriptor modelObject)
        {
            Gl.BindVertexArray(modelObject.Vao);
            Gl.BindBuffer(GLEnum.ElementArrayBuffer, modelObject.Indices);
            Gl.DrawElements(PrimitiveType.Triangles, modelObject.IndexArrayLength, DrawElementsType.UnsignedInt, null);
            Gl.BindBuffer(GLEnum.ElementArrayBuffer, 0);
            Gl.BindVertexArray(0);
        }

        private static unsafe void SetMatrix(uint program, Matrix4X4<float> mx, string uniformName)
        {
            int location = Gl.GetUniformLocation(program, uniformName);
            if (location == -1)
            {
                throw new Exception($"{uniformName} uniform not found on shader.");
            }

            Gl.UniformMatrix4(location, 1, false, (float*)&mx);
            CheckError();
        }

        public static void CheckError()
        {
            var error = (ErrorCode)Gl.GetError();
            if (error != ErrorCode.NoError)
                throw new Exception("GL.GetError() returned " + error.ToString());
        }
    }
}
using ImGuiNET;
using Silk.NET.Input;
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

        private static ModelObjectDescriptor cube;

        private static ModelObjectDescriptor custom;

        private static ModelObjectDescriptor skybox;

        private static ModelObjectDescriptor boost;
        private static ModelObjectDescriptor bot1;
        private static ModelObjectDescriptor bot2;
        private static BotsDescriptor botsDescriptor = new BotsDescriptor();

        private static CameraDescriptor camera = new CameraDescriptor();

        private static CubeArrangementModel cubeArrangementModel = new CubeArrangementModel();

        private static Vector3D<float>[] boostPositions = new Vector3D<float>[8];
        private static double[] boostTimeOffsets = new double[8];
        private static Random random = new Random();
        private static double gameTime = 0.0;
        private static bool[] boostCollected = new bool[8];

        private const string ModelMatrixVariableName = "uModel";
        private const string NormalMatrixVariableName = "uNormal";
        private const string ViewMatrixVariableName = "uView";
        private const string ProjectionMatrixVariableName = "uProjection";

        private const string LightColorVariableName = "uLightColor";
        private const string LightPositionVariableName = "uLightPos";
        private const string ViewPositionVariableName = "uViewPos";

        private const string ShinenessVariableName = "uShininess";

        private const string TextureVariableName = "uTexture";

        private static float shininess = 50;
        private static float lastDeltaTime = 0f;

        private static uint program;

        static void Main(string[] args)
        {
            WindowOptions windowOptions = WindowOptions.Default;
            windowOptions.Title = "Robot Game";
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
            cube.Dispose();
            custom.Dispose();
            skybox.Dispose();
            boost.Dispose();
            bot1.Dispose();
            bot2.Dispose();
            Gl.DeleteProgram(program);
        }

        private static void InitializeBoosts()
        {
            for (int i = 0; i < 8; i++)
            {
                boostPositions[i] = new Vector3D<float>(
                    (float)(random.NextDouble() * 50 - 10), 1.0f, (float)(random.NextDouble() * 50 - 10)
                );

                //not in sync
                boostTimeOffsets[i] = random.NextDouble() * Math.PI * 2;
                boostCollected[i] = false;
            }
        }

        private static void CheckBoostCollisions()
        {
            for (int i = 0; i < 8; i++)
            {
                if (!boostCollected[i] && cubeArrangementModel.CanCollectBoost(boostPositions[i]))
                {
                    boostCollected[i] = true;
                    cubeArrangementModel.CollectBoost();

                }
            }
        }

        private static void GraphicWindow_Load()
        {
            Gl = graphicWindow.CreateOpenGL();

            var inputContext = graphicWindow.CreateInput();
            foreach (var keyboard in inputContext.Keyboards)
            {
                keyboard.KeyDown += Keyboard_KeyDown;
                keyboard.KeyUp += Keyboard_KeyUp;
            }

            // Handle resizes
            graphicWindow.FramebufferResize += s =>
            {
                // Adjust the viewport to the new window size
                Gl.Viewport(s);
            };

            imGuiController = new ImGuiController(Gl, graphicWindow, inputContext);

            cube = ModelObjectDescriptor.CreateCube(Gl);
            custom = ModelObjectDescriptor.CreateCustom(Gl, "trojan_412.obj");
            skybox = ModelObjectDescriptor.CreateSkyBox(Gl);
            boost = ModelObjectDescriptor.CreateCustom(Gl, "boost.obj");
            bot1 = ModelObjectDescriptor.CreateCustom(Gl, "hydra_flak.obj");
            bot2 = ModelObjectDescriptor.CreateCustom(Gl, "hammerhead.obj");

            camera.SetVehicleReference(cubeArrangementModel);
            camera.SetCameraMode(CameraDescriptor.CameraMode.ThirdPerson);

            InitializeBoosts();

            Gl.ClearColor(System.Drawing.Color.White);

            //Gl.Enable(EnableCap.CullFace);
            //Gl.CullFace(TriangleFace.Back);

            Gl.Enable(EnableCap.DepthTest);
            Gl.DepthFunc(DepthFunction.Lequal);


            uint vshader = Gl.CreateShader(ShaderType.VertexShader);
            uint fshader = Gl.CreateShader(ShaderType.FragmentShader);

            Gl.ShaderSource(vshader, GetEmbeddedResourceAsString("Shaders.VertexShader.vert"));
            Gl.CompileShader(vshader);
            Gl.GetShader(vshader, ShaderParameterName.CompileStatus, out int vStatus);
            if (vStatus != (int)GLEnum.True)
                throw new Exception("Vertex shader failed to compile: " + Gl.GetShaderInfoLog(vshader));

            Gl.ShaderSource(fshader, GetEmbeddedResourceAsString("Shaders.FragmentShader.frag"));
            Gl.CompileShader(fshader);
            Gl.GetShader(fshader, ShaderParameterName.CompileStatus, out int fStatus);
            if (fStatus != (int)GLEnum.True)
                throw new Exception("Fragment shader failed to compile: " + Gl.GetShaderInfoLog(fshader));

            program = Gl.CreateProgram();
            Gl.AttachShader(program, vshader);
            Gl.AttachShader(program, fshader);
            Gl.LinkProgram(program);

            Gl.DetachShader(program, vshader);
            Gl.DetachShader(program, fshader);
            Gl.DeleteShader(vshader);
            Gl.DeleteShader(fshader);
            if ((ErrorCode)Gl.GetError() != ErrorCode.NoError)
            {

            }

            Gl.GetProgram(program, GLEnum.LinkStatus, out var status);
            if (status == 0)
            {
                Console.WriteLine($"Error linking shader {Gl.GetProgramInfoLog(program)}");
            }
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
                case Key.X:
                    camera.DecreaseZXAngle();
                    break;
                case Key.W:
                    cubeArrangementModel.IsMovingForward = true;
                    break;
                case Key.S:
                    cubeArrangementModel.IsMovingBackward = true;
                    break;

                case Key.A:
                    cubeArrangementModel.IsTurningLeft = true;
                    break;
                case Key.D:
                    cubeArrangementModel.IsTurningRight = true;
                    break;
                case Key.R:
                    cubeArrangementModel.Reset();
                    InitializeBoosts();
                    ResetBots();
                    break;
                case Key.F:
                    if (camera.Mode == CameraDescriptor.CameraMode.ThirdPerson)
                        camera.SetCameraMode(CameraDescriptor.CameraMode.FirstPerson);
                    else if (camera.Mode == CameraDescriptor.CameraMode.FirstPerson)
                        camera.SetCameraMode(CameraDescriptor.CameraMode.ThirdPerson);
                    break;
                case Key.Escape:
                    camera.SetCameraMode(CameraDescriptor.CameraMode.Free);
                    break;

            }
        }

        private static void Keyboard_KeyUp(IKeyboard keyboard, Key key, int arg3)
        {
            switch (key)
            {
                case Key.W:
                    cubeArrangementModel.IsMovingForward = false;
                    break;
                case Key.S:
                    cubeArrangementModel.IsMovingBackward = false;
                    break;
                case Key.A:
                    cubeArrangementModel.IsTurningLeft = false;
                    break;
                case Key.D:
                    cubeArrangementModel.IsTurningRight = false;
                    break;
            }
        }

        private static void GraphicWindow_Update(double deltaTime)
        {
            // NO OpenGL
            // make it threadsafe
            lastDeltaTime = (float)deltaTime;
            gameTime += deltaTime;

            cubeArrangementModel.UpdateMovement((float)deltaTime);
            cubeArrangementModel.AdvanceTime(deltaTime);

            CheckBoostCollisions();

            botsDescriptor.Update((float)deltaTime);

            imGuiController.Update((float)deltaTime);
        }

        private static unsafe void GraphicWindow_Render(double deltaTime)
        {
            Gl.Clear(ClearBufferMask.ColorBufferBit);
            Gl.Clear(ClearBufferMask.DepthBufferBit);

            Gl.UseProgram(program);

            SetUniform3(LightColorVariableName, new Vector3(1f, 1f, 1f));
            SetUniform3(LightPositionVariableName, new Vector3(7f, 7f, 7f));
            SetUniform3(ViewPositionVariableName, new Vector3(camera.Position.X, camera.Position.Y, camera.Position.Z));
            SetUniform1(ShinenessVariableName, shininess);

            var viewMatrix = Matrix4X4.CreateLookAt(camera.Position, camera.Target, camera.UpVector);
            SetMatrix(viewMatrix, ViewMatrixVariableName);

            var projectionMatrix = Matrix4X4.CreatePerspectiveFieldOfView<float>((float)(Math.PI / 2), 1024f / 768f, 0.1f, 100f);
            SetMatrix(projectionMatrix, ProjectionMatrixVariableName);

            DrawSkyBox();

            var modelMatrixCenterCube = cubeArrangementModel.GetTransformMatrix();
            SetModelMatrix(modelMatrixCenterCube);
            DrawModelObject(custom);

            DrawBoosts();

            var bots = botsDescriptor.GetBots();
            foreach (var bot in bots)
            {
                Matrix4X4<float> botModelMatrix = bot.GetTransformMatrix();
                SetModelMatrix(botModelMatrix);
                DrawModelObject(bot.IsHammerhead ? bot2 : bot1);

            }

            ImGui.Begin("Camera Controls");

            ImGui.Text($"Current Camera Mode: {camera.Mode}");
            ImGui.Separator();

            if (ImGui.Button("Third Person"))
            {
                camera.SetCameraMode(CameraDescriptor.CameraMode.ThirdPerson);
            }
            ImGui.SameLine();
            if (ImGui.Button("First Person"))
            {
                camera.SetCameraMode(CameraDescriptor.CameraMode.FirstPerson);
            }
            ImGui.End();

            imGuiController.Render();
        }

        private static unsafe void DrawSkyBox()
        {
            var modelMatrixSkyBox = Matrix4X4.CreateScale(100f);
            SetModelMatrix(modelMatrixSkyBox);

            // set the texture
            int textureLocation = Gl.GetUniformLocation(program, TextureVariableName);
            if (textureLocation == -1)
            {
                throw new Exception($"{TextureVariableName} uniform not found on shader.");
            }
            // set texture 0
            Gl.Uniform1(textureLocation, 0);
            Gl.ActiveTexture(TextureUnit.Texture0);
            Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (float)GLEnum.Linear);
            Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (float)GLEnum.Linear);
            Gl.BindTexture(TextureTarget.Texture2D, skybox.Texture.Value);

            DrawModelObject(skybox);

            CheckError();
            Gl.BindTexture(TextureTarget.Texture2D, 0);
            CheckError();
        }

        private static void DrawBoosts()
        {
            for (int i = 0; i < 8; i++)
            {
                if (!boostCollected[i])
                {
                    double oscillationPhase = gameTime * 2.0 + boostTimeOffsets[i];
                    float scale = 0.3f + 0.05f * (float)Math.Sin(oscillationPhase);

                    float rotation = (float)(gameTime + boostTimeOffsets[i]);

                    var boostMatrix = Matrix4X4.CreateScale(scale) * Matrix4X4.CreateRotationY(rotation) * Matrix4X4.CreateTranslation(boostPositions[i]);

                    SetModelMatrix(boostMatrix);
                    DrawModelObject(boost);
                }
            }
        }

        private static void ResetBots()
        {
            foreach (var obj in botsDescriptor.GetBots())
            {
                obj.Reset();
            }
        }

        private static unsafe void SetModelMatrix(Matrix4X4<float> modelMatrix)
        {
            SetMatrix(modelMatrix, ModelMatrixVariableName);

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

        private static unsafe void SetUniform1(string uniformName, float uniformValue)
        {
            int location = Gl.GetUniformLocation(program, uniformName);
            if (location == -1)
            {
                throw new Exception($"{uniformName} uniform not found on shader.");
            }

            Gl.Uniform1(location, uniformValue);
            CheckError();
        }

        private static unsafe void SetUniform3(string uniformName, Vector3 uniformValue)
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

        private static unsafe void SetMatrix(Matrix4X4<float> mx, string uniformName)
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
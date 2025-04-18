using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Szeminarium;
using System.Numerics;
using Silk.NET.OpenGL.Extensions.ImGui;

namespace GrafikaSzeminarium
{
    internal class Program
    {
        private static IWindow graphicWindow;

        private static GL Gl;
        private static ModelObjectDescriptor[] rubiksCubePieces;
        private const float GAP = 0.05f;
        private const float CUBE_SIZE = 0.4f;

        private static CameraDescriptor camera = new CameraDescriptor();

        private static CubeArrangementModel cubeArrangementModel = new CubeArrangementModel();

        private static ImGuiController imGuiController;

        private const string ModelMatrixVariableName = "uModel";
        private const string ViewMatrixVariableName = "uView";
        private const string ProjectionMatrixVariableName = "uProjection";
        private const string LayerRotationMatrixVariableName = "uLayerRotation";
        private const string LightPositionVariableName = "lightPos";
        private const string LightColorVariableName = "lightColor";
        private const string ViewPositionVariableName = "viewPos";

        private static Vector3 lightPosition = new(2.0f, 2.0f, 2.0f);
        private static Vector3 lightColor = new(1.0f, 1.0f, 1.0f);
        private static string lightPosX = "2.0";
        private static string lightPosY = "2.0";
        private static string lightPosZ = "2.0";

        private static readonly string VertexShaderSource = @"
        #version 330 core
        layout (location = 0) in vec3 vPos;
        layout (location = 1) in vec4 vCol;
        
        uniform mat4 uModel;
        uniform mat4 uView;
        uniform mat4 uProjection;
        uniform mat4 uLayerRotation;
        
        out vec4 outCol;
        out vec3 fragPos;
        out vec3 normal;
        
        void main()
        {
            outCol = vCol;
            vec4 worldPos = uLayerRotation * uModel * vec4(vPos, 1.0);
            fragPos = worldPos.xyz;
            
            normal = normalize(mat3(uLayerRotation * uModel) * normalize(vPos));
            
            gl_Position = uProjection * uView * worldPos;
        }";


        private static readonly string FragmentShaderSource = @"
        #version 330 core
        out vec4 FragColor;
        
        in vec4 outCol;
        in vec3 fragPos;
        in vec3 normal;
        
        uniform vec3 lightPos;
        uniform vec3 lightColor;
        uniform vec3 viewPos;
        
        void main()
        {
            // Ambient
            float ambientStrength = 0.3;
            vec3 ambient = ambientStrength * lightColor;
            
            // Diffuse
            vec3 norm = normalize(normal);
            vec3 lightDir = normalize(lightPos - fragPos);
            float diff = max(dot(norm, lightDir), 0.0);
            vec3 diffuse = diff * lightColor;
            
            // Specular
            float specularStrength = 0.5;
            vec3 viewDir = normalize(viewPos - fragPos);
            vec3 reflectDir = reflect(-lightDir, norm);
            float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
            vec3 specular = specularStrength * spec * lightColor;
            
            // Combine results
            vec3 result = (ambient + diffuse + specular) * outCol.rgb;
            FragColor = vec4(result, outCol.a);
        }
        ";

        private static uint program;

        static void Main(string[] args)
        {
            WindowOptions windowOptions = WindowOptions.Default;
            windowOptions.Title = "Grafika szeminárium";
            windowOptions.Size = new Silk.NET.Maths.Vector2D<int>(1024, 1024);

            graphicWindow = Window.Create(windowOptions);

            graphicWindow.Load += GraphicWindow_Load;
            graphicWindow.Update += GraphicWindow_Update;
            graphicWindow.Render += GraphicWindow_Render;
            graphicWindow.Closing += GraphicWindow_Closing;

            graphicWindow.Run();
        }

        private static void GraphicWindow_Closing()
        {
            foreach (var cube in rubiksCubePieces)
            {
                cube.Dispose();
            }
            Gl.DeleteProgram(program);
        }

        private static void GraphicWindow_Load()
        {
            Gl = graphicWindow.CreateOpenGL();

            var inputContext = graphicWindow.CreateInput();

            imGuiController = new ImGuiController(Gl, graphicWindow, inputContext);

            foreach (var keyboard in inputContext.Keyboards)
            {
                keyboard.KeyDown += Keyboard_KeyDown;
            }

            rubiksCubePieces = ModelObjectDescriptor.CreateRubiksCube(Gl);
            cubeArrangementModel.AnimationEnabled = true;

            Gl.ClearColor(System.Drawing.Color.White);

            Gl.Enable(EnableCap.CullFace);
            Gl.CullFace(TriangleFace.Back);

            Gl.Enable(EnableCap.DepthTest);
            Gl.DepthFunc(DepthFunction.Lequal);


            uint vshader = Gl.CreateShader(ShaderType.VertexShader);
            uint fshader = Gl.CreateShader(ShaderType.FragmentShader);

            Gl.ShaderSource(vshader, VertexShaderSource);
            Gl.CompileShader(vshader);
            Gl.GetShader(vshader, ShaderParameterName.CompileStatus, out int vStatus);
            if (vStatus != (int)GLEnum.True)
                throw new Exception("Vertex shader failed to compile: " + Gl.GetShaderInfoLog(vshader));

            Gl.ShaderSource(fshader, FragmentShaderSource);
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

        private static void Keyboard_KeyDown(IKeyboard keyboard, Key key, int arg3)
        {
            switch (key)
            {
                case Key.W:
                    camera.MoveForward();
                    break;
                case Key.S:
                    camera.MoveBackward();
                    break;
                case Key.A:
                    camera.MoveLeft();
                    break;
                case Key.D:
                    camera.MoveRight();
                    break;
                case Key.Q:
                    camera.MoveDown();
                    break;
                case Key.E:
                    camera.MoveUp();
                    break;
                case Key.Left:
                    camera.DecreaseYaw();
                    break;
                case Key.Right:
                    camera.IncreaseYaw();
                    break;
                case Key.Up:
                    camera.IncreasePitch();
                    break;
                case Key.Down:
                    camera.DecreasePitch();
                    break;
                case Key.M:
                    cubeArrangementModel.StartTopLayerRotationRight();
                    break;
                case Key.N:
                    cubeArrangementModel.StartTopLayerRotationLeft();
                    break;
                case Key.B:
                    cubeArrangementModel.StartMiddleLayerRotationRight();
                    break;
                case Key.V:
                    cubeArrangementModel.StartMiddleLayerRotationLeft();
                    break;
                case Key.C:
                    cubeArrangementModel.StartBottomLayerRotationRight();
                    break;
                case Key.X:
                    cubeArrangementModel.StartBottomLayerRotationLeft();
                    break;
                case Key.L:
                    cubeArrangementModel.StartLeftLayerRotationUp();
                    break;
                case Key.K:
                    cubeArrangementModel.StartLeftLayerRotationDown();
                    break;
                case Key.J:
                    cubeArrangementModel.StartCenterLayerRotationUp();
                    break;
                case Key.H:
                    cubeArrangementModel.StartCenterLayerRotationDown();
                    break;
                case Key.G:
                    cubeArrangementModel.StartRightLayerRotationUp();
                    break;
                case Key.F:
                    cubeArrangementModel.StartRightLayerRotationDown();
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

            Gl.UseProgram(program);

            var viewMatrix = Matrix4X4.CreateLookAt(camera.Position, camera.Target, camera.UpVector);
            SetMatrix(viewMatrix, ViewMatrixVariableName);

            var projectionMatrix = Matrix4X4.CreatePerspectiveFieldOfView<float>((float)(Math.PI / 2), 1024f / 768f, 0.1f, 100f);
            SetMatrix(projectionMatrix, ProjectionMatrixVariableName);

            int lightPosLocation = Gl.GetUniformLocation(program, LightPositionVariableName);
            Gl.Uniform3(lightPosLocation, lightPosition.X, lightPosition.Y, lightPosition.Z);

            int lightColorLocation = Gl.GetUniformLocation(program, LightColorVariableName);
            Gl.Uniform3(lightColorLocation, lightColor.X, lightColor.Y, lightColor.Z);

            int viewPosLocation = Gl.GetUniformLocation(program, ViewPositionVariableName);
            Gl.Uniform3(viewPosLocation, camera.Position.X, camera.Position.Y, camera.Position.Z);

            var topRotationMatrix = Matrix4X4.CreateRotationY((float)(cubeArrangementModel.TopLayerRotationAngle * Math.PI / 180.0));
            var middleRotationMatrix = Matrix4X4.CreateRotationY((float)(cubeArrangementModel.MiddleLayerRotationAngle * Math.PI / 180.0));
            var bottomRotationMatrix = Matrix4X4.CreateRotationY((float)(cubeArrangementModel.BottomLayerRotationAngle * Math.PI / 180.0));

            var leftRotationMatrix = Matrix4X4.CreateRotationX((float)(cubeArrangementModel.LeftLayerRotationAngle * Math.PI / 180.0));
            var centerRotationMatrix = Matrix4X4.CreateRotationX((float)(cubeArrangementModel.CenterLayerRotationAngle * Math.PI / 180.0));
            var rightRotationMatrix = Matrix4X4.CreateRotationX((float)(cubeArrangementModel.RightLayerRotationAngle * Math.PI / 180.0));

            int index = 0;
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    for (int z = -1; z <= 1; z++)
                    {
                        var translation = Matrix4X4.CreateTranslation(
                            x * (CUBE_SIZE + GAP),
                            y * (CUBE_SIZE + GAP),
                            z * (CUBE_SIZE + GAP));
                        var scale = Matrix4X4.CreateScale(CUBE_SIZE);
                        var modelMatrix = scale * translation;

                        Matrix4X4<float> layerRotation;
                        if (y == 1)
                            layerRotation = topRotationMatrix;
                        else if (y == 0)
                            layerRotation = middleRotationMatrix;
                        else
                            layerRotation = bottomRotationMatrix;

                        if (x == -1)
                            layerRotation = layerRotation * leftRotationMatrix;
                        else if (x == 0)
                            layerRotation = layerRotation * centerRotationMatrix;
                        else
                            layerRotation = layerRotation * rightRotationMatrix;

                        SetMatrix(layerRotation, LayerRotationMatrixVariableName);
                        SetMatrix(modelMatrix, ModelMatrixVariableName);
                        DrawModelObject(rubiksCubePieces[index]);
                        index++;
                    }
                }
            }

            ImGuiNET.ImGui.Begin("Lighting", ImGuiNET.ImGuiWindowFlags.AlwaysAutoResize | ImGuiNET.ImGuiWindowFlags.NoCollapse);
            ImGuiNET.ImGui.Text("Light Color");

            float[] lightColorArr = new float[] { lightColor.X, lightColor.Y, lightColor.Z };

            ImGuiNET.ImGui.SliderFloat("Red", ref lightColorArr[0], 0.0f, 1.0f);
            ImGuiNET.ImGui.SliderFloat("Green", ref lightColorArr[1], 0.0f, 1.0f);
            ImGuiNET.ImGui.SliderFloat("Blue", ref lightColorArr[2], 0.0f, 1.0f);

            lightColor = new Vector3(lightColorArr[0], lightColorArr[1], lightColorArr[2]);

            ImGuiNET.ImGui.Text("Light Position");

            ImGuiNET.ImGui.InputText("X", ref lightPosX, 10);
            ImGuiNET.ImGui.InputText("Y", ref lightPosY, 10);
            ImGuiNET.ImGui.InputText("Z", ref lightPosZ, 10);

            if (float.TryParse(lightPosX, out float xx) && float.TryParse(lightPosY, out float yy) && float.TryParse(lightPosZ, out float zz))
            {
                lightPosition = new Vector3(xx, yy, zz);
            }

            ImGuiNET.ImGui.Text("Cube Rotations");
            if (ImGuiNET.ImGui.Button("Top Layer Right"))
                cubeArrangementModel.StartTopLayerRotationRight();

            ImGuiNET.ImGui.SameLine();
            if (ImGuiNET.ImGui.Button("Top Layer Left"))
                cubeArrangementModel.StartTopLayerRotationLeft();

            if (ImGuiNET.ImGui.Button("Middle Layer Right"))
                cubeArrangementModel.StartMiddleLayerRotationRight();

            ImGuiNET.ImGui.SameLine();
            if (ImGuiNET.ImGui.Button("Middle Layer Left"))
                cubeArrangementModel.StartMiddleLayerRotationLeft();

            if (ImGuiNET.ImGui.Button("Bottom Layer Right"))
                cubeArrangementModel.StartBottomLayerRotationRight();

            ImGuiNET.ImGui.SameLine();
            if (ImGuiNET.ImGui.Button("Bottom Layer Left"))
                cubeArrangementModel.StartBottomLayerRotationLeft();

            if (ImGuiNET.ImGui.Button("Left Layer Up"))
                cubeArrangementModel.StartLeftLayerRotationUp();

            ImGuiNET.ImGui.SameLine();
            if (ImGuiNET.ImGui.Button("Left Layer Down"))
                cubeArrangementModel.StartLeftLayerRotationDown();

            if (ImGuiNET.ImGui.Button("Center Layer Up"))
                cubeArrangementModel.StartCenterLayerRotationUp();

            ImGuiNET.ImGui.SameLine();
            if (ImGuiNET.ImGui.Button("Center Layer Down"))
                cubeArrangementModel.StartCenterLayerRotationDown();

            if (ImGuiNET.ImGui.Button("Right Layer Up"))
                cubeArrangementModel.StartRightLayerRotationUp();

            ImGuiNET.ImGui.SameLine();
            if (ImGuiNET.ImGui.Button("Right Layer Down"))
                cubeArrangementModel.StartRightLayerRotationDown();

            ImGuiNET.ImGui.End();

            imGuiController.Render();
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
                throw new Exception($"{ViewMatrixVariableName} uniform not found on shader.");
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
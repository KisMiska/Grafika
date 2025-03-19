using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Szeminarium1
{
    internal static class Program
    {
        private static IWindow graphicWindow;

        private static GL Gl;

        private static uint program;

        private static readonly string VertexShaderSource = @"
        #version 330 core
        layout (location = 0) in vec3 vPos;
		layout (location = 1) in vec4 vCol;

		out vec4 outCol;
        
        void main()
        {
			outCol = vCol;
            gl_Position = vec4(vPos.x, vPos.y, vPos.z, 1.0);
        }
        ";


        private static readonly string FragmentShaderSource = @"
        #version 330 core
        out vec4 FragColor;
		
		in vec4 outCol;

        void main()
        {
            FragColor = outCol;
        }
        ";

        private static void ErrorCheck()
        {
            GLEnum error = Gl.GetError();
            if (error != GLEnum.NoError)
            {
                Console.WriteLine($"ERROR: {error}");
            }
        }


        static void Main(string[] args)
        {
            WindowOptions windowOptions = WindowOptions.Default;
            windowOptions.Title = "lab1-2 2D Kocka";
            windowOptions.Size = new Silk.NET.Maths.Vector2D<int>(500, 500);

            graphicWindow = Window.Create(windowOptions);

            graphicWindow.Load += GraphicWindow_Load;
            graphicWindow.Update += GraphicWindow_Update;
            graphicWindow.Render += GraphicWindow_Render;

            graphicWindow.Run();
        }

        private static void GraphicWindow_Load()
        {
            // egszeri beallitasokat
            //Console.WriteLine("Loaded");

            Gl = graphicWindow.CreateOpenGL();

            Gl.ClearColor(System.Drawing.Color.White);

            uint vshader = Gl.CreateShader(ShaderType.VertexShader);
            uint fshader = Gl.CreateShader(ShaderType.FragmentShader);

            Gl.ShaderSource(vshader, VertexShaderSource);
            Gl.CompileShader(vshader);
            Gl.GetShader(vshader, ShaderParameterName.CompileStatus, out int vStatus);
            if (vStatus != (int)GLEnum.True)
                throw new Exception("Vertex shader failed to compile: " + Gl.GetShaderInfoLog(vshader));
            ErrorCheck();

            Gl.ShaderSource(fshader, FragmentShaderSource);
            Gl.CompileShader(fshader);
            ErrorCheck();

            program = Gl.CreateProgram();
            Gl.AttachShader(program, vshader);
            Gl.AttachShader(program, fshader);
            Gl.LinkProgram(program);
            Gl.DetachShader(program, vshader);
            Gl.DetachShader(program, fshader);
            Gl.DeleteShader(vshader);
            Gl.DeleteShader(fshader);
            ErrorCheck();

            Gl.GetProgram(program, GLEnum.LinkStatus, out var status);
            if (status == 0)
            {
                Console.WriteLine($"Error linking shader {Gl.GetProgramInfoLog(program)}");
            }

        }

        private static void GraphicWindow_Update(double deltaTime)
        {
            // NO GL
            // make it threadsave
            //Console.WriteLine($"Update after {deltaTime} [s]");
        }

        private static unsafe void GraphicWindow_Render(double deltaTime)
        {
            //Console.WriteLine($"Render after {deltaTime} [s]");

            Gl.Clear(ClearBufferMask.ColorBufferBit);

            uint vao = Gl.GenVertexArray();
            Gl.BindVertexArray(vao);

            float[] vertexArray = new float[] {
                0f, 1.0f, 0.0f, //red H -0
                -0.87f, 0.5f, 0.0f, //red I -1
                -0.87f, 0.5f, 0.0f, //green I -2
                0.87f, 0.5f, 0.0f, //red J -3
                0.87f, 0.5f, 0.0f, //blue J -4
                0.0f, 0.0f, 0.0f, //red K -5
                0.0f, 0.0f, 0.0f, //green K -6
                0.0f, 0.0f, 0.0f, //blue K -7
                -0.87f, -0.5f, 0.0f, //green L -8
                0.87f, -0.5f, 0.0f, //blue N -9
                0.0f, -1.0f, 0.0f, //green M -10
                0.0f, -1.0f, 0.0f, //blue M -11
                //lines
                0.29f, 0.83f, 0.0f, //12
                -0.58f, 0.33f, 0.0f, //13
                0.33f, 0.81f, 0.0f, //14
                -0.54f, 0.31f, 0.0f, //15
                0.58f, 0.67f, 0.0f, //16
                -0.29f, 0.17f, 0.0f, //17
                0.61f, 0.65f, 0.0f, //18
                -0.25f, 0.15f, 0.0f, //19
                -0.29f, 0.83f, 0.0f, //20
                0.58f, 0.33f, 0.0f, //21
                -0.32f, 0.81f, 0.0f, //22
                0.54f, 0.31f, 0.0f, //23
                -0.58f, 0.67f, 0.0f, //24
                0.29f, 0.17f, 0.0f, //25
                -0.61f, 0.65f, 0.0f, //26
                0.25f, 0.15f, 0.0f, //27
                -0.58f, 0.33f, 0.0f, //28
                -0.58f, -0.67f, 0.0f, //29
                -0.54f, 0.31f, 0.0f, //30
                -0.54f, -0.69f, 0.0f, //31
                -0.29f, 0.17f, 0.0f, //32
                -0.29f, -0.83f, 0.0f, //33
                -0.25f, 0.15f, 0.0f, //34
                -0.25f, -0.85f, 0.0f, //35
                -0.87f, 0.17f, 0.0f, //36
                0.0f, -0.33f, 0.0f, //37
                -0.87f, 0.21f, 0.0f, //38
                0.0f, -0.29f, 0.0f, //39
                -0.87f, -0.17f, 0.0f, //40
                0.0f, -0.67f, 0.0f, //41
                -0.87f, -0.13f, 0.0f, //42
                0.0f, -0.63f, 0.0f, //43
                0.25f, 0.15f, 0.0f, //44
                0.25f, -0.85f, 0.0f, //45
                0.29f, 0.17f, 0.0f, //46
                0.29f, -0.83f, 0.0f, //47
                0.54f, 0.31f, 0.0f, //48
                0.54f, -0.69f, 0.0f, //49
                0.58f, 0.33f, 0.0f, //50
                0.58f, -0.67f, 0.0f, //51
                0.0f, -0.29f, 0.0f, //52
                0.87f, 0.21f, 0.0f, //53
                0.0f, -0.33f, 0.0f, //54
                0.87f, 0.17f, 0.0f, //55
                0.0f, -0.63f, 0.0f, //56
                0.87f, -0.13f, 0.0f, //57
                0.0f, -0.67f, 0.0f, //58
                0.87f, -0.17f, 0.0f, //59
            };

            float[] colorArray = new float[] {
                1.0f, 0.0f, 0.0f, 1.0f,
                1.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 1.0f, 0.0f, 1.0f,
                1.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f,
                1.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 1.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f,
                0.0f, 1.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f,
                0.0f, 1.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f,
                //lines
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
            };

            uint[] indexArray = new uint[] {
                0, 1, 5,
                0, 5, 3,
                2, 8, 10,
                2, 10, 6,
                7, 11, 4,
                11, 4, 9,
                //lines
                12, 13, 14,
                13, 14, 15,
                16, 17, 18,
                17, 18, 19,
                20, 21, 22,
                21, 22, 23,
                24, 25, 26,
                25, 26, 27,
                28, 29, 30,
                29, 30, 31,
                32, 33, 34,
                33, 34, 35,
                36, 37, 38,
                37, 38, 39,
                40, 41, 42,
                41, 42, 43,
                44, 45, 46,
                45, 46, 47,
                48, 49, 50,
                49, 50, 51,
                52, 53, 54,
                53, 54, 55,
                56, 57, 58,
                57, 58, 59,
            };

            uint vertices = Gl.GenBuffer();
            Gl.BindBuffer(GLEnum.ArrayBuffer, vertices);
            Gl.BufferData(GLEnum.ArrayBuffer, (ReadOnlySpan<float>)vertexArray.AsSpan(), GLEnum.StaticDraw);
            Gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, null);
            Gl.EnableVertexAttribArray(0);
            ErrorCheck();


            uint colors = Gl.GenBuffer();
            Gl.BindBuffer(GLEnum.ArrayBuffer, colors);
            Gl.BufferData(GLEnum.ArrayBuffer, (ReadOnlySpan<float>)colorArray.AsSpan(), GLEnum.StaticDraw);
            Gl.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 0, null);
            Gl.EnableVertexAttribArray(1);
            ErrorCheck();

            uint indices = Gl.GenBuffer();
            Gl.BindBuffer(GLEnum.ElementArrayBuffer, indices);
            Gl.BufferData(GLEnum.ElementArrayBuffer, (ReadOnlySpan<uint>)indexArray.AsSpan(), GLEnum.StaticDraw);
            ErrorCheck();

            Gl.BindBuffer(GLEnum.ArrayBuffer, 0);

            Gl.UseProgram(program);

            Gl.DrawElements(GLEnum.Triangles, (uint)indexArray.Length, GLEnum.UnsignedInt, null); // we used element buffer
            Gl.BindBuffer(GLEnum.ElementArrayBuffer, 0);
            Gl.BindVertexArray(vao);
            ErrorCheck();

            // always unbound the vertex buffer first, so no halfway results are displayed by accident
            Gl.DeleteBuffer(vertices);
            Gl.DeleteBuffer(colors);
            Gl.DeleteBuffer(indices);
            Gl.DeleteVertexArray(vao);
            ErrorCheck();
        }
    }
}

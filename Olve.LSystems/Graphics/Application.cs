using System.Reflection;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Olve.LSystems.Graphics;

public class Application
{
    private IWindow _window = null!;
    private GL _gl = null!;
    private string _vertexCode = string.Empty;
    private string _fragmentCode = string.Empty;
    private uint _vao;
    private uint _vbo;
    private uint _ebo;
    private uint _program;

    private uint[] _indices = [];
    private float[] _vertices = [];

    private uint Edges => (uint)(_indices.Length / 2);

    public void Run(Turtle.Turtle.Point[] points, uint[] indices)
    {
        var xMin = points.Min(x => x.X) - 0.1f;
        var xMax = points.Max(x => x.X) + 0.1f;
        var yMin = points.Min(x => x.Y) - 0.1f;
        var yMax = points.Max(x => x.Y) + 0.1f;
        
        _vertices = points.SelectMany(x => new[] { (x.X - xMin) / (xMax - xMin) * 2 - 1, (x.Y - yMin) / (yMax - yMin) * 2 - 1, 0f }).ToArray();
        _indices = indices;
        
        var options = WindowOptions.Default with
        {
            Size = new Vector2D<int>(1000, 1000),
            Title = "My first Silk.NET program!"
        };

        _window = Window.Create(options);

        _window.Load += OnLoad;
        _window.Update += OnUpdate;
        _window.Render += OnRender;
        
        var executablePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (executablePath == null)
        {
            throw new Exception("Could not get the executable path.");
        }
        
        _vertexCode = File.ReadAllText(Path.Combine(executablePath, "Shaders", "Vertex.glsl"));
        _fragmentCode = File.ReadAllText(Path.Combine(executablePath, "Shaders", "Fragment.glsl"));

        _window.Run();
    }

    private void OnLoad()
    {
        _gl = _window.CreateOpenGL();
        var input = _window.CreateInput();
            
        foreach (var keyboard in input.Keyboards)
        {
            keyboard.KeyDown += KeyDown;
        }

        _gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        _vao = _gl.GenVertexArray();
        _gl.BindVertexArray(_vao);
        
        
        _vbo = _gl.GenBuffer();
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);

        unsafe
        {
            fixed (float* buf = _vertices)
            {
                _gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint) (_vertices.Length * sizeof(float)), buf, BufferUsageARB.StaticDraw);
            }
        }
        
        _ebo = _gl.GenBuffer();
        _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _ebo);

        unsafe
        {
            fixed (uint* buf = _indices)
            {
                _gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint) (_indices.Length * sizeof(uint)), buf, BufferUsageARB.StaticDraw);
            }
        }
        
        uint vertexShader = _gl.CreateShader(ShaderType.VertexShader);
        _gl.ShaderSource(vertexShader, _vertexCode);
        
        _gl.CompileShader(vertexShader);

        _gl.GetShader(vertexShader, ShaderParameterName.CompileStatus, out int vStatus);
        if (vStatus != (int)GLEnum.True)
        {
            throw new Exception("Vertex shader failed to compile: " + _gl.GetShaderInfoLog(vertexShader));  
        }
        
        uint fragmentShader = _gl.CreateShader(ShaderType.FragmentShader);
        _gl.ShaderSource(fragmentShader, _fragmentCode);

        _gl.CompileShader(fragmentShader);

        _gl.GetShader(fragmentShader, ShaderParameterName.CompileStatus, out int fStatus);
        if (fStatus != (int) GLEnum.True)
        {
            throw new Exception("Fragment shader failed to compile: " + _gl.GetShaderInfoLog(fragmentShader));
        }
        
        _program = _gl.CreateProgram();
        
        _gl.AttachShader(_program, vertexShader);
        _gl.AttachShader(_program, fragmentShader);

        _gl.LinkProgram(_program);

        _gl.GetProgram(_program, ProgramPropertyARB.LinkStatus, out int lStatus);
        if (lStatus != (int) GLEnum.True)
        {
            throw new Exception("Program failed to link: " + _gl.GetProgramInfoLog(_program));
        }
        
        _gl.DetachShader(_program, vertexShader);
        _gl.DetachShader(_program, fragmentShader);
        _gl.DeleteShader(vertexShader);
        _gl.DeleteShader(fragmentShader);
        
        unsafe
        {
            const uint positionLoc = 0;
            _gl.EnableVertexAttribArray(positionLoc);
            _gl.VertexAttribPointer(positionLoc, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), (void*) 0);    
        }
        
        _gl.BindVertexArray(0);
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
        _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);
    }

    // These two methods are unused for this tutorial, aside from the logging we added earlier.
    private void OnUpdate(double deltaTime)
    {
    }

    private void OnRender(double deltaTime)
    {
        _gl.BindVertexArray(_vao);
        _gl.UseProgram(_program);
        
        unsafe
        {           
            _gl.DrawElements(PrimitiveType.Lines, Edges * 2, DrawElementsType.UnsignedInt, (void*) 0);
        }
    }

    private void KeyDown(IKeyboard keyboard, Key key, int keyCode)
    {
        if (key == Key.Escape)
        {
            _window.Close();
        }
    }
}
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Rendering.Guis;

public class GuiShader
{
    private readonly GL _gl;
    private readonly int _uScreenSize;
    private readonly int _uUseTexture;
    private readonly int _uTexture0;
    private readonly int _uAlphaThreshold;

    public uint Program { get; }

    private const string VertexShaderSource = @"
#version 330 core
layout (location = 0) in vec3 a_Position;
layout (location = 1) in vec2 a_TexCoord;
layout (location = 2) in vec4 a_Color;

uniform vec2 u_ScreenSize;

out vec2 v_TexCoord;
out vec4 v_Color;

void main()
{
    v_TexCoord = a_TexCoord;
    v_Color = a_Color;
    vec2 ndc = (a_Position.xy / u_ScreenSize) * 2.0 - 1.0;
    gl_Position = vec4(ndc.x, -ndc.y, a_Position.z, 1.0);
}
";

    private const string FragmentShaderSource = @"
#version 330 core
in vec2 v_TexCoord;
in vec4 v_Color;

uniform sampler2D u_Texture0;
uniform int u_UseTexture;
uniform float u_AlphaThreshold;

out vec4 FragColor;

void main()
{
    vec4 texColor = vec4(1.0);
    if (u_UseTexture != 0)
    {
        texColor = texture(u_Texture0, v_TexCoord);
    }
    FragColor = v_Color * texColor;
    if (FragColor.a < u_AlphaThreshold)
        discard;
}
";

    public GuiShader(GL gl)
    {
        _gl = gl;

        uint vertexShader = CompileShader(ShaderType.VertexShader, VertexShaderSource);
        uint fragmentShader = CompileShader(ShaderType.FragmentShader, FragmentShaderSource);

        Program = _gl.CreateProgram();
        _gl.AttachShader(Program, vertexShader);
        _gl.AttachShader(Program, fragmentShader);
        _gl.LinkProgram(Program);

        _gl.GetProgram(Program, GLEnum.LinkStatus, out int status);
        if (status == 0)
        {
            string infoLog = _gl.GetProgramInfoLog(Program);
            throw new Exception($"GuiShader linking failed: {infoLog}");
        }

        _gl.DetachShader(Program, vertexShader);
        _gl.DetachShader(Program, fragmentShader);
        _gl.DeleteShader(vertexShader);
        _gl.DeleteShader(fragmentShader);

        _uScreenSize = _gl.GetUniformLocation(Program, "u_ScreenSize");
        _uUseTexture = _gl.GetUniformLocation(Program, "u_UseTexture");
        _uTexture0 = _gl.GetUniformLocation(Program, "u_Texture0");
        _uAlphaThreshold = _gl.GetUniformLocation(Program, "u_AlphaThreshold");
    }

    private uint CompileShader(ShaderType type, string source)
    {
        uint shader = _gl.CreateShader(type);
        _gl.ShaderSource(shader, source);
        _gl.CompileShader(shader);

        _gl.GetShader(shader, GLEnum.CompileStatus, out int status);
        if (status == 0)
        {
            string infoLog = _gl.GetShaderInfoLog(shader);
            throw new Exception($"GuiShader {type} compile error: {infoLog}");
        }

        return shader;
    }

    public void Use() => _gl.UseProgram(Program);

    public void SetScreenSize(float width, float height) =>
        _gl.Uniform2(_uScreenSize, width, height);

    public void SetUseTexture(bool useTexture) =>
        _gl.Uniform1(_uUseTexture, useTexture ? 1 : 0);

    public void SetTexture0(int unit) =>
        _gl.Uniform1(_uTexture0, unit);

    public void SetAlphaThreshold(float threshold) =>
        _gl.Uniform1(_uAlphaThreshold, threshold);
}

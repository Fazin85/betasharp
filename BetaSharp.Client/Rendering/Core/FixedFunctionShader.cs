using System;
using Silk.NET.Maths;
using Silk.NET.OpenGL.Legacy;

namespace BetaSharp.Client.Rendering.Core;

public class FixedFunctionShader : IDisposable
{
    private readonly GL _gl;
    private readonly uint _program;

    private readonly int _uModelView;
    private readonly int _uProjection;
    private readonly int _uTextureMatrix;
    private readonly int _uUseTexture;
    private readonly int _uTexture0;
    private readonly int _uAlphaThreshold;

    public uint Program => _program;

    private const string VertexShaderSource = @"
#version 330 core
layout (location = 0) in vec3 a_Position;
layout (location = 1) in vec4 a_Color;
layout (location = 2) in vec2 a_TexCoord;
layout (location = 3) in vec3 a_Normal;

uniform mat4 u_ModelView;
uniform mat4 u_Projection;
uniform mat4 u_TextureMatrix;

out vec4 v_Color;
out vec2 v_TexCoord;

void main()
{
    v_Color = a_Color;
    vec4 tex = u_TextureMatrix * vec4(a_TexCoord, 0.0, 1.0);
    v_TexCoord = tex.xy;
    gl_Position = u_Projection * u_ModelView * vec4(a_Position, 1.0);
}";

    private const string FragmentShaderSource = @"
#version 330 core
in vec4 v_Color;
in vec2 v_TexCoord;

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
    
    // Alpha test emulation
    if (FragColor.a < u_AlphaThreshold)
        discard;
}";

    public FixedFunctionShader(GL gl)
    {
        _gl = gl;

        uint vertexShader = CompileShader(ShaderType.VertexShader, VertexShaderSource);
        uint fragmentShader = CompileShader(ShaderType.FragmentShader, FragmentShaderSource);

        _program = _gl.CreateProgram();
        _gl.AttachShader(_program, vertexShader);
        _gl.AttachShader(_program, fragmentShader);
        _gl.LinkProgram(_program);

        _gl.GetProgram(_program, GLEnum.LinkStatus, out var status);
        if (status == 0)
        {
            var infoLog = _gl.GetProgramInfoLog(_program);
            throw new Exception($"Program linking failed: {infoLog}");
        }

        _gl.DetachShader(_program, vertexShader);
        _gl.DetachShader(_program, fragmentShader);
        _gl.DeleteShader(vertexShader);
        _gl.DeleteShader(fragmentShader);

        _uModelView = _gl.GetUniformLocation(_program, "u_ModelView");
        _uProjection = _gl.GetUniformLocation(_program, "u_Projection");
        _uTextureMatrix = _gl.GetUniformLocation(_program, "u_TextureMatrix");
        _uUseTexture = _gl.GetUniformLocation(_program, "u_UseTexture");
        _uTexture0 = _gl.GetUniformLocation(_program, "u_Texture0");
        _uAlphaThreshold = _gl.GetUniformLocation(_program, "u_AlphaThreshold");
    }

    private uint CompileShader(ShaderType type, string source)
    {
        uint shader = _gl.CreateShader(type);
        _gl.ShaderSource(shader, source);
        _gl.CompileShader(shader);

        _gl.GetShader(shader, ShaderParameterName.CompileStatus, out var status);
        if (status == 0)
        {
            var infoLog = _gl.GetShaderInfoLog(shader);
            throw new Exception($"Error compiling {type}: {infoLog}");
        }

        return shader;
    }

    public void Use()
    {
        _gl.UseProgram(_program);
    }

    public unsafe void SetModelView(Matrix4X4<float> matrix)
    {
        _gl.UniformMatrix4(_uModelView, 1, false, (float*)&matrix);
    }

    public unsafe void SetProjection(Matrix4X4<float> matrix)
    {
        _gl.UniformMatrix4(_uProjection, 1, false, (float*)&matrix);
    }

    public unsafe void SetTextureMatrix(Matrix4X4<float> matrix)
    {
        _gl.UniformMatrix4(_uTextureMatrix, 1, false, (float*)&matrix);
    }

    public void SetUseTexture(bool useTexture)
    {
        _gl.Uniform1(_uUseTexture, useTexture ? 1 : 0);
    }

    public void SetTexture0(int unit)
    {
        _gl.Uniform1(_uTexture0, unit);
    }

    public void SetAlphaThreshold(float threshold)
    {
        _gl.Uniform1(_uAlphaThreshold, threshold);
    }

    public void Dispose()
    {
        _gl.DeleteProgram(_program);
    }
}

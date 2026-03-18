using System.Globalization;
using System.Text;

namespace BetaSharp.Client.Rendering.Entities.Models.FBX.Format;

public static class FbxLoader
{
    public static FbxModel load(string path)
    {
        FbxModel model = new FbxModel();
        string[] lines = File.ReadAllLines(path);

        FbxMesh currentMesh = null;
        List<float[]> rawVertices = new();
        List<int> rawIndices = new();
        List<float[]> rawUVs = new();
        List<int> rawUVIndices = new();

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();

            // Début d'un nouveau mesh
            if (line.StartsWith("Model:") && line.Contains("Mesh"))
            {
                if (currentMesh != null)
                    finalizeMesh(currentMesh, rawVertices, rawIndices, rawUVs, rawUVIndices, model);

                currentMesh = new FbxMesh { Name = extractName(line) };
                rawVertices.Clear(); rawIndices.Clear();
                rawUVs.Clear(); rawUVIndices.Clear();
            }

            // Vertices
            if (line.StartsWith("Vertices:"))
            {
                string data = readMultilineData(lines, ref i);
                rawVertices = parseFloatTriples(data);
            }

            // Indices de polygones
            if (line.StartsWith("PolygonVertexIndex:"))
            {
                string data = readMultilineData(lines, ref i);
                rawIndices = parseInts(data);
            }

            // UVs
            if (line.StartsWith("UV:") && currentMesh != null)
            {
                string data = readMultilineData(lines, ref i);
                rawUVs = parseFloatPairs(data);
            }

            if (line.StartsWith("UVIndex:") && currentMesh != null)
            {
                string data = readMultilineData(lines, ref i);
                rawUVIndices = parseInts(data);
            }
        }

        // Finalise le dernier mesh
        if (currentMesh != null)
            finalizeMesh(currentMesh, rawVertices, rawIndices, rawUVs, rawUVIndices, model);

        return model;
    }

    private static void finalizeMesh(FbxMesh mesh,
        List<float[]> verts, List<int> indices,
        List<float[]> uvs, List<int> uvIndices,
        FbxModel model)
    {
        List<FbxVertex> outVerts = new();
        List<FbxTriangle> outTris = new();

        // FBX encode les polygones avec -1 comme marqueur de fin de face
        // ex: [0, 1, 2, -3] = triangle 0,1,2 (le dernier index est ~val)
        List<int> face = new();
        int uvIdx = 0;

        for (int i = 0; i < indices.Count; i++)
        {
            int rawIdx = indices[i];
            bool lastInFace = rawIdx < 0;
            int vertIdx = lastInFace ? ~rawIdx : rawIdx;

            float[] pos = verts[vertIdx];
            float u = 0, v = 0;
            if (uvIdx < uvIndices.Count)
            {
                float[] uv = uvs[uvIndices[uvIdx++]];
                u = uv[0]; v = 1.0f - uv[1]; // FBX inverse V
            }

            face.Add(outVerts.Count);
            outVerts.Add(new FbxVertex { X = pos[0], Y = pos[1], Z = pos[2], U = u, V = v });

            if (lastInFace)
            {
                // Triangulation en éventail : supporte quads et N-gons
                for (int t = 1; t < face.Count - 1; t++)
                    outTris.Add(new FbxTriangle { V0 = face[0], V1 = face[t], V2 = face[t + 1] });
                face.Clear();
            }
        }

        mesh.Vertices = outVerts.ToArray();
        mesh.Triangles = outTris.ToArray();
        model.Meshes.Add(mesh);
    }

    // Helpers de parsing
    private static string readMultilineData(string[] lines, ref int i)
    {
        StringBuilder sb = new();
        while (++i < lines.Length)
        {
            string l = lines[i].Trim();
            if (l == "}") break;
            sb.Append(l).Append(',');
        }
        return sb.ToString();
    }

    private static List<float[]> parseFloatTriples(string data)
    {
        var nums = data.Split(',', StringSplitOptions.RemoveEmptyEntries);
        var result = new List<float[]>();
        for (int i = 0; i + 2 < nums.Length; i += 3)
            result.Add(new[] {
                float.Parse(nums[i],   CultureInfo.InvariantCulture),
                float.Parse(nums[i+1], CultureInfo.InvariantCulture),
                float.Parse(nums[i+2], CultureInfo.InvariantCulture)
            });
        return result;
    }

    private static List<float[]> parseFloatPairs(string data)
    {
        var nums = data.Split(',', StringSplitOptions.RemoveEmptyEntries);
        var result = new List<float[]>();
        for (int i = 0; i + 1 < nums.Length; i += 2)
            result.Add(new[] {
                float.Parse(nums[i],   CultureInfo.InvariantCulture),
                float.Parse(nums[i+1], CultureInfo.InvariantCulture)
            });
        return result;
    }

    private static List<int> parseInts(string data)
    {
        return data.Split(',', StringSplitOptions.RemoveEmptyEntries)
                   .Select(s => int.Parse(s.Trim()))
                   .ToList();
    }

    private static string extractName(string line)
    {
        int start = line.IndexOf('"') + 1;
        int end = line.IndexOf('"', start);
        return end > start ? line[start..end] : "unnamed";
    }
}

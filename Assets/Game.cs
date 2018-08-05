using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour 
{
    public struct Line {
        public Vector3 a;
        public Vector3 b;
    }

    public const int LINE_BUFFER_SIZE = 10000;

    ComputeShader _asteroidsShader;
    Shader _drawLinesShader;
    Material _drawLinesMaterial;

    ComputeBuffer _linesBuffer;


    public void Awake() {
        _asteroidsShader = Resources.Load<ComputeShader>("asteroids");
        _drawLinesShader = Shader.Find("DrawLines");
        _drawLinesMaterial = new Material(_drawLinesShader);
        _drawLinesMaterial.hideFlags = HideFlags.HideAndDontSave;

        _linesBuffer = ComputeBufferUtils.Alloc<Line>(LINE_BUFFER_SIZE);
    }

    public void OnPostRender() {

        Line[] testLines = new Line[3];
        testLines[0] = new Line { a = new Vector3(0, 0, 0), b = new Vector3(1, 0, 0) };
        testLines[1] = new Line { a = new Vector3(0, 0, 0), b = new Vector3(1, 1, 0) };
        testLines[2] = new Line { a = new Vector3(0, 0, 0), b = new Vector3(0, 1, 0) };
        _linesBuffer.SetData(testLines);

        _drawLinesMaterial.SetBuffer("lines", _linesBuffer);
        _drawLinesMaterial.SetPass(0);
        Graphics.DrawProcedural(MeshTopology.Lines, testLines.Length*2);
    }
}

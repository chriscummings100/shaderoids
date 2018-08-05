using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Game : MonoBehaviour 
{
    public struct Line {
        public Vector2 a;
        public Vector2 b;

        public override string ToString() {
            return string.Format("[{0},{1}]->[{2},{3}], ang {4}", a.x, a.y, b.x, b.y, Mathf.Atan2(b.y-a.y,b.x-a.x)*Mathf.Rad2Deg);
        }
    }
    public struct Counters {
        public int numLines;
    }
    public struct KeyState {
        public int down;
        public int pressed;
        public int released;
    }
    public struct PlayerState {
        public Vector2 position;
        public float rotation;
        public Vector2 speed;
        public bool alive;
    }

    public const int LINE_BUFFER_SIZE = 10000;
    public const int MAX_PLAYERS = 1;

    ComputeShader _asteroidsShader;
    Shader _drawLinesShader;
    Material _drawLinesMaterial;

    ComputeBuffer _linesBuffer;
    ComputeBuffer _countersBuffer;
    ComputeBuffer _dispatchBuffer;
    ComputeBuffer _keyStates;
    ComputeBuffer _playerState;

    CommandBuffer _commands;

    KeyState[] _cpuKeyStates;

    const int KEY_ESCAPE = 0;

    int kernelClearLines;
    int kernelGenerateTestLines;
    int kernelLineDispatchArgs;
    int kernelUpdateAndDrawPlayer;

    bool isInitialized = false;

    public void Init() {
        _asteroidsShader = Resources.Load<ComputeShader>("asteroids");
        _drawLinesShader = Shader.Find("DrawLines");
        _drawLinesMaterial = new Material(_drawLinesShader);
        _drawLinesMaterial.hideFlags = HideFlags.HideAndDontSave;

        _linesBuffer = ComputeBufferUtils.Alloc<Line>(LINE_BUFFER_SIZE);
        _countersBuffer = ComputeBufferUtils.Alloc<Counters>(1);
        _dispatchBuffer = ComputeBufferUtils.Alloc<uint>(8, ComputeBufferType.IndirectArguments);
        _keyStates = ComputeBufferUtils.Alloc<KeyState>(256);
        _playerState = ComputeBufferUtils.Alloc<PlayerState>(MAX_PLAYERS);

        kernelClearLines = _asteroidsShader.FindKernel("ClearLines");
        kernelGenerateTestLines = _asteroidsShader.FindKernel("GenerateTestLines");
        kernelLineDispatchArgs = _asteroidsShader.FindKernel("LineDispatchArgs");
        kernelUpdateAndDrawPlayer = _asteroidsShader.FindKernel("UpdateAndDrawPlayer");

        isInitialized = true;

        _cpuKeyStates = new KeyState[256];

        PlayerState[] initPlayer = new PlayerState[1];
        initPlayer[0].position = new Vector2(1024f, 768f) * 0.5f;
        initPlayer[0].alive = true;
        _playerState.SetData(initPlayer);
    }

    void DispatchOne(int kernel) {
        _asteroidsShader.Dispatch(kernel, 1, 1, 1);
    }
    void DispatchItems(int kernel, int items) {
        uint x,y,z;
        _asteroidsShader.GetKernelThreadGroupSizes(kernel, out x, out y, out z);
        _asteroidsShader.SetInt("_threadCount", items);
        _asteroidsShader.Dispatch(kernel, (items + (int)x - 1) / (int)x, 1, 1);
    }

    public void OnPostRender() {
        if (!isInitialized)
            Init();

        //read and store all key states
        for(int i = 0; i < 26; i++) {
            _cpuKeyStates['a' + i] = new KeyState {
                down = Input.GetKey(KeyCode.A + i)?1:0,
                pressed = Input.GetKeyDown(KeyCode.A + i) ? 1 : 0,
                released = Input.GetKeyUp(KeyCode.A + i) ? 1 : 0,
            };
        }
        for (int i = 0; i < 10; i++) {
            _cpuKeyStates['0' + i] = new KeyState {
                down = Input.GetKey(KeyCode.Alpha0 + i) ? 1 : 0,
                pressed = Input.GetKeyDown(KeyCode.Alpha0 + i) ? 1 : 0,
                released = Input.GetKeyUp(KeyCode.Alpha0 + i) ? 1 : 0,
            };
        }
        _cpuKeyStates[' '] = new KeyState {
            down = Input.GetKey(KeyCode.Space) ? 1 : 0,
            pressed = Input.GetKeyDown(KeyCode.Space) ? 1 : 0,
            released = Input.GetKeyUp(KeyCode.Space) ? 1 : 0,
        };
        _cpuKeyStates[KEY_ESCAPE] = new KeyState {
            down = Input.GetKey(KeyCode.Escape) ? 1 : 0,
            pressed = Input.GetKeyDown(KeyCode.Escape) ? 1 : 0,
            released = Input.GetKeyUp(KeyCode.Escape) ? 1 : 0,
        };
        if(_cpuKeyStates['a'].down == 1) {
            Debug.Log("A is down");
        }
        _keyStates.SetData(_cpuKeyStates);

        Line[] testLines = new Line[10];
        int[] testDispatch = new int[4];
        Counters[] testCounters = new Counters[1];

        _asteroidsShader.SetBuffer(kernelClearLines, "_counters", _countersBuffer);
        DispatchOne(kernelClearLines);

        _asteroidsShader.SetBuffer(kernelUpdateAndDrawPlayer, "_counters", _countersBuffer);
        _asteroidsShader.SetBuffer(kernelUpdateAndDrawPlayer, "_linesRW", _linesBuffer);
        _asteroidsShader.SetBuffer(kernelUpdateAndDrawPlayer, "_keyStates", _keyStates);
        _asteroidsShader.SetBuffer(kernelUpdateAndDrawPlayer, "_playersRW", _playerState);
        _asteroidsShader.SetFloat("_time", Time.time);
        _asteroidsShader.SetFloat("_timeStep", Time.deltaTime);
        _asteroidsShader.SetInt("_frame", Time.frameCount);
        DispatchItems(kernelUpdateAndDrawPlayer, MAX_PLAYERS);

        _asteroidsShader.SetBuffer(kernelLineDispatchArgs, "_counters", _countersBuffer);
        _asteroidsShader.SetBuffer(kernelLineDispatchArgs, "_dispatch", _dispatchBuffer);
        DispatchOne(kernelLineDispatchArgs);

        _linesBuffer.GetData(testLines);
        _dispatchBuffer.GetData(testDispatch);
        _countersBuffer.GetData(testCounters);

        _drawLinesMaterial.SetBuffer("lines", _linesBuffer);
        _drawLinesMaterial.SetPass(0);
        Graphics.DrawProceduralIndirect(MeshTopology.Lines, _dispatchBuffer);
    }
}

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
        public int nextBullet;
        public int nextAsteroid;
        public int numSoundRequests;
    }
    public struct KeyState {
        public int down;
        public int pressed;
        public int released;
    }
    public struct PlayerState {
        public Vector2 position;
        public float rotation;
        public Vector2 velocity;
        public int alive;
    }
    public struct AsteroidState {
        public Vector2 position;
        public float rotation;
        public Vector2 velocity;
        public int alive;
        public float radius;
        public int level;
    }
    public struct BulletState {
        public Vector2 position;
        public Vector2 velocity;
        public float lifetime;
    }
    public struct SoundRequest {
        public int id;
    }

    public const int LINE_BUFFER_SIZE = 10000;
    public const int MAX_PLAYERS = 1;
    public const int MAX_ASTEROIDS = 100;
    public const int MAX_BULLETS = 100;
    public const int START_ASTEROIDS = 4;
    public const int MAX_SOUND_REQUESTS = 20;

    public const int SOUND_FIRE = 0;
    public const int SOUND_EXPLODE = 1;
    public const int SOUND_BLOP = 2;
    public const int NUM_SOUNDS = 3;

    AudioSource[] _audioSources;

    ComputeShader _asteroidsShader;
    Shader _drawLinesShader;
    Material _drawLinesMaterial;

    ComputeBuffer _linesBuffer;
    ComputeBuffer _countersBuffer;
    ComputeBuffer _dispatchBuffer;
    ComputeBuffer _keyStates;
    ComputeBuffer _playerState;
    ComputeBuffer _asteroidState;
    ComputeBuffer _bulletState;
    ComputeBuffer _soundRequestBuffer;

    KeyState[] _cpuKeyStates;
    SoundRequest[] _cpuSoundRequests;
    Counters[] _cpuCounters;

    const int KEY_ESCAPE = 0;

    int kernelBeginFrame;
    int kernelGenerateTestLines;
    int kernelLineDispatchArgs;
    int kernelUpdateAndDrawPlayer;
    int kernelUpdateAndDrawAsteroid;
    int kernelUpdateAndDrawBullet;
    int kernelCollidePlayerAsteroid;
    int kernelCollideBulletAsteroid;

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
        _asteroidState = ComputeBufferUtils.Alloc<AsteroidState>(MAX_ASTEROIDS);
        _bulletState = ComputeBufferUtils.Alloc<BulletState>(MAX_BULLETS);
        _soundRequestBuffer = ComputeBufferUtils.Alloc<SoundRequest>(MAX_SOUND_REQUESTS);

        kernelBeginFrame = _asteroidsShader.FindKernel("BeginFrame");
        kernelGenerateTestLines = _asteroidsShader.FindKernel("GenerateTestLines");
        kernelLineDispatchArgs = _asteroidsShader.FindKernel("LineDispatchArgs");
        kernelUpdateAndDrawPlayer = _asteroidsShader.FindKernel("UpdateAndDrawPlayer");
        kernelUpdateAndDrawAsteroid = _asteroidsShader.FindKernel("UpdateAndDrawAsteroid");
        kernelUpdateAndDrawBullet = _asteroidsShader.FindKernel("UpdateAndDrawBullet");
        kernelCollidePlayerAsteroid = _asteroidsShader.FindKernel("CollidePlayerAsteroid");
        kernelCollideBulletAsteroid = _asteroidsShader.FindKernel("CollideBulletAsteroid");

        isInitialized = true;

        _cpuKeyStates = new KeyState[256];
        _cpuSoundRequests = new SoundRequest[MAX_SOUND_REQUESTS];

        PlayerState[] initPlayer = new PlayerState[MAX_PLAYERS];
        initPlayer[0].position = new Vector2(1024f, 768f) * 0.5f;
        initPlayer[0].alive = 1;
        _playerState.SetData(initPlayer);

        AsteroidState[] initAsteroids = new AsteroidState[MAX_ASTEROIDS];
        for(int i = 0; i < START_ASTEROIDS; i++) {
            while(true) {
                initAsteroids[i].position = new Vector2(Random.Range(0f, 1024f), Random.Range(0f, 768f));
                if ((initAsteroids[i].position - initPlayer[0].position).magnitude > 200f)
                    break;
            }
            initAsteroids[i].alive = 1;
            initAsteroids[i].radius = 30;
            initAsteroids[i].rotation = Random.Range(-Mathf.PI, Mathf.PI);
            initAsteroids[i].velocity = Random.insideUnitCircle * 50f;
            initAsteroids[i].level = 0;
        }
        _asteroidState.SetData(initAsteroids);

        Counters[] initCounters = new Counters[1];
        initCounters[0].nextAsteroid = START_ASTEROIDS;
        _countersBuffer.SetData(initCounters);

        _audioSources = new AudioSource[NUM_SOUNDS];
        _audioSources[SOUND_FIRE] = gameObject.AddComponent<AudioSource>();
        _audioSources[SOUND_FIRE].clip = Resources.Load<AudioClip>("fire");
        _audioSources[SOUND_EXPLODE] = gameObject.AddComponent<AudioSource>();
        _audioSources[SOUND_EXPLODE].clip = Resources.Load<AudioClip>("explode");
        _audioSources[SOUND_BLOP] = gameObject.AddComponent<AudioSource>();
        _audioSources[SOUND_BLOP].clip = Resources.Load<AudioClip>("blop");
    }

    void DispatchOne(int kernel) {
        BindEverything(kernel);
        _asteroidsShader.Dispatch(kernel, 1, 1, 1);
    }
    void DispatchItems(int kernel, int items) {
        uint x,y,z;
        BindEverything(kernel);
        _asteroidsShader.GetKernelThreadGroupSizes(kernel, out x, out y, out z);
        _asteroidsShader.SetInt("_threadCount", items);
        _asteroidsShader.Dispatch(kernel, (items + (int)x - 1) / (int)x, 1, 1);
    }

    void BindEverything(int kernel) {
        _asteroidsShader.SetInt("_maxBullets", MAX_BULLETS);
        _asteroidsShader.SetInt("_maxPlayers", MAX_PLAYERS);
        _asteroidsShader.SetInt("_maxAsteroids", MAX_ASTEROIDS);
        _asteroidsShader.SetInt("_maxSoundRequests", MAX_SOUND_REQUESTS);
        _asteroidsShader.SetFloat("_time", Time.time);
        _asteroidsShader.SetFloat("_timeStep", Time.deltaTime);
        _asteroidsShader.SetInt("_frame", Time.frameCount);

        _asteroidsShader.SetBuffer(kernel, "_dispatch", _dispatchBuffer);
        _asteroidsShader.SetBuffer(kernel, "_counters", _countersBuffer);
        _asteroidsShader.SetBuffer(kernel, "_linesRW", _linesBuffer);
        _asteroidsShader.SetBuffer(kernel, "_keyStates", _keyStates);
        _asteroidsShader.SetBuffer(kernel, "_playersRW", _playerState);
        _asteroidsShader.SetBuffer(kernel, "_bulletsRW", _bulletState);
        _asteroidsShader.SetBuffer(kernel, "_asteroidsRW", _asteroidState);
        _asteroidsShader.SetBuffer(kernel, "_soundRequestsRW", _soundRequestBuffer);
    }

    private void Update() {
        if (!isInitialized)
            Init();

        //read and store all key states
        for (int i = 0; i < 26; i++) {
            _cpuKeyStates['a' + i] = new KeyState {
                down = Input.GetKey(KeyCode.A + i) ? 1 : 0,
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

        _keyStates.SetData(_cpuKeyStates);

        DispatchOne(kernelBeginFrame);
        DispatchItems(kernelUpdateAndDrawPlayer, MAX_PLAYERS);
        DispatchItems(kernelUpdateAndDrawAsteroid, MAX_ASTEROIDS);
        DispatchItems(kernelUpdateAndDrawBullet, MAX_ASTEROIDS);
        DispatchItems(kernelCollidePlayerAsteroid, MAX_PLAYERS * MAX_ASTEROIDS);
        DispatchItems(kernelCollideBulletAsteroid, MAX_BULLETS * MAX_ASTEROIDS);
        DispatchOne(kernelLineDispatchArgs);

        _soundRequestBuffer.GetData(_cpuSoundRequests);
        _countersBuffer.GetData(_cpuCounters);
        for (int i = 0; i < _cpuCounters[0].numSoundRequests; i++)
            _audioSources[_cpuSoundRequests[i].id].Play();

    }

    public void OnPostRender() {

        _drawLinesMaterial.SetBuffer("lines", _linesBuffer);
        _drawLinesMaterial.SetPass(0);
        Graphics.DrawProceduralIndirect(MeshTopology.Lines, _dispatchBuffer);
    }
}

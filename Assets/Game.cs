using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public struct Character {
        public Vector2 pos;
        public Vector2 scl;
        public int id;
    }
    public struct Globals {
        public int numLines;
        public int numCharacters;

        public int nextBullet;
        public int nextAsteroid;
        public int numSoundRequests;
        public int gameMode;

        public int requestClearAsteroids;
        public int requestSpawnAsteroids;
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
        public int waitingToSpawn;
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
    public const int CHARACTER_BUFFER_SIZE = 10000;
    public const int MAX_PLAYERS = 1;
    public const int MAX_ASTEROIDS = 100;
    public const int MAX_BULLETS = 100;
    public const int START_ASTEROIDS = 4;
    public const int MAX_SOUND_REQUESTS = 20;
    public const int LINES_PER_CHARACTER = 16;

    public const int KID_SPAWN_ASTEROIDS = 0;
    public const int KID_CLEAR_ASTEROIDS = 1;

    //AudioSource[] _audioSources;

    ComputeShader _asteroidsShader;
    Shader _drawLinesShader;
    Material _drawLinesMaterial;

    ComputeBuffer _linesBuffer;
    ComputeBuffer _charactersBuffer;
    ComputeBuffer _globalsBuffer;
    ComputeBuffer _dispatchBuffer;
    ComputeBuffer _keyStates;
    ComputeBuffer _playerState;
    ComputeBuffer _asteroidState;
    ComputeBuffer _bulletState;
    ComputeBuffer _soundRequestBuffer;
    ComputeBuffer _fontBuffer;

    KeyState[] _cpuKeyStates;
    SoundRequest[] _cpuSoundRequests;
    Globals[] _cpuGlobals;

    const int KEY_ESCAPE = 0;

    int kernelBeginFrame;
    int kernelGenerateTestLines;
    int kernelDrawDispatchArgs;
    int kernelUpdateAndDrawPlayer;
    int kernelUpdateAndDrawAsteroid;
    int kernelUpdateAndDrawBullet;
    int kernelCollidePlayerAsteroid;
    int kernelCollideBulletAsteroid;
    int kernelBuildFont;
    int kernelDrawFont;
    int kernelUpdateGame;
    int kernelSetupDispatch;
    int kernelSpawnAsteroids;
    int kernelClearAsteroids;

    bool isInitialized = false;

    public void Init() {
        _asteroidsShader = Resources.Load<ComputeShader>("asteroids");
        _drawLinesShader = Shader.Find("DrawLines");
        _drawLinesMaterial = new Material(_drawLinesShader);
        _drawLinesMaterial.hideFlags = HideFlags.HideAndDontSave;

        _linesBuffer = ComputeBufferUtils.Alloc<Line>(LINE_BUFFER_SIZE);
        _charactersBuffer = ComputeBufferUtils.Alloc<Character>(CHARACTER_BUFFER_SIZE);
        _globalsBuffer = ComputeBufferUtils.Alloc<Globals>(1);
        _dispatchBuffer = ComputeBufferUtils.Alloc<uint>(1024, ComputeBufferType.IndirectArguments);
        _keyStates = ComputeBufferUtils.Alloc<KeyState>(256);
        _playerState = ComputeBufferUtils.Alloc<PlayerState>(MAX_PLAYERS);
        _asteroidState = ComputeBufferUtils.Alloc<AsteroidState>(MAX_ASTEROIDS);
        _bulletState = ComputeBufferUtils.Alloc<BulletState>(MAX_BULLETS);
        _soundRequestBuffer = ComputeBufferUtils.Alloc<SoundRequest>(MAX_SOUND_REQUESTS);
        _fontBuffer = ComputeBufferUtils.Alloc<Line>(LINES_PER_CHARACTER * 256);

        kernelBeginFrame = _asteroidsShader.FindKernel("BeginFrame");
        kernelGenerateTestLines = _asteroidsShader.FindKernel("GenerateTestLines");
        kernelDrawDispatchArgs = _asteroidsShader.FindKernel("DrawDispatchArgs");
        kernelUpdateAndDrawPlayer = _asteroidsShader.FindKernel("UpdateAndDrawPlayer");
        kernelUpdateAndDrawAsteroid = _asteroidsShader.FindKernel("UpdateAndDrawAsteroid");
        kernelUpdateAndDrawBullet = _asteroidsShader.FindKernel("UpdateAndDrawBullet");
        kernelCollidePlayerAsteroid = _asteroidsShader.FindKernel("CollidePlayerAsteroid");
        kernelCollideBulletAsteroid = _asteroidsShader.FindKernel("CollideBulletAsteroid");
        kernelBuildFont = _asteroidsShader.FindKernel("BuildFont");
        kernelDrawFont = _asteroidsShader.FindKernel("DrawFont");
        kernelUpdateGame = _asteroidsShader.FindKernel("UpdateGame");
        kernelSetupDispatch = _asteroidsShader.FindKernel("SetupDispatch");
        kernelSpawnAsteroids = _asteroidsShader.FindKernel("SpawnAsteroids");
        kernelClearAsteroids = _asteroidsShader.FindKernel("ClearAsteroids");

        isInitialized = true;

        _cpuKeyStates = new KeyState[256];
        _cpuSoundRequests = new SoundRequest[MAX_SOUND_REQUESTS];
        _cpuGlobals = new Globals[1];

        _cpuGlobals[0].nextAsteroid = START_ASTEROIDS;
        _globalsBuffer.SetData(_cpuGlobals);

        LoadClips("fire", "explode", "blop");
    }

    //array of clips + list of allocated audio sources
    AudioClip[] _clips;
    List<AudioSource> _audioSources;

    //load list of clips
    void LoadClips(params string[] names) {
        _audioSources = new List<AudioSource>();
        _clips = names.Select(a => Resources.Load<AudioClip>(a)).ToArray();
    }

    //find a free audio source, allocating if necessary, and play clip
    void PlayClip(int idx) {
        AudioSource src = null;
        for(int i = 0; i < _audioSources.Count; i++) {
            if(!_audioSources[i].isPlaying) {
                src = _audioSources[i];
                break;
            }
        }
        if (!src) {
            src = gameObject.AddComponent<AudioSource>();
            _audioSources.Add(src);
        }
        src.clip = _clips[idx];
        src.Play();
       
    }

    void BuildFont() {
        _asteroidsShader.SetBuffer(kernelBuildFont, "_fontRW", _fontBuffer);
        DispatchOne(kernelBuildFont);
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
    void DispatchIndirect(int kernel, int dispatchIndex) {
        _asteroidsShader.SetInt("_kernelIdRequested", dispatchIndex);
        DispatchOne(kernelSetupDispatch);

        BindEverything(kernel);
        _asteroidsShader.SetInt("_threadCount", -1);
        _asteroidsShader.DispatchIndirect(kernel, _dispatchBuffer);
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
        _asteroidsShader.SetBuffer(kernel, "_globals", _globalsBuffer);
        _asteroidsShader.SetBuffer(kernel, "_linesRW", _linesBuffer);
        _asteroidsShader.SetBuffer(kernel, "_charactersRW", _charactersBuffer);
        _asteroidsShader.SetBuffer(kernel, "_keyStates", _keyStates);
        _asteroidsShader.SetBuffer(kernel, "_playersRW", _playerState);
        _asteroidsShader.SetBuffer(kernel, "_bulletsRW", _bulletState);
        _asteroidsShader.SetBuffer(kernel, "_asteroidsRW", _asteroidState);
        _asteroidsShader.SetBuffer(kernel, "_soundRequestsRW", _soundRequestBuffer);
        _asteroidsShader.SetBuffer(kernel, "_font", _fontBuffer);
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

        BuildFont();

        DispatchOne(kernelBeginFrame);
        DispatchOne(kernelUpdateGame);
        DispatchIndirect(kernelClearAsteroids, KID_CLEAR_ASTEROIDS);
        DispatchIndirect(kernelSpawnAsteroids, KID_SPAWN_ASTEROIDS);
        DispatchItems(kernelUpdateAndDrawPlayer, MAX_PLAYERS);
        DispatchItems(kernelUpdateAndDrawAsteroid, MAX_ASTEROIDS);
        DispatchItems(kernelUpdateAndDrawBullet, MAX_ASTEROIDS);
        DispatchItems(kernelCollidePlayerAsteroid, MAX_PLAYERS * MAX_ASTEROIDS);
        DispatchItems(kernelCollideBulletAsteroid, MAX_BULLETS * MAX_ASTEROIDS);
        DispatchOne(kernelDrawDispatchArgs);

        _soundRequestBuffer.GetData(_cpuSoundRequests);
        _globalsBuffer.GetData(_cpuGlobals);
        for (int i = 0; i < _cpuGlobals[0].numSoundRequests; i++)
            PlayClip(_cpuSoundRequests[i].id);

    }

    public void OnPostRender() {
        uint[] dpargs = new uint[8];
        _dispatchBuffer.GetData(dpargs);


        _drawLinesMaterial.SetBuffer("lines", _linesBuffer);
        _drawLinesMaterial.SetPass(0);
        Graphics.DrawProceduralIndirect(MeshTopology.Lines, _dispatchBuffer, 0);

        _drawLinesMaterial.SetBuffer("lines", _fontBuffer);
        _drawLinesMaterial.SetBuffer("characters", _charactersBuffer);
        _drawLinesMaterial.SetPass(1);
        Graphics.DrawProceduralIndirect(MeshTopology.Lines, _dispatchBuffer, 16);
    }
}

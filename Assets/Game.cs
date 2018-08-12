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

        public int liveAsteroids;
        public int level;

        public float totalLeveltime;
        public float lastBlopTime;
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
        public int wantsToSpawn;
        public int canSpawn;
        public int lives;
        public int score;
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
    int kernelDrawDispatchArgs;
    int kernelUpdateAndDrawPlayer;
    int kernelUpdateAndDrawAsteroid;
    int kernelUpdateAndDrawBullet;
    int kernelCollidePlayerAsteroid;
    int kernelCollideBulletAsteroid;
    int kernelBuildFont;
    int kernelUpdateGame;
    int kernelSetupDispatch;
    int kernelSpawnAsteroids;
    int kernelClearAsteroids;
    int kernelPreparePlayerSpawning;
    int kernelUpdatePlayerSpawning;

    bool isInitialized = false;

    //sets everything up
    public void Init() {
        //load shaders + material
        _asteroidsShader = Resources.Load<ComputeShader>("asteroids");
        _drawLinesShader = Shader.Find("DrawLines");
        _drawLinesMaterial = new Material(_drawLinesShader);
        _drawLinesMaterial.hideFlags = HideFlags.HideAndDontSave;

        //allocate loads of buffers
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

        //find all kernels
        kernelBeginFrame = _asteroidsShader.FindKernel("BeginFrame");
        kernelDrawDispatchArgs = _asteroidsShader.FindKernel("DrawDispatchArgs");
        kernelUpdateAndDrawPlayer = _asteroidsShader.FindKernel("UpdateAndDrawPlayer");
        kernelUpdateAndDrawAsteroid = _asteroidsShader.FindKernel("UpdateAndDrawAsteroid");
        kernelUpdateAndDrawBullet = _asteroidsShader.FindKernel("UpdateAndDrawBullet");
        kernelCollidePlayerAsteroid = _asteroidsShader.FindKernel("CollidePlayerAsteroid");
        kernelCollideBulletAsteroid = _asteroidsShader.FindKernel("CollideBulletAsteroid");
        kernelBuildFont = _asteroidsShader.FindKernel("BuildFont");
        kernelUpdateGame = _asteroidsShader.FindKernel("UpdateGame");
        kernelSetupDispatch = _asteroidsShader.FindKernel("SetupDispatch");
        kernelSpawnAsteroids = _asteroidsShader.FindKernel("SpawnAsteroids");
        kernelClearAsteroids = _asteroidsShader.FindKernel("ClearAsteroids");
        kernelPreparePlayerSpawning = _asteroidsShader.FindKernel("PreparePlayerSpawning");
        kernelUpdatePlayerSpawning = _asteroidsShader.FindKernel("UpdatePlayerSpawning");

        //setup cpu side arrays
        _cpuKeyStates = new KeyState[256];
        _cpuSoundRequests = new SoundRequest[MAX_SOUND_REQUESTS];
        _cpuGlobals = new Globals[1];

        //clear globals buffer (probably unnecessary)
        _globalsBuffer.SetData(_cpuGlobals);

        //load audio clips
        LoadClips("fire", "explode", "blop");

        isInitialized = true;
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

    //fires off the 'build font' shader that fills in writable version of fonts buffer
    void BuildFont() {
        _asteroidsShader.SetBuffer(kernelBuildFont, "_fontRW", _fontBuffer);
        DispatchOne(kernelBuildFont);
    }

    //bind all + dispatch 1 thread group of a kernel
    void DispatchOne(int kernel) {
        BindEverything(kernel);
        _asteroidsShader.Dispatch(kernel, 1, 1, 1);
    }

    //bind all then work out / dispatch thread groups required to fire off a certain 
    //number of threads. Also sets the '_threadCount' uniform, so compute side can know
    //exactly how many threads were wanted
    void DispatchThreads(int kernel, int thread_count) {
        uint x,y,z;
        BindEverything(kernel);
        _asteroidsShader.GetKernelThreadGroupSizes(kernel, out x, out y, out z);
        _asteroidsShader.SetInt("_threadCount", thread_count);
        _asteroidsShader.Dispatch(kernel, (thread_count + (int)x - 1) / (int)x, 1, 1);
    }

    //dispatches the 'setup dispatch' shader to configer the dispatch args for a given
    //dispatch type, then does an actual dispatch indirect on the requested kernel
    void DispatchIndirect(int kernel, int dispatchIndex) {
        _asteroidsShader.SetInt("_kernelIdRequested", dispatchIndex);
        DispatchOne(kernelSetupDispatch);

        BindEverything(kernel);
        _asteroidsShader.SetInt("_threadCount", -1);
        _asteroidsShader.DispatchIndirect(kernel, _dispatchBuffer);
    }

    //binds all the uniforms and buffers we could possibly need
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

    //dumbly reads a load of key states into cpu side buffer, then writes them into compute buffer
    void ReadInputs() {
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
    }

    //reads audio playback requests and plays them
    void OutputSounds() {
        _soundRequestBuffer.GetData(_cpuSoundRequests);
        _globalsBuffer.GetData(_cpuGlobals);
        for (int i = 0; i < _cpuGlobals[0].numSoundRequests; i++)
            PlayClip(_cpuSoundRequests[i].id);
    }

    //reads inputs, does a load of dispatches, generates draw args and fires off sounds
    private void Update() {
        if (!isInitialized)
            Init();

        ReadInputs();

        BuildFont();

        PlayerState[] players = new PlayerState[1];

        DispatchOne(kernelBeginFrame);
        DispatchOne(kernelUpdateGame);
        _playerState.GetData(players);
        DispatchIndirect(kernelClearAsteroids, KID_CLEAR_ASTEROIDS);
        DispatchIndirect(kernelSpawnAsteroids, KID_SPAWN_ASTEROIDS);
        _playerState.GetData(players);
        DispatchThreads(kernelPreparePlayerSpawning, MAX_PLAYERS);
        _playerState.GetData(players);
        DispatchThreads(kernelUpdatePlayerSpawning, MAX_PLAYERS * MAX_ASTEROIDS);
        _playerState.GetData(players);
        DispatchThreads(kernelUpdateAndDrawPlayer, MAX_PLAYERS);
        DispatchThreads(kernelUpdateAndDrawAsteroid, MAX_ASTEROIDS);
        DispatchThreads(kernelUpdateAndDrawBullet, MAX_ASTEROIDS);
        DispatchThreads(kernelCollidePlayerAsteroid, MAX_PLAYERS * MAX_ASTEROIDS);
        DispatchThreads(kernelCollideBulletAsteroid, MAX_BULLETS * MAX_ASTEROIDS);
        DispatchOne(kernelDrawDispatchArgs);

        OutputSounds();
    }

    //uses draw args built during update to fire DrawProceduralIndirect calls that draw lines / text
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

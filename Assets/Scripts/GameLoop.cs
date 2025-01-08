using UnityEngine;

public class GameLoop : MonoBehaviour
{
    
    public GameState gameState;
    public GameObject waveCratePrefab;
    public GameObject boltPickupPrefab;

    public UIController UI;
    private WaveManager waveManager;
    private CurrentWaveData upcomingWave;
    private GameObject progressBar;

    private float waveProgress = 1f;


    void Start()
    {   
        progressBar = GameObject.FindWithTag("ProgressBar");
        waveManager = new WaveManager();
        StateChange(gameState);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void StateChange(GameState newState){
        gameState = newState;

        switch (gameState){
            case GameState.WaveReset:
                StateChange(GameState.GameIdle);
                SpawnWaveCrate();
                break;
            case GameState.GameIdle:
                ShowProgressBar(false);
                upcomingWave = waveManager.GetNewWaveData();

                break;
            case GameState.GameWaveInProgress:

                //spawning

                break;
            case GameState.GameEnd:
                //player died

                break;
            default:
                Debug.Log("game state invalid");
                break;
        }
    }

    public void WaveCrateBreakEvent(Vector3 cratePosition){
        ShowProgressBar(true);
        SetWaveProgressBar(1f);
        StateChange(GameState.GameWaveInProgress);
        SpawnBoltPickups(cratePosition);
    }



    void SetWaveProgressBar(float p){
        waveProgress = p;
        UI.displayProgress = waveProgress;
    }

    void SpawnBoltPickups(Vector3 spawnPostion){
        int spawnCount = upcomingWave.spawnBolts;

        Quaternion rot30 = Quaternion.Euler(0, 60, 0);
        Quaternion boltRot = Quaternion.identity;

        for (int i = 0; i < spawnCount; i++){
            Instantiate(boltPickupPrefab, spawnPostion, boltRot);
            boltRot = boltRot * rot30;
        }
    }
    void SpawnWaveCrate(){
        Instantiate(waveCratePrefab, transform.position, transform.rotation);
    }

    void ShowProgressBar(bool enabled){
        progressBar.SetActive(enabled);
    }
}

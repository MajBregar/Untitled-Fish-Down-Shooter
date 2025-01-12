using UnityEngine;

public class GameLoop : MonoBehaviour
{
    public float spawnDistance = 40f;
    public GameState gameState;
    public GameObject waveCratePrefab;
    public GameObject boltPickupPrefab;
    public UIController UI;
    public GameObject standardEnemyPrefab;
    public GameObject fastEnemyPrefab;
    public GameObject tankEnemyPrefab;

    private WaveManager waveManager;
    private CurrentWaveData upcomingWave;
    private GameObject progressBar;
    private float waveProgress = 1f;

    private GameObject playerObject;


    void Start()
    {   
        playerObject = GameObject.FindGameObjectWithTag("Player");

        progressBar = GameObject.FindWithTag("ProgressBar");
        waveManager = new WaveManager();
        StateChange(gameState);

    }

    // Update is called once per frame
    void Update() {
        if (gameState == GameState.GameWaveInProgress && upcomingWave.spawningEnemies)
        {
            SpawnEnemies();
        }
    }

    void StateChange(GameState newState){
        gameState = newState;

        switch (gameState){
            case GameState.WaveReset:
                StateChange(GameState.GameIdle);
                SpawnWaveCrate();
                UI.ShowGameTip();
                break;
            case GameState.GameIdle:
                ShowProgressBar(false);
                upcomingWave = waveManager.GetNewWaveData();

                break;
            case GameState.GameWaveInProgress:
                upcomingWave.spawningEnemies = true;

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

        if (UI.showingGameTip == true){
            UI.HideGameTip();
        }
    }

    void SpawnEnemies() 
    {

        System.Random random = new System.Random();

        upcomingWave.enemiesToSpawn += Time.deltaTime * 4;
        for (int i = 0; i < upcomingWave.enemiesToSpawn - 1; i++)
        {

            int enemyToSpawn = -1;
            if (upcomingWave.tankEnemyCount > 0)
            {
                enemyToSpawn = 2;

            } else if (upcomingWave.standardEnemyCount > 0)
            {
                enemyToSpawn = 0;
            } else if (upcomingWave.fastEnemyCount > 0)
            {
                enemyToSpawn = 1;
            } else
            {
                enemyToSpawn = random.Next(3);
            }

            float angle = (float) random.NextDouble() * 2 * Mathf.PI;

            Vector3 spawnLocation = new Vector3(
                (float) Mathf.Cos(angle) * spawnDistance,
                0,
                (float) Mathf.Sin(angle) * spawnDistance
            ) + playerObject.transform.position;

            switch (enemyToSpawn)
            {
                case 0:
                    Instantiate(standardEnemyPrefab, spawnLocation, transform.rotation);
                    upcomingWave.standardEnemyCount--;
                    break;

                case 1:
                    Instantiate(fastEnemyPrefab, spawnLocation, transform.rotation);
                    upcomingWave.fastEnemyCount--;
                    break;

                case 2:
                    Instantiate(tankEnemyPrefab, spawnLocation, transform.rotation);
                    upcomingWave.tankEnemyCount--;
                    break;
            }

            
        }
        upcomingWave.enemiesToSpawn %= 1;

        if (upcomingWave.standardEnemyCount <= 0 && upcomingWave.fastEnemyCount <= 0 && upcomingWave.tankEnemyCount <= 0)
        {
            upcomingWave.spawningEnemies = false;
        }

    }

    public void EnemyDeathEvent(){
        upcomingWave.enemiesKilled += 1;
        float p = ((float) upcomingWave.enemiesKilled) / ((float) upcomingWave.totalEnemyCount);
        SetWaveProgressBar(1f - p);

        if (upcomingWave.enemiesKilled == upcomingWave.totalEnemyCount){
            StateChange(GameState.WaveReset);
        }
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

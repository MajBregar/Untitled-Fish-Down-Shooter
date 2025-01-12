using UnityEngine;


public class CurrentWaveData {
    public int totalEnemyCount;
    public int standardEnemyCount;
    public int fastEnemyCount;
    public int tankEnemyCount;
    public int enemiesKilled;
    public int spawnBolts;
    public double enemiesToSpawn;
    public bool spawningEnemies = false;
}


public class WaveManager {
    public int currentWaveID = 0;
    public CurrentWaveData GetNewWaveData(){
        currentWaveID += 1;
        CurrentWaveData newWave = new CurrentWaveData();

        if (currentWaveID - 1 < waveMobCounts.Length){
            newWave.standardEnemyCount = waveMobCounts[currentWaveID - 1, 0];
            newWave.fastEnemyCount = waveMobCounts[currentWaveID - 1, 1];
            newWave.tankEnemyCount = waveMobCounts[currentWaveID - 1, 2];
            newWave.totalEnemyCount = newWave.standardEnemyCount + newWave.fastEnemyCount + newWave.tankEnemyCount;
            newWave.enemiesKilled = 0;
            newWave.spawnBolts = waveMobCounts[currentWaveID - 1, 3];
        } else {
            int lastWaveID = waveMobCounts.Length - 1;
            int multiplier = (1 << (currentWaveID - lastWaveID));
            newWave.standardEnemyCount = waveMobCounts[lastWaveID, 0] * multiplier;
            newWave.fastEnemyCount = waveMobCounts[lastWaveID, 1] * multiplier;
            newWave.tankEnemyCount = waveMobCounts[lastWaveID, 2] * multiplier;
            newWave.totalEnemyCount = newWave.standardEnemyCount + newWave.fastEnemyCount + newWave.tankEnemyCount;
            newWave.enemiesKilled = 0;
            newWave.spawnBolts = 0;
        }
        newWave.enemiesToSpawn = 1.0;
        return newWave;
    }

    public int[,] waveMobCounts = {
        {1,0,0, 0},
        {2,0,0, 1},
        {5,0,0, 0},
        {1,2,0, 0},
        {0,6,0, 2},
        {5,0,1, 0},
        {8,2,2, 0},
        {10,4,3, 0},
        {15,5,3, 2},
        {25,0,6, 0},
        {20,10,5, 0},
    };

}

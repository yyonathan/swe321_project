using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera _cam;

    [Header("Chunk Prefabs")]
    [SerializeField] private GameObject[] _startingChunks;
    [SerializeField] private GameObject[] _easyChunks;
    [SerializeField] private GameObject[] _mediumChunks;
    [SerializeField] private GameObject[] _hardChunks;

    [Header("Spawn Settings")]
    [SerializeField] private float _initialSpawnY = 0f;
    [SerializeField] private float _spawnBuffer = 15f;

    [Header("Difficulty Phases")]
    [SerializeField] private DifficultyPhase[] _phases;

    private float _gameTimer;
    private float _nextSpawnY;
    private List<Chunk> _activeChunks = new();
    private GameObject _lastSpawnedPrefab;

    // max attempts to find a non-repeating chunk before giving up and allowing the repeat
    private const int MaxRerollAttempts = 10;

    private void Start()
    {
        if (_cam == null) _cam = Camera.main;
        _nextSpawnY = _initialSpawnY;

        float camTop = _cam.transform.position.y + _cam.orthographicSize;
        while (_nextSpawnY < camTop + _spawnBuffer)
            SpawnNextChunk();
    }

    private void Update()
    {
        _gameTimer += Time.deltaTime;

        float camTop = _cam.transform.position.y + _cam.orthographicSize;
        while (_nextSpawnY < camTop + _spawnBuffer)
            SpawnNextChunk();

        float camBottom = _cam.transform.position.y - _cam.orthographicSize;
        for (int i = _activeChunks.Count - 1; i >= 0; i--)
        {
            if (_activeChunks[i] == null)
            {
                _activeChunks.RemoveAt(i);
                continue;
            }

            if (_activeChunks[i].UpperTransitionBorder < camBottom)
            {
                Destroy(_activeChunks[i].gameObject);
                _activeChunks.RemoveAt(i);
            }
        }
    }

    private void SpawnNextChunk()
    {
        GameObject prefab = SelectChunkPrefab();
        if (prefab == null)
        {
            Debug.LogError("ChunkManager: no valid chunk prefab found.");
            return;
        }

        // instantiate at 0,0 so offsets are clean
        GameObject instance = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        Chunk chunk = instance.GetComponent<Chunk>();

        if (chunk == null)
        {
            Debug.LogError("ChunkManager: chunk prefab is missing Chunk component.");
            Destroy(instance);
            return;
        }

        // move chunk so its lower border sits exactly at _nextSpawnY
        instance.transform.position = new Vector3(0f, _nextSpawnY - chunk.LowerTransitionBorder, 0f);
        _nextSpawnY = chunk.UpperTransitionBorder;

        _lastSpawnedPrefab = prefab;
        _activeChunks.Add(chunk);
    }

    private GameObject SelectChunkPrefab()
    {
        if (_phases == null || _phases.Length == 0)
            return GetRandom(_startingChunks);

        DifficultyPhase phase = GetCurrentPhase();
        int total = phase.StartingWeight + phase.EasyWeight + phase.MediumWeight + phase.HardWeight;
        if (total == 0) return GetRandom(_startingChunks);

        // attempt to find a valid prefab, rerolling if we land on a non-repeatable chunk that was just spawned
        for (int attempt = 0; attempt < MaxRerollAttempts; attempt++)
        {
            GameObject candidate = RollPrefab(phase, total);
            if (candidate == null) continue;

            // check if this prefab is non-repeatable and was the last one spawned
            Chunk candidateChunk = candidate.GetComponent<Chunk>();
            bool isRepeatViolation = candidateChunk != null && !candidateChunk.CanRepeat && candidate == _lastSpawnedPrefab;

            if (!isRepeatViolation)
                return candidate;
        }

        // fallback: max attempts exceeded, just spawn whatever (avoids infinite loop when only one non-repeatable chunk exists in tier)
        return RollPrefab(phase, total);
    }

    private GameObject RollPrefab(DifficultyPhase phase, int total)
    {
        int roll = Random.Range(0, total);
        int cumulative = 0;

        cumulative += phase.StartingWeight;
        if (roll < cumulative) return GetRandom(_startingChunks);

        cumulative += phase.EasyWeight;
        if (roll < cumulative) return GetRandom(_easyChunks);

        cumulative += phase.MediumWeight;
        if (roll < cumulative) return GetRandom(_mediumChunks);

        return GetRandom(_hardChunks);
    }

    private GameObject GetRandom(GameObject[] arr)
    {
        if (arr != null && arr.Length > 0)
            return arr[Random.Range(0, arr.Length)];

        if (_startingChunks != null && _startingChunks.Length > 0) return _startingChunks[Random.Range(0, _startingChunks.Length)];
        if (_easyChunks != null && _easyChunks.Length > 0) return _easyChunks[Random.Range(0, _easyChunks.Length)];
        if (_mediumChunks != null && _mediumChunks.Length > 0) return _mediumChunks[Random.Range(0, _mediumChunks.Length)];
        if (_hardChunks != null && _hardChunks.Length > 0) return _hardChunks[Random.Range(0, _hardChunks.Length)];

        return null;
    }

    private DifficultyPhase GetCurrentPhase()
    {
        DifficultyPhase current = _phases[0];
        foreach (DifficultyPhase phase in _phases)
        {
            if (_gameTimer >= phase.StartTime)
                current = phase;
        }
        return current;
    }
}

[System.Serializable]
public struct DifficultyPhase
{
    public float StartTime;
    public int StartingWeight;
    public int EasyWeight;
    public int MediumWeight;
    public int HardWeight;
}
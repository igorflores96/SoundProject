using System.Collections.Generic;
using UnityEngine;

public class MapSpawner : MonoBehaviour
{

    [Header("Map Infos")]
    [SerializeField] private int _width;
    [SerializeField] private int _height; 
    [SerializeField] private float _cellSize; 
    [SerializeField] private GameObject[] _floors; 
    [SerializeField] private AudioSource _waveSound;
        
    private Dictionary<Vector2Int, GameObject> _objects = new Dictionary<Vector2Int, GameObject>();


    [SerializeField] private int spectrumIndex = 0;
    [SerializeField] private float scaleMultiplier = 10.0f;

    private float[] spectrum = new float[256];

    private void Update()
    {
        _waveSound.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);

        float intensity = spectrum[spectrumIndex] * scaleMultiplier;

        Vector2Int waveOrigin = new Vector2Int(_objects.Keys.Count, _objects.Keys.Count / 4);

        foreach (KeyValuePair<Vector2Int, GameObject> entry in _objects)
        {
            Vector2Int position = entry.Key;
            GameObject obj = entry.Value;

            float distanceFromCenter = Vector2.Distance(position, waveOrigin);
            float wave = Mathf.Sin(-distanceFromCenter + Time.time * 2.0f) * intensity;

            Vector3 newScale = new Vector3(obj.transform.localScale.x, Mathf.Lerp(obj.transform.localScale.y, wave, Time.deltaTime * 5.0f), obj.transform.localScale.z);

            obj.transform.localScale = newScale;
        }
    }

    private void Awake() 
    {
        _objects.Clear();
        GenerateMap();
    }

    private void GenerateMap()
    {
        for(int xSize = 0; xSize < _width; xSize++)
        {
            for(int zSize = 0; zSize < _height; zSize++)
            {
                Vector3 spawnPosition = GetWorldPosition(xSize, zSize);
                spawnPosition = new Vector3(spawnPosition.x, 0.5f, spawnPosition.z);
                GameObject objectPrefab = Instantiate(_floors[0].gameObject, spawnPosition, Quaternion.identity);
                objectPrefab.transform.SetParent(this.transform);
                objectPrefab.name = "Hex: " + xSize + "." + zSize;

                _objects.Add(new Vector2Int(xSize, zSize), objectPrefab);
            }
        }
    }


    private Vector3 GetWorldPosition(int x, int z)
    {
        return new Vector3(x, 0, z) * _cellSize + Vector3.zero;
    }
}

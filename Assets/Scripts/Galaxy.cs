using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class Galaxy : MonoBehaviour
{
#if UNITY_EDITOR
    public Transform gridObject_EDITOR;
#endif

    public static Galaxy instance;

    public GameObject starPrefab;
    Star[] stars;
    DynamicGrid<Star> starCells;

    [HideInInspector] public List<Constellation> constellations;

    [Space(10)]
    public int numStars;
    public float galaxySize;

    [Space(10)]
    public float minStarDistance;
    public int maxPositionIterations = 100;
    public float starCellSize;

    [Space(10)]
    [Range(0f, 1f)] public float armSpawnWeight;
    [Range(0f, 6f)] public float centerClusterSize;
    public float centerClusterArea;
    public float spiralAreaRange;
    public float centerClusterPower;
    public float armLength = 1f;
    public int numArms;
    [Range(-1f, 1f)] public float armOffset;

    [Space(20)]
    public RectTransform loadingBar;
    public float loadingBarWidth = 3040f;
    float targetLoadingBarWidth;
    public float loadingBarLerpSpeed = 5f;
    public TextMeshProUGUI loadingBarPercent;
    public TextMeshProUGUI anomalousStars;
    public TextMeshProUGUI loadingBarTextBottom;
    public TextMeshProUGUI loadingBarTextTop;

    [Space(20)]
    public TextAsset starNames;
    public TextAsset starNamePrefixes;
    [Range(0f, 1f)] public float starNumberPrefixChance;
    [Range(0f, 1f)] public float nameHyphonationChance;

    [Space(20)]
    public float maxHyperlaneDistance;
    public Color[] hyperlaneColors;

    [Space(20)]
    public SpectralClass[] spectralClasses;
    int spectralClassesWeightSum;

    public float GalaxySize { get; set; }
    public float NumStars { get; set; }
    public float NumArms { get; set; }
    public float ArmLength { get; set; }
    public float ArmOffset { get; set; }

    class DynamicGrid<T>
    {
        public readonly Dictionary<Vector2Int, List<T>> list;

        public List<T> this[Vector2Int cell]
        {
            get
            {
                if (!list.ContainsKey(cell))
                {
                    list.Add(cell, new List<T>());
                }

                return list[cell];
            }
        }

        public Dictionary<Vector2Int, List<T>>.ValueCollection Values { get => list.Values; }

        public DynamicGrid()
        {
            list = new Dictionary<Vector2Int, List<T>>();
        }
    }

    public class Constellation : List<Star>
    {
        public Color color;
        public string name;
    }

    public void ResetGalaxy()
    {
        StopAllCoroutines();

        foreach (var star in stars)
        {
            if (!star)
            {
                continue;
            }

            Destroy(star.gameObject);
        }

        stars = null;

        if (starCells != null)
        {
            starCells.list.Clear();
        }
        if (constellations != null)
        {
            constellations.Clear();
        }

        galaxySize = GalaxySize;
        numStars = Mathf.RoundToInt(NumStars);
        numArms = Mathf.RoundToInt(NumArms);
        armLength = ArmLength;
        armOffset = ArmOffset;

        CameraController.instance.SetBounds();

        StartCoroutine(GenerateGalaxy());
    }

    void Awake()
    {
        instance = this;
        
        // Setup spectral class weights
        spectralClassesWeightSum = 0;
        for (int i = 0; i < spectralClasses.Length; i++)
        {
            spectralClassesWeightSum += spectralClasses[i].spawnWeight;
        }

        for (int i = 0; i < spectralClasses.Length; i++)
        {
            if (i < spectralClasses.Length - 1 && spectralClasses[i].spawnWeight > spectralClasses[i + 1].spawnWeight)
            {
                SpectralClass obj = spectralClasses[i];
                spectralClasses[i] = spectralClasses[i + 1];
                spectralClasses[i + 1] = obj;
            }
        }

        GalaxySize = galaxySize;
        NumStars = numStars;
        NumArms = numArms;
        ArmLength = armLength;
        ArmOffset = armOffset;
    }

    void Start()
    {
        StartCoroutine(GenerateGalaxy());
    }

    void Update()
    {
        loadingBar.sizeDelta = new Vector2(Mathf.Lerp(loadingBar.rect.width, targetLoadingBarWidth, loadingBarLerpSpeed * Time.deltaTime), loadingBar.rect.height);
    }

    IEnumerator GenerateGalaxy()
    {
        loadingBarTextBottom.text = "Mapping Stars";
        loadingBarTextTop.text = "Anomalous Stars";

        stars = new Star[numStars];
        starCells = new DynamicGrid<Star>();
        int invalidStars = 0;

        for (int i = 0; i < numStars; i++)
        {
            Star star = Instantiate(starPrefab, transform).GetComponent<Star>();
            Vector2Int cell = Vector2Int.zero;

            if (Random.value <= armSpawnWeight)
            {
                if (!SpawnStar(() => SetStarInSpiral(star, Random.Range(0, numArms))))
                {
                    invalidStars++;
                }
            }
            else
            {
                if (!SpawnStar(() => SetStarInCircle(star)))
                {
                    invalidStars++;
                }
            }

            bool SpawnStar(System.Action SetStar)
            {
                int j;
                for (j = 0; j < maxPositionIterations; j++)
                {
                    SetStar();
                    cell = new Vector2Int(Mathf.FloorToInt(star.transform.position.x / starCellSize), Mathf.FloorToInt(star.transform.position.y / starCellSize));

                    int starCount = starCells[cell].Count;
                    if (starCount == 0)
                    {
                        return true;
                    }

                    bool valid = true;
                    for (int k = 0; k < starCount; k++)
                    {
                        if ((star.transform.position - stars[k].transform.position).sqrMagnitude < minStarDistance * minStarDistance)
                        {
                            valid = false;
                            break;
                        }
                    }

                    if (valid)
                    {
                        return true;
                    }
                }

                return j == maxPositionIterations - 1;
            }

            SetStarClass(star);

            stars[i] = star;
            starCells[cell].Add(star);

            if (i % 10 == 0)
            {
                yield return null;
            }

            // Set loading bar
            float percent = (float)i / (numStars - 1);
            targetLoadingBarWidth = loadingBarWidth * percent;
            loadingBarPercent.text = (percent * 100f).ToString("0.0") + "%";

            anomalousStars.text = invalidStars.ToString();
        }

        StartCoroutine(DrawHyperlanes());
    }

    void SetStarClass(Star star)
    {
        int spawnValue = Random.Range(0, spectralClassesWeightSum);
        for (int i = 0; i < spectralClasses.Length; i++)
        {
            if (spawnValue < spectralClasses[i].spawnWeight)
            {
                star.spectralClass = spectralClasses[i];

                MaterialPropertyBlock properties = new MaterialPropertyBlock();
                properties.SetColor("_BaseColor", star.spectralClass.color);
                star.renderer.SetPropertyBlock(properties);

                star.mass = Random.Range(star.spectralClass.massRange.x, star.spectralClass.massRange.y);
                star.radius = Random.Range(star.spectralClass.radiusRange.x, star.spectralClass.radiusRange.y);

                star.transform.localScale = new Vector3(star.spectralClass.size, star.spectralClass.size, star.spectralClass.size);

                break;
            }

            spawnValue -= spectralClasses[i].spawnWeight;
        }
    }

    IEnumerator DrawHyperlanes()
    {
        loadingBarTextBottom.text = "Identifying Hyperlanes";
        loadingBarTextTop.text = "Hyperlanes Found";

        int numStars = stars.Length;
        for (int i = 0; i < numStars; i++)
        {
            Star star = stars[i];
            float minDistance = float.MaxValue;
            int bestDistanceIndex = -1;

            for (int j = 0; j < numStars; j++)
            {
                if (j != i)
                {
                    float distance = (star.transform.position - stars[j].transform.position).sqrMagnitude;
                    if (distance < maxHyperlaneDistance * maxHyperlaneDistance && !stars[j].connectedStars.Contains(star))
                    {
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            bestDistanceIndex = j;
                        }
                    }
                }
            }

            if (bestDistanceIndex == -1)
            {
                yield return null;
                continue;
            }

            Star other = stars[bestDistanceIndex];

            star.hyperlaneRenderer.SetPositions(new Vector3[2] { star.transform.position, other.transform.position });
            star.connectedStars.Add(other);
            other.connectedStars.Add(star);

            // Set loading bar
            float percent = (float)i / (numStars - 1);
            targetLoadingBarWidth = loadingBarWidth * percent;
            loadingBarPercent.text = (percent * 100f).ToString("0.0") + "%";
            anomalousStars.text = i.ToString();

            if  (i % 10 == 0)
            {
                yield return null;
            }
        }

        StartCoroutine(FindConstellations());
    }

    IEnumerator FindConstellations()
    {
        loadingBarTextBottom.text = "Mapping Constellations";
        loadingBarTextTop.text = "Constellations Found";

        string[] names = starNames.text.Split('\n');
        string[] prefixes = starNamePrefixes.text.Split('\n');
        int[] prefixNumbers = Enumerable.Range(0, 999).ToArray();

        List<int> nameIndices = Enumerable.Range(0, names.Length).ToList();
        List<int> prefixIndices;
        List<int> prefixNumbersIndices = Enumerable.Range(0, prefixNumbers.Length).ToList();

        constellations = new List<Constellation>();

        int addedStars = 0;
        for (int i = 0; i < stars.Length; i++)
        {
            Star star = stars[i];
            if (star.constellationIndex > -1)
            {
                continue;
            }

            addedStars++;

            int index = constellations.Count;
            constellations.Add(new Constellation()
            {
                star
            });
            prefixIndices = Enumerable.Range(0, prefixes.Length).ToList();
            //constellations[index].color = Color.HSVToRGB(Random.value, 1f, 1f);
            constellations[index].color = hyperlaneColors[Random.Range(0, hyperlaneColors.Length)];

            int nameIndex = Random.Range(0, nameIndices.Count);
            string name = names[nameIndices[nameIndex]].Replace("\r", "");
            nameIndices.RemoveAt(nameIndex);
            if (nameIndices.Count == 0)
            {
                nameIndices = Enumerable.Range(0, names.Length).ToList();
            }
            constellations[index].name = name;

            star.constellationIndex = index;
            SetHyperlaneColor(star, constellations[index].color);

            string prefix;
            if (Random.value <= starNumberPrefixChance)
            {
                nameIndex = Random.Range(0, prefixNumbersIndices.Count);
                prefix = prefixNumbers[prefixNumbersIndices[nameIndex]].ToString();
                prefixNumbersIndices.RemoveAt(nameIndex);
                if (prefixNumbersIndices.Count == 0)
                {
                    prefixNumbersIndices = Enumerable.Range(0, prefixNumbers.Length).ToList();
                }
            }
            else
            {
                nameIndex = Random.Range(0, prefixIndices.Count);
                prefix = prefixes[prefixIndices[nameIndex]];
                prefixIndices.RemoveAt(nameIndex);
                if (prefixIndices.Count == 0)
                {
                    prefixIndices = Enumerable.Range(0, prefixes.Length).ToList();
                }
            }
            star.name = prefix.Replace("\r", "") + (Random.value <= nameHyphonationChance ? "-" : " ") + name;

            yield return AddConnections(star);

            yield return null;

            IEnumerator AddConnections(Star star)
            {
                foreach (Star connection in star.connectedStars)
                {
                    if (connection.constellationIndex != index)
                    {
                        addedStars++;

                        constellations[index].Add(connection);
                        connection.constellationIndex = index;
                        SetHyperlaneColor(connection, constellations[index].color);

                        string prefix;
                        if (Random.value <= starNumberPrefixChance)
                        {
                            nameIndex = Random.Range(0, prefixNumbersIndices.Count);
                            prefix = prefixNumbers[prefixNumbersIndices[nameIndex]].ToString();
                            prefixNumbersIndices.RemoveAt(nameIndex);
                            if (prefixNumbersIndices.Count == 0)
                            {
                                prefixNumbersIndices = Enumerable.Range(0, prefixNumbers.Length).ToList();
                            }
                        }
                        else
                        {
                            nameIndex = Random.Range(0, prefixIndices.Count);
                            prefix = prefixes[prefixIndices[nameIndex]];
                            prefixIndices.RemoveAt(nameIndex);
                            if (prefixIndices.Count == 0)
                            {
                                prefixIndices = Enumerable.Range(0, prefixes.Length).ToList();
                            }
                        }
                        connection.name = prefix.Replace("\r", "") + (Random.value <= nameHyphonationChance ? "-" : " ") + name;

                        // Set loading bar
                        float percent = (float)addedStars / (numStars - 1);
                        targetLoadingBarWidth = loadingBarWidth * percent;
                        loadingBarPercent.text = (percent * 100f).ToString("0.0") + "%";
                        anomalousStars.text = constellations.Count.ToString();

                        yield return AddConnections(connection);
                    }
                }
            }
        }

        loadingBarPercent.text = "";
        anomalousStars.text = "";
        loadingBarTextBottom.text = "Galaxy Generated";
        loadingBarTextTop.text = "";
        targetLoadingBarWidth = loadingBarWidth;
    }

    void SetHyperlaneColor(Star star, Color color)
    {
        star.hyperlaneRenderer.startColor = color;
        star.hyperlaneRenderer.endColor = color;
    }

    void SetStarInCircle(Star star)
    {
        float angle = Mathf.PI * 2f * Random.value;
        float radius = Mathf.Pow(Random.Range(centerClusterArea, 1f), centerClusterPower);
        star.transform.position = new Vector3(Mathf.Cos(angle) * galaxySize * centerClusterSize * radius, Mathf.Sin(angle) * galaxySize * centerClusterSize * radius);
    }

    void SetStarInSpiral(Star star, float arm)
    {
        float angle = Mathf.PI * armLength * Random.Range(spiralAreaRange / armLength * centerClusterSize, 1f);
        float offset = Random.value * armOffset * galaxySize;
        star.transform.position = Quaternion.Euler(0f, 0f, arm / numArms * 360f) * new Vector3(Mathf.Cos(angle) * angle * galaxySize + Random.Range(-offset, offset), Mathf.Sin(angle) * angle * galaxySize + Random.Range(-offset, offset));
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (starCellSize == 0f)
        {
            return;
        }

        Gizmos.color = new Color32(39, 39, 68, 245);

        int numCells = Mathf.RoundToInt(galaxySize / starCellSize);
        for (int y = -numCells; y < numCells; y++)
        {
            for (int x = -numCells; x < numCells; x++)
            {
                Gizmos.DrawLine(new Vector3(x * starCellSize, y * starCellSize), new Vector3((x + 1) * starCellSize, y * starCellSize));
                Gizmos.DrawLine(new Vector3(x * starCellSize, y * starCellSize), new Vector3(x * starCellSize, (y + 1) * starCellSize));

                if (y == numCells - 1)
                {
                    Gizmos.DrawLine(new Vector3(x * starCellSize, (y + 1) * starCellSize), new Vector3((x + 1) * starCellSize, (y + 1) * starCellSize));
                }
                if (x == numCells - 1)
                {
                    Gizmos.DrawLine(new Vector3((x + 1) * starCellSize, y * starCellSize), new Vector3((x + 1) * starCellSize, (y + 1) * starCellSize));
                }
            }
        }

        if (gridObject_EDITOR)
        {
            int cellX = Mathf.FloorToInt(gridObject_EDITOR.position.x / starCellSize);
            int cellY = Mathf.FloorToInt(gridObject_EDITOR.position.y / starCellSize);
            Gizmos.DrawCube(new Vector3(cellX * starCellSize + starCellSize * 0.5f, cellY * starCellSize + starCellSize * 0.5f), new Vector3(starCellSize, starCellSize, 0f));
        }
    }
#endif
}
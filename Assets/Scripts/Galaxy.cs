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
    public float centerClusterPower;
    public float armLength = 1f;
    public int numArms;
    [Range(-1f, 1f)] public float armOffset;

    [Space(20)]
    public RectTransform loadingBar;
    public float loadingBarWidth = 3040f;
    public float loadingBarLerpSpeed = 5f;
    public TextMeshProUGUI loadingBarPercent;
    public TextMeshProUGUI anomalousStars;

    [Space(10)]
    public TextMeshProUGUI starNameText;

    [Space(20)]
    public TextAsset starNames;
    public TextAsset starNamePrefixes;
    [Range(0f, 1f)] public float starNumberPrefixChance;

    [Space(20)]
    public Material hyperlaneMaterial;
    public float maxHyperlaneDistance;

    class DynamicGrid<T>
    {
        readonly Dictionary<Vector2Int, List<T>> list;

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

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        StartCoroutine(GenerateGalaxy());
    }

    void Update()
    {
        //DrawHyperlanes();
    }

    IEnumerator GenerateGalaxy()
    {
        stars = new Star[numStars];
        starCells = new DynamicGrid<Star>();
        int invalidStars = 0;

        string[] names = starNames.text.Split('\n');
        string[] prefixes = starNamePrefixes.text.Split('\n');
        int[] prefixNumbers = Enumerable.Range(0, 999).ToArray();

        List<int> nameIndices = Enumerable.Range(0, names.Length).ToList();
        List<int> prefixIndices = Enumerable.Range(0, prefixes.Length).ToList();
        List<int> prefixNumbersIndices = Enumerable.Range(0, prefixNumbers.Length).ToList();

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

            string name;
            int index = Random.Range(0, nameIndices.Count);
            name = names[nameIndices[index]];
            nameIndices.RemoveAt(index);
            if (nameIndices.Count == 0)
            {
                nameIndices = Enumerable.Range(0, names.Length).ToList();
            }

            string prefix;
            if (Random.value <= starNumberPrefixChance)
            {
                index = Random.Range(0, prefixNumbersIndices.Count);
                prefix = prefixNumbers[prefixNumbersIndices[index]].ToString();
                prefixNumbersIndices.RemoveAt(index);
                if (prefixNumbersIndices.Count == 0)
                {
                    prefixNumbersIndices = Enumerable.Range(0, prefixNumbers.Length).ToList();
                }
            }
            else
            {
                index = Random.Range(0, prefixIndices.Count);
                prefix = prefixes[prefixIndices[index]];
                prefixIndices.RemoveAt(index);
                if (prefixIndices.Count == 0)
                {
                    prefixIndices = Enumerable.Range(0, prefixes.Length).ToList();
                }
            }

            star.name = (prefix + " " + name).Replace("\r", "");

            stars[i] = star;
            starCells[cell].Add(star);

            if (i % 10 == 0)
            {
                yield return null;
            }

            // Set loading bar
            float percent = (float)i / (numStars - 1);
            loadingBar.sizeDelta = new Vector2(Mathf.Lerp(loadingBar.rect.width, loadingBarWidth * percent, loadingBarLerpSpeed * Time.deltaTime), loadingBar.rect.height);
            loadingBarPercent.text = (percent * 100f).ToString("0.0") + "%";

            anomalousStars.text = invalidStars.ToString();
        }

        // Set loading bar
        loadingBar.sizeDelta = new Vector2(loadingBarWidth, loadingBar.rect.height);
        loadingBarPercent.text = "100%";

        StartCoroutine(DrawHyperlanes());
    }

    IEnumerator DrawHyperlanes()
    {
        //GL.PushMatrix();
        //hyperlaneMaterial.SetPass(0);
        //GL.MultMatrix(transform.localToWorldMatrix);
        //GL.Begin(GL.LINES);
        //GL.Vertex3(0f, 0f, 0f);
        //GL.Vertex3(100f, 100f, 0f);
        //GL.End();
        //GL.PopMatrix();
        //var stars = starCells[Vector2Int.zero];

        int numStars = stars.Length;
        for (int i = 0; i < numStars; i++)
        {
            Star star = stars[i];
            int minHyperlanes = int.MaxValue;
            int bestIndex = -1;

            for (int j = 0; j < numStars; j++)
            {
                if (j != i && (star.transform.position - stars[j].transform.position).sqrMagnitude < maxHyperlaneDistance * maxHyperlaneDistance)
                {
                    int hyperlanes = stars[j].hyperlanes;
                    if (hyperlanes < minHyperlanes)
                    {
                        minHyperlanes = hyperlanes;
                        bestIndex = j;
                    }
                }
            }

            if (bestIndex == -1)
            {
                yield return null;
                break;
            }

            star.renderer.SetPositions(new Vector3[2] { star.transform.position, stars[bestIndex].transform.position });
            star.hyperlanes++;
            stars[bestIndex].hyperlanes++;
            yield return null;
        }
    }

    //void OnRenderObject()
    //{
    //    DrawHyperlanes();
    //}

    void SetStarInCircle(Star star)
    {
        float angle = Mathf.PI * 2f * Random.value;
        float radius = Mathf.Pow(Random.value, centerClusterPower);
        star.transform.position = new Vector3(Mathf.Cos(angle) * galaxySize * centerClusterSize * radius, Mathf.Sin(angle) * galaxySize * centerClusterSize * radius);
    }

    void SetStarInSpiral(Star star, float arm)
    {
        float angle = Mathf.PI * armLength * Random.value;
        float offset = Random.value * armOffset * galaxySize;
        star.transform.position = Quaternion.Euler(0f, 0f, arm / numArms * 360f) * new Vector3(Mathf.Cos(angle) * angle * galaxySize + Random.Range(-offset, offset), Mathf.Sin(angle) * angle * galaxySize + Random.Range(-offset, offset));
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
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
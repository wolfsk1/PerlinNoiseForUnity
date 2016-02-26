using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MapGenataor : MonoBehaviour
{


    public GameObject prefab;
    public GameObject WidthText;
    public GameObject HeightText;
    public GameObject SeedText;
    public GameObject OctaveText;

    private int width = 100;
    private int height = 100;
    private int seed = 9;
    private int octave = 4;

    private float[,] map;
    private List<GameObject> mapGameObject = new List<GameObject>();

	// Use this for initialization
	void Start ()
	{
	    WidthText.GetComponent<InputField>().text = width.ToString();
        HeightText.GetComponent<InputField>().text = height.ToString();
        SeedText.GetComponent<InputField>().text = seed.ToString();
        OctaveText.GetComponent<InputField>().text = octave.ToString();
	}


    IEnumerator GenerateMap()
    {

        for (int i = 0; i < mapGameObject.Count; i++)
        {
            Destroy(mapGameObject[i]);
        }
        mapGameObject.Clear();
        
        for(int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject temp = (GameObject)GameObject.Instantiate(prefab, new Vector3(i, 0, j), Quaternion.identity);
                temp.transform.localScale = new Vector3(1,/*(int)(map[i,j] * 10)*/map[i,j], 1);
                //if ((int)(map[i, j] * 10) >= 3)
                //{
                //    temp.GetComponent<Renderer>().material.color = new Color(0,  (int)(map[i, j] * 10) / 10.0f, 0);
                //}
                //else
                //{
                temp.GetComponent<Renderer>().material.color = new Color(0, 0, map[i, j]);
                //}
                mapGameObject.Add(temp);
            }
            yield return null;
        }
    }

    #region How To Version
    public void GenerateByHowToVersion()
    {
        Debug.Log("OnClick");
        width = Int32.Parse(WidthText.GetComponent<InputField>().text);
        height = Int32.Parse(HeightText.GetComponent<InputField>().text);
        seed = Int32.Parse(SeedText.GetComponent<InputField>().text);
        octave = Int32.Parse(OctaveText.GetComponent<InputField>().text);
        map = GeneratePerlinNoise(width, height, seed, octave);
        StartCoroutine(GenerateMap());
    }

    static float[,] GenerateWhiteNotice(int width, int height, int seed)
    {
        System.Random random = new System.Random(seed);
        float[,] whiteNoise = new float[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                whiteNoise[i, j] = (float)random.NextDouble() % 1;
            }
        }
        return whiteNoise;
    }

    static float[,] GenerateSmoothNoise(float[,] baseNoise, int octave)
    {
        int width = baseNoise.GetLength(0);
        int height = baseNoise.GetLength(1);

        float[,] smoothNoise = new float[width, height];

        int waveLength = 1 << octave;
        float frequency = 1.0f / waveLength;

        for (int i = 0; i < width; i++)
        {
            int leftBound = (i / waveLength) * waveLength;
            int rightBound = (leftBound + waveLength) % width;
            float horizontalBlendValue = (i - leftBound) * frequency;
            horizontalBlendValue = horizontalBlendValue*horizontalBlendValue*(3 - 2*horizontalBlendValue);

            for (int j = 0; j < height; j++)
            {
                int bottomBound = (j / waveLength) * waveLength;
                int topBound = (bottomBound + waveLength) % height;
                float verticalBlendValue = (j - bottomBound) * frequency;
                verticalBlendValue = verticalBlendValue * verticalBlendValue * (3 - 2 * verticalBlendValue);

                float bottom = Lerp(baseNoise[leftBound, bottomBound], baseNoise[rightBound, bottomBound], horizontalBlendValue);

                float top = Lerp(baseNoise[leftBound, topBound], baseNoise[rightBound, topBound], horizontalBlendValue);

                smoothNoise[i, j] = Lerp(top, bottom, verticalBlendValue);
            }

        }
        return smoothNoise;
    }

    static float[,] GeneratePerlinNoise(int width, int height, int seed, int octaveCount)
    {
        float[,] baseNoise = GenerateWhiteNotice(width, height, seed);

        float[][,] smoothNoise = new float[octaveCount][,];
        float persistance = 0.5f;

        for (int i = 0; i < octaveCount; i++)
        {
            smoothNoise[i] = GenerateSmoothNoise(baseNoise, i);
        }

        float[,] perlinNoise = new float[width, height];
        float amlitude = 5.0f;
        float totalAmplitude = 0.0f;

        for (int octave = octaveCount - 1; octave >= 0; octave--)
        {
            amlitude *= persistance;
            totalAmplitude += amlitude;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    perlinNoise[i, j] += smoothNoise[octave][i, j] * amlitude;
                }
            }
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                perlinNoise[i, j] /= totalAmplitude;
            }
        }

        return perlinNoise;

    }
    #endregion

    #region Candy Version

    private Vector2[] RandomVector2s;

    private void GenerateRanomVector2s(int height, int width, int seed)
    {
        int maxCount = height > width ? height : width;
        RandomVector2s = new Vector2[maxCount];
    }

    #endregion

    static float Lerp(float a, float b, float blend)
    {
        return (1 - blend) * a + blend * b;
    }
}

﻿using System;
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
                if ((int)(map[i, j] * 10) >= 3)
                {
                    temp.GetComponent<Renderer>().material.color = new Color(0,  (int)(map[i, j] * 10) / 10.0f, 0);
                }
                else
                {
                temp.GetComponent<Renderer>().material.color = new Color(0, 0, map[i, j]);
                }
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

    private float newSeed;

    public void GenerateByCandyVersion()
    {
        Debug.Log("OnClick");
        width = Int32.Parse(WidthText.GetComponent<InputField>().text);
        height = Int32.Parse(HeightText.GetComponent<InputField>().text);
        seed = Int32.Parse(SeedText.GetComponent<InputField>().text);
        octave = Int32.Parse(OctaveText.GetComponent<InputField>().text);
        newSeed = (float)new System.Random(seed).NextDouble();
        map = GeneratePerlinNoiseFBM(width, height, seed, octave);
        StartCoroutine(GenerateMap());
    }


    private float[,] GeneratePerlinNoiseFBM(int height, int width, int seed, int octave)
    {

        float[,] result =  new float[width,height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                for (int k = 0; k < octave; k++)
                {
                    float frequency = 1 << k;
                    float amplitude = 1/frequency;
                    result[i, j] += GeneratePerlinNoise(new Vector2((i + 0.5f) * frequency, (j + 0.5f) * frequency)) * amplitude;
                }

            }
        }

        return result;
    }

    private float GeneratePerlinNoise(Vector2 p)
    {
        // Find p Left-Bottom Point
        Vector2 pi = new Vector2((int)p.x, (int)p.y);
        // Find pi to p
        Vector2 pf = p - pi;
        // Calculate ease weight
        float weightX = pf.x * pf.x * (3 - 2 * pf.x);
        float weightY = pf.y * pf.y * (3 - 2 * pf.y);

        Vector2 lb = pi + Vector2.zero;
        Vector2 rb = pi + new Vector2(1f, 0f);
        Vector2 lt = pi + new Vector2(0f, 1f);
        Vector2 rt = pi + Vector2.one;

        float bottomX = Lerp(Vector2.Dot(Hash22(lb), (pf - Vector2.zero)), Vector2.Dot(Hash22(rb), (pf - new Vector2(1f, 0f))), weightX);
        float topX = Lerp(Vector2.Dot(Hash22(lt), (pf - new Vector2(0f, 1f))), Vector2.Dot(Hash22(rt), (pf - Vector2.one)), weightX);

        return Lerp(bottomX, topX, weightY);

    }

    private Vector2 Hash22(Vector2 p)
    {
        Vector3 p3 = new Vector3(Fract(p.x), Fract(p.y), Fract(p.x));
        p3 += Vector3.one * Vector3.Dot(p3, (p3 + Vector3.one * newSeed)); 
        return new Vector2(Fract((p3.x + p3.y) * p3.z), Fract((p3.x + p3.z) * p3.y));
    }

    #endregion


    static float Fract(float num)
    {
        return num - (int)num;
    }

    static float Lerp(float a, float b, float blend)
    {
        return (1 - blend) * a + blend * b;
    }
}

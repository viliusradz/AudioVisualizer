using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.InputManagerEntry;

[RequireComponent(typeof(MeshFilter))]
public class MeshControl : MonoBehaviour
{
    public Material defMat;
    public int xSize = 10;
    public int ySize = 10;
    public bool revesedBuffer = false;
    public bool autoSize = false;

    public float inputMulti = 100;
    public float changeDamp = 3;

    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;

    private FrequencyAnalizer analyze;


    void Start()
    {
        analyze = FrequencyAnalizer.inst;

        if (autoSize)
        {
            ySize = (int)Mathf.Sqrt(analyze.buffSize) - 1;
            xSize = ySize;
        }
        else
            ySize = (analyze.buffSize / (xSize + 1)) - 1;
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        CreateShape();
        analyze.soundSamples.AddListener(FrequencyToMesh);
        UpdateMesh();
        GetComponent<MeshRenderer>().material = defMat;
    }

    void CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (ySize + 1)];
        for (int i = 0, z = 0; z <= ySize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                vertices[i] = new Vector3(x, Mathf.Cos(i), z);
                i++;
            }
        }

        triangles = new int[xSize * ySize * 6];

        int vert = 0;
        int tries = 0;
        for (int z = 0; z < ySize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tries] = vert;
                triangles[tries + 1] = vert + xSize + 1;
                triangles[tries + 2] = vert + 1;
                triangles[tries + 3] = vert + 1;
                triangles[tries + 4] = vert + xSize + 1;
                triangles[tries + 5] = vert + xSize + 2;

                vert++;
                tries += 6;
            }
            vert++;
        }

    }
    private void FrequencyToMesh(float[] buffer)
    {
        
        //if (revesedBuffer)
        //{
        //    System.Array.Reverse(buffer);
        //}
        if (revesedBuffer)
        {
            int i = 0;
            for (int ind = i + xSize, y = 0; y <= ySize; y++)
            {
                for (int x = 0; x <= xSize; x++)
                {
                    var temp = buffer[ind - x];
                    buffer[ind - x] = buffer[i];
                    buffer[i] = temp;
                    i++;
                }
            }
            for (int k = 0; k < vertices.Length; k++)
            {
                vertices[k].y = Mathf.Lerp(vertices[k].y, buffer[k] * inputMulti, changeDamp * Time.deltaTime);
            }
        }
        if(!revesedBuffer)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].y = Mathf.Lerp(vertices[i].y, buffer[i] * inputMulti, changeDamp * Time.deltaTime);
            }
        }
        //int i = 0;
        //for (int ind = i + xSize, z = 0; z <= ySize; z++)
        //{
        //    for (int x = 0; x <= xSize; x++)
        //    {
        //        var temp = vertices[ind - x].y;
        //        vertices[ind - x].y = vertices[i].y;
        //        vertices[i].y = temp;
        //        i++;
        //    }
        //}
        //for (int i = 0; i < xSize+1; i++)
        //{
        //    var temp = vertices[(xSize - i)*ySize].y;
        //    vertices[(xSize - i) * ySize].y = vertices[i*ySize].y;
        //    vertices[i*ySize].y = temp;
        //}


        UpdateMesh();
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }
}

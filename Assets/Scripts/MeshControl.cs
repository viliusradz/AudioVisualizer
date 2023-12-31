using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshControl : MonoBehaviour
{
    public Material defMat;
    public bool revesedBuffer = false;
    public bool autoSize = false;

    [HideInInspector]
    public InfinityCycler cycler;
    //public float inputMulti = 100;
    //public float changeDamp = 3;

    Mesh mesh;
    Vector3[] vertices;
    Vector3[] uvs;
    int[] triangles;

    private FrequencyAnalizer analyze;
    //private InfinityCycler cycler;
    private int xSize = 10;
    private int ySize = 10;


    void Start()
    {
        //analyze = FrequencyAnalizer.inst;
        xSize = cycler.xSize;
        ySize =cycler.ySize;

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        CreateShape();
        UpdateMesh();
        mesh.SetUVs(0, uvs);
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

        uvs = new Vector3[vertices.Length];
        var xMax = vertices[xSize].x;
        var yMax = vertices[(xSize+1)*ySize].y;
        int ind = 0;
        for (int y = 0; y < ySize; y++)
        {
            for (int x = 0; x < xSize; x++)
            {
                var xNormalized = x / xMax;
                var yNormalized = y/yMax; 
                uvs[ind] = new Vector3(xNormalized, yNormalized);
            }
        }

    }
    private void Update()
    {
        if (revesedBuffer)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].y = cycler.reversedVertices[i].y;
            }
        }
        else
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].y = cycler.vertices[i].y;
            }
        }
        UpdateMesh();
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }
    //private void FrequencyToMesh(float[] buffer)
    //{

    //    if (revesedBuffer)
    //    {
    //        int i = 0;
    //        for (int y = 0; y <= ySize; y++)
    //        {
    //            int ind = i + xSize;
    //            for (int x = 0; x <= xSize; x++)
    //            {
    //                vertices[ind-x].y = Mathf.Lerp(vertices[ind-x].y, buffer[i] * inputMulti, changeDamp * Time.deltaTime);
    //                vertices[i].y = Mathf.Lerp(vertices[i].y, buffer[ind-x] * inputMulti, changeDamp * Time.deltaTime);
    //                i++;
    //            }
    //        }
    //    }
    //    if(!revesedBuffer)
    //    {
    //        for (int i = 0; i < vertices.Length; i++)
    //        {
    //            vertices[i].y = Mathf.Lerp(vertices[i].y, buffer[i] * inputMulti, changeDamp * Time.deltaTime);
    //        }
    //    }
    //    UpdateMesh();
    //}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTemplateProjects;

public enum TileState
{
    Alive, Dead
}
public class GameHandler : MonoBehaviour
{

    public TileBuilder tileBuilder;
    public float tickTime = 0.5f;
    public bool gameRunning = false;
    public bool turnMeshOffIfDead = false;

    private static GameHandler instance;

    public static GameHandler Instance
    {
        get
        {
            return instance;
        }
    }


    private void Awake()
    {
        if (instance == null) instance = this;

        float maxDis = Mathf.Max(tileBuilder.boardSizeX, tileBuilder.boardSizeY, tileBuilder.boardSizeZ);

        Vector3 campos = this.transform.position + Vector3.one * maxDis * 1.2f;//(Vector3.forward * ((tileBuilder.boardSizeX + tileBuilder.boardSizeY + tileBuilder.boardSizeZ) / 3) * -1.2f);
        Camera.main.transform.position = campos;
        Camera.main.transform.LookAt(this.transform.position);
        Camera.main.GetComponent<SimpleCameraController>().enabled = true;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            gameRunning = false;
            StopAllCoroutines();
            tileBuilder.CreateGrid();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //if (!gameRunning) PrintTilesValues();

            gameRunning = !gameRunning;

            if (gameRunning) StartCoroutine(IntervalRun());
            else if(turnMeshOffIfDead) tileBuilder.EnableAll();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }


    IEnumerator IntervalRun()
    {
        for (int i = 0; i < tileBuilder.tiles.Count; i++)
        {
            tileBuilder.tiles[i].SetNextState();
        }

        yield return new WaitForSeconds(tickTime);

        for (int i = 0; i < tileBuilder.tiles.Count; i++)
        {
            tileBuilder.tiles[i].SetState();
        }

        if (gameRunning) StartCoroutine(IntervalRun());
    }

    public void PrintTilesValues()
    {
        string values = "{";
        for (int i = 0; i < tileBuilder.tiles.Count; i++)
        {
            if (tileBuilder.tiles[i].State == TileState.Alive) values += "," + i;
        }
        values += "};";
        Debug.Log(values);
    }
}

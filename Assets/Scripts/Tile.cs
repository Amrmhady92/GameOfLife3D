using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Tile : MonoBehaviour
{
    public int x;
    public int y;
    public int z;

    public int listIndex;


    public List<Tile> neighbouringTiles;


    public float size = 1;

    public Color liveMaterialColor;
    public Color deadMaterialColor;
    public Color deadMaterialColorPlayMode;

    private MeshRenderer meshRend;
    private TileState state;

    public TileState nextState;

    public TileState State
    {
        get
        {
            return state;
        }

        set
        {

            if (meshRend == null) meshRend = this.GetComponent<MeshRenderer>();
            switch (value)
            {
                case TileState.Alive:
                    gameObject.SetActive(true);
                    meshRend.material.color = liveMaterialColor;
                    break;
                case TileState.Dead:
                    if (GameHandler.Instance.gameRunning)
                    {
                        if (GameHandler.Instance.turnMeshOffIfDead)
                            gameObject.SetActive(false);
                        else meshRend.material.color = deadMaterialColorPlayMode;
                    }
                    else
                    {

                        meshRend.material.color = deadMaterialColor;
                    }
                    break;
            }
            state = value;
        }
    }



    private int liveNeighbours = 0; 
    public void SetNextState()
    {
        liveNeighbours = 0;

        if(neighbouringTiles == null)
        {
            Debug.LogError("neighbouringTiles is Null");
            return;
        }

        for (int i = 0; i < neighbouringTiles.Count; i++)
        {
            if(neighbouringTiles[i] != null && neighbouringTiles[i].State == TileState.Alive)
            {
                liveNeighbours++;
            }
        }

        if(State == TileState.Alive)
        {
            if (liveNeighbours <= 1 || liveNeighbours >= 4) nextState = TileState.Dead;
            else nextState = TileState.Alive;
        }
        else
        {
            if (liveNeighbours == 3) nextState = TileState.Alive;
            else nextState = TileState.Dead;
        }
    }
    public void SetState()
    {
        State = nextState;
    }

    private void OnMouseDown()
    {
        if (!GameHandler.Instance.gameRunning)
        {
            State = (State == TileState.Alive) ? TileState.Dead : TileState.Alive;
        }
        //GetNeighbours(2);
    }

    Collider[] hits;
    Tile tile;
    public void GetNeighbours(float distance)
    {
        neighbouringTiles = new List<Tile>();
        size = distance;

        hits = Physics.OverlapSphere(this.transform.position, distance);
        if(hits.Length > 0)
        {
            Debug.Log("found hits = " + hits.Length);
            for (int i = 0; i < hits.Length; i++)
            {
                tile = hits[i].gameObject.GetComponent<Tile>();
                if(tile != null && tile != this)
                {
                    neighbouringTiles.Add(tile);
                }
            }
        }

    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawWireSphere(this.transform.position, size);
    //}
}

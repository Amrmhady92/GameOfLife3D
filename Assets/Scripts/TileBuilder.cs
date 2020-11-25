using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBuilder : MonoBehaviour
{
    public int boardSizeX = 9;
    public int boardSizeY = 9;
    public int boardSizeZ = 9;

    public float separation = 0;
    public float objectSize = 1;
    public Pooler pooler;

    public bool createGridOnStart = true;
    private GameObject cursor;
    public List<Tile> tiles;

    void Start()
    {
        tiles = new List<Tile>();
        if(createGridOnStart) CreateGrid();
    }


    public void CreateGrid()
    {
        if (tiles != null)
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                tiles[i].gameObject.SetActive(false);
            }
            tiles = new List<Tile>();
        }
        cursor = new GameObject("cursor");
        cursor.transform.position = this.transform.position;
        float startPosX = 0;
        float startPosY = 0;
        float startPosZ = 0;

        if (boardSizeX % 2 == 0) startPosX = ((boardSizeX - 1) / 2) * (objectSize + separation) + (objectSize / 2 + separation / 2);
        else startPosX = (boardSizeX / 2) * (objectSize + separation);

        if (boardSizeY % 2 == 0) startPosY = ((boardSizeY - 1) / 2) * (objectSize + separation) + (objectSize / 2 + separation / 2);
        else startPosY = (boardSizeY / 2) * (objectSize + separation);

        if (boardSizeZ % 2 == 0) startPosZ = ((boardSizeZ - 1) / 2) * (objectSize + separation) + (objectSize / 2 + separation / 2);
        else startPosZ = (boardSizeZ / 2) * (objectSize + separation);


        cursor.transform.position = new Vector3(cursor.transform.position.x - startPosX, cursor.transform.position.y + startPosY, cursor.transform.position.z + startPosZ);
        float startX = cursor.transform.position.x;
        float startY = cursor.transform.position.y;
        //float startZ = cursor.transform.position.z;


        Tile currentTile;
        for (int i = 0; i < boardSizeY; i++)
        {
            for (int j = 0; j < boardSizeX; j++)
            {
                for (int k = 0; k < boardSizeZ; k++)
                {

                    currentTile = pooler.Get(true).GetComponent<Tile>();
                    tiles.Add(currentTile);

                    currentTile.x = k;
                    currentTile.y = j;
                    currentTile.z = i;
                    currentTile.State = TileState.Dead;
                    currentTile.listIndex = tiles.Count - 1;

                    currentTile.transform.position = cursor.transform.position;
                    cursor.transform.position = new Vector3(cursor.transform.position.x + (objectSize + separation), cursor.transform.position.y, cursor.transform.position.z);

                }
                cursor.transform.position = new Vector3(startX, cursor.transform.position.y - (objectSize + separation), cursor.transform.position.z);
            }
            cursor.transform.position = new Vector3(startX , startY , cursor.transform.position.z - (objectSize + separation));

        }
        Destroy(cursor);
        //SetTilesNeighbours();
        Invoke("SetTilesNeighbours", 1);
    
    }

    public void SetTilesNeighbours()
    {
        if (tiles == null) return;
        float dist = (separation + objectSize) * Mathf.Sqrt(2);
        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i] != null) tiles[i].GetNeighbours(dist);
        }

        return;

        //XX

        Tile tempTile = null;
        int xMax = boardSizeX;
        int yMax = boardSizeY;
        int zMax = boardSizeZ;

        bool farLeft, farRight, farTop, farBot, farIn, farOut;
        for (int i = 0; i < tiles.Count; i++)
        {
            //  1 2 3       1: x > 0 && y > 0 , 2: y > 0 , 3: x < max && y > 0 
            //  4 - 5       4: x > 0 , 5: x < max
            //  6 7 8       6: x > 0 && y < max , 7: y < max , 8: x < max && y < max;



            //  1 2 3       1: i - 1 - xMax , 2: i - xMax , 3: i + 1 - xMax 
            //  4 - 5       4: i - 1        , 9:          , 5: i + 1
            //  6 7 8       6: i - 1 + xMax , 7: i + xMax , 8: i + 1 + xMax

            // z + 1
            //  1 2 3       1: i - 1 - xMax , 2: i - xMax , 3: i + 1 - xMax 
            //  4 9 5       4: i - 1        , 9:          , 5: i + 1
            //  6 7 8       6: i - 1 + xMax , 7: i + xMax , 8: i + 1 + xMax

            // z - 1
            //  1 2 3       1: i - 1 - xMax , 2: i - xMax , 3: i + 1 - xMax 
            //  4 9 5       4: i - 1        , 9:                 , 5: i + 1
            //  6 7 8       6: i - 1 + xMax , 7: i + xMax  , 8: i + 1 + xMax


            tiles[i].neighbouringTiles = new List<Tile>();

            farLeft = tiles[i].x == 0;
            farRight = tiles[i].x == xMax - 1;

            farTop = tiles[i].y == 0;
            farBot = tiles[i].y == yMax - 1;

            farIn = tiles[i].z == 0;
            farOut = tiles[i].z == zMax - 1;


            tempTile = i - 1 - xMax >= 0 && !farLeft && !farTop ? tiles[i - 1 - xMax] : null; // 1
            tiles[i].neighbouringTiles.Add(tempTile);

            tempTile = i - xMax >= 0 && !farTop ? tiles[i - xMax] : null; // 2
            tiles[i].neighbouringTiles.Add(tempTile);

            tempTile = i + 1 - xMax > 0 && !farRight && !farTop ? tiles[i + 1 - xMax] : null; // 3
            tiles[i].neighbouringTiles.Add(tempTile);

            tempTile = i - 1 >= 0 && !farLeft ? tiles[i - 1] : null; // 4
            tiles[i].neighbouringTiles.Add(tempTile);

            tempTile = i + 1 < tiles.Count && !farRight? tiles[i + 1] : null; // 5
            tiles[i].neighbouringTiles.Add(tempTile);

            tempTile = i - 1 + xMax < tiles.Count && !farLeft && !farBot? tiles[i - 1 + xMax] : null; // 6
            tiles[i].neighbouringTiles.Add(tempTile);

            tempTile = i + xMax < tiles.Count && !farBot ? tiles[i + xMax] : null; // 7
            tiles[i].neighbouringTiles.Add(tempTile);

            tempTile = i + 1 + xMax < tiles.Count && !farRight && !farBot ? tiles[i + 1 + xMax] : null; // 8
            tiles[i].neighbouringTiles.Add(tempTile);

        }

    }


    public void EnableAll()
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            if(tiles[i] != null)

            tiles[i].gameObject.SetActive(true);
            tiles[i].State = tiles[i].State;
        }
    }

}


//void Game::UpdateFrontBuffer()
//{
//    //For each cell i and j, with
//    //i = 0 ... I-1 and j = 0 ... J-1,
//    //evaluate mM[mFrontBufferIdx][i][j] based on previous grid
//    //mM[1-mFrontBufferIdx][i][j]

//    // 1 2 3      1: i-1 , j-1  , 2: i , j-1 , 3: i+1 , j-1
//    // 4 - 5      4: i-1 , j    ,            , 5: i+1 , j
//    // 6 7 8      6: i-1 , j+1  , 7: i , j+1 , 8: i+1 , j+1

//    int previousFrontBuffer = 1 - mFrontBufferIdx; /*== 0 ? 1 : 0;*/
//    int liveNeighbors = 0;

//    for (int i = 0; i < GRID_SIZE_Y; i++)
//    {
//        for (int j = 0; j < GRID_SIZE_X; j++)
//        {

//            //1
//            if (i - 1 >= 0 && j - 1 >= 0/* && i != 0 && j != 0*/) if (mM[previousFrontBuffer][i - 1][j - 1]) liveNeighbors++;

//            //2
//            if (j - 1 >= 0 /*&& j != 0*/) if (mM[previousFrontBuffer][i][j - 1]) liveNeighbors++;

//            //3
//            if (i + 1 < GRID_SIZE_Y && j - 1 >= 0 /*&& i != (GRID_SIZE_Y - 1) && j != 0*/) if (mM[previousFrontBuffer][i + 1][j - 1]) liveNeighbors++;

//            //4
//            if (i - 1 >= 0 /*&& i != (0)*/) if (mM[previousFrontBuffer][i - 1][j]) liveNeighbors++;

//            //5
//            if (i + 1 < GRID_SIZE_Y /*&& i != (GRID_SIZE_Y - 1)*/) if (mM[previousFrontBuffer][i + 1][j]) liveNeighbors++;

//            //6
//            if (i - 1 >= 0 && j + 1 < GRID_SIZE_X /*&& i != 0 && j != (GRID_SIZE_X - 1)*/) if (mM[previousFrontBuffer][i - 1][j + 1]) liveNeighbors++;

//            //7
//            if (j + 1 < GRID_SIZE_X /*&& j != (GRID_SIZE_X - 1)*/) if (mM[previousFrontBuffer][i][j + 1]) liveNeighbors++;

//            //8
//            if (i + 1 < GRID_SIZE_Y && j + 1 < GRID_SIZE_X /*&& i != (GRID_SIZE_Y - 1) && j != (GRID_SIZE_X - 1)*/) if (mM[previousFrontBuffer][i + 1][j + 1]) liveNeighbors++;


//            if (mM[previousFrontBuffer][i][j]) //Alive
//            {
//                if (liveNeighbors <= 1 || liveNeighbors >= 4) mM[mFrontBufferIdx][i][j] = false;
//                else mM[mFrontBufferIdx][i][j] = true;
//            }
//            else // Dead 
//            {
//                if (liveNeighbors == 3) mM[mFrontBufferIdx][i][j] = true;
//                else mM[mFrontBufferIdx][i][j] = false;
//            }

//            liveNeighbors = 0;
//        }
//    }
   
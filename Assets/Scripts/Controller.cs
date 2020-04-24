using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    //GameObjects
    public GameObject board;
    public GameObject[] cops = new GameObject[2];
    public GameObject robber;
    public Text rounds;
    public Text finalMessage;
    public Button playAgainButton;

    public GameObject piecePrefab;
    public Material blackMaterial;
    public Material whiteMaterial;
    public Material tileMaterial;
    public Material tileSelectableMaterial;

    private List<int> selectableTiles = new List<int>();

    //Otras variables
    Tile[] tiles = new Tile[Constants.NumTiles];
    GameObject[] pieces = new GameObject[Constants.NumTiles];
    private int roundCount = 0;
    private int estado;
    private int clickedTile = -1;
    private int clickedCop = 0;
    private int turn = 1;
                    
    void Start()
    {        
        InitTiles();
        InitAdjacencyLists();
        estado = Constants.Init;
    }
        
    //Rellenamos el array de casillas y posicionamos las fichas
    void InitTiles()
    {
        for (int fil = 0; fil < Constants.TilesPerRow; fil++)
        {
            GameObject rowchild = board.transform.GetChild(fil).gameObject;            

            for (int col = 0; col < Constants.TilesPerRow; col++)
            {
                GameObject tilechild = rowchild.transform.GetChild(col).gameObject;                
                tiles[fil * Constants.TilesPerRow + col] = tilechild.GetComponent<Tile>();
                //tilechild.GetComponent<Renderer>().material = tileMaterial;
            }
        }
               
    }

    public void InitAdjacencyLists()
    {
        //Matriz de adyacencia
        int[,] matriu = new int[Constants.NumTiles, Constants.NumTiles];

        //TODO: Inicializar matriz a 0's
        for (int i = 0; i < Constants.NumTiles; i++)
        {
            for (int j = 0; j < Constants.NumTiles; j++)
            {
                matriu[i, j] = 0;
            }
        }

        //TODO: Para cada posición, rellenar con 1's las casillas adyacentes (arriba, abajo, izquierda, derecha y diagonales)
        for (int i = 0; i < Constants.NumTiles; i++)
        {
            int dreta = i + 1;
            int amunt = i + 8;
            int esquerre = i - 1;
            int avall = i - 8;

            if ((i + 1) % 8 != 0)
                matriu[i, dreta] = 1;
            if (i < 56)
                matriu[i, amunt] = 1;
            if (i % 8 != 0)
                matriu[i, esquerre] = 1;
            if (i > 7)
                matriu[i, avall] = 1;

            //Diagonals
            if (((i + 1) % 8 != 0) && (i > 7))
                matriu[i, avall + 1] = 1;
            if (((i + 1) % 8 != 0) && (i < 56))
                matriu[i, amunt + 1] = 1;
            if ((i % 8 != 0) && (i > 7))
                matriu[i, avall - 1] = 1;
            if ((i % 8 != 0) && (i < 56))
                matriu[i, amunt - 1] = 1;

        }

        //TODO: Rellenar "adyacencia" de cada casilla con sus adyacentes
        foreach (Tile tile in tiles)
        {            
            for (int i = 0; i < Constants.NumTiles; i++)
            {
                if (matriu[tile.NumTile, i] == 1)
                {
                    tile.adyacencia.Add(i);
                }
            }
        }
    }

    //Reseteamos cada casilla: color, padre, distancia y visitada
    public void ResetTiles()
    {        
        foreach (Tile tile in tiles)
        {
            tile.Reset();
        }
    }
    
    

    public void ClickOnTile(int row, int col)
    {

        float coordrow = row - 3.5f;
        float coordcol = col - 3.5f;
        int clickedtile = row * 8 + col;

        //Debug.Log("["+ clickedtile + "]: "+"He fet clic en " + row+" "+col+" coordenades "+ coordrow + " "+ coordcol);

        Vector3 pos = new Vector3(coordcol, 0.238f, coordrow);
        // Instantiate at position (0, 0, 0) and zero rotation.
        pieces[clickedtile] = Instantiate(piecePrefab, pos, Quaternion.identity);
        if(turn==1)
            pieces[clickedtile].GetComponent<Renderer>().material = blackMaterial;
        else
            pieces[clickedtile].GetComponent<Renderer>().material = whiteMaterial;

        tiles[clickedtile].value = turn;

        //Cambio las fichas con movimiento
        swapPieces(clickedtile);

        turn = -turn;
        if (turn == -1)
            Debug.Log("Canvie el torn a blanques");
        else
            Debug.Log("Canvie el torn a negre");

        // Reseteamos color
        foreach (int t in selectableTiles)
        {
            tiles[t].GetComponent<Renderer>().material = tileMaterial;
        }
        
        // Limpiamos la lista
        selectableTiles.Clear();

        // La rellenamos con las nuevas casillas seleccionables
        FindSelectableTiles(turn);

        



        /*switch (estado)
        {            
            case Constants.CopSelected:
                //Si es una casilla roja, nos movemos
                if (tiles[clickedTile].selectable)
                {                  
                    cops[clickedCop].GetComponent<PlayerMove>().MoveToTile(tiles[clickedTile]);
                    cops[clickedCop].GetComponent<PlayerMove>().currentTile=tiles[clickedTile].NumTile;
                    tiles[clickedTile].current = true;   

                    estado = Constants.TileSelected;
                }                
                break;
            case Constants.TileSelected:
                estado = Constants.Init;
                break;
            case Constants.RobberTurn:
                estado = Constants.Init;
                break;
        }*/
    }

    public void FinishTurn()
    {
        switch (estado)
        {            
            case Constants.TileSelected:
                ResetTiles();

                estado = Constants.RobberTurn;
                RobberTurn();
                break;
            case Constants.RobberTurn:                
                ResetTiles();
                IncreaseRoundCount();
                if (roundCount <= Constants.MaxRounds)
                    estado = Constants.Init;
                else
                    EndGame(false);
                break;
        }

    }

    public void RobberTurn()
    {
        

    }

    public void EndGame(bool end)
    {
        if(end)
            finalMessage.text = "You Win!";
        else
            finalMessage.text = "You Lose!";
        playAgainButton.interactable = true;
        estado = Constants.End;
    }

    public void PlayAgain()
    {       
                        
        ResetTiles();

        playAgainButton.interactable = false;
        finalMessage.text = "";
        roundCount = 0;
        rounds.text = "Rounds: ";

        estado = Constants.Restarting;
    }

    public void InitGame()
    {
        estado = Constants.Init;
         
    }

    public void IncreaseRoundCount()
    {
        roundCount++;
        rounds.text = "Rounds: " + roundCount;
    }

    public bool checkNext(int val)
    {
        Debug.Log("Check next de " + val);
        //Si hay casilla y está vacía, puedo ir
        if (tiles[val].value == Constants.Empty)
        {
            //Si no la he marcado ya como seleccionable, la marco
            if (tiles[val].selectable == false)
            {
                Debug.Log("Clave " + val);
                selectableTiles.Add(val);
                tiles[val].GetComponent<Renderer>().material = tileSelectableMaterial;
            }
            return true;
        }

        //Si hay casilla pero hay una ficha del mismo color, no puedo ir
        if (tiles[val].value == turn)
            return true;

        return false;
    }

    public void FindSelectableTiles(int turn)
    {                

        for (int tile = 0; tile < Constants.NumTiles; tile++)
        {
           if(tiles[tile].value == turn)
           {
                Debug.Log("TILE [" + tile + "]---------------------------------");
                /*int dreta = tile + 1;
                int amunt = tile + 8;
                int esquerre = tile - 1;
                int avall = tile - 8;*/
                                
                //DERECHA: Si no estoy en una esquina derecha y si hay una ficha contraria a mi derecha. La comprovació 2 és simplemente perquè no pete la 3
                if (((tile + 1) % 8 != 0) && ((tile + 1) <= 63) && tiles[tile+1].value == -turn ){

                    Debug.Log("Vei " + (tile + 1));
                    int n = tile+1;
                    bool stop = false;
                    while (!stop)
                    {
                        //Miro a la derecha de mi vecino. Si mi vecino está en una esquina derecha, paro
                        //if ((n + 1) > 63)
                        if ((n + 1) % 8 == 0)
                            stop = true;

                        else
                        {
                            stop = checkNext(n + 1);
                            //Si después de pintar, ya no hay casillas a la derecha, paro
                            /*if ((n + 1 + 1) % 8 == 0)
                                stop = true;*/
                        }
                        
                        n = n + 1;
                    }
                }

                //IZQUIERDA: Si no estoy en una esquina izquierda y si hay una ficha contraria a mi izquierda. La comprovació 2 és simplemente perquè no pete la 3
                if ((tile % 8 != 0) && ((tile - 1) >= 0) && tiles[tile - 1].value == -turn)
                {
                    Debug.Log("Vei " + (tile - 1));
                    int n = tile - 1;
                    bool stop = false;
                    while (!stop)
                    {
                        //Miro a la izquierda de mi vecino. Si mi vecino está en una esquina izquierda, paro
                        //if ((n - 1) < 0)
                        if (n % 8 == 0)
                            stop = true;

                        else
                        {
                            stop = checkNext(n - 1);
                            //Si después de pintar, ya no hay casillas a la izquierda, paro
                            /*if ((n - 1) % 8 == 0)
                                stop = true;*/
                        }

                        n = n - 1;
                    }
                }

                //ARRIBA: Si no estoy en la última fila y si hay una ficha contraria arriba de la mia. La comprovació 2 és simplemente perquè no pete la 3
                if ((tile < 56) && ((tile + 8) <= 63) && tiles[tile + 8].value == -turn)
                {
                    Debug.Log("Vei " + (tile + 8));
                    int n = tile + 8;
                    bool stop = false;
                    while (!stop)
                    {
                        //Miro arriba de mi vecino. Si mi vecino está en la última fila, paro
                        if (n >= 56)                    
                            stop = true;
                        
                        else                        
                            stop=checkNext(n + 8);                          
                                                
                        n = n + 8;
                    }
                }

                //ABAJO: Si no estoy en la primera fila y si hay una ficha contraria abajo de la mia. La comprovació 2 és simplemente perquè no pete la 3
                if ((tile > 7) && ((tile - 8) >= 0) && tiles[tile - 8].value == -turn)
                {
                    Debug.Log("Vei " + (tile - 8));
                    int n = tile - 8;
                    bool stop = false;
                    while (!stop)
                    {
                        //Miro abajo de mi vecino. Si mi vecino está en la primera fila, paro
                        if (n <= 7)
                            stop = true;

                        else
                            stop = checkNext(n - 8);

                        n = n - 8;
                    }
                }

                //DIAGONAL DERECHA-ABAJO: Si no estoy en la esquina derecha ni en la primera fila y si hay una ficha contraria derecha-abajo mia. La comprovació 3 és perquè no pete la 4
                if (((tile + 1) % 8 != 0) && (tile > 7) && ((tile - 8 + 1) >= 0) && tiles[tile - 8 + 1].value == -turn)
                {                    

                    int n = tile - 8 + 1;//este és el veí
                    bool stop = false;
                    while (!stop)
                    {
                        //Si mi vecino está a la derecha o en la primera fila, paro
                        if ((( n + 1 ) % 8 == 0) || (n <= 7))
                            stop = true;

                        else
                        {
                            stop = checkNext(n - 8 + 1);
                            //Si después de pintar,ya no hay casillas en la diagonal derecha-abajo, paro
                            /*if ((n - 8 + 1 + 1) % 8 == 0)
                                stop = true;*/
                        }

                        n = n - 8 + 1;
                    }
                }


                //DIAGONAL DERECHA-ARRIBA: Si no estoy en la esquina derecha ni en la última fila y si hay una ficha contraria derecha-arriba mia. La comprovació 3 és perquè no pete la 4
                //if (((tile + 1) % 8 != 0) && (tile < 56) && ((tile + 8 + 1) <= 63) && tiles[tile + 8 + 1].value == -turn)
                if (((tile + 1) % 8 != 0) && (tile < 56) && ((tile + 8 + 1) <= 63) && tiles[tile + 8 + 1].value == -turn)
                {                    

                    int n = tile + 8 + 1;//este és el veí
                    bool stop = false;
                    while (!stop)
                    {
                        //Si me paso de rango, paro
                        //Si mi vecino está a la derecha o en la última fila, paro
                        if (((n + 1) % 8 == 0) || (n >= 56))
                            stop = true;

                        else
                        {
                            stop = checkNext(n + 8 + 1);

                            //Si después de pintar, ya no hay casillas en la diagonal derecha-abajo, paro
                            /*if ((n + 8 + 1 + 1) % 8 == 0)
                                stop = true;*/
                        }

                        n = n + 8 + 1;
                    }
                }


                //DIAGONAL IZQUIERDA-ABAJO: Si no estoy en la esquina izquierda ni en la primera fila y si hay una ficha contraria izquierda-abajo mia. La comprovació 3 és perquè no pete la 4
                if ((tile % 8 != 0) && (tile > 7) && ((tile - 8 - 1) >= 0) && tiles[tile - 8 - 1].value == -turn)
                {
                    Debug.Log("Vei " + (tile - 8 - 1));

                    int n = tile - 8 - 1;//este és el veí
                    bool stop = false;
                    while (!stop)
                    {
                        //Si mi vecino está a la izquierda o en la primera fila, paro
                        if ((n % 8 == 0) || (n <= 7))
                            stop = true;

                        else
                        {
                            stop = checkNext(n - 8 - 1);

                            //Si después de pintarla, ya no hay casillas en la diagonal derecha-abajo, paro
                            /*if ((n - 8 - 1) % 8 == 0)
                                stop = true;*/
                        }

                        n = n - 8 - 1;
                    }
                }


                //DIAGONAL IZQUIERDA-ARRIBA: Si no estoy en la esquina izquierda ni en la última fila y si hay una ficha contraria izquierda-abajo mia. La comprovació 3 és perquè no pete la 4
                if ((tile % 8 != 0) && (tile < 56) && ((tile + 8 - 1) <= 63) && tiles[tile + 8 - 1].value == -turn)
                {
                    Debug.Log("Vei " + (tile + 8 - 1));

                    int n = tile + 8 - 1;//este és el veí
                    bool stop = false;
                    while (!stop)
                    {
                        //Si mi vecino está a la izquierda o en la última fila, paro
                        if ((n % 8 == 0) || (n >= 56))
                            stop = true;

                        else
                        {
                            stop = checkNext(n + 8 - 1);

                            //Si después de pintarla, ya no hay casillas en la diagonal izquierda-arriba, paro
                            /*if ((n + 8 - 1) % 8 == 0)
                                stop = true;*/
                        }

                        n = n + 8 - 1;
                    }
                }

                /*int dreta = tile + 1;
                int amunt = tile + 8;
                int esquerre = tile - 1;
                int avall = tile - 8;*/

                /*if ((tile + 1) % 8 != 0)
                    matriu[i, dreta] = 1;
                if (tile < 56)
                    matriu[i, amunt] = 1;
                if (tile % 8 != 0)
                    matriu[i, esquerre] = 1;
                if (tile > 7)
                    matriu[i, avall] = 1;*/

                //Diagonals
                /*if (((tile + 1) % 8 != 0) && (tile > 7))
                    matriu[i, avall + 1] = 1;
                if (((tile + 1) % 8 != 0) && (tile < 56))
                    matriu[i, amunt + 1] = 1;
                if ((tile % 8 != 0) && (tile > 7))
                    matriu[i, avall - 1] = 1;
                if ((tile % 8 != 0) && (tile < 56))
                    matriu[i, amunt - 1] = 1;*/
            }
                                           
           
        }
                
    }
    
    public bool checkNextCandidates(int val,List<int> candidates)
    {
        //Si es del color propio, pinto los candidatos y paro
        if (tiles[val].value == turn)
        {            
            foreach (int c in candidates)
            {
                Debug.Log("Pinto "+c);
                tiles[c].value = turn;
                if (turn == 1)
                    pieces[c].GetComponent<Renderer>().material = blackMaterial;
                else
                    pieces[c].GetComponent<Renderer>().material = whiteMaterial;

            }

            return true;
        }

        //Si está vacía, paro
        if (tiles[val].value == Constants.Empty)
            return true;

        //Si llega hasta aquí, es que es del color oponente, añado y continuo
        //if (tiles[val].value == -turn)       
        candidates.Add(val);
        return false;

    }
   
    public void swapPieces(int tile)
    {
        List<int> candidates = new List<int>();

        Debug.Log("FICHA [" + tile + "]------------------ ");

        //DERECHA: Si hay una casilla y si hay una ficha contraria en esa casilla. La comprovació 2 és perquè no pete la 3
        if (((tile + 1) % 8 != 0) && ((tile + 1) <= 63) && tiles[tile + 1].value == -turn)
        {
            Debug.Log("Veí " + (tile + 1));
            candidates.Clear();
            candidates.Add(tile + 1);
            int n = tile + 1;
            bool stop = false;
            while (!stop)
            {
                //Si ya no hay casillas a la derecha de mi vecino, paro
                if ((n + 1) % 8 == 0)
                    stop = true;

                else
                {
                    Debug.Log("Dins de else");
                    stop = checkNextCandidates(n+1,candidates);

                }

                n = n + 1; 

            }

        }

        //IZQUIERDA: Si hay una casilla y si hay una ficha contraria en esa casilla. La comprovació 2 és perquè no pete la 3       
        if ((tile % 8 != 0) && ((tile - 1) >= 0) && tiles[tile - 1].value == -turn)
        {
            Debug.Log("Veí esquerre " + (tile - 1) + " i té " + tiles[tile - 1].value);
            candidates.Clear();
            candidates.Add(tile - 1);
            int n = tile - 1;
            bool stop = false;
            while (!stop)
            {
                //Si ya no hay casillas a la izquierda de mi vecino, paro
                if (n % 8 == 0)
                    stop = true;

                else
                {
                    stop = checkNextCandidates(n - 1, candidates);

                }

                n = n - 1;

            }
        }

        //ARRIBA: Si hay una casilla y si hay una ficha contraria en esa casilla. La comprovació 2 és perquè no pete la 3 però en este cas, no faria falta
        if ((tile < 56) && ((tile + 8) <= 63) && tiles[tile + 8].value == -turn)
        {
            Debug.Log("Veí " + (tile + 8));
            candidates.Clear();
            candidates.Add(tile + 8);
            int n = tile + 8;
            bool stop = false;
            while (!stop)
            {
                //Si ya no hay casillas arriba de mi vecino, paro
                if (n >= 56)//>= 56)
                    stop = true;

                else
                {
                    stop = checkNextCandidates(n + 8, candidates);

                }

                n = n + 8;

            }
        }

        //ABAJO: Si hay una casilla y si hay una ficha contraria en esa casilla. La comprovació 2 és perquè no pete la 3 però en este cas, no faria falta
        if ((tile > 7) && ((tile - 8) >= 0) && tiles[tile - 8].value == -turn)
        {
            Debug.Log("Veí baix" + (tile - 8) + " i té " + tiles[tile - 8].value);
            candidates.Clear();
            candidates.Add(tile - 8);
            int n = tile - 8;
            bool stop = false;
            while (!stop)
            {
                //Si ya no hay casillas abajo de mi vecino, paro
                if (n <= 7)
                    stop = true;

                else
                {
                    stop = checkNextCandidates(n - 8, candidates);

                }

                n = n - 8;

            }
        }

        //DIAGONAL DERECHA-ABAJO: Si no estoy ni en la esquina derecha ni en la primera fila y si mi vecino derecha-abajo tiene una ficha del oponente. La comprovació 3 és perquè no pete la 4
        //if (((tile + 1) % 8 != 0) && (tile > 7) && ((tile - 8 + 1) >= 0) && tiles[tile - 8 + 1].value == -turn)
        if (((tile + 1) % 8 != 0) && (tile > 7) && ((tile - 8 + 1) >= 0) && tiles[tile - 8 + 1].value == -turn)
        {
            candidates.Clear();
            candidates.Add(tile - 8 + 1);
            int n = tile - 8 + 1;
            bool stop = false;
            while (!stop)
            {
                //Si mi vecino está en la esquina derecha o en la primera fila, paro
                if (((n + 1) % 8 == 0) || (n <= 7))
                    stop = true;

                else
                {
                    stop = checkNextCandidates(n - 8 + 1, candidates);

                }

                n = n - 8 + 1;

            }
        }


        //DIAGONAL DERECHA-ARRIBA: Si no estoy ni en la esquina derecha ni en la última fila y si mi vecino derecha-arriba tiene una ficha del oponente. La comprovació 3 és perquè no pete la 4
        //if (((tile + 1) % 8 != 0) && (tile < 56) && ((tile + 8 + 1) <= 63) && tiles[tile + 8 + 1].value == -turn)
        if (((tile + 1) % 8 != 0) && (tile < 56) && ((tile + 8 + 1) <= 63) && tiles[tile + 8 + 1].value == -turn)
        {
            candidates.Clear();
            candidates.Add(tile + 8 + 1);
            int n = tile + 8 + 1;
            bool stop = false;
            while (!stop)
            {
                //Si mi vecino está en la esquina derecha o en la última fila, paro
                if (((n + 1) % 8 == 0) || (n >= 56))
                    stop = true;

                else
                {
                    stop = checkNextCandidates(n + 8 + 1, candidates);

                }

                n = n + 8 + 1;

            }
        }


        //DIAGONAL IZQUIERDA-ABAJO: Si no estoy ni en la esquina izquierda ni en la primera fila y si mi vecino izquierda-abajo tiene una ficha del oponente. La comprovació 3 és perquè no pete la 4
        //if ((tile % 8 != 0) && (tile > 7) && ((tile - 8 - 1) >= 0) && tiles[tile - 8 - 1].value == -turn)
        if ((tile % 8 != 0) && (tile > 7) && ((tile - 8 - 1) >= 0) && tiles[tile - 8 - 1].value == -turn)
        {
            candidates.Clear();
            candidates.Add(tile - 8 - 1);
            int n = tile - 8 - 1;
            bool stop = false;
            while (!stop)
            {
                //Si mi vecino está en la esquina izquierda o en la primera fila, paro
                if ((n % 8 == 0) || (n <= 7))
                    stop = true;

                else
                {
                    stop = checkNextCandidates(n - 8 - 1, candidates);

                }

                n = n - 8 - 1;

            }
        }

        //DIAGONAL IZQUIERDA-ARRIBA: Si no estoy ni en la esquina izquierda ni en la última fila y si mi vecino izquierda-arriba tiene una ficha del oponente. La comprovació 3 és perquè no pete la 4
        //if ((tile % 8 != 0) && (tile < 56) && ((tile + 8 - 1) <= 63) && tiles[tile + 8 - 1].value == -turn)
        if ((tile % 8 != 0) && (tile < 56) && ((tile + 8 - 1) <= 63) && tiles[tile + 8 - 1].value == -turn)
        {
            candidates.Clear();
            candidates.Add(tile + 8 - 1);
            int n = tile + 8 - 1;
            bool stop = false;
            while (!stop)
            {
                //Si mi vecino está en la esquina izquierda o en la última fila, paro
                if ((n % 8 == 0) || (n >= 56))
                    stop = true;

                else
                {
                    stop = checkNextCandidates(n + 8 - 1, candidates);

                }

                n = n + 8 - 1;

            }
        }

    }


    

   

       
}

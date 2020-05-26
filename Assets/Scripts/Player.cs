using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Tile[] board = new Tile[Constants.NumTiles];
    public Node parent;
    public List<Node> childList = new List<Node>();
    public int type;//Constants.MIN o Constants.MAX
    public double utility;
    public double alfa;
    public double beta;
    public int nextPosition;

    public Node(Tile[] tiles)
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            this.board[i] = new Tile();
            this.board[i].value = tiles[i].value;
        }

    }    

}

public class Player : MonoBehaviour
{
    public int turn;    
    private BoardManager boardManager;

    void Start()
    {
        boardManager = GameObject.FindGameObjectWithTag("BoardManager").GetComponent<BoardManager>();
    }
       
    /*
     * Entrada: Dado un tablero
     * Salida: Posición donde mueve  
     */
    public int SelectTile(Tile[] board)
    {
        //Generamos el nodo raíz del árbol (MAX)
        Node root = new Node(board);
        root.type = Constants.MAX;

        //Generamos primer nivel de nodos hijos
        List<int> selectableTiles = boardManager.FindSelectableTiles(board, turn);

        foreach (int s in selectableTiles)
        {
            //Creo un nuevo nodo hijo con el tablero padre
            Node n = new Node(root.board);
            //Lo añadimos a la lista de nodos hijo
            root.childList.Add(n);
            //Enlazo con su padre
            n.parent = root;
            //En nivel 1, los hijos son MIN
            n.type = Constants.MIN;
            //Aplico un movimiento, generando un nuevo tablero con ese movimiento
            boardManager.Move(n.board, s, turn);
            //si queremos imprimir el nodo generado (tablero hijo)
            //boardManager.PrintBoard(n.board);

            //Genero la nueva lista de movientos del nodo hijo
            List<int> selectableTilesNivelDos = boardManager.FindSelectableTiles(n.board, -turn);
            
            //NIVEL 2
            foreach (int s2 in selectableTilesNivelDos)
            {
                //Creo un nuevo nodo MAX con el tablero anterior MIN como padre
                Node o = new Node(n.board);
                n.childList.Add(o);
                o.parent = n;
                o.type = Constants.MAX;
                boardManager.Move(o.board, s2, -turn);
                calcularHeuristica(o, s2, turn);
                volcado(o);
            }
            volcado(n);
        }

        //Selecciono un movimiento aleatorio. Esto habrá que modificarlo para elegir el mejor movimiento según MINIMAX
        int movimiento = Random.Range(0, selectableTiles.Count);

        return selectableTiles[root.nextPosition];

    }

    public void calcularHeuristica(Node o, int s, int turn)
    {
        List<int> listaCambioColor = boardManager.FindSwappablePieces(o.board, s, turn);
        int contador = 0;
        foreach(int miVariable in listaCambioColor)
        {
            contador = contador + 1;
        }
        o.utility = contador * 0.1;
    }

    public void volcado(Node nodo)
    {
        if (nodo.parent.type == Constants.MIN)
        {
            if (nodo.parent.utility > nodo.utility)
            {
                nodo.parent.utility = nodo.utility;
                nodo.parent.nextPosition = nodo.nextPosition;
            }
        }
        else if (nodo.parent.type == Constants.MAX)
        {
            if (nodo.parent.utility < nodo.utility)
            {
                nodo.parent.utility = nodo.utility;
                nodo.parent.nextPosition = nodo.nextPosition;
            }
        }
    }
}

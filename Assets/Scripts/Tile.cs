using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public GameObject controller;
    public int NumTile;
    public int fila, columna;
    public int value = Constants.Empty;
    public List<int> adyacencia = new List<int>();

    public bool current = false;
    public bool selectable = false;
            
    //Variables para el BFS
    public bool visited = false;
    public Tile parent = null;//Casilla a través de la que llegamos
    public int distance = 0;//Distancia del original
        
    private void Awake()
    {     
        Vector3 pos = transform.position; 
        fila = (int)(pos.z + 3.5);
        columna = (int)(pos.x + 3.5);
        this.NumTile = fila * 8 + columna;
    }

    //Coloreamos cada casilla
    private void Update()
    {
       /* if (current)
        {
            GetComponent<Renderer>().material.color = Color.magenta;
        }
        else if (selectable)
        {
            GetComponent<Renderer>().material.color = Color.red;
        }
        else
        {
            GetComponent<Renderer>().material.color = Color.white;
        }*/
    }

    //Hacemos clic en casilla
    private void OnMouseDown()
    {
        //Debug.Log("He hecho clic en " + fila + " " + columna);
        string res = "";

        foreach(int a in adyacencia)
        {
            res=res+a+" ";
        }
        //Debug.Log("Adjacents " + res);
        controller.GetComponent<Controller>().ClickOnTile(fila,columna);
        
    }
        
    public void Reset()
    {
        current = false;
        selectable = false;
   
        visited = false;
        parent = null;
        distance = 0;
    }
 
}

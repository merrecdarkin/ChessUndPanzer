using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : MonoBehaviour
{
    protected Transform playerTransform;// Player Transform
	public float moverate = 3.0f;

	public enum PieceType
    {
        Pawn,
        Knight,
        Bishop,
        Rook,
        Queen,
        Lord,
        Devil
    }
	public PieceType TypeChess;



    // Start is called before the first frame update
    void Start()
    {
    TypeChess=PieceType.Bishop;
    GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");
    playerTransform = objPlayer.transform;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

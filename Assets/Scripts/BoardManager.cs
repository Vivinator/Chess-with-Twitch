using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour {

	public static BoardManager Instance{ set; get;}
	private bool[,] allowedMoves{ set; get;}

	public ChessPiece[,] ChessPieces{ set; get;}
	private ChessPiece selectedChessPiece;

	private const float TILE_SIZE = 1.0f;
	private const float TILE_OFFSET = 0.5f;

	private int selectionX = -1;
	private int selectionY = -1;

	public List<GameObject> chessPiecePrefabs;
	private List<GameObject> activeChessPiece = new List<GameObject>();

	private Quaternion orientation;

	public bool isWhiteTurn = true;

	private void Start()
	{
		SpawnAllChessPieces ();
	}
	
	// Update is called once per frame
	private void Update ()
	{
		UpdateSelection ();
		DrawChessboard ();

		if (Input.GetMouseButtonDown (0))
		{
			if (selectionX >= 0 && selectionY >= 0)
			{
				if(selectedChessPiece == null)
				{
					//Select the chess piece
					SelectChessPiece (selectionX, selectionY);
				}
				else
				{
					// Move the chess piece
					MoveChessPiece (selectionX, selectionY);
				}
			}
		}
	}

	private void SelectChessPiece(int x, int y)
	{
		if (ChessPieces [x, y] == null)
		{
			return;
		}
		if (ChessPieces[x,y].isWhite != isWhiteTurn)
		{
			return;
		}
		allowedMoves = ChessPieces [x, y].PossibleMove ();
		selectedChessPiece = ChessPieces [x, y];
		BoardHighlights.Instance.HighlightAllowedMoves (allowedMoves);
	}

	private void MoveChessPiece (int x, int y)
	{
		if (allowedMoves[x,y])
		{
			ChessPieces [selectedChessPiece.CurrentX, selectedChessPiece.CurrentY] = null;
			selectedChessPiece.transform.position = GetTileCenter (x, y);
			selectedChessPiece.SetPosition(x,y);
			ChessPieces [x,y] = selectedChessPiece;
			isWhiteTurn = !isWhiteTurn;
		}

		BoardHighlights.Instance.HideHighlights ();
		selectedChessPiece = null;
	}

	private void UpdateSelection()
	{
		if (!Camera.main)
		{
			return;
		}

		RaycastHit hit;
		if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, 25.0f, LayerMask.GetMask ("ChessPlane")))
		{
			selectionX = (int)hit.point.x;
			selectionY = (int)hit.point.z;
		}
		else
		{
			selectionX = -1;
			selectionY = -1;
		}
	}

	private void SpawnChessPiece(int index, int x, int y)
	{
		GameObject go = Instantiate (chessPiecePrefabs [index], GetTileCenter (x, y), orientation) as GameObject;
		go.transform.SetParent (transform);
		ChessPieces [x, y] = go.GetComponent<ChessPiece> ();
		ChessPieces [x, y].SetPosition (x, y);
		activeChessPiece.Add (go); 
	}

	private void SpawnAllChessPieces()
	{
		activeChessPiece = new List<GameObject> ();
		ChessPieces = new ChessPiece[8, 8];

		// SPAWN BLACK TEAM
		orientation = Quaternion.Euler (0, 90, 0);

		// King
		SpawnChessPiece (0,3,0);

		// Queen
		SpawnChessPiece (1,4,0);

		// Rooks
		SpawnChessPiece (2,0,0);
		SpawnChessPiece (2,7,0);

		// Bishops
		SpawnChessPiece (3,2,0);
		SpawnChessPiece (3,5,0);

		// Knights
		SpawnChessPiece (4,1,0);
		SpawnChessPiece (4,6,0);

		// Pawns
		for (int i = 0; i < 8; i++)
		{
			SpawnChessPiece (5,i,1);
		}

		// SPAWN WHITE TEAM
		orientation = Quaternion.Euler (0, 270, 0);

		// King
		SpawnChessPiece (6,4,7);

		// Queen
		SpawnChessPiece (7,3,7);

		// Rooks
		SpawnChessPiece (8,0,7);
		SpawnChessPiece (8,7,7);

		// Bishops
		SpawnChessPiece (9,2,7);
		SpawnChessPiece (9,5,7);

		// Knights
		SpawnChessPiece (10,1,7);
		SpawnChessPiece (10,6,7);

		// Pawns
		for (int i = 0; i < 8; i++)
		{
			SpawnChessPiece (11,i,6);
		}
	}

	private Vector3 GetTileCenter (int x, int y)
	{
		Vector3 origin = Vector3.zero;
		origin.x += (TILE_SIZE * x) + TILE_OFFSET;
		origin.z += (TILE_SIZE * y) + TILE_OFFSET;
		return origin;

	}

	private void DrawChessboard()
	{
		Vector3 widthLine = Vector3.right * 8;
		Vector3 heightLine = Vector3.forward * 8;

		for (int i = 0; i <= 8; i++)
		{
			Vector3 start = Vector3.forward * i;
			Debug.DrawLine (start, start + widthLine);
			for (int j = 0; j <= 8; j++)
			{
				start = Vector3.right * j;
				Debug.DrawLine (start, start + heightLine);
			}
		}

		// Draw the selection
		if (selectionX >= 0 && selectionY >= 0)
		{
			Debug.DrawLine (
				Vector3.forward * selectionY + Vector3.right * selectionX,
				Vector3.forward * (selectionY + 1) + Vector3.right * (selectionX + 1));
			Debug.DrawLine (
				Vector3.forward * (selectionY + 1) + Vector3.right * selectionX,
				Vector3.forward * selectionY + Vector3.right * (selectionX + 1));
		}
	}
}

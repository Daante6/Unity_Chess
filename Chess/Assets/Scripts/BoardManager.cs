using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BoardManager : MonoBehaviour
{
	public Camera camera1 = new Camera();
	public Transform whiteTransform;
	public Transform blackTransform;

    public static BoardManager Instance { set; get; }
	private bool[,] allowedMoves { set; get; }

    public Chessman[,] Chessmans { set; get; }
	private Chessman selectedChessman;

	private const float TILE_SIZE = 1.0f;
	private const float TILE_OFFSET = 0.5f;
	
	private int selectionX = -1;
	private int selectionY = -1;
	
	public List<GameObject> chessmanPrefabs;
	private List<GameObject> activeChessman;

	private Material previousMat;
	public Material selectedMat;

	public int[] EnPassantMove { set; get; }
	public int[] CastleWhiteLeft { set; get; }
	public int[] CastleBlackLeft { set; get; }
	public int[] CastleWhiteRight { set; get; }
	public int[] CastleBlackRight { set; get; }


	private Quaternion orientation = Quaternion.Euler(0,180,0);

	public bool isWhiteTurn = true;

	private void Start()
	{
		Instance = this;
		SpawnAllChessmans();
	}
	
	private void Update()
	{
		UpdateSelection();
		DrawChessBoard();
		
		if(Input.GetMouseButtonDown(0))
        {
			if(selectionX >= 0 && selectionY >= 0)
            {
				if(selectedChessman == null)
                {
					//select the chessman
					SelectChessman(selectionX,selectionY);
                }
                else
                {
					//Move the chessman
					MoveChessman(selectionX, selectionY);
                }
            }
        }

	}

	private void SelectChessman(int x, int y)
    {
		if (Chessmans[x, y] == null)
			return;

		if (Chessmans[x, y].isWhite != isWhiteTurn)
			return;

		bool hasAtLeastOneMove = false;

		allowedMoves = Chessmans[x, y].PossibleMove();

		for(int i=0; i<8;i++)
        {
			for(int j = 0 ; j < 8 ; j++)
            {
				if (allowedMoves [i, j])
					hasAtLeastOneMove = true;
            }
        }
		if (!hasAtLeastOneMove)
			return;

		selectedChessman = Chessmans[x, y];
		previousMat = selectedChessman.GetComponent<MeshRenderer>().material;
		selectedMat.mainTexture = previousMat.mainTexture;
		selectedChessman.GetComponent<MeshRenderer>().material = selectedMat;
		BoardHighlights.Instance.HighlightAllowedMoves(allowedMoves);
    }

	private void MoveChessman(int x, int y)
    {
		Chessman c1;
		Chessman c2;
		Chessman c3;

		if (allowedMoves[x,y] == true)
        {
			Chessman c = Chessmans[x, y];

			if(c != null && c.isWhite != isWhiteTurn)
            {
				//if king
				if(c.GetType() == typeof(King))
                {
					EndGame();
					return;
                }
				//capture a piece
				activeChessman.Remove(c.gameObject);
				Destroy(c.gameObject);
            }

			if(x == EnPassantMove[0] && y == EnPassantMove[1])
            {
				if(isWhiteTurn) //white turn
					c = Chessmans[x, y - 1];
                else //black turn
					c = Chessmans[x, y + 1];
				//DESTROY PIECE NOT WALKING ON IT
				activeChessman.Remove(c.gameObject);
				Destroy (c.gameObject);
			}
			EnPassantMove[0] = -1;
			EnPassantMove[1] = -1;
			//promotion and en passant move
			if(selectedChessman.GetType() == typeof(Pawn))
            {
				//promotion
				if(y == 7)
                {
					activeChessman.Remove(selectedChessman.gameObject);
					Destroy(selectedChessman.gameObject);
					SpawnChessman(4, x, y);
					selectedChessman = Chessmans[x, y];
				}
				else if(y == 0)
                {
					activeChessman.Remove(selectedChessman.gameObject);
					Destroy(selectedChessman.gameObject);
					SpawnChessman(10, x, y);
				}
				//en passant move
				if (selectedChessman.CurrentY == 1 && y == 3)
                {
					EnPassantMove[0] = x;
					EnPassantMove[1] = y - 1;
				}
				else if (selectedChessman.CurrentY == 6 && y == 4)
				{
					EnPassantMove[0] = x;
					EnPassantMove[1] = y + 1;
				}
			}
			if (selectedChessman.GetType() == typeof(King))
			{
				//CASTLE
				//white left castle
				if (selectedChessman.CurrentX == 4 && x == 2 && isWhiteTurn == true)
                {
					Chessmans[selectedChessman.CurrentX, selectedChessman.CurrentY] = null;
					selectedChessman.transform.position = GetTileCenter(x, y);
					selectedChessman.SetPosition(x, y);
					Chessmans[x, y] = selectedChessman;
					//isWhiteTurn = !isWhiteTurn;
					selectedChessman.GetComponent<MeshRenderer>().material = previousMat;
					BoardHighlights.Instance.HideHighlights();
					selectedChessman = null;
					selectedChessman = Chessmans[0, 0];
					MoveChessman(3, 0);
					return;
				}

				//white right castle
				if (selectedChessman.CurrentX == 4 && x == 6 && isWhiteTurn == true)
				{
					Chessmans[selectedChessman.CurrentX, selectedChessman.CurrentY] = null;
					selectedChessman.transform.position = GetTileCenter(x, y);
					selectedChessman.SetPosition(x, y);
					Chessmans[x, y] = selectedChessman;
					//isWhiteTurn = !isWhiteTurn;
					selectedChessman.GetComponent<MeshRenderer>().material = previousMat;
					BoardHighlights.Instance.HideHighlights();
					selectedChessman = null;
					selectedChessman = Chessmans[7, 0];
					MoveChessman(5, 0);
					return;
				}
				//black left castle
				if (selectedChessman.CurrentX == 4 && x == 2 && isWhiteTurn == false)
				{
					Chessmans[selectedChessman.CurrentX, selectedChessman.CurrentY] = null;
					selectedChessman.transform.position = GetTileCenter(x, y);
					selectedChessman.SetPosition(x, y);
					Chessmans[x, y] = selectedChessman;
					//isWhiteTurn = !isWhiteTurn;
					selectedChessman.GetComponent<MeshRenderer>().material = previousMat;
					BoardHighlights.Instance.HideHighlights();
					selectedChessman = null;
					selectedChessman = Chessmans[0, 7];
					MoveChessman(3, 7);
					return;
				}
				//black right castle
				if (selectedChessman.CurrentX == 4 && x == 6 && isWhiteTurn == false)
				{
					Chessmans[selectedChessman.CurrentX, selectedChessman.CurrentY] = null;
					selectedChessman.transform.position = GetTileCenter(x, y);
					selectedChessman.SetPosition(x, y);
					Chessmans[x, y] = selectedChessman;
					//isWhiteTurn = !isWhiteTurn;
					selectedChessman.GetComponent<MeshRenderer>().material = previousMat;
					BoardHighlights.Instance.HideHighlights();
					selectedChessman = null;
					selectedChessman = Chessmans[7, 7];
					MoveChessman(5, 7);
					return;
				}
			}

			Chessmans[selectedChessman.CurrentX, selectedChessman.CurrentY] = null;
			selectedChessman.transform.position = GetTileCenter(x, y);
			selectedChessman.SetPosition(x, y);
			Chessmans[x, y] = selectedChessman;
			changeCamera(isWhiteTurn);
			isWhiteTurn = !isWhiteTurn;

			//check castle
			//white left
			c1 = Chessmans[1, 0];
			c2 = Chessmans[2, 0];
			c3 = Chessmans[3, 0];
			if(c1 == null && c2 == null && c3 == null)
            {
				CastleWhiteLeft[0] = 2;
				CastleWhiteLeft[1] = 0;
            }
			//white right
			c1 = Chessmans[5, 0];
			c2 = Chessmans[6, 0];
			if (c1 == null && c2 == null)
			{
				CastleWhiteRight[0] = 6;
				CastleWhiteRight[1] = 0;
			}
			//black left
			c1 = Chessmans[1, 7];
			c2 = Chessmans[2, 7];
			c3 = Chessmans[3, 7];
			if (c1 == null && c2 == null && c3 == null)
			{
				CastleBlackLeft[0] = 2;
				CastleBlackLeft[1] = 7;
			}
			//black right
			c1 = Chessmans[5, 7];
			c2 = Chessmans[6, 7];
			if (c1 == null && c2 == null)
			{
				CastleBlackRight[0] = 6;
				CastleBlackRight[1] = 7;
			}
		}

		selectedChessman.GetComponent<MeshRenderer>().material = previousMat;
		BoardHighlights.Instance.HideHighlights();
		selectedChessman = null;

		

	}

	private void DrawChessBoard()
	{
		Vector3 widthLine = Vector3.right * 8;
		Vector3 heightLine = Vector3.forward * 8;
		
		for(int i=0;i <= 8;i++)
		{
			Vector3 start = Vector3.forward * i;
			Debug.DrawLine (start, start + widthLine);
			for(int j=0;j <= 8;j++)
			{
				start = Vector3.right * j;
				Debug.DrawLine (start, start + heightLine);
			}
		}
		
		if(selectionX >= 0 && selectionY >= 0){
			Debug.DrawLine(Vector3.forward * selectionY + Vector3.right * selectionX, Vector3.forward * (selectionY+1) + Vector3.right * (selectionX+1));
			Debug.DrawLine(Vector3.forward * (selectionY+1) + Vector3.right * selectionX , Vector3.forward * selectionY + Vector3.right * (selectionX+1));
		}
	}
	
	private void UpdateSelection()
	{
		if(!Camera.main){
			return;
		}
		RaycastHit hit;
		if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hit,25.0f,LayerMask.GetMask("ChessPlane")))
		{
			selectionX = (int)hit.point.x;
			selectionY = (int)hit.point.z;
		}else{
			selectionX = -1;
			selectionY = -1;
		}
	}
	
	private void SpawnAllChessmans()
	{
		activeChessman = new List<GameObject>();
		Chessmans = new Chessman[8, 8];
		EnPassantMove = new int[2] { -1, -1 };
		CastleWhiteLeft = new int[2] { -1, -1 };
		CastleWhiteRight = new int[2] { -1, -1 };
		CastleBlackRight = new int[2] { -1, -1 };
		CastleBlackLeft = new int[2] { -1, -1 };

        //white team
        switch (StaticNameController.fractionWhite)
        {
			case 0:
				for (int i = 0; i < 8; i++)
				{
					SpawnChessman(0, i, 1);
				}
				SpawnChessman(1, 0, 0);   //WHO, X, Y
				SpawnChessman(1, 7, 0);
				SpawnChessman(2, 1, 0);
				SpawnChessman(2, 6, 0);
				SpawnChessman(3, 2, 0);
				SpawnChessman(3, 5, 0);
				SpawnChessman(4, 3, 0); //queenW
				SpawnChessman(5, 4, 0); //kingW
				
				break;
			case 1:
				for (int i = 0; i < 8; i++)
				{
					SpawnChessman(0, i, 1);
				}
				SpawnChessman(1, 0, 0);   //WHO, X, Y
				SpawnChessman(1, 7, 0);
				SpawnChessman(3, 1, 0);
				SpawnChessman(3, 6, 0);
				SpawnChessman(2, 2, 0);
				SpawnChessman(2, 5, 0);
				SpawnChessman(4, 3, 0); //queenW
				SpawnChessman(5, 4, 0); //kingW
				break;
			case 2:
				for (int i = 0; i < 8; i++)
				{
					SpawnChessman(0, i, 1);
				}
				SpawnChessman(1, 0, 0);   //WHO, X, Y
				SpawnChessman(1, 7, 0);
				SpawnChessman(2, 1, 0);
				SpawnChessman(2, 6, 0);
				SpawnChessman(3, 2, 0);
				SpawnChessman(3, 5, 0);
				SpawnChessman(4, 3, 0); //queenW
				SpawnChessman(5, 4, 0); //kingW
				break;
        }
		//black team
		switch (StaticNameController.fractionBlack)
        {
			case 0:
				for (int i = 0; i < 8; i++)
				{
					SpawnChessman(6, i, 6);
				}
				SpawnChessman(7, 0, 7);
				SpawnChessman(7, 7, 7);
				SpawnChessman(8, 1, 7);
				SpawnChessman(8, 6, 7);
				SpawnChessman(9, 2, 7);
				SpawnChessman(9, 5, 7);
				SpawnChessman(10, 3, 7);
				SpawnChessman(11, 4, 7);
				break;
			case 1:
				for (int i = 0; i < 8; i++)
				{
					SpawnChessman(6, i, 6);
				}
				SpawnChessman(7, 0, 7);
				SpawnChessman(7, 7, 7);
				SpawnChessman(8, 1, 7);
				SpawnChessman(8, 6, 7);
				SpawnChessman(9, 2, 7);
				SpawnChessman(9, 5, 7);
				SpawnChessman(10, 3, 7);
				SpawnChessman(11, 4, 7);
				break;
			case 2:
				for (int i = 0; i < 8; i++)
				{
					SpawnChessman(6, i, 6);
				}
				SpawnChessman(7, 0, 7);
				SpawnChessman(7, 7, 7);
				SpawnChessman(8, 1, 7);
				SpawnChessman(8, 6, 7);
				SpawnChessman(9, 2, 7);
				SpawnChessman(9, 5, 7);
				SpawnChessman(10, 3, 7);
				SpawnChessman(11, 4, 7);
				break;
        }
	}
	
	private Vector3 GetTileCenter(int x, int y)
	{
		Vector3 origin = Vector3.zero;
		origin.x += (TILE_SIZE*x) + TILE_OFFSET;
		origin.z += (TILE_SIZE*y) + TILE_OFFSET;
		return origin;
	}
	
	private void SpawnChessman(int index, int x, int y)
	{
		GameObject go = Instantiate(chessmanPrefabs[index],GetTileCenter(x,y),orientation) as GameObject;
		go.transform.SetParent (transform);
		Chessmans[x, y] = go.GetComponent<Chessman>();
		Chessmans[x, y].SetPosition(x, y);
		activeChessman.Add(go);
		if (Chessmans[x,y].isWhite)
		{
			go.transform.Rotate(0.0f, 180.0f, 0.0f);
		}
	}
	
	private void EndGame()
    {
		if (isWhiteTurn)
        {
			StaticNameController.Winner = "White";
		}

        else
        {
			StaticNameController.Winner = "Black";
		}

		StaticNameController.Winner += " wins!";

		foreach (GameObject go in activeChessman)
			Destroy(go);

		isWhiteTurn = true;
		BoardHighlights.Instance.HideHighlights();
		SceneManager.LoadScene("Menu");
	}
	public void changeCamera(bool turn)
    {
        if (turn)
        {
			Camera.main.transform.position = blackTransform.position;
			Camera.main.transform.rotation = blackTransform.rotation;
			//camera1.transform.position = blackTransform.position;
			//camera1.transform.rotation = blackTransform.rotation;
		}
        else
        {
			Camera.main.transform.position = whiteTransform.position;
			Camera.main.transform.rotation = whiteTransform.rotation;
			//camera1.transform.position = whiteTransform.position;
			//camera1.transform.rotation = whiteTransform.rotation;
		}
    }
}

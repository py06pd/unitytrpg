using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : StateMachine
{
	public CameraRig cameraRig;
	public Board board;
	public LevelData levelData;
	public Transform tileSelectionIndictor;
	public Point position;
	
	void Start()
	{
		ChangeStart<InitBattleState>();
	}
}

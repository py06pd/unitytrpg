﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class BoardCreator : MonoBehaviour
{
	[SerializeField]
	GameObject tileView;
	
	[SerializeField]
	GameObject tileSelectionIndicator;
	
	[SerializeField]
	int width = 10;
	
	[SerializeField]
	int depth = 10;
	
	[SerializeField]
	int height = 8;
	
	[SerializeField]
	Point position;
	
	[SerializeField]
	LevelData levelData;
	
	Transform _marker;
	Dictionary<Point, Tile> tiles = new Dictionary<Point, Tile>();
	
	Transform marker
	{
		get
		{
			if (_marker == null)
			{
				GameObject instance = Instantiate(tileSelectionIndicator) as GameObject;
				_marker = instance.transform;
			}
			
			return _marker;
		}
	}
	
	public void GrowArea()
	{
		Rect r = RandomRect();
		GrowRect(r);
	}
	
	public void ShrinkArea()
	{
		Rect r = RandomRect();
		ShrinkRect(r);
	}
	
	Rect RandomRect()
	{
		int x = UnityEngine.Random.Range(0, width);
		int y = UnityEngine.Random.Range(0, depth);
		int w = UnityEngine.Random.Range(1, width - x + 1);
		int h = UnityEngine.Random.Range(1, depth - y + 1);
		
		return new Rect(x, y, w, h);
	}
	
	void GrowRect(Rect rect)
	{
		for (int y = (int)rect.yMin; y < (int)rect.yMax; ++y)
		{
			for (int x = (int)rect.xMin; x < (int)rect.xMax; ++x)
			{
				Point a = new Point(x, y);
				GrowSingle(a);
			}
		}
	}
	
	void ShrinkRect(Rect rect)
	{
		for (int y = (int)rect.yMin; y < (int)rect.yMax; ++y)
		{
			for (int x = (int)rect.xMin; x < (int)rect.xMax; ++x)
			{
				Point a = new Point(x, y);
				ShrinkSingle(a);
			}
		}
	}
	
	Tile Create()
	{
		GameObject instance = Instantiate(tileView) as GameObject;
		instance.transform.parent = transform;
		return instance.GetComponent<Tile>();
	}
	
	Tile GetOrCreate(Point a)
	{
		if (tiles.ContainsKey(a)) {
			return tiles[a];
		}
		
		Tile t = Create();
		t.Load(a, 0);
		tiles.Add(a, t);
		
		return t;
	}
	
	void GrowSingle(Point a)
	{
		Tile t = GetOrCreate(a);
		if (t.height < height) {
			t.Grow();
		}
	}
	
	void ShrinkSingle(Point a)
	{
		if (!tiles.ContainsKey(a)) {
			return;
		}
		
		Tile t = tiles[a];
		t.Shrink();
		
		if (t.height <= 0) {
			tiles.Remove(a);
			DestroyImmediate(t.gameObject);
		}
	}
	
	public void Grow()
	{
		GrowSingle(position);
	}
	
	public void Shrink()
	{
		ShrinkSingle(position);
	}
	
	public void UpdateMarker()
	{
		Tile t = tiles.ContainsKey(position) ? tiles[position] : null;
		marker.localPosition = t != null ? t.center : new Vector3(position.x, 0, position.y);
	}
	
	public void Clear()
	{
		for (int i = transform.childCount - 1; i >= 0; --i) {
			DestroyImmediate(transform.GetChild(i).gameObject);
		}
		
		tiles.Clear();
	}
	
	public void Save()
	{
		string filePath = Application.dataPath + "/Resources/Levels";
		if (!Directory.Exists(filePath)) {
			CreateSaveDirectory();
		}
		
		LevelData board = ScriptableObject.CreateInstance<LevelData>();
		board.tiles = new List<Vector3>(tiles.Count);
		foreach (Tile t in tiles.Values) {
			board.tiles.Add(new Vector3(t.position.x, t.height, t.position.y));
		}
		
		string fileName = string.Format("Assets/Resources/Levels/{1}.asset", filePath, name);
		AssetDatabase.CreateAsset(board, fileName);
	}
	
	void CreateSaveDirectory()
	{
		string filePath = Application.dataPath + "/Resources";
		if (!Directory.Exists(filePath)) {
			AssetDatabase.CreateFolder("Assets", "Resources");
		}
		
		filePath += "/Levels";
		if (!Directory.Exists(filePath)) {
			AssetDatabase.CreateFolder("Assets/Resources", "Levels");
		}
		
		AssetDatabase.Refresh();
	}
	
	public void Load()
	{
		Clear();
		if (levelData == null) {
			return;
		}
		
		foreach (Vector3 v in levelData.tiles) {
			Tile t = Create();
			t.Load(v);
			tiles.Add(t.position, t);
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
	public const float stepHeight = 0.25f;
	public Point position;
	public int height;
	
	public Vector3 center
	{
		get { return new Vector3(position.x, height * stepHeight, position.y); }
	}
	
	void Match ()
	{
		transform.localPosition = new Vector3(position.x, height * stepHeight / 2f, position.y);
		transform.localScale = new Vector3(1, height * stepHeight, 1);
	}

	public void Grow ()
	{
		height++;
		Match();
	}
	
	public void Shrink ()
	{
		height--;
		Match();
	}
	
	public void Load(Point a, int height)
	{
		this.position = a;
		this.height = height;
		Match();
	}
	
	public void Load(Vector3 v)
	{
		Load (new Point((int)v.x, (int)v.z), (int)v.y);
	}
}

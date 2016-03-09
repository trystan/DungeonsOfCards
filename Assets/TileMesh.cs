using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public interface ITileMeshSource {
	int Width { get; }
	int Height { get; }
	int GetTileIndex(int x, int y);
	bool HasChangedSinceLastRender { get; set; }
}

public class TileMesh : MonoBehaviour {
	public MeshRenderer meshRenderer;
	public MeshFilter meshFilter;
	public int viewWidth;
	public int viewHeight;
	public bool renderEverything = true;
	public Texture2D terrainTiles;
	public int terrainTileSize = 32;

	private int previousTerrainTileSize;
	private int levelWidth;
	private int levelHeight;
	private int lastX = -1;
	private int lastY = -1;
	
	ITileMeshSource currentLevel;
	int[,] imageIndex;
	
	void Start() {
		meshRenderer.material.mainTexture = terrainTiles;
	}
	
	public void ShowLevel(ITileMeshSource level) {
		currentLevel = level;
		levelWidth = level.Width;
		levelHeight = level.Height;
		BuildMesh();
		Retexture(0,0);
	}
	
	void Update() {
		if (currentLevel == null)
			return;
		
		var x = (int)Camera.main.transform.position.x;
		var y = (int)Camera.main.transform.position.y;
		
		if (currentLevel.HasChangedSinceLastRender || !renderEverything && x != lastX && y != lastY) {
			if (currentLevel.Width != levelWidth || currentLevel.Height != levelHeight 
			    	|| previousTerrainTileSize != terrainTileSize) {
				levelWidth = currentLevel.Width;
				levelHeight = currentLevel.Height;
				previousTerrainTileSize = terrainTileSize;
				BuildMesh();
			}

			Retexture(x, y);
			
			lastX = x;
			lastY = y;
		}
	}
	
	void BuildMesh() {
		var vertices = new List<Vector3>();
		var normals = new List<Vector3>();
		var uv = new List<Vector2>();
		
		var triangles = new List<int>();
		var width = renderEverything ? levelWidth : viewWidth;
		var height = renderEverything ? levelHeight : viewHeight;
		
		for (var y = 0; y < height; y++)
		for (var x = 0; x < width; x++) {
			var vertexCount = vertices.Count;
			
			vertices.Add(new Vector3(x,y,0));
			vertices.Add(new Vector3(x+1,y,0));
			vertices.Add(new Vector3(x+1,y+1,0));
			vertices.Add(new Vector3(x,y+1,0));
			
			normals.Add(Vector3.up);
			normals.Add(Vector3.up);
			normals.Add(Vector3.up);
			normals.Add(Vector3.up);
			
			uv.Add(new Vector2((float)x / width, (float)y / height));
			uv.Add(new Vector2((float)(x+1) / width, (float)y / height));
			uv.Add(new Vector2((float)(x+1) / width, (float)(y+1) / height));
			uv.Add(new Vector2((float)x / width, (float)(y+1) / height));
			
			triangles.Add(vertexCount + 2);
			triangles.Add(vertexCount + 1);
			triangles.Add(vertexCount + 0);
			
			triangles.Add(vertexCount + 0);
			triangles.Add(vertexCount + 3);
			triangles.Add(vertexCount + 2);
		}
		
		var mesh = new Mesh();
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.normals = normals.ToArray();
		mesh.uv = uv.ToArray();
		
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		mesh.Optimize();
		
		meshFilter.mesh = mesh;
	}
	
	void Retexture(int x, int y) {
		var width = renderEverything ? levelWidth : viewWidth;
		var height = renderEverything ? levelHeight : viewHeight;
		
		if (renderEverything) {
			x = width / 2;
			y = height / 2;
		}
		
		transform.position = new Vector3(x - width / 2 - 0.5f, y - height / 2 - 0.5f, transform.position.z);
		
		var widthInTiles = terrainTiles.width / terrainTileSize;
		var heightInTiles = terrainTiles.height / terrainTileSize;
		var widthOfTile = 1f / widthInTiles;
		var heightOfTile = 1f / heightInTiles;
		var uvs = meshFilter.mesh.uv;
		
		var fudgeFactor = 0.0004f;
		
		for (var ix = 0; ix < width; ix++)
		for (var iy = 0; iy < height; iy++) {
			var tileX = x - width / 2 + ix;
			var tileY = y - height / 2 + iy;
			
			var textureIndex = -1;
			
			if (tileX >= 0 && tileY >= 0 && tileX < levelWidth && tileY < levelHeight)
				textureIndex = currentLevel.GetTileIndex(tileX, tileY);
			
			if (textureIndex < 0) {
				var i = (iy * width + ix) * 4;
				uvs[i + 0] = Vector2.zero;
				uvs[i + 1] = Vector2.zero;
				uvs[i + 2] = Vector2.zero;
				uvs[i + 3] = Vector2.zero;
			} else {
				var textureX = textureIndex % widthInTiles;
				var textureY = heightInTiles - (textureIndex / widthInTiles);
				var i = (iy * width + ix) * 4;
				var left = textureX * widthOfTile + fudgeFactor;
				var right = (textureX + 1) * widthOfTile - fudgeFactor;
				var top = (textureY - 1) * heightOfTile + fudgeFactor;
				var bottom = textureY * heightOfTile - fudgeFactor;
				uvs[i + 0] = new Vector2(left, top);
				uvs[i + 1] = new Vector2(right, top);
				uvs[i + 2] = new Vector2(right, bottom);
				uvs[i + 3] = new Vector2(left, bottom);
			}
		}
		meshFilter.mesh.uv = uvs;
		currentLevel.HasChangedSinceLastRender = false;
	}
}

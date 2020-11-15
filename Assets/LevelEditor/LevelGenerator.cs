using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEditor;
public class LevelGenerator : MonoBehaviour
{
	[Header("Base Map")]
	public string levelName;
	public bool saveLevel;
	private string mapPath;
	public Texture2D mapTexture;
	public GameObject floorPrefab;
	private Color pixelColor;
	public PixelToObject[] pixelColorMappings;          //contains mappings for level layer
	[Header("Detail Map")]
	public Texture2D decorationLayer;
	public PixelToObject[] decorationColorMappings;
	[Header("Normal Map Generation")]
	public bool generateNormalMaps;
	public Texture2D defaultNormalMap;
	public Material defaultMaterial;
	private string materialFolderPath;
	private List<KeyValuePair<Vector2, GameObject>> floorTiles;
	[Header("Path Finding")]
	public GameObject pathFindingNode;
	public List<GameObject> nodeList;
	void Start()
	{
		if (!ValidateName())
			return;
		if (generateNormalMaps)
			generateNormalMaps = NormalMapInit();
		GenerateLevel();
		if(generateNormalMaps)
			GenerateNormals();
		if (saveLevel)
		{
			GameObject saver = new GameObject();
			saver.AddComponent<LevelSaver>().SaveLevel(gameObject, mapPath);
		}
	}

	void GenerateLevel()
	{
		// Scan whole Texture and get positions of objects
		for (int i = 0; i < mapTexture.width; i++)
		{
			for (int j = 0; j < mapTexture.height; j++)
			{
				GenerateObject(i, j, false);
			}
		}

		if (decorationLayer)
		{
			for (int i = 0; i < decorationLayer.width; i++)
			{
				for (int j = 0; j < decorationLayer.height; j++)
				{
					GenerateObject(i, j, true);
				}
			}
		}
	}

	private bool ValidateName()
	{
		name = levelName;
		mapPath = "Assets/LevelPrefabs/" + name + ".prefab";
		if (levelName == "")
		{
			Debug.LogError("Level name invalid, map not generated");
			return false;
		}
		GameObject map = (AssetDatabase.LoadAssetAtPath<GameObject>(mapPath));
		if (map != null)
		{
			Debug.LogError("No map generated, prefab already exists for map with name \"" + levelName + "\"");
			Debug.Log("Loaded saved map");
			Instantiate(map);
			return false;
		}
		return true;
	}

	void GenerateObject(int x, int y, bool decoration)
	{
		if (!decoration)
		{
			#region level layer prefabs
			// Read pixel color
			pixelColor = mapTexture.GetPixel(x, y);

			#region Color Glossary
			// black = wall
			// white = floor
			// yellow = player
			// red = enemy
			// teal = exit
			// green = rails
			#endregion

			foreach (PixelToObject pixelColorMapping in pixelColorMappings)
			{
				// Scan pixelColorMappings Array for matching color maping
				if (pixelColorMapping.pixelColor.Equals(pixelColor))
				{
					if (pixelColorMapping.prefab.Length == 1)
					{
						Vector2 position = new Vector2(x, y);

						GameObject floor = Instantiate(floorPrefab, position, Quaternion.identity, transform);     //if not a wall, floor needs to be 
																										//spawned in addition to other objects
						if (generateNormalMaps) //add tile to data structure to create normals later
							floorTiles.Add(new KeyValuePair<Vector2, GameObject>(position, floor));

						if (pixelColorMapping.prefab[0])
							Instantiate(pixelColorMapping.prefab[0], position, Quaternion.identity, transform); //spawning any other object
					}
					else    //for deciding which wall type to create
					{
						#region Picking Correct Wall Tile
						if (pixelColorMapping.pixelColor.Equals(Color.black))
						{
							Color rightPixel = Color.black;   //pixel to right of current
							Color topPixel = Color.black;     //pixel above current
							Color leftPixel = Color.black;    //pixel to left of current
							Color botPixel = Color.black;     //pixel below current

							Vector2 position = new Vector2(x, y);

							if (x < mapTexture.width)
								rightPixel = mapTexture.GetPixel(x + 1, y);
							if (y < mapTexture.height)
								topPixel = mapTexture.GetPixel(x, y + 1);
							if (x > 0)
								leftPixel = mapTexture.GetPixel(x - 1, y);
							if (y > 0)
								botPixel = mapTexture.GetPixel(x, y - 1);

							// 0 = oneSided
							// 1 = twoSidedCorner
							// 2 = blank

							if (rightPixel != Color.black && topPixel != Color.black)
							{
								Instantiate(pixelColorMapping.prefab[1], position, Quaternion.identity, transform).transform.Rotate(new Vector3(0.0f, 0.0f, 0.0f));
								nodeList.Add(Instantiate(pathFindingNode, new Vector2(x + 1, y + 1), Quaternion.identity, transform));	//adds pathfinding node on corner
							}
							else if (topPixel != Color.black && leftPixel != Color.black)
							{
								Instantiate(pixelColorMapping.prefab[1], position, Quaternion.identity, transform).transform.Rotate(new Vector3(0.0f, 0.0f, 90.0f));
								nodeList.Add(Instantiate(pathFindingNode, new Vector2(x - 1, y + 1), Quaternion.identity, transform));  //adds pathfinding node on corner
							}
							else if (leftPixel != Color.black && botPixel != Color.black)
							{
								Instantiate(pixelColorMapping.prefab[1], position, Quaternion.identity, transform).transform.Rotate(new Vector3(0.0f, 0.0f, 180.0f));
								nodeList.Add(Instantiate(pathFindingNode, new Vector2(x - 1, y - 1), Quaternion.identity, transform));  //adds pathfinding node on corner
							}
							else if (botPixel != Color.black && rightPixel != Color.black)
							{
								Instantiate(pixelColorMapping.prefab[1], position, Quaternion.identity, transform).transform.Rotate(new Vector3(0.0f, 0.0f, 270.0f));
								nodeList.Add(Instantiate(pathFindingNode, new Vector2(x + 1, y - 1), Quaternion.identity, transform));  //adds pathfinding node on corner
							}
							else if (rightPixel != Color.black)
							{
								Instantiate(pixelColorMapping.prefab[0], position, Quaternion.identity, transform).transform.Rotate(new Vector3(0.0f, 0.0f, 0.0f));
							}
							else if (topPixel != Color.black)
							{
								Instantiate(pixelColorMapping.prefab[0], position, Quaternion.identity, transform).transform.Rotate(new Vector3(0.0f, 0.0f, 90.0f));
							}
							else if (leftPixel != Color.black)
							{
								Instantiate(pixelColorMapping.prefab[0], position, Quaternion.identity, transform).transform.Rotate(new Vector3(0.0f, 0.0f, 180.0f));
							}
							else if (botPixel != Color.black)
							{
								Instantiate(pixelColorMapping.prefab[0], position, Quaternion.identity, transform).transform.Rotate(new Vector3(0.0f, 0.0f, 270.0f));
							}
							else
							{
								Instantiate(pixelColorMapping.prefab[2], position, Quaternion.identity, transform);
							}
						}
						#endregion
						#region Picking correct rail tile
						if (pixelColorMapping.pixelColor.Equals(Color.green))
						{
							Color rightPixel = Color.white;   //pixel to right of current
							Color topPixel = Color.white;     //pixel above current
							Color leftPixel = Color.white;    //pixel to left of current
							Color botPixel = Color.white;     //pixel below current

							Vector2 position = new Vector2(x, y);

							if (x < mapTexture.width)
								rightPixel = mapTexture.GetPixel(x + 1, y);
							if (y < mapTexture.height)
								topPixel = mapTexture.GetPixel(x, y + 1);
							if (x > 0)
								leftPixel = mapTexture.GetPixel(x - 1, y);
							if (y > 0)
								botPixel = mapTexture.GetPixel(x, y - 1);


							// 0 = straightRail
							// 1 = cornerRail
							// 2 = tRail
							// 3 = xRail

							//xRail
							if (rightPixel == Color.green && topPixel == Color.green && leftPixel == Color.green && botPixel == Color.green)
							{
								Instantiate(pixelColorMapping.prefab[3], position, Quaternion.identity, transform);
							}
							//tRails
							else if (botPixel == Color.green && rightPixel == Color.green && topPixel == Color.green)
							{
								Instantiate(pixelColorMapping.prefab[2], position, Quaternion.identity, transform).transform.Rotate(new Vector3(0.0f, 0.0f, 0.0f));
							}
							else if (rightPixel == Color.green && topPixel == Color.green && leftPixel == Color.green)
							{
								Instantiate(pixelColorMapping.prefab[2], position, Quaternion.identity, transform).transform.Rotate(new Vector3(0.0f, 0.0f, 90.0f));
							}
							else if (topPixel == Color.green && leftPixel == Color.green && botPixel == Color.green)
							{
								Instantiate(pixelColorMapping.prefab[2], position, Quaternion.identity, transform).transform.Rotate(new Vector3(0.0f, 0.0f, 180.0f));
							}
							else if (leftPixel == Color.green && botPixel == Color.green && rightPixel == Color.green)
							{
								Instantiate(pixelColorMapping.prefab[2], position, Quaternion.identity, transform).transform.Rotate(new Vector3(0.0f, 0.0f, 270.0f));
							}
							//corner Rails
							else if (botPixel == Color.green && rightPixel == Color.green)
							{
								Instantiate(pixelColorMapping.prefab[1], position, Quaternion.identity, transform).transform.Rotate(new Vector3(0.0f, 0.0f, 0.0f));
							}
							else if (rightPixel == Color.green && topPixel == Color.green)
							{
								Instantiate(pixelColorMapping.prefab[1], position, Quaternion.identity, transform).transform.Rotate(new Vector3(0.0f, 0.0f, 90.0f));
							}
							else if (topPixel == Color.green && leftPixel == Color.green)
							{
								Instantiate(pixelColorMapping.prefab[1], position, Quaternion.identity, transform).transform.Rotate(new Vector3(0.0f, 0.0f, 180.0f));
							}
							else if (leftPixel == Color.green && botPixel == Color.green)
							{
								Instantiate(pixelColorMapping.prefab[1], position, Quaternion.identity, transform).transform.Rotate(new Vector3(0.0f, 0.0f, 270.0f));
							}
							//straight rails
							else if (botPixel == Color.green || topPixel == Color.green)
							{
								Instantiate(pixelColorMapping.prefab[0], position, Quaternion.identity, transform).transform.Rotate(new Vector3(0.0f, 0.0f, 0.0f));
							}
							else if (rightPixel == Color.green || leftPixel == Color.green)
							{
								Instantiate(pixelColorMapping.prefab[0], position, Quaternion.identity, transform).transform.Rotate(new Vector3(0.0f, 0.0f, 90.0f));
							}
						}
						#endregion
					}
				}
			}
			#endregion
		}
		else
		{
			#region decoration layer prefabs
			// Read pixel color
			pixelColor = decorationLayer.GetPixel(x, y);

			#region Color Glossary
			// black = wires
			#endregion

			foreach (PixelToObject pixelColorMapping in decorationColorMappings)
			{
				if (pixelColorMapping.pixelColor.Equals(pixelColor))
				{
					#region Picking correct wire tile
					Color rightPixel = Color.white;   //pixel to right of current
					Color topPixel = Color.white;     //pixel above current
					Color leftPixel = Color.white;    //pixel to left of current
					Color botPixel = Color.white;     //pixel below current

					Vector2 position = new Vector2(x, y);

					if (x < mapTexture.width)
						rightPixel = decorationLayer.GetPixel(x + 1, y);
					if (y < mapTexture.height)
						topPixel = decorationLayer.GetPixel(x, y + 1);
					if (x > 0)
						leftPixel = decorationLayer.GetPixel(x - 1, y);
					if (y > 0)
						botPixel = decorationLayer.GetPixel(x, y - 1);


					// 0 = straightWire
					// 1 = cornerWire
					// 2 = tWire
					// 3 = xWire

					//xRail
					if (rightPixel == Color.black && topPixel == Color.black && leftPixel == Color.black && botPixel == Color.black)
					{
						Instantiate(pixelColorMapping.prefab[3], position, Quaternion.identity, transform);
					}
					//tRails
					else if (botPixel == Color.black && rightPixel == Color.black && topPixel == Color.black)
					{
						Instantiate(pixelColorMapping.prefab[2], position, Quaternion.identity, transform).transform.Rotate(new Vector3(0.0f, 0.0f, 0.0f));
					}
					else if (rightPixel == Color.black && topPixel == Color.black && leftPixel == Color.black)
					{
						Instantiate(pixelColorMapping.prefab[2], position, Quaternion.identity, transform).transform.Rotate(new Vector3(0.0f, 0.0f, 90.0f));
					}
					else if (topPixel == Color.black && leftPixel == Color.black && botPixel == Color.black)
					{
						Instantiate(pixelColorMapping.prefab[2], position, Quaternion.identity, transform).transform.Rotate(new Vector3(0.0f, 0.0f, 180.0f));
					}
					else if (leftPixel == Color.black && botPixel == Color.black && rightPixel == Color.black)
					{
						Instantiate(pixelColorMapping.prefab[2], position, Quaternion.identity, transform).transform.Rotate(new Vector3(0.0f, 0.0f, 270.0f));
					}
					//corner Rails
					else if (botPixel == Color.black && rightPixel == Color.black)
					{
						Instantiate(pixelColorMapping.prefab[1], position, Quaternion.identity, transform).transform.Rotate(new Vector3(0.0f, 0.0f, 0.0f));
					}
					else if (rightPixel == Color.black && topPixel == Color.black)
					{
						Instantiate(pixelColorMapping.prefab[1], position, Quaternion.identity, transform).transform.Rotate(new Vector3(0.0f, 0.0f, 90.0f));
					}
					else if (topPixel == Color.black && leftPixel == Color.black)
					{
						Instantiate(pixelColorMapping.prefab[1], position, Quaternion.identity, transform).transform.Rotate(new Vector3(0.0f, 0.0f, 180.0f));
					}
					else if (leftPixel == Color.black && botPixel == Color.black)
					{
						Instantiate(pixelColorMapping.prefab[1], position, Quaternion.identity, transform).transform.Rotate(new Vector3(0.0f, 0.0f, 270.0f));
					}
					//straight rails
					else if (botPixel == Color.black || topPixel == Color.black)
					{
						Instantiate(pixelColorMapping.prefab[0], position, Quaternion.identity, transform).transform.Rotate(new Vector3(0.0f, 0.0f, 0.0f));
					}
					else if (rightPixel == Color.black || leftPixel == Color.black)
					{
						Instantiate(pixelColorMapping.prefab[0], position, Quaternion.identity, transform).transform.Rotate(new Vector3(0.0f, 0.0f, 90.0f));
					}
					#endregion
				}
			}
			#endregion
		}
	}

	//returns false if errors were encountered
	bool NormalMapInit()
	{
		floorTiles = new List<KeyValuePair<Vector2, GameObject>>();
		materialFolderPath = "Assets/LevelPrefabs/Materials";
		try
		{
			if (AssetDatabase.IsValidFolder(materialFolderPath + '/' + name))
			{
				Debug.LogError("No normals generated, normals already exist for map \"" + name + "\"");
				return false;
			}
			string id = AssetDatabase.CreateFolder(materialFolderPath, name);
			materialFolderPath = AssetDatabase.GUIDToAssetPath(id);
			if(materialFolderPath == "")
			{
				Debug.LogError("Invalid Path");
				return false;
			}

			return true;
		}
		catch(System.Exception e)
		{
			Debug.LogError(e.Message);
			return false;
		}
	}

	void GenerateNormals()
	{
		Vector2 NoiseTwoOffset = new Vector2((Random.value - .5f) * 10, (Random.value - .5f) * 10);
		Vector2 NoiseThreeOffset = new Vector2((Random.value - .5f) * 10, (Random.value - .5f) * 10);
		Vector2 NoiseFourOffset = new Vector2((Random.value - .5f) * 10, (Random.value - .5f) * 10);
		Color[] normals = null;

		//create new assets
		void InitMaterials(Vector2 pos)
		{
			string assetName = '/' + name + pos.x.ToString() + '_' + pos.y.ToString();
			AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(defaultMaterial), materialFolderPath + assetName + ".mat");
		}
		Texture2D CreateNormalMap(Vector2 pos)
		{
			Texture2D normal = new Texture2D(defaultNormalMap.width, defaultNormalMap.height);

			//modify normals
			int width = normal.width;
			int height = normal.height;
			float offset = 1.0f / width;
			if (normals == null)
				normals = new Color[width * height];
			Vector3 norm;
			float xLocation;
			float yLocation;

			float HeightMap(float x, float y)
			{
				const float scale = 30;
				const float scale2 = 3;
				const float scale3 = 45;
				//return (Mathf.Atan((Mathf.PerlinNoise(x * scale2 + NoiseThreeOffset.x, y * scale2 + NoiseThreeOffset.y) - .5f) * 100)/ Mathf.PI + .5f);
				return Mathf.Min(
						(
							Mathf.PerlinNoise(x * scale, y * scale) + 
							Mathf.PerlinNoise((x + NoiseTwoOffset.x) * scale, (y + NoiseTwoOffset.y) * scale)
						) / 2 +
						(Mathf.Atan((Mathf.PerlinNoise(x * scale2 + NoiseThreeOffset.x, y * scale2 + NoiseThreeOffset.y) - .5f) * 100) / Mathf.PI + .5f)
						, 0.4f)  +
						Mathf.Min(
							(Mathf.PerlinNoise(x * scale3, y * scale3) + 
							Mathf.PerlinNoise((x + NoiseFourOffset.x) * scale3, (y + NoiseFourOffset.y) * scale3)) / 2
						, 0.3f);

			}

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					int index = (x + y * width);
					xLocation = pos.x + (float)x / width;
					yLocation = pos.y + (float)y / height;
					norm = Vector3.Cross(
						new Vector3(0,
						2 * offset,
						HeightMap(xLocation, yLocation - offset) - HeightMap(xLocation, yLocation + offset)
						),
						new Vector3(
						2 * offset,
						0,
						HeightMap(xLocation - offset, yLocation) - HeightMap(xLocation + offset, yLocation))
						).normalized;
					normals[index] = new Vector4(norm.x, norm.y, norm.z, 0) / 2 + new Vector4(0.5f, 0.5f, 0.5f, 0.9f);
				}
			}
			normal.SetPixels(0, 0, width, height, normals);
			normal.Apply(false);
			
			return normal;
		}

		//load assets
		void AddNormalMapToDatabase(Vector2 pos, Texture2D toSave)
		{
			string assetName = '/' + name + pos.x.ToString() + '_' + pos.y.ToString() + ".png";
			System.IO.File.WriteAllBytes(materialFolderPath + assetName, toSave.EncodeToPNG());
			AssetDatabase.ImportAsset(materialFolderPath + assetName);
		}
		Texture2D LoadNormalMap(Vector2 pos)
		{
			string assetName = '/' + name + pos.x.ToString() + '_' + pos.y.ToString() + ".png";
			return AssetDatabase.LoadAssetAtPath<Texture2D>(materialFolderPath + assetName);
		}
		Material LoadMaterial(Vector2 pos, Texture2D normalMap)
		{
			string assetName = '/' + name + pos.x.ToString() + '_' + pos.y.ToString();
			Material newMat = AssetDatabase.LoadAssetAtPath<Material>(materialFolderPath + assetName + ".mat");
			newMat.SetTexture("_BumpMap", AssetDatabase.LoadAssetAtPath<Texture2D>(materialFolderPath + assetName + ".png"));
			return newMat;
		}


		AssetDatabase.StartAssetEditing();
		Vector2 location;
		foreach(var pair in floorTiles)
		{
			location = pair.Key;
			InitMaterials(location);
			AddNormalMapToDatabase(location, CreateNormalMap(location));
		}
		AssetDatabase.StopAssetEditing();
		AssetDatabase.Refresh();

		AssetDatabase.StartAssetEditing();
		Texture2D loadedImage; 
		foreach (var pair in floorTiles)
		{
			location = pair.Key;
			loadedImage = LoadNormalMap(location);
			pair.Value.GetComponent<MeshRenderer>().material = LoadMaterial(location, loadedImage);
		}
		AssetDatabase.StopAssetEditing();
		AssetDatabase.Refresh();
		AssetDatabase.SaveAssets();


		
		
	}
}



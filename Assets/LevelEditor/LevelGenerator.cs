using UnityEngine;
public class LevelGenerator : MonoBehaviour
{
	public Texture2D mapTexture;
	public PixelToObject[] pixelColorMappings;
	private Color pixelColor;
	public GameObject floorPrefab;
	void Start()
	{
		GenerateLevel();
	}
	void GenerateLevel()
	{
		// Scan whole Texture and get positions of objects
		for (int i = 0; i < mapTexture.width; i++)
		{
			for (int j = 0; j < mapTexture.height; j++)
			{
				GenerateObject(i, j);
			}
		}
	}
	void GenerateObject(int x, int y)
	{
		// Read pixel color
		pixelColor = mapTexture.GetPixel(x, y);

		#region Color Glossary
		// black = wall
		// white = floor
		// green = player
		// red = enemy
		// teal = exit
		#endregion

		foreach (PixelToObject pixelColorMapping in pixelColorMappings)
		{
			// Scan pixelColorMappings Array for matching color maping
			Debug.Log(pixelColorMapping.pixelColor == pixelColor);
			if (pixelColorMapping.pixelColor.Equals(pixelColor))
			{
				if (pixelColorMapping.prefab.Length == 1)
				{
					
					Vector2 position = new Vector2(x, y);
					Instantiate(floorPrefab, position, Quaternion.identity, transform);		//if not a wall, floor needs to be 
																							//spawned in addition to other objects
					Instantiate(pixelColorMapping.prefab[0], position, Quaternion.identity, transform);	//spawning any other object
				}
				else	//for deciding which wall type to create
				{
					#region Picking Correct Wall Tile

					Color rightPixel = Color.black;   //pixel to right of current
					Color topPixel = Color.black;     //pixel above current
					Color leftPixel = Color.black;    //pixel to left of current
					Color botPixel = Color.black;	  //pixel below current

					if (x < mapTexture.width)
						rightPixel = mapTexture.GetPixel(x + 1, y);
					if (y < mapTexture.height)
						topPixel = mapTexture.GetPixel(x, y + 1);
					if (x > 0)
						leftPixel = mapTexture.GetPixel(x - 1, y);
					if (y > 0)
						botPixel = mapTexture.GetPixel(x, y - 1);

					// 0 = right
					// 1 = top
					// 2 = left
					// 3 = bottom
					// 4 = topRight
					// 5 = topLeft
					// 6 = bottomLeft
					// 7 = bottomRight
					// 8 = blank

					if (rightPixel != Color.black && topPixel != Color.black)
					{
						Vector2 position = new Vector2(x, y);
						Instantiate(pixelColorMapping.prefab[4], position, Quaternion.identity, transform);
					}
					else if (topPixel != Color.black && leftPixel != Color.black)
					{
						Vector2 position = new Vector2(x, y);
						Instantiate(pixelColorMapping.prefab[5], position, Quaternion.identity, transform);
					}
					else if (leftPixel != Color.black && botPixel != Color.black)
					{
						Vector2 position = new Vector2(x, y);
						Instantiate(pixelColorMapping.prefab[6], position, Quaternion.identity, transform);
					}
					else if (botPixel != Color.black && rightPixel != Color.black)
					{
						Vector2 position = new Vector2(x, y);
						Instantiate(pixelColorMapping.prefab[7], position, Quaternion.identity, transform);
					}
					else if (rightPixel != Color.black)
					{
						Vector2 position = new Vector2(x, y);
						Instantiate(pixelColorMapping.prefab[0], position, Quaternion.identity, transform);
					}
					else if (topPixel != Color.black)
					{
						Vector2 position = new Vector2(x, y);
						Instantiate(pixelColorMapping.prefab[1], position, Quaternion.identity, transform);
					}
					else if (leftPixel != Color.black)
					{
						Vector2 position = new Vector2(x, y);
						Instantiate(pixelColorMapping.prefab[2], position, Quaternion.identity, transform);
					}
					else if (botPixel != Color.black)
					{
						Vector2 position = new Vector2(x, y);
						Instantiate(pixelColorMapping.prefab[3], position, Quaternion.identity, transform);
					}
					else
					{
						Vector2 position = new Vector2(x, y);
						Instantiate(pixelColorMapping.prefab[8], position, Quaternion.identity, transform);
					}

					#endregion
				}
			}

		}
	}
}

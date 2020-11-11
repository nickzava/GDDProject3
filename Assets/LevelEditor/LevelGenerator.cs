using System.Net.Http.Headers;
using UnityEngine;
public class LevelGenerator : MonoBehaviour
{
	public Texture2D mapTexture;
	public Texture2D decorationLayer;
	public PixelToObject[] pixelColorMappings;			//contains mappings for level layer
	public PixelToObject[] decorationColorMappings;		//contains mappings for decoration layer
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
				GenerateObject(i, j, false);
			}
		}

		for (int i = 0; i < decorationLayer.width; i++)
		{
			for (int j = 0; j < decorationLayer.height; j++)
			{
				GenerateObject(i, j, true);
			}
		}
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
						Instantiate(floorPrefab, position, Quaternion.identity, transform);     //if not a wall, floor needs to be 
																								//spawned in addition to other objects
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
							}
							else if (topPixel != Color.black && leftPixel != Color.black)
							{
								Instantiate(pixelColorMapping.prefab[1], position, Quaternion.identity, transform).transform.Rotate(new Vector3(0.0f, 0.0f, 90.0f));
							}
							else if (leftPixel != Color.black && botPixel != Color.black)
							{
								Instantiate(pixelColorMapping.prefab[1], position, Quaternion.identity, transform).transform.Rotate(new Vector3(0.0f, 0.0f, 180.0f));
							}
							else if (botPixel != Color.black && rightPixel != Color.black)
							{
								Instantiate(pixelColorMapping.prefab[1], position, Quaternion.identity, transform).transform.Rotate(new Vector3(0.0f, 0.0f, 270.0f));
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
}



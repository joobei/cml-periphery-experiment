using UnityEngine;
using System.Collections;

public class Writing : MonoBehaviour {

	private int penColorNum = 0;
	private Color[] penColors = {new Color(1.0f, 1.0f, 1.0f), new Color(1.0f, 0.0f, 0.0f), new Color(0.0f, 1.0f, 0.0f), new Color(0.0f, 0.0f, 1.0f), new Color(1.0f, 1.0f, 0.0f), new Color(1.0f, 0.0f, 1.0f), new Color(0.0f, 1.0f, 1.0f)};

	private Color[] originalBoardPixels;
	private Color[] boardPixels;
	
    Texture2D boardTexture;

    private BlackBoard myBlackBoardScript;
	
	private Vector2 previousCoord = new Vector2(0.5f, 0.5f);
	private bool previousShouldDraw = false;
	private bool previousButton1State = false;

	public GameObject pen;
    public int deviceNb;

	const int eraserRadius = 25;
	const int blobRadius = 15;
	const int blobSteps = 16;



	// Use this for initialization
	void Start () {

        if (deviceNb == 0)
            deviceNb = 1;

        myBlackBoardScript = GetComponent<BlackBoard>();

		//boardTexture = (Texture2D)this.GetComponent<Renderer>().material.mainTexture;
        boardTexture = myBlackBoardScript.boardTexture;
		boardPixels = boardTexture.GetPixels();
		originalBoardPixels = boardTexture.GetPixels();

/*		int i = 0;
		float darkR = 0.1f;
		float darkG = 0.2f;
		float darkB = 0.1f;
		for (int y=0; y<boardTexture.height; y++)
		{
			for (int x=0; x<boardTexture.width; x++)
			{
				boardPixels[i] = new Color(boardPixels[i].r * darkR, boardPixels[i].g * darkG, boardPixels[i].b * darkB);
				i++;
			}
		}*/

		boardTexture.SetPixels(boardPixels);
		boardTexture.Apply();

		changePenColor(penColorNum);
	}
	
	void drawBlob(Vector2 texCoord, int radius, Color col, float alphaStrength, bool erase)
	{
		if (erase) radius = eraserRadius;

		int xCenter = (int)(texCoord.x * boardTexture.width);
		int yCenter = (int)(texCoord.y * boardTexture.height);

		for (int y=-radius; y<=radius; y++)
		{
			int yp = yCenter + y;
			if (yp < 0) yp = 0;
			if (yp > boardTexture.height - 1) yp = boardTexture.height - 1;
			for (int x=-radius; x<=radius; x++)
			{
				int xp = xCenter + x;
				if (xp < 0) xp = 0;
				if (xp > boardTexture.width - 1) xp = boardTexture.width - 1;

				float alpha = 1.0f - Mathf.Sqrt((float)(x * x + y * y)) / radius;
				if (alpha < 0.0f) alpha = 0.0f;
				if (alpha > 1.0f) alpha = 1.0f;

				int pixelOffset = yp * boardTexture.width + xp;
				Color bgColor = boardPixels[pixelOffset];
				Color origBgColor = originalBoardPixels[pixelOffset];
				if (erase)
				{
					boardPixels[pixelOffset] = new Color(bgColor.r * (1.0f-alpha) + origBgColor.r * alpha, bgColor.g * (1.0f-alpha) + origBgColor.g * alpha, bgColor.b * (1.0f-alpha) + origBgColor.b * alpha);
				}
				else
				{
					alpha *= alphaStrength;
					boardPixels[pixelOffset] = new Color(bgColor.r * (1.0f-alpha) + col.r * alpha, bgColor.g * (1.0f-alpha) + col.g * alpha, bgColor.b * (1.0f-alpha) + col.b * alpha);
				}
			}
		}
	}
	
	void drawBlobLine(Vector2 startCoord, Vector2 endCoord, int radius, Color col, float alphaStrength, int steps, bool erase)
	{
		Vector2 stepCoord = startCoord;
		Vector2 step;
		step.x = (endCoord.x - startCoord.x) / (float)steps;
		step.y = (endCoord.y - startCoord.y) / (float)steps;

		for (int i=0; i<steps; i++)
		{
			drawBlob(stepCoord, radius, col, alphaStrength, erase);
			stepCoord.x += step.x;
			stepCoord.y += step.y;
		}
	}
	
	void changePenColor(Color color)
	{
		pen.GetComponent<Renderer>().material.color = color;
		pen.transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(color.r * 0.75f, color.g * 0.75f, color.b * 0.75f);
	}

	void changePenColor(int colorNum)
	{
		changePenColor(penColors[colorNum]);
	}
	
	public void cleanBoard()
	{
		for (int i=0; i<boardPixels.Length; i++)
			boardPixels[i] = originalBoardPixels[i];
		resetBoard();
	}
	
	void resetBoard()
	{
		boardTexture.SetPixels(originalBoardPixels);
		boardTexture.Apply();
	}

	int myCounter = 0;
	// Update is called once per frame
	void Update () {

        //Get Pixels - Needed if two Haptic Devices
        boardPixels = boardTexture.GetPixels();

        bool button1State = PluginImport.GetButtonState(deviceNb,1);
		if (button1State && !previousButton1State)
		{
			penColorNum = (penColorNum + 1) % penColors.Length;
		}
		previousButton1State = button1State;

        //if (PluginImport.GetButtonState(deviceNb,2)) cleanBoard();
        bool eraseState = PluginImport.GetButtonState(deviceNb,2);
		if (eraseState)
		{
			changePenColor(new Color(0.25f, 0.25f, 0.25f));
		}
		else
		{
			changePenColor(penColorNum);
		}

        //updated the new interface for Dual Haptic Devices
		bool shouldDraw = PluginImport.GetHapticContact(deviceNb) && (myCounter > 0);
		
        if (shouldDraw)
		{
			//double[] pos = ConverterClass.ConvertIntPtrToDouble3(PluginImport.GetProxyPosition());
            double[] pos = ConverterClass.SelectHalfdouble6toDouble3(ConverterClass.ConvertIntPtrToDouble6(PluginImport.GetProxyPosition()), deviceNb);
			//double[] dir = ConverterClass.ConvertIntPtrToDouble3(PluginImport.GetProxyDirection());
            double[] dir = ConverterClass.SelectHalfdouble6toDouble3(ConverterClass.ConvertIntPtrToDouble6(PluginImport.GetProxyDirection()), deviceNb);
			
            Vector3 position = new Vector3((float)pos[0], (float)pos[1], (float)pos[2]);
			Vector3 direction = new Vector3((float)dir[0], (float)dir[1], (float)dir[2]);

			//double[] realPos = ConverterClass.ConvertIntPtrToDouble3(PluginImport.GetDevicePosition());
            double[] realPos = ConverterClass.SelectHalfdouble6toDouble3(ConverterClass.ConvertIntPtrToDouble6(PluginImport.GetDevicePosition()), deviceNb);
			Vector3 realPosition = new Vector3((float)realPos[0], (float)realPos[1], (float)realPos[2]);

			float force = (realPosition - position).magnitude;

			if (force > 1.0f) force = 1.0f;

			RaycastHit hitInfo = new RaycastHit();
			bool hasHit = Physics.Raycast(position, direction, out hitInfo);

			if (previousShouldDraw)
			{
				drawBlobLine(previousCoord, hitInfo.textureCoord, blobRadius, penColors[penColorNum], force, blobSteps, eraseState);
			}
			else
			{
				drawBlob(hitInfo.textureCoord, blobRadius, penColors[penColorNum], force, eraseState);
			}
			previousCoord = hitInfo.textureCoord;
          
			boardTexture.SetPixels(boardPixels);
			boardTexture.Apply();
		}
		previousShouldDraw = shouldDraw;
		myCounter++;
	}
	
	void OnApplicationQuit()
	{
		resetBoard();
	}
}

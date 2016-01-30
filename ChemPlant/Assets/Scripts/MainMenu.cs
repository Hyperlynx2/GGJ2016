using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour 
{
	public string[] levelSceneNames;

	public const float HEIGHT_OFFSET = 25F;
	public const float ELEMENT_HEIGHT = 50;
	public const float ELEMENT_WIDTH = 200;
	public const float SEPARATION = 5;

	public const string MENU_SCENE = "mainmenu";

	public void OnGUI()
	{
		//GUI.skin = skin;
		
		float heightOffset = HEIGHT_OFFSET;
		
		GUI.Label(new Rect(Screen.width/2 - ELEMENT_WIDTH/2, heightOffset, ELEMENT_WIDTH, ELEMENT_HEIGHT),
		    "ChemPlant");

		heightOffset += ELEMENT_HEIGHT *2;
			
		//god help you if you haven't got the labels and scenes right sized!
		for(int i = 0; i < levelSceneNames.Length; ++i)
		{
			//TODO: change to be percentage of screen rather than absolute position.

			if(GUI.Button(new Rect(Screen.width /2 - ELEMENT_WIDTH/2, heightOffset, ELEMENT_WIDTH, ELEMENT_HEIGHT), levelSceneNames[i]))
			{
				Application.LoadLevel(levelSceneNames[i].ToLower());
			}
				
			heightOffset += ELEMENT_HEIGHT + SEPARATION;
				
		}
	}
}

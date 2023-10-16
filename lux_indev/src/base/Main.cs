using Godot;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace luxterra {
public partial class Main : Node
{
	//Scenes
	private PackedScene menuScene;
	private Control menuControl;
	private PackedScene gameScene;
	private Node2D gameNode;  

	//Buttons
	private Button StartServer;
	private Button StartClient; 

	private LineEdit SetName; 
	private LineEdit SetAddress;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		LuxData.userInfo = new Dictionary<string, string>();
		LuxData.userInfo.Add("id"  , "default");
		LuxData.userInfo.Add("name", "default");


		menuScene = (PackedScene)ResourceLoader.Load("res://scenes/base/menu/Menu.tscn");
		gameScene = (PackedScene)ResourceLoader.Load("res://scenes/base/game/Game.tscn");


		menuControl = (Control)menuScene.Instantiate();
		AddChild(menuControl);

		StartServer = GetNode<Button>("Menu/Title/StartServer");
		StartClient = GetNode<Button>("Menu/Title/StartClient");
		
		SetName = GetNode<LineEdit>("Menu/Title/Name");
		SetAddress = GetNode<LineEdit>("Menu/Title/Ip");

		
		StartServer.Pressed += () => {
			LuxData.multiType = "server";
			menuControl.QueueFree();
			gameNode = gameScene.Instantiate<Node2D>();
			AddChild(gameNode);
		};

		StartClient.Pressed += () => {
			LuxData.multiType = "client"; 
			menuControl.QueueFree();
			gameNode = gameScene.Instantiate<Node2D>();
			AddChild(gameNode);
		};

		SetName.TextChanged += (string text) => LuxData.USERNAME = text;
		SetAddress.TextChanged += (string text) => LuxData.ADDRESS = text;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

}
	
	public static class LuxData
	{
		public static Dictionary<string,string> userInfo { get; set; } 
		public static string USERNAME = "default";
		public static int USERID = 0;
		public static string ADDRESS = "localhost";
		public static int 	 PORT    = 48256; 
		public static string multiType = "default";

		public static int CHUNKSIZE = 8;
		public static int RENDERDISTANCE = 8;

		public static Dictionary<string,Vector2I> enviromentReadableTiles = new Dictionary<string, Vector2I>{
			{"stone", Vector2I(1,3)},
			{"dirt",  Vector2I(1,8)},
			{"metal", Vector2I(1,4)},
			{"grass", Vector2I(3,8)}
			};
		public static Dictionary<int,Vector2I> enviromentChunktiles = new Dictionary<int, Vector2I>{
			{1, Vector2I(1,3)},
			{2,  Vector2I(1,8)},
			{3, Vector2I(1,4)},
			{4, Vector2I(3,8)}
			};
		
		public static Dictionary<string, int> enviromentReadableChunkTiles = new Dictionary<string, int> {
			{"stone", 1},
			{"dirt",  2},
			{"metal", 3},
			{"grass", 4}
		};

		//This should not work at all. Remove this later. Seriously. Bad.
        public static Vector2I Vector2I(int v1, int v2)
        {
            Vector2I p = new Vector2I();
			p.X = v1; 
			p.Y = v2; 
			return p;
        }
    }
}

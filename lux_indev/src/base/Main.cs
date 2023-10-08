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
		public static string ADDRESS = "localhost";
		public static int 	 PORT    = 48256; 
		public static string multiType = "default";
	}
}

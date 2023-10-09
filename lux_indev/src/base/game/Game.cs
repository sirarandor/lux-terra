using Godot;
using luxterra;
using System;

public partial class Game : Node2D
{

	private ENetMultiplayerPeer multiUser;
	public Node2D entityList;
	
	public World gameWorld;

	private PackedScene playerScene;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		playerScene = ResourceLoader.Load<PackedScene>("res://scenes/Entity/Player/Player.tscn");


		multiUser = new ENetMultiplayerPeer(); 
		
		
		entityList = GetNode<Node2D>("World/Entities");
		gameWorld  = GetNode<World>("World");


		Multiplayer.PeerConnected += (long id) => onUserConnected(id);

		if (LuxData.multiType == "server") {
			ServerBootstrapper();
		}

		if (LuxData.multiType == "client") {
			ClientBootstrapper();
			
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	//Spawn a player in the world at a certain position
	public void SpawnPlayer(string name, int id) { 
		Player np = playerScene.Instantiate<Player>();
		np.playerName = name;
		np.playerId = id;
		entityList.AddChild(np, true);
	}



	// ALL MULTIPLAYER FUNCTIONS GO HERE
	//General User Functions
	public void U_Finit() {
		SpawnPlayer(LuxData.USERNAME, Multiplayer.GetUniqueId());
	}

	public void onUserConnected(long id) {
		GD.Print("User Connected with id ", id);
		SpawnPlayer(id.ToString(), Convert.ToInt32(id));
		RpcId(id, "U_RegUser", Multiplayer.GetUniqueId());
	}

	//Register ourself when asked by any peer 
	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	public void U_RegUser(long id) {
		RpcId(id, "U_RecUser", LuxData.USERNAME);
	}

	//Receive the user we asked to register
	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	public void U_RecUser(string name) {
		GD.Print("Received user with name", name);
		entityList.GetNode<Player>(Multiplayer.GetRemoteSenderId().ToString()).SetName(name);
	}

	//Server-Specific Functions
	private int ServerBootstrapper() {
		multiUser.CreateServer(LuxData.PORT);
		Multiplayer.MultiplayerPeer = multiUser; 

		U_Finit();
		return 0; 
	}




	//Client-Specific Functions
	private int ClientBootstrapper() { 
		multiUser.CreateClient(LuxData.ADDRESS, LuxData.PORT);
		Multiplayer.MultiplayerPeer = multiUser;

		U_Finit();
		return 0;
	}
}

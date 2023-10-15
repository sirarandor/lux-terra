using Godot;
using luxterra;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks; 

public partial class Game : Node2D
{

	private ENetMultiplayerPeer multiUser;
	public Node2D entityList;
	
	public World gameWorld;
	private Vector2I pChunkPos;
	private byte[][][] pChunkAr;
	public Dictionary<(int x, int y), byte> pExploredChunks; 

	private PackedScene playerScene;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
		playerScene = ResourceLoader.Load<PackedScene>("res://scenes/Entity/Player.tscn");

		pExploredChunks = new Dictionary<(int x, int y), byte>{};

		pChunkPos = new Vector2I();


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
		//GD.Print(entityList.GetNode<Player>(Multiplayer.GetUniqueId().ToString()));
		//GD.Print(Multiplayer.GetUniqueId());
		//Fix this later so we aren't converting a string every single frame. Good heavens.
		if (entityList.GetNode<Player>(Multiplayer.GetUniqueId().ToString()) != null) {
			//GD.Print("We have an id: ", entityList.GetNode<Player>(Multiplayer.GetUniqueId().ToString()).Name);
			WorldGenerationHandler(); 
		}
	}

	public void WorldGenerationHandler() {
		Vector2I pPos = gameWorld.LocalToMap(entityList.GetNode<Player>(Multiplayer.GetUniqueId().ToString()).Position);
			pChunkPos = LuxData.Vector2I(pPos.X/LuxData.CHUNKSIZE, pPos.Y/LuxData.CHUNKSIZE); 
			//GD.Print(pPos, pChunkPos);
			//gameWorld.RequestChunk(pChunkPos.X, pChunkPos.Y);
			byte b;

			for (int i = pChunkPos.X-LuxData.RENDERDISTANCE; i <= pChunkPos.X+LuxData.RENDERDISTANCE; i++) {
				for (int j = pChunkPos.Y-LuxData.RENDERDISTANCE; j <= pChunkPos.Y+LuxData.RENDERDISTANCE; j++) {
						if (pExploredChunks.TryGetValue((i, j), out b) == false) {

							if (Multiplayer.IsServer()) { 
								gameWorld.RequestChunk(i,j, false);
							} else {
								RpcId(1, "ServerSendChunk",i,j); 
							}

							pExploredChunks.Add((i, j), 1);
							
					}
				}
			}
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
		GD.Print("We are ", Multiplayer.GetUniqueId(), ", and a User Connected with id: ", id);
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

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void ServerSendChunk(int x, int y) {
		Godot.Collections.Array<Vector4I> c = gameWorld.RequestChunk(x, y, true);
		RpcId(Multiplayer.GetRemoteSenderId(), "ClientRecieveChunk", c);
	} 




	//Client-Specific Functions
	private int ClientBootstrapper() { 
		try {
			multiUser.CreateClient(LuxData.ADDRESS, LuxData.PORT);
			Multiplayer.MultiplayerPeer = multiUser;
		} catch {
			GD.Print("Somethign went wrong...");
		}


		U_Finit();
		return 0;
	}

	[Rpc(MultiplayerApi.RpcMode.Authority)]
	private void ClientRecieveChunk(Godot.Collections.Array<Vector4I> c) {
		gameWorld.DrawChunk(c); 
	}
}

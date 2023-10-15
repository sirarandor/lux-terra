using Godot;
using System;
using luxterra;
using System.Numerics;
using System.Linq;
using Godot.Collections;
using System.Net.Http.Headers;
public partial class World : TileMap
{
	private RandomNumberGenerator rng;
	private FastNoiseLite genNoise;

	private Node entityList; 

	private PackedScene treeScene;

	//private byte[][][] chunkAr;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		rng = new RandomNumberGenerator();
		genNoise = new FastNoiseLite();

		// Set up the base noise type and seed. We will have to change all this when saving to a file. 
		genNoise.NoiseType = FastNoiseLite.NoiseTypeEnum.ValueCubic;
		genNoise.Frequency = 0.008f;
		genNoise.Seed      = (int)rng.Randi();

		//Set up the fractal part of the noise generator in order to construct a reasonably shaped world.
		//This is subject to change after introducing biomes. Likely multiple noise generators will be used.
		genNoise.FractalType       = FastNoiseLite.FractalTypeEnum.Fbm;
		genNoise.FractalOctaves    = 8;
		genNoise.FractalLacunarity = 4;
		genNoise.FractalGain       = 0.25f;

		entityList = GetNode<Node2D>("Entities");

		treeScene = ResourceLoader.Load<PackedScene>("res://scenes/entity/living/TreeAdult.tscn");
		
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void PlaceTile( Vector2I pos, (int l, string t) data) {
		CallDeferred("set_cell", data.l, pos, 0, LuxData.enviromentReadableTiles[data.t]);
		
	}

	private void  PlaceTile(Vector2I pos, (int l, int t) data) {
		CallDeferred("set_cell", data.l, pos, 0, LuxData.enviromentChunktiles[data.t]);
	}

	public Godot.Collections.Array<Vector4I> RequestChunk(int x, int y, bool m) {
		byte[][][] chunkAr = new byte[LuxData.CHUNKSIZE][][];

		Godot.Collections.Array<Vector4I> a = new Array<Vector4I>(); 

		//Set up the bounds of the chunk relative to the world. 
		//Currently this can break when loading chunk 0,0. 
		int sx = x * LuxData.CHUNKSIZE;
		int sy = y * LuxData.CHUNKSIZE;
		
		int ex = sx + LuxData.CHUNKSIZE;
		int ey = sy + LuxData.CHUNKSIZE;

		int c = 0;
		//GD.Print("Requested chunk...");

		//Check if the first and last tiles (diagonally) don't contain anything. 
		if (GetCellTileData(0, LuxData.Vector2I(sx, sy)) == null | GetCellTileData(0, LuxData.Vector2I(ex-1, ey-1)) == null | m) {

			//There's likely a way to make this loop even faster, but I'll change it later. Enumerable.Range() makes sure I don't mess anything up with my math.
			for (int i = sx; i <= ex; i++) {
				for (int j = sy; j <= ey; j++) {
					float genP = genNoise.GetNoise2D(i, j);
					//GD.Print(i, "-",j); 
					//Currently the only biome generator, placing stone mountains. This will later be called "Mountains" in the biome descriptors. 
					if (genP < -0.0025) {
						PlaceTile(LuxData.Vector2I(i, j), (1, "stone")); 
						a.Add(new Vector4I(i, j, 1, LuxData.enviromentReadableChunkTiles["stone"]));
						PlaceTile(LuxData.Vector2I(i,j), (0, "dirt"));
						a.Add(new Vector4I(i, j, 0, LuxData.enviromentReadableChunkTiles["dirt"]));
					} else {
						//Place grass where there isn't stone.
						PlaceTile(LuxData.Vector2I(i, j), (0, "grass"));
						a.Add(new Vector4I(i, j, 0, LuxData.enviromentReadableChunkTiles["grass"]));
						if (rng.Randf() > 0.9995f) {
							luxterra.Tree baseTree = treeScene.Instantiate<luxterra.Tree>();
							baseTree.Position = MapToLocal(LuxData.Vector2I(i,j)); 
							entityList.AddChild(baseTree);
						}
					}
					/* THIS IS UNUSED FOR NOW
					for (byte k = 0; k <= 1; k++) {
						TileData td = GetCellTileData(k, LuxData.Vector2I(i, j));
						if (td != null) {
							Godot.Variant d = td.GetCustomData("td"); 
							a[c] = new Vector4I(i, j, k, d); 

						}
					}
					*/
					c++; 
				}
			} 
		}
		//GD.Print("Sawned chunk")
		return a; 
	}

	public void DrawChunk(Array<Vector4I> c)
	{
		for (int i = 0; i < c.Count; i++) {
			PlaceTile(LuxData.Vector2I(c[i].X, c[i].Y), (c[i].Z, c[i].W));
			//Dear god I hope this works. 
		}
	}
}

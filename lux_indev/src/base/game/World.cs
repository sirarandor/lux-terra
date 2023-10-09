using Godot;
using System;
using luxterra;
using System.Numerics;
public partial class World : TileMap
{
	private RandomNumberGenerator rng;
	private FastNoiseLite genNoise;

	private byte[][][] chunkAr;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		rng = new RandomNumberGenerator();

		// Set up the base noise type and seed. We will have to change all this when saving to a file. 
		genNoise.NoiseType = FastNoiseLite.NoiseTypeEnum.ValueCubic;
		genNoise.Frequency = 0.008f;
		genNoise.Seed      = Convert.ToInt32(rng.Randi());

		//Set up the fractal part of the noise generator in order to construct a reasonably shaped world.
		//This is subject to change after introducing biomes. Likely multiple noise generators will be used.
		genNoise.FractalType       = FastNoiseLite.FractalTypeEnum.Fbm;
		genNoise.FractalOctaves    = 8;
		genNoise.FractalLacunarity = 4;
		genNoise.FractalGain       = 0.25f;


		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void PlaceTile( Vector2I pos, (int l, string t) data) {
		
		SetCell(data.l, pos, 0, LuxData.enviromentTiles["data.t"]);
	}

	public byte[][][] RequestChunk() {
		chunkAr = new byte[LuxData.CHUNKSIZE][][];

		return chunkAr;
	}

	public void DrawChunk()
	{

	}
}

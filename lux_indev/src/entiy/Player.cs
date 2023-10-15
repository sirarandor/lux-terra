using Godot;
using luxterra;
using System;
using System.Runtime.Serialization;

public partial class Player : CharacterBody2D
{

	[Export]
	public int playerId {get; set;}

	[Export]
	public string playerName {get; set;}

	//Player Nodes: 
	//public CharacterBody2D PlayerCharacter;
	private Label NameTag;

	//Player specs: 
	private int PlayerWalkingSpeed = 4;

	public Player() {
	}

	public void SetName(string name) {
		playerName = name; 
		NameTag.Text = name;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//PlayerCharacter = GetNode<CharacterBody2D>("PlayerCharacter");
		NameTag = GetNode<Label>("NameTag");
		NameTag.Text = playerName;

		Name = playerId.ToString();
		SetMultiplayerAuthority(playerId);

		SetPhysicsProcess(IsMultiplayerAuthority());
		SetProcessInput(IsMultiplayerAuthority());
		SetProcess(IsMultiplayerAuthority());

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Label L_FPS = GetNode<Label>("GUI/Control/FPS");
		Camera2D Cam = GetNode<Camera2D>("Camera2D"); 
		L_FPS.Text = Engine.GetFramesPerSecond().ToString(); 
		//Position = PlayerCharacter.Position; 

		if (Input.IsActionJustPressed("scroll_up") && Cam.Zoom.X < 4) {
			Cam.Zoom = new Vector2(Cam.Zoom.X + 0.1f, Cam.Zoom.Y + 0.1f); 
		}
		if (Input.IsActionJustPressed("scroll_down") && Cam.Zoom.X > 0) {
			Cam.Zoom = new Vector2(Cam.Zoom.X - 0.1f, Cam.Zoom.Y - 0.1f); 
		}
	}

    public override void _PhysicsProcess(double delta)
    {

		Vector2 inputDir = Input.GetVector("left", "right", "up", "down");
		Position = (inputDir * PlayerWalkingSpeed) + Position;
		MoveAndSlide();
		
    }
}

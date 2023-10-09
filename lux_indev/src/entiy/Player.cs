using Godot;
using System;
using System.Runtime.Serialization;

public partial class Player : Node2D
{

	[Export]
	public int playerId {get; set;}

	[Export]
	public string playerName {get; set;}

	//Player Nodes: 
	private CharacterBody2D PlayerCharacter;
	private Label NameTag;

	//Player specs: 
	private int PlayerWalkingSpeed = 2;

	public Player() {
	}

	public void SetName(string name) {
		playerName = name; 
		NameTag.Text = name;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		PlayerCharacter = GetNode<CharacterBody2D>("PlayerCharacter");
		NameTag = GetNode<Label>("PlayerCharacter/NameTag");
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
		
	}

    public override void _PhysicsProcess(double delta)
    {
		Godot.Vector2 ChangedPos = Position;

		if (Input.IsActionPressed("ui_up")) {
			ChangedPos.Y = ChangedPos.Y - PlayerWalkingSpeed; 
		}
		if (Input.IsActionPressed("ui_down")) {
			ChangedPos.Y = ChangedPos.Y + PlayerWalkingSpeed;
		}
		if (Input.IsActionPressed("ui_right")) {
			ChangedPos.X = ChangedPos.X + PlayerWalkingSpeed;
		}
		if (Input.IsActionPressed("ui_left")) {
			ChangedPos.X = ChangedPos.X - PlayerWalkingSpeed;
		}
		
		Position = ChangedPos;
		PlayerCharacter.MoveAndSlide();
    }

    public override void _Input(InputEvent @event)
    {

    }
}

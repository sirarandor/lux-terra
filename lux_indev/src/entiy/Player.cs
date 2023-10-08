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
	private int PlayerWalkingSpeed = 16;

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
		PlayerCharacter.MoveAndSlide();
    }

    public override void _Input(InputEvent @event)
    {
		Godot.Vector2 ChangedPos = Position;

        base._Input(@event);

		if (@event is InputEventKey eventKey) {
			if (eventKey.Pressed && eventKey.Keycode == Key.Up) {
				ChangedPos.Y = ChangedPos.Y - PlayerWalkingSpeed; 
			}
			if (eventKey.Pressed && eventKey.Keycode == Key.Down) {
				ChangedPos.Y = ChangedPos.Y + PlayerWalkingSpeed;
			}
			if (eventKey.Pressed && eventKey.Keycode == Key.Right) {
				ChangedPos.X = ChangedPos.X + PlayerWalkingSpeed;
			}
			if (eventKey.Pressed && eventKey.Keycode == Key.Left) {
				ChangedPos.X = ChangedPos.X - PlayerWalkingSpeed;
			}
		}
		
		Position = ChangedPos;
    }
}

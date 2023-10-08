using Godot;
using System;
using luxterra; 

public partial class Menu : Control
{

	private LineEdit lineName;
	private LineEdit lineIp;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		lineName = GetNode<LineEdit>("Title/Name");
		lineIp   = GetNode<LineEdit>("Title/Ip");
	}

//	 Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		LuxData.userInfo["name"] = lineName.Text; 
		LuxData.userInfo["ip"]   = lineIp.Text;
	}
}



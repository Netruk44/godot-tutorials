using Godot;
using System;

public partial class ball : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	
	private VisibleOnScreenNotifier3D visibleOnScreenNotifier;
	private double totalTime = 0.0f;

	public override void _Ready()
	{
		visibleOnScreenNotifier = GetNode<VisibleOnScreenNotifier3D>("VisibleOnScreenNotifier3D");
	}

	public override void _PhysicsProcess(double delta)
	{
		totalTime += delta;
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;
		
		if (!visibleOnScreenNotifier.IsOnScreen())
		{
			// Move in circles based on total time.
			Vector3 moveDirection = new Vector3(Mathf.Cos((float)totalTime), 0, Mathf.Sin((float)totalTime));
			velocity = moveDirection.Normalized() * Speed;
		}
		else
		{
			// Stop the object.
			velocity.X = velocity.Z = 0;
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}

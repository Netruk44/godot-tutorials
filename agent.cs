using Godot;
using System;

public partial class agent : CharacterBody3D
{
	public const float Speed = 5.0f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	private Node3D player;
	private NavigationAgent3D navigationAgent;

	// How often to update the target (in seconds)
	private const double TargetUpdateInterval = 0.5;

	// How long since we last updated the target
	private double lastTargetUpdate = TargetUpdateInterval; // Start at the interval so we update immediately.

	public override void _Ready()
	{
		// Get the navigation agent
		navigationAgent = GetNode<NavigationAgent3D>("NavigationAgent3D");
		navigationAgent.VelocityComputed += OnVelocityComputed;

		// Get the player node
		var player_group = GetTree().GetNodesInGroup("player");

		if (player_group.Count > 0)
		{
			player = player_group[0] as Node3D;
		}
		else
		{
			GD.PrintErr("No player found!");
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		// Check if we should update the target
		lastTargetUpdate += delta;
		if (lastTargetUpdate >= TargetUpdateInterval)
		{
			lastTargetUpdate = 0.0;
			navigationAgent.TargetPosition = player.GlobalTransform.Origin;
		}

		Vector3 nextPathPosition = navigationAgent.GetNextPathPosition();
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		// Move towards the next point in the path.
		Vector2 direction = new Vector2(nextPathPosition.X, nextPathPosition.Z) - new Vector2(GlobalTransform.Origin.X, GlobalTransform.Origin.Z);
		direction = direction.Normalized();

		velocity.X = direction.X * Speed;
		velocity.Z = direction.Y * Speed;

		navigationAgent.Velocity = velocity;
	}

	private void OnVelocityComputed(Vector3 safeVelocity)
	{
		Velocity = safeVelocity;
		MoveAndSlide();
	}
}

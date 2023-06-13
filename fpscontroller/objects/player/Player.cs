using Godot;
using System;

public partial class Player : CharacterBody3D
{
	[ExportCategory("SpeedMenu")]
	[Export]
	private float Speed = 5.0f;
	[Export]
	private float acceleration = 0.2f;
	[Export]
	private float JumpVelocity = 4.5f;
	[Export]
	private float MaxSpeed = 8.0f;

	private float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	private Node3D Head;
	private Camera3D Camera;
	private CollisionShape3D Col;
	private RayCast3D CanStandUpRay;
	private RayCast3D FootSoundRay;
	private bool CanStandUp;
	[ExportCategory("CameraMenu")]
	[Export]
	private float LookCameraSensivity = 0.006f;

	public override void _Ready()
	{

		Head = GetNode<Node3D>("Head");
		Camera = GetNode<Camera3D>("Head/Camera3D");
		Col = GetNode<CollisionShape3D>("CollisionShape3D");
		CanStandUpRay = GetNode<RayCast3D>("CanStandUpRay");
	
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}
	public override void _PhysicsProcess(double delta)
	{
		// Can Stand Up Ray
		CanStandUp = CanStandUpRay.IsColliding();

		Vector3 velocity = Velocity;

		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		// Jump
		if (Input.IsActionJustPressed("Jump") && IsOnFloor() && CanStandUp == false)
		{
			velocity.Y = JumpVelocity;
		}
		
		Vector2 inputDir = Input.GetVector("Move_Left", "Move_Right", "Move_Forward", "Move_Backward");
		inputDir = inputDir.Normalized();
		Vector3 direction = (Head.GlobalTransform.Basis * new Vector3(inputDir.X , 0 , inputDir.Y)).Normalized();

		SprintandCrouch();

		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, acceleration);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, acceleration);
		}

		Velocity = velocity;
	 	MoveAndSlide();

	}
	public override void _Input(InputEvent @event)
	{
		// Camera Rotation
		if (@event is InputEventMouseMotion moition)
		{
			InputEventMouseMotion inputMouseMoition = @event as InputEventMouseMotion;
			Head.RotateY(-inputMouseMoition.Relative.X * LookCameraSensivity);
			Camera.RotateX(-inputMouseMoition.Relative.Y * LookCameraSensivity);

			Vector3 CameraRot = Camera.Rotation;

			CameraRot.X = Mathf.Clamp(CameraRot.X,Mathf.DegToRad(-80),Mathf.DegToRad(80f));

			Camera.Rotation = CameraRot;
		}
	}

	public void SprintandCrouch()
	{
		
		// Sprint
		if (Input.IsActionPressed("Sprint") && ((CapsuleShape3D)Col.Shape).Height == 2) 
		{
			Speed = MaxSpeed;
		}
		// Crouch
		else if (Input.IsActionPressed("Crouch"))
		{
			Speed = 3f;
			((CapsuleShape3D)Col.Shape).Height -= 0.1f;
			((CapsuleShape3D)Col.Shape).Height = Mathf.Clamp(((CapsuleShape3D)Col.Shape).Height,1f,2f);
		}
		else 
		{
			if (CanStandUp == false)
			{
				Speed = 5f;
				((CapsuleShape3D)Col.Shape).Height += 0.1f;
				((CapsuleShape3D)Col.Shape).Height = Mathf.Clamp(((CapsuleShape3D)Col.Shape).Height,1f,2f);
			}

		}
	}
}

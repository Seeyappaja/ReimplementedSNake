using Godot;
using System;
using System.Collections.Generic;
using System.Linq;


public partial class SnakeGame : Node2D
{
	private const int Width = 32;
	private const int Height = 16;
	private readonly Random _random = new Random();
	private readonly List<Vector2> _snakeBody = new List<Vector2>();
	private Vector2 _berryPosition;
	private Vector2 _snakeDirection;
	private double _elapsedTime;
	private bool _isGameOver;
	private const int Scale = 16; // Adjust the scale factor as needed
	private Texture2D imageTexture;
	private Texture2D imageBerry = GD.Load<Texture2D>("res://Graphics/berry.png");
	private Texture2D imageWall = GD.Load<Texture2D>("res://Graphics/wall.png");
	private Texture2D imageField = GD.Load<Texture2D>("res://Graphics/field.png");
	private Texture2D imageBody = GD.Load<Texture2D>("res://Graphics/snake_body.png");
	private Texture2D imageHead = GD.Load<Texture2D>("res://Graphics/snake_head.png");
	private Texture2D imageGO = GD.Load<Texture2D>("res://Graphics/go.png");

	public override void _Ready()
	{
		_snakeDirection = Vector2.Right;
		_isGameOver = false;
		SpawnSnake();
		SpawnBerry();
		SetProcess(true);
		GD.Print("Main Loop");
	}

	public override void _Process(double delta)
	{
		_elapsedTime += delta;
		if (_elapsedTime > 0.5)
		{
			_elapsedTime = 0;
			MoveSnake();
			CheckCollision();
			QueueRedraw();
		}
	}
	
	public override void _Draw()
	{
		Rect2 destinationRect = new Rect2(0,0,0,0);
		// Draw borders
		DrawBorder();
		// Draw snake body
		foreach (var segment in _snakeBody)
		{
			destinationRect = new Rect2(segment[0] * Scale, segment[1] * Scale, Scale, Scale);
			DrawTextureRect(imageBody, destinationRect, false);
		}
		destinationRect = new Rect2(_snakeBody[0].X * Scale, _snakeBody[0].Y * Scale, Scale, Scale);
		DrawTextureRect(imageHead, destinationRect, false);
		// Draw berry
		destinationRect = new Rect2(_berryPosition[0] * Scale, _berryPosition[1] * Scale, Scale, Scale);
		DrawTextureRect(imageBerry, destinationRect, false);
		if (_isGameOver)
		{
			destinationRect = new Rect2(0, 0, Width * Scale, Height * Scale);
			DrawTextureRect(imageGO, destinationRect, false);
			Engine.TimeScale = 0;
		}
	}

	private void DrawBorder()
	{
		//Draw Wall
		Rect2 destinationRect = new Rect2(0, 0, 0, 0);
		destinationRect = new Rect2(0, 0, Width * Scale, Height * Scale);
		DrawTextureRect(imageWall, destinationRect, true);
		//Draw Field
		destinationRect = new Rect2(0 + Scale, 0 + Scale, Width * Scale - 2 * Scale, Height * Scale - 2 * Scale);
		DrawTextureRect(imageField, destinationRect, true);
	}

	
	private void SpawnSnake()
	{
		_snakeBody.Clear();
		for (int i = 0; i < 3; i++)
		{
			_snakeBody.Add(new Vector2(Width / 2 - i, Height / 2));
		}
	}

	private void SpawnBerry()
	{
		_berryPosition = new Vector2(_random.Next(1, Width-1), _random.Next(1, Height-1));
	}

	private void MoveSnake()
	{
		var newHeadPosition = _snakeBody.First() + _snakeDirection;
		_snakeBody.Insert(0, newHeadPosition);
		_snakeBody.RemoveAt(_snakeBody.Count - 1);
	}

	private void CheckCollision()
	{
		var head = _snakeBody.First();
		if (head[0] < 1 || head[0] >= Width-1 || head[1] < 1 || head[1] >= Height-1)
		{
			_isGameOver = true;
			return;
		}

		for (int i = 1; i < _snakeBody.Count; i++)
		{
			if (_snakeBody[i] == head)
			{
				_isGameOver = true;
				return;
			}
		}

		if (head == _berryPosition)
		{
			SpawnBerry();
			_snakeBody.Add(_snakeBody.Last());
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey eventKey)
		{
			if (eventKey.IsActionPressed("ui_up") && _snakeDirection != Vector2.Down)
				_snakeDirection = Vector2.Up;
			else if (eventKey.IsActionPressed("ui_down") && _snakeDirection != Vector2.Up)
				_snakeDirection = Vector2.Down;
			else if (eventKey.IsActionPressed("ui_left") && _snakeDirection != Vector2.Right)
				_snakeDirection = Vector2.Left;
			else if (eventKey.IsActionPressed("ui_right") && _snakeDirection != Vector2.Left)
				_snakeDirection = Vector2.Right;
		}
	}
}

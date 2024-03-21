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
	private const int Scale = 16;

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
		if (_isGameOver)
		{
			GD.Print("Game over");
			return;
		}

		_elapsedTime += delta;
		GD.Print("Adjusted Delta: " + delta);
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
		// Clear screen
		DrawRect(new Rect2(0, 0, Width * Scale, Height * Scale), Colors.Black, true);

		// Draw borders
		DrawBorder();

		// Draw snake body
		foreach (var segment in _snakeBody)
		{
			DrawRect(new Rect2(segment[0] * Scale, segment[1] * Scale, Scale, Scale), Colors.Green, true);
		}

		// Draw berry
		DrawRect(new Rect2(_berryPosition[0] * Scale, _berryPosition[1] * Scale, Scale, Scale), Colors.Red, true);
	}

	private void DrawBorder()
	{
		// Top border
		DrawRect(new Rect2(0, 0, Width * Scale, Scale), Colors.White, true);
		// Bottom border
		DrawRect(new Rect2(0, (Height - 1) * Scale, Width * Scale, Scale), Colors.White, true);
		// Left border
		DrawRect(new Rect2(0, 0, Scale, Height * Scale), Colors.White, true);
		// Right border
		DrawRect(new Rect2((Width - 1) * Scale, 0, Scale, Height * Scale), Colors.White, true);
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
		if (head[0] < 0 || head[0] >= Width || head[1] < 0 || head[1] >= Height)
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

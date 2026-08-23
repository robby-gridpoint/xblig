using System;
using System.Collections.Generic;
using BunnyOfWar.AI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BunnyOfWar;

public class InputManagerObject
{
	public void addToButtonSequence(Buttons b, FighterObject player)
	{
		if (QuickTimeEventsManager.hasQTE)
		{
			QuickTimeEventsManager.ButtonWasPressed(b, player);
			return;
		}
		if (player.PROPERTIES.recentButtonPressTime.AddMilliseconds(500.0) < DateTime.Now || player.PROPERTIES.recentButtonSequencePosition == 100)
		{
			player.PROPERTIES.recentButtonSequencePosition = 0L;
		}
		player.PROPERTIES.recentButtonSequence[player.PROPERTIES.recentButtonSequencePosition] = b;
		player.PROPERTIES.recentButtonSequencePosition++;
		player.PROPERTIES.recentButtonPressTime = DateTime.Now;
	}

	public Definitions.FighterSpecialMoves lookupPossibleCombo(FighterObject player)
	{
		Combo[] combosList = Definitions.CombosList;
		foreach (Combo combo in combosList)
		{
			if (combo.enabled && CompareMoves(combo, player))
			{
				return combo.SpecialMove;
			}
		}
		return Definitions.FighterSpecialMoves.nulll;
	}

	public bool CompareMoves(Combo move, FighterObject player)
	{
		if (player.PROPERTIES.recentButtonSequencePosition < move.Sequence.Length)
		{
			return false;
		}
		for (int i = 1; i <= move.Sequence.Length; i++)
		{
			if (player.PROPERTIES.recentButtonSequence[player.PROPERTIES.recentButtonSequencePosition - i] != move.Sequence[move.Sequence.Length - i])
			{
				return false;
			}
		}
		if (!move.IsSubMove)
		{
			player.PROPERTIES.recentButtonSequencePosition = 0L;
		}
		return true;
	}

	public static void WipeControllerStates()
	{
		for (int i = 0; i < FighterManager.humanPlayers.Count; i++)
		{
			if (FighterManager.humanPlayers[i].PROPERTIES.PlayerIndexControllerNumber.HasValue)
			{
				FighterManager.humanPlayers[i].PROPERTIES.previousGamePadState = null;
			}
		}
	}

	public void processRedBaronInput(FighterObject player, List<FighterObject> enemies)
	{
		if (!player.PROPERTIES.areWeHuman)
		{
			BunnyOfWar.AI.AI.doSomething(player, enemies);
			return;
		}
		InputFromAnywhere playerInput = InputManager.GetPlayerInput(player);
		handleMessagesInput(playerInput, player);
		handleQTEInput(playerInput, player);
		if (playerInput.START_pressed)
		{
			RandomStaticGlobals.pauseButtonPressed(broadcastThis: true);
		}
		if (playerInput.UP_held)
		{
			player.moveUp();
			player.moveUp();
		}
		if (playerInput.DOWN_held)
		{
			player.moveDown();
			player.moveDown();
		}
		if (playerInput.RIGHT_pressed)
		{
			FighterAttackCoordinator.ShootLaser(player);
		}
		if (playerInput.X_pressed)
		{
			FighterAttackCoordinator.ShootBullet(player);
		}
		if (playerInput.Y_pressed)
		{
			FighterAttackCoordinator.DropBomb(player);
		}
	}

	public static bool handleQTEInput(InputFromAnywhere anywhereInput, FighterObject player)
	{
		if (QuickTimeEventsManager.hasQTE)
		{
			int num = 0;
			if (anywhereInput.A_pressed)
			{
				num += QuickTimeEventsManager.ButtonWasPressed(Buttons.A, player);
			}
			if (anywhereInput.B_pressed)
			{
				num += QuickTimeEventsManager.ButtonWasPressed(Buttons.B, player);
			}
			if (anywhereInput.X_pressed)
			{
				num += QuickTimeEventsManager.ButtonWasPressed(Buttons.X, player);
			}
			if (anywhereInput.Y_pressed)
			{
				num += QuickTimeEventsManager.ButtonWasPressed(Buttons.Y, player);
			}
		}
		return QuickTimeEventsManager.hasQTE;
	}

	public static bool handleMessagesInput()
	{
		if (GraphicsManager.messages == null || GraphicsManager.messages.Count <= 0)
		{
			return false;
		}
		if (ScreenManager.CurrentScreen == ScreenManager.screens.Blank)
		{
			if (FighterManager.humanPlayers == null || FighterManager.humanPlayers.Count <= 0)
			{
				return false;
			}
			for (int i = 0; i < FighterManager.humanPlayers.Count; i++)
			{
				InputFromAnywhere playerInput = InputManager.GetPlayerInput(FighterManager.humanPlayers[i]);
				handleMessagesInput(playerInput, FighterManager.humanPlayers[i]);
			}
		}
		else
		{
			for (int i = 0; i < GraphicsManager.messages.Count; i++)
			{
				GraphicsManager.messages[i].HandleInput();
			}
		}
		return true;
	}

	public static void handleMessagesInput(InputFromAnywhere anywhereInput, FighterObject player)
	{
		if (GraphicsManager.messages == null || GraphicsManager.messages.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < GraphicsManager.messages.Count; i++)
		{
			if (GraphicsManager.messages[i].isActive)
			{
				GraphicsManager.messages[i].FigureOutInput(anywhereInput, player.PROPERTIES.PlayerIndexControllerNumber.Value);
				break;
			}
		}
	}

	public void processCutsceneOrQTEInput(FighterObject player)
	{
		InputFromAnywhere playerInput = InputManager.GetPlayerInput(player);
		handleMessagesInput(playerInput, player);
		handleQTEInput(playerInput, player);
		if (playerInput.START_pressed)
		{
			RandomStaticGlobals.pauseButtonPressed(broadcastThis: true);
		}
		if (player.PROPERTIES.health <= 0f)
		{
			player.onDeath();
			TriggerManager.SetTriggerEvent("QTEFAILEDLEVEL");
			if (FighterManager.getHumanPlayers(onlyLiving: true, canBeDying: false).Count == 0)
			{
				ScreenManager.GameOver();
			}
		}
	}

	public void processBrawlerInput(FighterObject player, List<FighterObject> enemies)
	{
		if (!player.PROPERTIES.areWeHuman)
		{
			BunnyOfWar.AI.AI.doSomething(player, enemies);
			return;
		}
		InputFromAnywhere playerInput = InputManager.GetPlayerInput(player);
		if (playerInput.START_pressed)
		{
			RandomStaticGlobals.pauseButtonPressed(broadcastThis: true);
		}
		if (handleQTEInput(playerInput, player))
		{
			return;
		}
		handleMessagesInput(playerInput, player);
		if (player.PROPERTIES.isDying || !player.PROPERTIES.isAlive)
		{
			return;
		}
		if (player.PROPERTIES.isStunned)
		{
			if (player.PROPERTIES.stunExpires < DateTime.Now)
			{
				player.PROPERTIES.isStunned = false;
			}
			if (player.PROPERTIES.isStunned)
			{
				return;
			}
		}
		if (playerInput.A_pressed)
		{
			player.JUMP.JumpUp();
		}
		if (playerInput.RIGHT_held)
		{
			player.moveRight();
		}
		if (playerInput.LEFT_held)
		{
			player.moveLeft();
		}
		if (playerInput.UP_held)
		{
			player.moveUp();
		}
		if (playerInput.DOWN_held)
		{
			player.moveDown();
		}
		if (playerInput.X_pressed || playerInput.Y_pressed)
		{
			FighterAttackCoordinator.quickAttack(player);
		}
	}

	public void processGunSmokeInput(FighterObject player, List<FighterObject> enemies)
	{
		if (!player.PROPERTIES.areWeHuman)
		{
			BunnyOfWar.AI.AI.doSomething(player, enemies);
			return;
		}
		InputFromAnywhere playerInput = InputManager.GetPlayerInput(player);
		handleMessagesInput(playerInput, player);
		handleQTEInput(playerInput, player);
		if (playerInput.START_pressed)
		{
			RandomStaticGlobals.pauseButtonPressed(broadcastThis: true);
		}
		if (player.PROPERTIES.isDying || !player.PROPERTIES.isAlive)
		{
			return;
		}
		if (player.PROPERTIES.isStunned)
		{
			if (player.PROPERTIES.stunExpires < DateTime.Now)
			{
				player.PROPERTIES.isStunned = false;
			}
			if (player.PROPERTIES.isStunned)
			{
				return;
			}
		}
		if (playerInput.UP_pressed)
		{
			player.moveUp();
		}
		if (playerInput.DOWN_pressed)
		{
			player.moveDownV2();
		}
		if (playerInput.LEFT_pressed)
		{
			player.moveLeft();
		}
		if (playerInput.RIGHT_pressed)
		{
			player.moveRight();
		}
		if (playerInput.UP_held)
		{
			player.moveUp();
		}
		if (playerInput.DOWN_held)
		{
			player.moveDownV2();
		}
		if (playerInput.LEFT_held)
		{
			player.moveLeft();
		}
		if (playerInput.RIGHT_held)
		{
			player.moveRight();
		}
		if (playerInput.X_pressed || playerInput.KB_A_pressed)
		{
			ProjectileManager.addNewProjectile(player.X - 20, player.Y, new Vector2(-1f, -1f), 1f, areWeHuman: true, player, ProjectileManager.ProjectileType.bullet, 20, 20, 10, broadcast: true);
			ProjectileManager.addNewProjectile(player.X + 20, player.Y, new Vector2(-0.75f, -1f), 1f, areWeHuman: true, player, ProjectileManager.ProjectileType.bullet, 20, 20, 10, broadcast: true);
		}
		if (playerInput.B_pressed || playerInput.D_pressed)
		{
			ProjectileManager.addNewProjectile(player.X + 20, player.Y, new Vector2(1f, -1f), 1f, areWeHuman: true, player, ProjectileManager.ProjectileType.bullet, 20, 20, 10, broadcast: true);
			ProjectileManager.addNewProjectile(player.X - 20, player.Y, new Vector2(0.75f, -1f), 1f, areWeHuman: true, player, ProjectileManager.ProjectileType.bullet, 20, 20, 10, broadcast: true);
		}
		if (playerInput.Y_pressed || playerInput.W_pressed)
		{
			ProjectileManager.addNewProjectile(player.X + 100, player.Y, new Vector2(0f, -1f), 1f, areWeHuman: true, player, ProjectileManager.ProjectileType.bullet, 20, 20, 10, broadcast: true);
			ProjectileManager.addNewProjectile(player.X - 100, player.Y, new Vector2(0f, -1f), 1f, areWeHuman: true, player, ProjectileManager.ProjectileType.bullet, 20, 20, 10, broadcast: true);
		}
	}

	public void processSpaceInput(FighterObject player, List<FighterObject> enemies)
	{
		if (!player.PROPERTIES.areWeHuman)
		{
			BunnyOfWar.AI.AI.doSomething(player, enemies);
			return;
		}
		InputFromAnywhere playerInput = InputManager.GetPlayerInput(player);
		handleMessagesInput(playerInput, player);
		handleQTEInput(playerInput, player);
		if (playerInput.START_pressed)
		{
			RandomStaticGlobals.pauseButtonPressed(broadcastThis: true);
		}
		if (player.PROPERTIES.isDying || !player.PROPERTIES.isAlive)
		{
			return;
		}
		if (player.PROPERTIES.isStunned)
		{
			if (player.PROPERTIES.stunExpires < DateTime.Now)
			{
				player.PROPERTIES.isStunned = false;
			}
			if (player.PROPERTIES.isStunned)
			{
				return;
			}
		}
		if (playerInput.UP_pressed)
		{
			player.moveUp();
		}
		if (playerInput.DOWN_pressed)
		{
			player.moveDownV2();
		}
		if (playerInput.LEFT_pressed)
		{
			player.moveLeft();
		}
		if (playerInput.RIGHT_pressed)
		{
			player.moveRight();
		}
		if (playerInput.UP_held)
		{
			player.moveUp();
		}
		if (playerInput.DOWN_held)
		{
			player.moveDownV2();
		}
		if (playerInput.LEFT_held)
		{
			player.moveLeft();
		}
		if (playerInput.RIGHT_held)
		{
			player.moveRight();
		}
		if (!playerInput.X_pressed && !playerInput.RIGHT_TRIGGER_pressed)
		{
			return;
		}
		for (int i = 0; i < enemies.Count; i++)
		{
			if (player.getWhereBodyIs().Intersects(enemies[i].getWhereBodyIs()))
			{
				enemies[i].onDeath();
			}
		}
	}

	public void processShooterInput(FighterObject player, List<FighterObject> enemies)
	{
		if (!player.PROPERTIES.areWeHuman)
		{
			BunnyOfWar.AI.AI.doSomething(player, enemies);
			return;
		}
		InputFromAnywhere playerInput = InputManager.GetPlayerInput(player);
		handleMessagesInput(playerInput, player);
		handleQTEInput(playerInput, player);
		if (playerInput.START_pressed)
		{
			RandomStaticGlobals.pauseButtonPressed(broadcastThis: true);
		}
		if (player.PROPERTIES.isDying || !player.PROPERTIES.isAlive)
		{
			return;
		}
		if (player.PROPERTIES.isStunned)
		{
			if (player.PROPERTIES.stunExpires < DateTime.Now)
			{
				player.PROPERTIES.isStunned = false;
			}
			if (player.PROPERTIES.isStunned)
			{
				return;
			}
		}
		if (playerInput.UP_pressed)
		{
			player.moveUp();
		}
		if (playerInput.DOWN_pressed)
		{
			player.moveDownV2();
		}
		if (playerInput.LEFT_pressed)
		{
			player.moveLeft();
		}
		if (playerInput.RIGHT_pressed)
		{
			player.moveRight();
		}
		if (playerInput.UP_held)
		{
			player.moveUp();
		}
		if (playerInput.DOWN_held)
		{
			player.moveDownV2();
		}
		if (playerInput.LEFT_held)
		{
			player.moveLeft();
		}
		if (playerInput.RIGHT_held)
		{
			player.moveRight();
		}
		if (!playerInput.X_pressed && !playerInput.RIGHT_TRIGGER_pressed)
		{
			return;
		}
		for (int i = 0; i < enemies.Count; i++)
		{
			if (player.getWhereBodyIs().Intersects(enemies[i].getWhereBodyIs()))
			{
				enemies[i].onDeath();
			}
		}
	}

	public void processFlappyInput(FighterObject player, List<FighterObject> enemies)
	{
		InputFromAnywhere playerInput = InputManager.GetPlayerInput(player);
		handleMessagesInput(playerInput, player);
		handleQTEInput(playerInput, player);
		if (GraphicsManager.messages != null && GraphicsManager.messages.Count > 0)
		{
			return;
		}
		if (playerInput.START_pressed)
		{
			RandomStaticGlobals.pauseButtonPressed(broadcastThis: true);
		}
		if (player.PROPERTIES.isDying || !player.PROPERTIES.isAlive)
		{
			return;
		}
		if (player.PROPERTIES.isStunned)
		{
			if (player.PROPERTIES.stunExpires < DateTime.Now)
			{
				player.PROPERTIES.isStunned = false;
			}
			if (player.PROPERTIES.isStunned)
			{
				return;
			}
		}
		if (playerInput.A_pressed)
		{
			player.Y -= 200;
			player.PlayAnimation(FighterObjectProperties.AnimationName.Jumping, broadcastThis: true);
		}
		if (playerInput.B_pressed)
		{
			InputManager.button1LastPressed = DateTime.Now;
		}
		if (playerInput.B_held)
		{
			if (player.PROPERTIES.CpuAttackCooldown < DateTime.Now)
			{
				player.PlayAnimation(FighterObjectProperties.AnimationName.PooingStarted, broadcastThis: true);
			}
			if (InputManager.button1LastPressed < DateTime.Now.AddSeconds(0f - RandomStaticGlobals.FlappyPoopFuse))
			{
				FighterAttackCoordinator.ShootFlappyProjectile(player);
				SoundManager.playNextFartStereo(0f);
				InputManager.button1LastPressed = DateTime.Now.AddSeconds(RandomStaticGlobals.FlappyPoopFuse * 4f);
				player.PROPERTIES.CpuAttackCooldown = DateTime.Now.AddSeconds(RandomStaticGlobals.FlappyPoopFuse * 4f);
				player.PlayAnimation(FighterObjectProperties.AnimationName.PooingFinished, broadcastThis: true);
			}
		}
		Vector2 zero = Vector2.Zero;
		float speed = 1.5f;
		int damage = 5;
		if (playerInput.LEFT_vector2 != Vector2.Zero)
		{
			ProjectileManager.addNewProjectile(direction: new Vector2(playerInput.LEFT_vector2.X + 0.5f, 0f - playerInput.LEFT_vector2.Y), x: player.X, y: player.Y, speed: speed, areWeHuman: player.PROPERTIES.areWeHuman, shooter: player, whatsIsIt: ProjectileManager.ProjectileType.pooSmall, width: 128, height: 128, damage: damage, broadcast: true);
		}
		player.moveDownV2(5 + RandomStaticGlobals.FlappySpeedUpDropDown);
	}

	public void processHelicopterInput(FighterObject player, List<FighterObject> enemies)
	{
		if (!player.PROPERTIES.areWeHuman)
		{
			BunnyOfWar.AI.AI.doSomething(player, enemies);
			return;
		}
		InputFromAnywhere playerInput = InputManager.GetPlayerInput(player);
		handleMessagesInput(playerInput, player);
		handleQTEInput(playerInput, player);
		if (playerInput.START_pressed)
		{
			RandomStaticGlobals.pauseButtonPressed(broadcastThis: true);
		}
		if (player.PROPERTIES.isDying || !player.PROPERTIES.isAlive)
		{
			return;
		}
		if (player.PROPERTIES.isStunned)
		{
			if (player.PROPERTIES.stunExpires < DateTime.Now)
			{
				player.PROPERTIES.isStunned = false;
			}
			if (player.PROPERTIES.isStunned)
			{
				return;
			}
		}
		if (playerInput.A_pressed || playerInput.UP_pressed)
		{
			player.moveUp();
		}
		if (playerInput.A_held || playerInput.UP_held)
		{
			player.moveUp();
			player.moveUp();
			player.moveUp();
		}
		if (playerInput.X_pressed || playerInput.RIGHT_pressed)
		{
			FighterAttackCoordinator.ShootLaser(player);
		}
		player.moveDownV2();
		player.moveDownV2();
	}

	public void processSwimmerInput(FighterObject player, List<FighterObject> enemies)
	{
		if (!player.PROPERTIES.areWeHuman)
		{
			BunnyOfWar.AI.AI.doSomething(player, enemies);
			return;
		}
		InputFromAnywhere playerInput = InputManager.GetPlayerInput(player);
		handleMessagesInput(playerInput, player);
		handleQTEInput(playerInput, player);
		if (playerInput.START_pressed)
		{
			RandomStaticGlobals.pauseButtonPressed(broadcastThis: true);
		}
		if (player.PROPERTIES.isDying || !player.PROPERTIES.isAlive)
		{
			return;
		}
		if (player.PROPERTIES.isStunned)
		{
			if (player.PROPERTIES.stunExpires < DateTime.Now)
			{
				player.PROPERTIES.isStunned = false;
			}
			if (player.PROPERTIES.isStunned)
			{
				return;
			}
		}
		if (playerInput.A_pressed || playerInput.UP_pressed)
		{
			player.JUMP.JumpUp();
		}
		if (playerInput.RIGHT_held)
		{
			player.moveRight();
		}
		if (playerInput.LEFT_held)
		{
			player.moveLeft();
		}
	}

	public void processRunnerInput(FighterObject player, List<FighterObject> enemies)
	{
		if (!player.PROPERTIES.areWeHuman)
		{
			BunnyOfWar.AI.AI.doSomething(player, enemies);
			return;
		}
		InputFromAnywhere playerInput = InputManager.GetPlayerInput(player);
		handleMessagesInput(playerInput, player);
		handleQTEInput(playerInput, player);
		if (playerInput.START_pressed)
		{
			RandomStaticGlobals.pauseButtonPressed(broadcastThis: true);
		}
		if (player.PROPERTIES.isDying || !player.PROPERTIES.isAlive)
		{
			return;
		}
		if (player.PROPERTIES.isStunned)
		{
			if (player.PROPERTIES.stunExpires < DateTime.Now)
			{
				player.PROPERTIES.isStunned = false;
			}
			if (player.PROPERTIES.isStunned)
			{
				return;
			}
		}
		if (playerInput.DOWN_pressed || playerInput.B_pressed)
		{
			player.onCrouch();
		}
		else if (player.PROPERTIES.isCrouching && player.PROPERTIES.crouchExpires < DateTime.Now)
		{
			player.onUnCrouch();
		}
		if (playerInput.A_pressed || playerInput.UP_pressed)
		{
			player.JUMP.JumpUp();
		}
		if (playerInput.Y_pressed || playerInput.RIGHT_pressed)
		{
			player.onKick();
		}
		else if (player.PROPERTIES.isKicking && player.PROPERTIES.kickExpires < DateTime.Now)
		{
			player.onUnKick();
		}
	}

	public void processInput(FighterObject player, List<FighterObject> enemies)
	{
		if (!player.PROPERTIES.areWeHuman)
		{
			BunnyOfWar.AI.AI.doSomething(player, enemies);
			return;
		}
		GamePadState? gamePadState = null;
		GamePadState? gamePadState2 = null;
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		if (player.PROPERTIES.PlayerIndexControllerNumber.HasValue)
		{
			gamePadState = GamePad.GetState(player.PROPERTIES.PlayerIndexControllerNumber.Value, GamePadDeadZone.Circular);
			gamePadState2 = ((!player.PROPERTIES.previousGamePadState.HasValue || !player.PROPERTIES.previousGamePadState.HasValue) ? gamePadState : player.PROPERTIES.previousGamePadState);
			player.PROPERTIES.previousGamePadState = gamePadState.Value;
			if (gamePadState.HasValue && gamePadState.Value.Buttons.Start == ButtonState.Released && gamePadState2.Value.Buttons.Start == ButtonState.Pressed)
			{
				RandomStaticGlobals.pauseButtonPressed(broadcastThis: true);
			}
			if (gamePadState.HasValue && !gamePadState.Value.IsConnected && !RandomStaticGlobals.isGamePaused && player.PROPERTIES.isAlive)
			{
				RandomStaticGlobals.pauseButtonPressed(broadcastThis: true);
				GraphicsManager.Message("Hey your controller was just unplugged!! Do you need new batteries?");
			}
			if (RandomStaticGlobals.isGamePaused)
			{
				player.PROPERTIES.previousKeyboardState = Keyboard.GetState();
				if (gamePadState.HasValue)
				{
					player.PROPERTIES.previousGamePadState = gamePadState.Value;
				}
			}
			else
			{
				if (player.PROPERTIES.isDying || !player.PROPERTIES.isAlive)
				{
					return;
				}
				if (player.PROPERTIES.isStunned)
				{
					if (player.PROPERTIES.stunExpires < DateTime.Now)
					{
						player.PROPERTIES.isStunned = false;
					}
					if (player.PROPERTIES.isStunned)
					{
						return;
					}
				}
				if (!player.PROPERTIES.PlayerIndexControllerNumber.HasValue || (!gamePadState.Value.IsConnected && player.PROPERTIES.isAlive && player.ID != -1))
				{
					BunnyOfWar.AI.AI.doSomething(player, enemies);
					return;
				}
				if (RandomStaticGlobals.isSkullSlingshotMode)
				{
					SkullSlingshotInput(player, gamePadState, gamePadState2);
					return;
				}
				if (player.PROPERTIES.isCountering)
				{
					if (gamePadState.Value.IsButtonDown(player.PROPERTIES.counterButton))
					{
						player.onCounter();
						return;
					}
					if (gamePadState.Value.IsButtonDown(Buttons.A) || gamePadState.Value.IsButtonDown(Buttons.B) || gamePadState.Value.IsButtonDown(Buttons.X) || gamePadState.Value.IsButtonDown(Buttons.Y))
					{
						player.PROPERTIES.isCountering = false;
					}
				}
				if (gamePadState.Value.DPad.Left == ButtonState.Pressed || gamePadState.Value.IsButtonDown(Buttons.LeftThumbstickLeft))
				{
					bool run = false;
					if (gamePadState.Value.Buttons.LeftStick == ButtonState.Pressed)
					{
						run = true;
					}
					flag = player.moveLeft(run);
					if (!gamePadState2.Value.IsButtonDown(Buttons.LeftThumbstickLeft))
					{
						addToButtonSequence(Buttons.LeftThumbstickLeft, player);
					}
				}
				if (gamePadState.Value.DPad.Right == ButtonState.Pressed || gamePadState.Value.IsButtonDown(Buttons.LeftThumbstickRight))
				{
					bool run = false;
					if (gamePadState.Value.Buttons.LeftStick == ButtonState.Pressed)
					{
						run = true;
					}
					flag = player.moveRight(run);
					if (!gamePadState2.Value.IsButtonDown(Buttons.LeftThumbstickRight))
					{
						addToButtonSequence(Buttons.LeftThumbstickRight, player);
					}
				}
				if (gamePadState.Value.DPad.Up == ButtonState.Pressed || gamePadState.Value.IsButtonDown(Buttons.LeftThumbstickUp))
				{
					flag = player.moveUp();
					if (!gamePadState2.Value.IsButtonDown(Buttons.LeftThumbstickUp))
					{
						addToButtonSequence(Buttons.LeftThumbstickUp, player);
					}
				}
				if (gamePadState.Value.DPad.Down == ButtonState.Pressed || gamePadState.Value.IsButtonDown(Buttons.LeftThumbstickDown))
				{
					flag = player.moveDown();
					if (!gamePadState2.Value.IsButtonDown(Buttons.LeftThumbstickDown))
					{
						addToButtonSequence(Buttons.LeftThumbstickDown, player);
					}
				}
				if (gamePadState.Value.Buttons.X == ButtonState.Pressed && gamePadState2.Value.Buttons.X == ButtonState.Released)
				{
					addToButtonSequence(Buttons.X, player);
				}
				if (gamePadState.Value.Buttons.Y == ButtonState.Pressed && gamePadState2.Value.Buttons.Y == ButtonState.Released)
				{
					addToButtonSequence(Buttons.Y, player);
				}
				if (gamePadState.Value.Buttons.B == ButtonState.Pressed && gamePadState2.Value.Buttons.B == ButtonState.Released)
				{
					addToButtonSequence(Buttons.B, player);
					player.X = 0;
					player.Y = 0;
					FighterManager.PickupOrThrowObject(player, gamePadState.Value.ThumbSticks.Right.X, gamePadState.Value.ThumbSticks.Right.Y);
				}
				if (gamePadState.Value.Buttons.A == ButtonState.Pressed)
				{
					if (gamePadState2.Value.Buttons.A == ButtonState.Released)
					{
						player.JUMP.JumpUp();
					}
					flag2 = true;
					flag3 = true;
					if (gamePadState2.Value.Buttons.A == ButtonState.Released)
					{
						addToButtonSequence(Buttons.A, player);
					}
				}
				if (gamePadState.Value.Buttons.RightShoulder == ButtonState.Pressed && gamePadState2.Value.Buttons.RightShoulder == ButtonState.Released)
				{
					addToButtonSequence(Buttons.RightShoulder, player);
				}
				if (gamePadState.Value.ThumbSticks.Right.X > Definitions.ControllerRightThumbstickMax && gamePadState2.Value.ThumbSticks.Right.X < Definitions.ControllerRightThumbstickMax)
				{
					addToButtonSequence(Buttons.RightThumbstickRight, player);
				}
				if (gamePadState.Value.ThumbSticks.Right.X < 0f - Definitions.ControllerRightThumbstickMax && gamePadState2.Value.ThumbSticks.Right.X > 0f - Definitions.ControllerRightThumbstickMax)
				{
					addToButtonSequence(Buttons.RightThumbstickLeft, player);
				}
				if (gamePadState.Value.ThumbSticks.Right.Y > Definitions.ControllerRightThumbstickMax && gamePadState2.Value.ThumbSticks.Right.Y < Definitions.ControllerRightThumbstickMax)
				{
					addToButtonSequence(Buttons.RightThumbstickUp, player);
				}
				if (gamePadState.Value.ThumbSticks.Right.Y < 0f - Definitions.ControllerRightThumbstickMax && gamePadState2.Value.ThumbSticks.Right.Y > 0f - Definitions.ControllerRightThumbstickMax)
				{
					addToButtonSequence(Buttons.RightThumbstickDown, player);
				}
				if (gamePadState.Value.Triggers.Right > 0.25f && gamePadState2.Value.Triggers.Right < 0.25f)
				{
					if (player.PROPERTIES.holdingObstacleObject != null)
					{
						FighterManager.PickupOrThrowObject(player, gamePadState.Value.ThumbSticks.Right.X, gamePadState.Value.ThumbSticks.Right.Y);
					}
					else if (player.PROPERTIES.carryingFighter != null)
					{
						FighterManager.PickupOrThrowObject(player, gamePadState.Value.ThumbSticks.Right.X, gamePadState.Value.ThumbSticks.Right.Y);
					}
					else
					{
						player.RANGED.rangedRelease(gamePadState.Value.ThumbSticks.Right.X, gamePadState.Value.ThumbSticks.Right.Y);
						FighterManager.BroadcastRangedAttack(player.ID, gamePadState.Value.ThumbSticks.Right.X, gamePadState.Value.ThumbSticks.Right.Y);
					}
					flag2 = true;
				}
				int num = 0;
				if (gamePadState.Value.Buttons.Y == ButtonState.Pressed)
				{
					num++;
				}
				if (lookupPossibleCombo(player) != Definitions.FighterSpecialMoves.nulll && RandomStaticGlobals.GameMode == Definitions.GameMode.brawler)
				{
					flag2 = true;
				}
				if (gamePadState.Value.Triggers.Left > 0f && !flag)
				{
					if (player.onBlock())
					{
						flag2 = true;
						if (gamePadState2.Value.Triggers.Left == 0f)
						{
							player.PROPERTIES.recentButtonPressTime = DateTime.Now;
							player.PROPERTIES.recentBlockPressTime = DateTime.Now;
						}
					}
				}
				else if (gamePadState.Value.Triggers.Left == 0f && gamePadState2.Value.Triggers.Left > 0.1f)
				{
					player.onUnBlock();
					flag2 = true;
				}
				if (!flag && !flag2)
				{
					player.onIdle();
					if (Definitions.Options.MercyOnOff)
					{
						player.healthBoostForResting();
					}
				}
				else
				{
					player.healthBoostTimerReset();
				}
				if (flag3)
				{
					NetworkGameplayManager.SendJumping(player.ID, player.X, player.Y, player.JUMP.jumpPixelsOffGround, player.PROPERTIES.areWeHuman);
				}
				else if (flag)
				{
					NetworkGameplayManager.SendFighterPosition(player.ID, player.X, player.Y);
				}
				else if (player.JUMP.isJumping && player.JUMP.jumpPixelsOffGround > 0)
				{
					NetworkGameplayManager.SendJumping(player.ID, player.X, player.Y, player.JUMP.jumpPixelsOffGround, player.PROPERTIES.areWeHuman);
				}
			}
		}
		else if (!player.PROPERTIES.areWeHuman)
		{
			BunnyOfWar.AI.AI.doSomething(player, enemies);
		}
	}

	private static void SkullSlingshotInput(FighterObject player, GamePadState? gamePadState, GamePadState? gamePadStatPrevious)
	{
		int iD = player.ID;
		float num = Definitions.SkullSlingMaxRange;
		float x = gamePadState.Value.ThumbSticks.Right.X;
		float num2 = gamePadState.Value.ThumbSticks.Right.Y * -1f;
		ref Vector2 reference = ref RandomStaticGlobals.SkullSlingshotCurrentPosition[iD];
		reference = new Vector2(RandomStaticGlobals.SkullSlingshotOrigin[iD].X + x * num, RandomStaticGlobals.SkullSlingshotOrigin[iD].Y + num2 * num);
		if (!(gamePadState.Value.Triggers.Right > 0.25f) || !(gamePadStatPrevious.Value.Triggers.Right < 0.25f))
		{
			return;
		}
		for (int i = 0; i < FighterManager.humanPlayers.Count; i++)
		{
			try
			{
				x = FighterManager.humanPlayers[i].PROPERTIES.previousGamePadState.Value.ThumbSticks.Right.X;
				num2 = FighterManager.humanPlayers[i].PROPERTIES.previousGamePadState.Value.ThumbSticks.Right.Y * -1f;
				Vector2 direction = new Vector2(x, num2 / 3f);
				direction.X *= -1f;
				direction.Y *= -1f;
				if (FighterManager.humanPlayers[i].PROPERTIES.isAlive && FighterManager.humanPlayers[i].PROPERTIES.isLocal)
				{
					ProjectileManager.addNewProjectile((int)RandomStaticGlobals.SkullSlingshotCurrentPosition[i].X, (int)RandomStaticGlobals.SkullSlingshotCurrentPosition[i].Y, direction, 3.3f, areWeHuman: true, player, ProjectileManager.ProjectileType.skull, Definitions.SkullSlingshotSize, Definitions.SkullSlingshotSize, 5, broadcast: true);
					ref Vector2 reference2 = ref RandomStaticGlobals.SkullSlingshotCurrentPosition[i];
					reference2 = RandomStaticGlobals.SkullSlingshotOrigin[i];
				}
			}
			catch (Exception)
			{
			}
		}
	}
}

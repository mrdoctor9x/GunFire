using Quantum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public unsafe class GameManager : MonoBehaviour
{
	public static int LocalPlayerID { get; private set; } = -1;
	public static EntityRef LocalPlayerEntity { get; private set; }
	private void Update()
	{
		if (QuantumRunner.Default == null)
			return;
		if (LocalPlayerID < 0)
		{
			var game = QuantumRunner.Default.Game;
			if (game == null)
				return;

			var localPlayers = game.GetLocalPlayers();
			if (localPlayers.Length > 0)
			{
				SetLocalPlayer(localPlayers[0]);
			}
		}

		if (LocalPlayerEntity == EntityRef.None)
		{
			SetLocalAgentEntity();
		}
	}
	private bool SetLocalPlayer(int index)
	{
		if (LocalPlayerID == index)
			return false;

		LocalPlayerID = index;
		LocalPlayerEntity = default;

		SetLocalAgentEntity();
		return true;
	}

	private void SetLocalAgentEntity()
	{
		if (QuantumRunner.Default == null)
			return;

		var frame = QuantumRunner.Default.Game.Frames.Predicted;
		if (frame == null)
			return;

		foreach (var pair in frame.Unsafe.GetComponentBlockIterator<PlayerLink>())
		{
            if (pair.Component->Player == LocalPlayerID)
            {
                LocalPlayerEntity = pair.Entity;
				break;
            }
        }
	}
}

using Photon.Deterministic;

namespace Quantum.Character;

unsafe class PlayerSpawnSystem : SystemSignalsOnly, ISignalOnPlayerDataSet
{
    public void OnPlayerDataSet(Frame frame, PlayerRef player)
    {
        var data = frame.GetPlayerData(player);

        // resolve the reference to the prototpye.
        var prototype = frame.FindAsset<EntityPrototype>(data.CharacterPrototype.Id);

        // Create a new entity for the player based on the prototype.
        var entity = frame.Create(prototype);

        // Create a PlayerLink component. Initialize it with the player. Add the component to the player entity.
        var playerLink = new PlayerLink()
        {
            Player = player,
        };
        frame.Add(entity, playerLink);
        
        if (frame.Unsafe.TryGetPointer<Transform3D>(entity, out var transform))
        {
            if (frame.Unsafe.TryGetPointer<MovementInfo>(entity, out var info))
            {
                info->bodyRotation = transform->Rotation;
            }
            var spawnPoints = frame.Unsafe.GetComponentBlockIterator<SpawnPoint>();
            foreach (var point in spawnPoints)
            {
                var pos = frame.Unsafe.GetPointer<Transform3D>(point.Entity)->Position;
                if (point.Component->index == player)
                {
                    transform->Position = pos;
                }
                
            }
        }
    }
}
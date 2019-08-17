using System;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Common.Utilities;
using Recube.Api.Block.Impl;
using Recube.Api.Entities;
using Recube.Api.Network.Entities;
using Recube.Api.Network.Extensions;
using Recube.Api.Network.Impl.Packets.Play;
using Recube.Api.Network.NetworkPlayer;
using Recube.Api.Network.Packets;
using Recube.Api.Network.Packets.Handler;

namespace Recube.Core.Network.Impl
{
	public class PlayPacketHandler : PacketHandler
	{
		private Player _player;

		public PlayPacketHandler(INetworkPlayer networkPlayer) : base(networkPlayer)
		{
		}

		public override void OnActive()
		{
			((NetworkPlayer.NetworkPlayer) NetworkPlayer).SetState(NetworkPlayerState.Play);

			NetworkPlayer.SendPacketAsync(new JoinGameOutPacket
			{
				EntityId = 1,
				Gamemode = 1,
				Dimension = 0,
				Difficulty = 0,
				MaxPlayers = 0,
				LevelType = "",
				ReducedDebugInfo = false
			});

			for (var x = 0; x < 1; x++)
			{
				for (var y = 0; y < 1; y++)
				{
					var fullChunk = false;
					var bitMask = 0b0000_0000_0000_0001;
					var buf = ByteBufferUtil.DefaultAllocator.Buffer();
					for (var i = 0; i < 4095; i++)
					{
						var b = Recube.Instance.BlockStateRegistry
							.GetStateByProperty(typeof(GrassBlock), typeof(GrassBlock.SnowyProperty), false);
						buf.WriteVarInt(b.Id);
					}

					var blockEntities = 0;
					/*Task.Run(() =>
					{*/
					_player.NetworkPlayer.WriteAsync(new ChunkDataPacketOutPacket
					{
						ChunkX = x,
						ChunkY = y,
						FullChunk = false,
						PrimaryBitMask = bitMask,
						ByteSize = buf.ReadableBytes,
						Data = buf.Array,
						NumberOfBlockEntities = 0
					});
					buf.SafeRelease();
					/*});*/
				}
			}


			Console.WriteLine("OKKK");
			_player.NetworkPlayer.FlushChannel();
			NetworkPlayer.SendPacketAsync(new SpawnPositionOutPacket
			{
				X = 0,
				Y = 100,
				Z = 0
			});
			NetworkPlayer.SendPacketAsync(new PlayerPositionAndLookOutPacket
			{
				X = 0,
				Y = 100,
				Z = 0,
				Yaw = 0,
				Pitch = 0,
				Flags = 0,
				TeleportId = 1
			});
		}

		public override void OnDisconnect()
		{
			if (_player == null) return;

			Recube.Instance.PlayerRegistry.Deregister(_player);
			Recube.Instance.EntityRegistry.DeregisterEntity(_player.EntityId);
			NetworkBootstrap.Logger.Info($"Player {_player.Username}[{_player.Uuid}] disconnected");
		}

		public override Task Fallback(IInPacket packet)
		{
			return Task.CompletedTask;
		}

		internal void SetPlayer(UUID uuid, string username)
		{
			_player = (Player) Recube.Instance.EntityRegistry.RegisterEntity(id =>
				new Player(id, uuid, NetworkPlayer, username));
			Recube.Instance.PlayerRegistry.Register(_player);
			NetworkBootstrap.Logger.Info(
				$"Player {_player.Username}[{_player.Uuid}] connected from {NetworkPlayer.Channel.RemoteAddress}");
		}
	}
}
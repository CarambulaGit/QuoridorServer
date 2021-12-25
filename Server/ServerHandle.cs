using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Project.Classes.Field;
using Project.Classes.Player;

namespace GameServer {
    class ServerHandle
    {

        public static Dictionary<Client, bool> restartRequest = new Dictionary<Client, bool>();
        public static void WelcomeResponse(int _fromClient, Packet _packet) {
            int _clientIdCheck = _packet.ReadInt();
            string _username = _packet.ReadString();
            restartRequest.Add(Server.clients[_fromClient], false);
            Console.WriteLine(
                $"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}.");
            if (_fromClient != _clientIdCheck) {
                Console.WriteLine(
                    $"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
            }
        }

        public static void MoveRequestReceived(int _fromClient, Packet _packet) {
            var moveType = (Player.MoveType) _packet.ReadInt();
            var pos = _packet.ReadPoint();
            var player = GameLogic.Game.FindPlayerWithNetworkId(_fromClient);
            switch (moveType) {
                case Player.MoveType.PlacingWall:
                    var wallType = (Wall.Type) _packet.ReadInt();
                    var wall = new Wall(pos, wallType);
                    if (player.TrySetWall(wall)) {
                        ServerSend.SetWall(player, wall);
                    }
                    Console.WriteLine($"Server received request SetWall {pos} by {player}");

                    break;
                case Player.MoveType.Moving:
                    if (player.TryMovePawn(pos)) {
                        ServerSend.Move(player, pos);
                    }
                    Console.WriteLine($"Server received request Move {pos} by {player}");

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public static void RestartRequestReceived(int _fromClient, Packet _packet) {
            restartRequest[Server.clients[_fromClient]] = true;
            if (restartRequest.Values.All(wantPlayAgain => wantPlayAgain))
            {
                ServerSend.RestartGame();
                foreach (var key in restartRequest.Keys)
                {
                    restartRequest[key] = false;
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Project.Classes.Field;
using Project.Classes.Player;

namespace GameServer {
    class ServerSend {
        private static void SendTCPData(int _toClient, Packet _packet) {
            _packet.WriteLength();
            Server.clients[_toClient].tcp.SendData(_packet);
        }

        private static void SendTCPDataToAll(Packet _packet) {
            _packet.WriteLength();
            for (int i = 1; i <= Server.NumOfPlayersToStart; i++) {
                Server.clients[i].tcp.SendData(_packet);
            }
        }

        private static void SendTCPDataToAll(int _exceptClient, Packet _packet) {
            _packet.WriteLength();
            for (int i = 1; i <= Server.NumOfPlayersToStart; i++) {
                if (i != _exceptClient) {
                    Server.clients[i].tcp.SendData(_packet);
                }
            }
        }

        public static void Welcome(int _toClient, string _msg) {
            using Packet _packet = new Packet((int) ServerPackets.welcome);
            _packet.Write(_msg);
            _packet.Write(_toClient);

            SendTCPData(_toClient, _packet);

            if (Server.clients.Values.Count(client => client.tcp.socket != null) != Server.NumOfPlayersToStart) return;
            StartGame();
        }

        public static void StartGame() {
            using Packet _packet = new Packet((int) ServerPackets.startGame);
            _packet.Write(Server.clients[1].id);
            _packet.Write(Server.clients[2].id);
            GameLogic.StartGame(Server.clients[1].id, Server.clients[2].id);
            SendTCPDataToAll(_packet);
        }

        public static void SetWall(Player player, Wall wall) {
            using Packet _packet = new Packet((int) ServerPackets.makeMove);
            _packet.Write(player.NetworkId);
            _packet.Write((int) Player.MoveType.PlacingWall);
            _packet.Write(wall.Pos);
            _packet.Write((int) wall.WallType);

            SendTCPDataToAll(_packet);
            
            Console.WriteLine($"Server send SetWall {wall} by {player}");
        }

        public static void Move(Player player, Point newPos) {
            using Packet _packet = new Packet((int) ServerPackets.makeMove);
            _packet.Write(player.NetworkId);
            _packet.Write((int) Player.MoveType.Moving);
            _packet.Write(newPos);

            SendTCPDataToAll(_packet);
            Console.WriteLine($"Server send Move {newPos} by {player}");
        }
        
        public static void RestartGame() {
            using Packet _packet = new Packet((int) ServerPackets.restartGame);
            // _packet.Write(true);
            GameLogic.Game.Restart();
            SendTCPDataToAll(_packet);
        }
    }
}
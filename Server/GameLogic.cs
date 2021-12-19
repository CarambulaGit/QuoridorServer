using Project.Classes;

namespace GameServer {
    class GameLogic {
        public static Game Game { get; private set; }

        public static void Update() {
            Game?.Tick();
        }

        public static void StartGame(int firstPlayerId, int secondPlayerId) {
            Game = Game.CreatePlayerVsPlayer(firstPlayerId, secondPlayerId);
            Game.StartGame();
        }
    }
}
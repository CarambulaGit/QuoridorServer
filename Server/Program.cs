using System;
using System.Threading;
using Project.Classes;

namespace GameServer {
    class Program {
        private static bool isRunning = false;

        private static void Main(string[] args) {
            Console.Title = "Game Server";
            isRunning = true;

            // Thread mainThread = new Thread(MainThread);
            // mainThread.Start();

            Server.Start(2, 26950);
            MainThread();
        }

        private static void MainThread() {
            Console.WriteLine($"Main thread started. Running at {Constants.TICKS_PER_SEC} ticks per second.");
            DateTime _nextLoop = DateTime.Now;

            while (isRunning) {
                while (_nextLoop < DateTime.Now) {
                    ThreadManager.UpdateMain();
                    
                    GameLogic.Update();

                    _nextLoop = _nextLoop.AddMilliseconds(Constants.MS_PER_TICK);

                    if (_nextLoop > DateTime.Now) {
                        Thread.Sleep(_nextLoop - DateTime.Now);
                    }
                }
            }
        }
    }
}
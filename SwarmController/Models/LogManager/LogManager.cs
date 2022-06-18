using SwarmController.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace SwarmController.Models.Log
{
    public class LogManager
    {
        private static LogManager logManager = null;
        public static LogManager getLogManager()
        {
            if (logManager == null)
            {
                logManager = new LogManager();
            }

            return logManager;
        }

        public Queue<string> logList = new Queue<string>();

        public void addLog(string log)
        {
            //(Application.Current.MainWindow as MainWindow).addLog(log);
            logList.Enqueue(getCurrentTime() + " | " + log);
        }

        public string getCurrentTime()
        {
            return DateTime.Now.ToString();

        }

    }
}

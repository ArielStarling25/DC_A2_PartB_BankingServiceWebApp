namespace BankServiceGUI.Models
{
    public class LogClass
    {
        public static List<Log> logs = new List<Log>();
        public static int counter = 0;
        public static void LogItem(string admin, string severity, string message)
        {
            counter++;
            Log log = new Log();
            log.Id = counter;
            log.admin = admin;
            log.severity = severity;
            log.message = message;
            logs.Add(log);
        }

        public static List<Log> getLogs()
        {
            return logs;
        }
    }
}

using System.IO;

namespace ServerApp.Models
{
    public class Logger : Extensions.BindableBase
    {
        private string _logText;

        public string LogText
        {
            get => _logText;
            set
            {
                var tmpStrArr = value.Split('\n');
                LogToFile(tmpStrArr[tmpStrArr.Length - 2]); //-1 т.к. последний элемент всегда "", -1 т.к. это длина а не индекс
                SetProperty(ref _logText, value);
            }
        }

        public Logger()
        {
            using (StreamWriter sWriter = new StreamWriter("logs.txt", false, System.Text.Encoding.Default))
            {
            }
        }

        public void AddInfo(string information)
        {
            LogText += information + "\n";
        }

        public void LogToFile(string textToFile)
        {
            using (StreamWriter sWriter = new StreamWriter("logs.txt", true, System.Text.Encoding.Default))
            {
                sWriter.WriteLine(textToFile);
            }
        }
    }
}
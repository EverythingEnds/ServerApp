using System;

namespace ServerApp.Models
{
    [Serializable]
    public class AnswerToClient
    {
        public int Number { get; set; }
        public DateTime Date { get; set; }

        public AnswerToClient(int number, DateTime date)
        {
            Number = number;
            Date = date;
        }

        public AnswerToClient() //для сериализации
        {
        }
    }
}
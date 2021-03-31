using Extensions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace ServerApp.Models
{
    public class ConnectionDispatcher : BindableBase
    {
        #region Props and fields

        public delegate void GasEventHandler(GasMeter gasObj);

        public event GasEventHandler GasValueChanged;

        private TcpListener serverSocket = new TcpListener(IPAddress.Parse("127.0.0.1"), 8070);

        private GasMeter _recievedGasValues;

        public GasMeter RecievedGasValues
        {
            get => _recievedGasValues;
            private set
            {
                SetProperty(ref _recievedGasValues, value);
                var tmpDots = new KeyValuePair<DateTime, double>[_graph.Dots.Length + 1];
                for (int i = 0; i < tmpDots.Length - 1; i++)
                {
                    tmpDots[i] = _graph.Dots[i];
                }
                tmpDots[tmpDots.Length - 1] = new KeyValuePair<DateTime, double>(value.Date, value.Oxygen);
                _graph.Dots = tmpDots;
                GasValueChanged.Invoke(value);
            }
        }

        private Graph _graph;
        public bool Saving { get; set; }

        #endregion Props and fields

        public ConnectionDispatcher(Graph ScatterPlot)
        {
            _graph = ScatterPlot;
        }

        public void StartHosting()
        {
            serverSocket.Start();
            while (true)
            {
                TcpClient clientSocket = serverSocket.AcceptTcpClient();
                NetworkStream stream = clientSocket.GetStream();
                byte[] bytes = new byte[256];
                int countOfReading = stream.Read(bytes, 0, bytes.Length);
                string gasJson = Encoding.ASCII.GetString(bytes, 0, countOfReading);
                RecievedGasValues = JsonSerializer.Deserialize<GasMeter>(gasJson);

                var answer = new AnswerToClient(RecievedGasValues.Number, DateTime.Now);
                var answerJson = JsonSerializer.Serialize(answer);
                var message = Encoding.ASCII.GetBytes(answerJson);
                stream.Write(message, 0, message.Length);
                stream.Flush();
                clientSocket.Close();
            }
        }

        public void StopHosting()
        {
            serverSocket.Stop();
        }
    }
}
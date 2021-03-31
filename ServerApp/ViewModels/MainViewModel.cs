using Extensions;
using ServerApp.Models;
using System.Threading.Tasks;

namespace ServerApp.ViewModels
{
    public class MainViewModel : BindableBase
    {
        #region Props and fields

        public ConnectionDispatcher Server { get; private set; }
        public Graph ScatterPlotDots { get; private set; } = new Graph();
        public DataBaseController DataBase { get; private set; } = new DataBaseController();
        public Logger AppLogger { get; private set; } = new Logger();
        private bool _isHosting;

        public bool IsHosting
        {
            get => _isHosting;
            set => SetProperty(ref _isHosting, value);
        }

        private bool _isConnected;

        public bool IsConnected
        {
            get => _isConnected;
            set => SetProperty(ref _isConnected, value);
        }

        private bool _isSaving;

        public bool IsSaving
        {
            get => _isSaving;
            set => SetProperty(ref _isSaving, value);
        }

        private string _lastDbPath;

        public RelayCommand ConnectDB => new RelayCommand
            (
            () => DbChoose()
            );

        public RelayCommand CreateTable => new RelayCommand
        (
        () =>
        {
            if (DataBase.CreateGasEntity())
                AppLogger.AddInfo("Создана сущность GAS_VALUES.");
            else
                AppLogger.AddInfo("Не удалось создать сущность, возможно таблица не подключена или заблокирована другим процессом.");
        }
        );

        public RelayCommand HostingSwitch => new RelayCommand(
            () =>
            {
                if (IsHosting)
                {
                    AppLogger.AddInfo("Остановка работы сервера.");
                    IsHosting = false;
                    Task.Run
                    (
                    () => Server.StopHosting()
                    );
                }
                else
                {
                    AppLogger.AddInfo("Начало работы сервера.");
                    IsHosting = true;
                    Task.Run
                   (
                   () => Server.StartHosting()
                   );
                }
            });

        #endregion Props and fields

        public MainViewModel()
        {
            AppLogger.AddInfo("Начало работы программы.");
            Server = new ConnectionDispatcher(ScatterPlotDots);
            IsSaving = true;
            Server.GasValueChanged += GasWrite;
        }

        private void DbChoose()
        {
            var openFileDialog1 = new Microsoft.Win32.OpenFileDialog();
            openFileDialog1.Filter = "Файлы DB (*.mdb)|*.mdb";
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName.Length > 0)
            {
                if (_lastDbPath != openFileDialog1.FileName)
                    if (DataBase.ConnectTry(openFileDialog1.FileName))
                    {
                        _lastDbPath = openFileDialog1.FileName;
                        IsConnected = true;
                        AppLogger.AddInfo("База данных подключена.");
                    }
                    else
                    {
                        IsConnected = false;
                        AppLogger.AddInfo("Не удалось подключить БД.");
                    }
                else
                    AppLogger.AddInfo("Данная БД уже была подключена.");
            }
        }

        private void GasWrite(GasMeter gasObj)
        {
            if (IsSaving)
                if (IsConnected)
                    if (DataBase.TryFillAndUpdate(gasObj))
                        AppLogger.AddInfo("Данные получены и записаны в базу данных.");
                    else
                        AppLogger.AddInfo("Данные получены, но не были записаны в базу данных.");
                else
                    AppLogger.AddInfo("Данные не записываются по причине не подключённого файла БД.");
        }
    }
}
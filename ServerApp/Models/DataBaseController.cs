using System;
using System.Data;
using System.Data.OleDb;

namespace ServerApp.Models
{
    public class DataBaseController
    {
        private string _dBConnection;
        public OleDbConnection MyConnection;
        public bool IsOpened;
        private OleDbDataAdapter _adapter;
        private int _keyCounter;
        private DataTable _currentTable;
        private OleDbCommandBuilder _builder;

        public DataBaseController()
        {
            IsOpened = false;
            _currentTable = new DataTable();
        }

        public bool TryFillAndUpdate(GasMeter gasValues)
        {
            var select = "SELECT * FROM GAS_VALUES";
            _adapter.SelectCommand = new OleDbCommand(select, MyConnection);
            try
            {
                _adapter.Fill(_currentTable);
                var tmpGasValues = new object[5];
                _keyCounter = GetLastKey();
                tmpGasValues[0] = ++_keyCounter;
                tmpGasValues[1] = gasValues.Number;
                tmpGasValues[2] = DateTime.Now;
                tmpGasValues[3] = gasValues.Hydrogen;
                tmpGasValues[4] = gasValues.Oxygen;
                _currentTable.Rows.Add(tmpGasValues);
                _adapter.UpdateCommand = _builder.GetUpdateCommand();
                _adapter.Update(_currentTable);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool CreateGasEntity()
        {
            var createCommand = new OleDbCommand();
            createCommand.Connection = MyConnection;
            createCommand.CommandText = "CREATE TABLE GAS_VALUES (KeyID INT,GAS_VAL_ID INT,GAS_VAL_DATE DATETIME,H2_VAL DOUBLE,O2_VAL DOUBLE,PRIMARY KEY (KeyID))";
            try
            {
                createCommand.ExecuteNonQuery();
            }
            catch
            {
                try
                {
                    createCommand.CommandText = "DROP TABLE GAS_VALUES";
                    createCommand.ExecuteNonQuery();
                    createCommand.CommandText = "CREATE TABLE GAS_VALUES (KeyID INT,GAS_VAL_ID INT,GAS_VAL_DATE DATETIME,H2_VAL DOUBLE,O2_VAL DOUBLE,PRIMARY KEY (KeyID))";
                    createCommand.ExecuteNonQuery();
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        private int GetLastKey()
        {
            var lastSelectCommand = _adapter.SelectCommand;
            var command = "SELECT KeyID FROM GAS_VALUES";
            _adapter.SelectCommand = new OleDbCommand(command, MyConnection);
            _adapter.SelectCommand.ExecuteNonQuery();
            var tmpIdTable = new DataTable();
            _adapter.Fill(tmpIdTable);
            var indexOfLast = tmpIdTable.Rows.Count - 1;
            _adapter.SelectCommand = lastSelectCommand;
            if (indexOfLast >= 0)
                return (int) tmpIdTable.Rows[tmpIdTable.Rows.Count - 1][0];
            else
                return 0;
        }

        public bool ConnectTry(string fileName)
        {
            if (!IsOpened)
            {
                _dBConnection = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileName;
                MyConnection = new OleDbConnection(_dBConnection);
                try
                {
                    MyConnection.Open();
                }
                catch
                {
                    return false;
                }
                IsOpened = true;
                _adapter = new OleDbDataAdapter("", MyConnection);
                _builder = new OleDbCommandBuilder(_adapter);
                return true;
            }
            else
                return false;
        }

        public bool DisconnectTry()
        {
            if (IsOpened)
            {
                MyConnection.Close();
                IsOpened = false;
                this.Clear();
                return true;
            }
            else
                return false;
        }

        public void Clear()
        {
            _adapter?.Dispose();
            _builder?.Dispose();
            _currentTable?.Clear();
        }
    }
}
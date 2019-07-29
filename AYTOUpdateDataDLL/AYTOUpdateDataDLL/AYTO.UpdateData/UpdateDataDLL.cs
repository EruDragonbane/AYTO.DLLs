using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace AYTO.UpdateData
{
    public class UpdateDataDLL
    {
        SqlConnection updateDataConnection = new SqlConnection("server=ERU; Initial Catalog=deneme;Integrated Security=SSPI");

        //AdminPanel'den gelen verinin bilgiler
        public Tuple<string, string, string, string, string, string, string, Tuple<string, string>> TextGridFromAdminPanel(string TableName, string DataFromAdminPanel)
        {
            string updateDataCmdText = "";
            string columnName = "";
            string returnValue = "";
            string userName = "";
            string userSurname = "";
            string userID = "";
            string userPosition = "";
            string userAuthority = "";
            string userCorp = "";
            string statusName = "";
            string positionName = "";

            updateDataConnection.Close();

            if (TableName == "durumlar")
            {
                columnName = "durum";
            }
            else if (TableName == "gorevler")
            {
                columnName = "gorev";
            }
            else
            {
                columnName = "";
            }

            if (TableName == "kullanicilar")
            {
                updateDataCmdText = "SELECT klnc.kullaniciAdi, klnc.kullaniciSoyadi, klnc.kullaniciGiris, grv.gorevAdi, ytk.yetkiAdi, klnc.kullaniciKurumu FROM kullanicilar AS klnc INNER JOIN gorevler AS grv ON klnc.gorevNo = grv.gorevNo INNER JOIN yetkiler AS ytk ON klnc.yetkiNo = ytk.yetkiNo WHERE kullaniciNo = @gelenVeri";
            }
            else if (TableName == "durumlar" || TableName == "gorevler")
            {
                updateDataCmdText = "SELECT " + columnName + "Adi FROM " + TableName + " WHERE " + columnName + "No = @gelenVeri";
            }
            else
            {
                returnValue = "error";
            }

            SqlCommand updateDataCmd = new SqlCommand(updateDataCmdText, updateDataConnection);
            updateDataCmd.Parameters.AddWithValue("@gelenVeri", DataFromAdminPanel);
            updateDataConnection.Open();
            SqlDataReader updateDataReader = updateDataCmd.ExecuteReader();
            if (updateDataReader.Read())
            {
                if(TableName == "kullanicilar")
                {
                    userName = updateDataReader["kullaniciAdi"].ToString();
                    userSurname= updateDataReader["kullaniciSoyadi"].ToString();
                    userID= updateDataReader["kullaniciGiris"].ToString();
                    userPosition = updateDataReader["gorevAdi"].ToString();
                    userAuthority = updateDataReader["yetkiAdi"].ToString();
                    userCorp = updateDataReader["kullaniciKurumu"].ToString();
                }
                else if(TableName == "durumlar")
                {
                    statusName = updateDataReader["durumAdi"].ToString();
                }
                else if(TableName == "gorevler")
                {
                    positionName = updateDataReader["gorevAdi"].ToString();
                }
                else
                {
                    returnValue = "error";
                }
            }
            updateDataReader.Close();
            updateDataConnection.Close();
            var statusPositionTuple = Tuple.Create(statusName, positionName);
            var textGridTuple = new Tuple<string, string, string, string, string, string, string, Tuple<string, string>>(returnValue, userName, userSurname, userID, userPosition, userAuthority, userCorp, statusPositionTuple);
            return textGridTuple;
        }
        //Güncellenme durumunda değişen combobox verilerinin birincil anahtarlarını veritabanından çeker.
        public int ComboBoxNameTableValue(int inputValue, string position, string authority)
        {
            string inputTableName = "";
            string inputColumnName = "";
            string inputComboBoxText = "";
            int returnTableColumnNoValue = 0;

            if (inputValue == 1)
            {
                inputTableName = "gorevler";
                inputColumnName = "gorev";
                inputComboBoxText = position;
            }
            else if (inputValue == 2)
            {
                inputTableName = "yetkiler";
                inputColumnName = "yetki";
                inputComboBoxText = authority;
            }
            else
            {
                inputTableName = "";
                inputColumnName = "";
                inputComboBoxText = "";
            }

            updateDataConnection.Close();

            string inputDataCmdText = "SELECT " + inputColumnName + "No FROM " + inputTableName + " WHERE " + inputColumnName + "Adi = @gelenVeri";
            SqlCommand inputDataCmd = new SqlCommand(inputDataCmdText, updateDataConnection);
            inputDataCmd.Parameters.AddWithValue("@gelenVeri", inputComboBoxText);
            updateDataConnection.Open();
            SqlDataReader inputDataReader = inputDataCmd.ExecuteReader();
            if (inputDataReader.Read())
            {
                returnTableColumnNoValue = Convert.ToInt32(inputDataReader[inputColumnName + "No"]);
            }
            inputDataReader.Close();
            updateDataConnection.Close();

            return returnTableColumnNoValue;
        }

        public Tuple<string, string> UpdateData(string TableName, string status, string position, string userName, string userSurname, string userID, string userAuthority, string userCorp, string DataFromAdminPanel)
        {
            updateDataConnection.Close();

            string updateDataCmdText = "";
            string updateColumnName = "";
            string comboBoxValue = "";
            string updateTitle = "";
            if (TableName == "durumlar")
            {
                updateColumnName = "durum";
                comboBoxValue = status;
                updateTitle = "Durum";
            }
            else if (TableName == "gorevler")
            {
                updateColumnName = "gorev";
                comboBoxValue = position;
                updateTitle = "Görev";
            }
            else
            {
                updateColumnName = "";
                updateTitle = "Kullanıcı";
            }

            SqlCommand updateDataCmd;

            if (TableName == "kullanicilar")
            {
                updateDataCmdText = "UPDATE kullanicilar SET kullaniciAdi = @kullaniciAdi, kullaniciSoyadi = @kullaniciSoyadi, kullaniciGiris = @kullaniciGiris, gorevNo = @gorevNo, yetkiNo = @yetkiNo, kullaniciKurumu = @kullaniciKurumu WHERE kullaniciNo = @kullaniciNo";
                updateDataCmd = new SqlCommand(updateDataCmdText, updateDataConnection);
                updateDataCmd.Parameters.AddWithValue("@kullaniciAdi", userName);
                updateDataCmd.Parameters.AddWithValue("@kullaniciSoyadi", userSurname);
                updateDataCmd.Parameters.AddWithValue("@kullaniciGiris", userID);
                updateDataCmd.Parameters.AddWithValue("@gorevNo", ComboBoxNameTableValue(1, position, userAuthority));
                updateDataCmd.Parameters.AddWithValue("@yetkiNo", ComboBoxNameTableValue(2, position, userAuthority));
                updateDataCmd.Parameters.AddWithValue("@kullaniciKurumu", userCorp);
                updateDataCmd.Parameters.AddWithValue("@kullaniciNo", DataFromAdminPanel);
                updateDataConnection.Open();
                updateDataCmd.ExecuteNonQuery();
                updateDataCmd.Dispose();

                updateDataConnection.Close();
            }
            else if (TableName == "durumlar" || TableName == "gorevler")
            {
                updateDataCmdText = "UPDATE " + TableName + " SET " + updateColumnName + "Adi = @gelenVeri WHERE " + updateColumnName + "No = @gelenVeriNo";
                updateDataCmd = new SqlCommand(updateDataCmdText, updateDataConnection);
                updateDataCmd.Parameters.AddWithValue("@gelenVeri", comboBoxValue);
                updateDataCmd.Parameters.AddWithValue("@gelenVeriNo", DataFromAdminPanel);
                updateDataConnection.Open();
                updateDataCmd.ExecuteNonQuery();
                updateDataCmd.Dispose();

                updateDataConnection.Close();
            }
            else
            {
                updateDataCmdText = "";
                updateColumnName = "";
                comboBoxValue = "";
                updateTitle = "";
            }

            var updateDataTuple = new Tuple<string, string>(updateTitle, comboBoxValue);
            return updateDataTuple;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace AYTO.NewData
{
    public class NewDataDLL
    {
        SqlConnection newDataConnection = new SqlConnection("server=ERU; Initial Catalog=deneme;Integrated Security=SSPI");

        //Eklenmek istenen kullanını var olup olmadığını kontrol eder.
        public string CheckUser(string TableName, string userID, string statusName, string positionName)
        {
            string returnValue = "";
            string inputData = "";
            string inputTableName = "";
            string checkColumn = "";
            if(TableName == "kullanicilar")
            {
                inputData = userID;
                inputTableName = "kullanicilar";
                checkColumn = "kullaniciGiris";
            }
            else if(TableName == "durumlar")
            {
                inputData = statusName;
                inputTableName = "durumlar";
                checkColumn = "durumAdi";
            }
            else if(TableName == "gorevler")
            {
                inputData = positionName;
                inputTableName = "gorevler";
                checkColumn = "gorevAdi";
            }
            else
            {
                inputData = "";
                inputTableName = "";
                checkColumn = "";
            }

            newDataConnection.Close();

            string checkCmdText = "SELECT " + checkColumn + " FROM " + inputTableName + " WHERE " + checkColumn + " = @gelenVeri";
            using (SqlCommand checkCmd = new SqlCommand(checkCmdText, newDataConnection))
            {
                checkCmd.Parameters.AddWithValue("@gelenVeri", inputData);
                newDataConnection.Open();
                using (SqlDataReader checkCmdReader = checkCmd.ExecuteReader())
                {
                    if (checkCmdReader.Read() == false)
                    {
                        returnValue = "false";
                    }
                    else
                    {
                        returnValue = "true";
                    }
                }
            }
            newDataConnection.Close();
            return returnValue;
        }
        //Verilerin veritabanına kaydedilmesi
        public string ComboboxNameTableValue(int selectedColumnNo, string selectedComboItem)
        {
            string returnColumnValue = "";
            string columnName = "";
            string tableName = "";

            newDataConnection.Close();

            if(selectedColumnNo == 1)
            {

                columnName = "gorev";
                tableName = "gorevler";
            }
            else if(selectedColumnNo == 2)
            {
                columnName = "yetki";
                tableName = "yetkiler";
            }
            else
            {
                columnName = "";
                tableName = "";
                selectedComboItem = "";
            }

            if (columnName != string.Empty || tableName != string.Empty || selectedComboItem != string.Empty)
            {
                string itemNoCmdText = "SELECT " + columnName + "No FROM " + tableName + " WHERE " + columnName + "Adi = @sutunAdi";
                using (SqlCommand itemNoCmd = new SqlCommand(itemNoCmdText, newDataConnection))
                {
                    itemNoCmd.Parameters.AddWithValue("@sutunAdi", selectedComboItem);
                    newDataConnection.Open();
                    using (SqlDataReader itemNoReader = itemNoCmd.ExecuteReader())
                    {
                        if (itemNoReader.Read())
                        {
                            returnColumnValue = itemNoReader[columnName + "No"].ToString();
                        }
                    }
                }
                newDataConnection.Close();
            }
            return returnColumnValue;
        }
        //Girilen parolayı md5 ile şifreleyerek veritabanına ekler
        public string Mda5Hash(string inputPass)
        {
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                StringBuilder hashString = new StringBuilder();
                byte[] hashArray = md5.ComputeHash(Encoding.UTF8.GetBytes(inputPass));
                foreach (byte byteHash in hashArray)
                    hashString.Append(byteHash.ToString("x2"));
                return hashString.ToString();
            }
        }
        public void AddNewUser(string userName, string userSurname, string userID, string userPassword, string userCorp, string positionSelectedItem, string authoritySelectedItem)
        {
            newDataConnection.Close();

            string addNewUserCmdText = "INSERT INTO kullanicilar (kullaniciAdi, kullaniciSoyadi, kullaniciGiris, kullaniciParola, gorevNo, sistemKayitTarihi, kullaniciKurumu, yetkiNo) VALUES (@kullaniciAdi, @kullaniciSoyadi, @kullaniciGiris, @kullaniciParola, @gorevNo, @sistemKayitTarihi, @kullaniciKurumu, @yetkiNo)";
            using (SqlCommand addNewUserCmd = new SqlCommand(addNewUserCmdText, newDataConnection))
            {
                addNewUserCmd.Parameters.AddWithValue("@kullaniciAdi", userName);
                addNewUserCmd.Parameters.AddWithValue("@kullaniciSoyadi", userSurname);
                addNewUserCmd.Parameters.AddWithValue("@kullaniciGiris", userID);
                addNewUserCmd.Parameters.AddWithValue("@kullaniciParola", Mda5Hash(userPassword));
                addNewUserCmd.Parameters.AddWithValue("@gorevNo", ComboboxNameTableValue(1, positionSelectedItem));
                addNewUserCmd.Parameters.AddWithValue("@sistemKayitTarihi", DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")));
                addNewUserCmd.Parameters.AddWithValue("@kullaniciKurumu", userCorp);
                addNewUserCmd.Parameters.AddWithValue("@yetkiNo", ComboboxNameTableValue(2, authoritySelectedItem));
                newDataConnection.Open();
                addNewUserCmd.ExecuteNonQuery();
            }
            newDataConnection.Close();
        }

        public void AddNewStatus(string statusName)
        {
            newDataConnection.Close();

            string addNewStatusCmdText = "INSERT INTO durumlar (durumAdi) VALUES (@durumAdi)";
            using (SqlCommand addNewStatusCmd = new SqlCommand(addNewStatusCmdText, newDataConnection))
            {
                addNewStatusCmd.Parameters.AddWithValue("@durumAdi", statusName);
                newDataConnection.Open();
                addNewStatusCmd.ExecuteNonQuery();
            }
            newDataConnection.Close();
        }

        public void AddNewPosition(string positionName)
        {
            newDataConnection.Close();

            string addNewStatusCmdText = "INSERT INTO gorevler (gorevAdi) VALUES (@gorevAdi)";
            using (SqlCommand addNewStatusCmd = new SqlCommand(addNewStatusCmdText, newDataConnection))
            {
                addNewStatusCmd.Parameters.AddWithValue("@gorevAdi", positionName);
                newDataConnection.Open();
                addNewStatusCmd.ExecuteNonQuery();
            }
            newDataConnection.Close();
        }
    }
}

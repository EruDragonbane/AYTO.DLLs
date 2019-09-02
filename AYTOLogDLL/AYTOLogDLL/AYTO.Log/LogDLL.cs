using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SqlClient;
namespace AYTO.Log
{
    public class LogDLL
    {
        SqlConnection logConnection = new SqlConnection("Data Source = ERU; Initial Catalog = deneme; Integrated Security = SSPI");
        private const string logPath = @"C:\Users\Fatih\Desktop\ServerLogKaydi\";

        public void LoginLog(int kullaniciNo, string userAuthority, string kullaniciAdi, string kullaniciSoyadi)
        {
            string logFilePath = logPath + @"LoginLog.txt";
            string writeText = "[" + DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss") + "]: Giriş " + userAuthority + " No: " + kullaniciNo + "\tAdı: " + kullaniciAdi + ' ' + kullaniciSoyadi;
            FileStream loginLogFS = new FileStream(logFilePath, FileMode.OpenOrCreate, FileAccess.Write);
            loginLogFS.Close();
            File.AppendAllText(logFilePath, Environment.NewLine + writeText);
        }

        public void UpdateFileLog(int UserId, string BelgeNo, string fileName, string oldFileName)
        {
            string logFilePath = logPath + @"UpdateFileLog.txt";
            string writeText = "[" + DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss") + "]: Belge güncelleme işlemini yapan Kullanıcı No: " + UserId + "\tBelge No: " + BelgeNo + "\tEski Belge Adı: " + oldFileName + "\tYeni Belge Adı: " + fileName;
            FileStream adminLogFS = new FileStream(logFilePath, FileMode.OpenOrCreate, FileAccess.Write);
            adminLogFS.Close();
            File.AppendAllText(logFilePath, Environment.NewLine + writeText);
        }

        public void UpdateDataLog(string TableName, int UpdateDataUserId, string DataFromAdminPanel, string title, string userName, string userSurname, string comboBoxDataName, string oldDataName)
        {
            string columnNameValue = "";
            if (TableName == "kullanicilar")
            {
                columnNameValue = userName + ' ' + userSurname;
            }
            else
            {
                columnNameValue = comboBoxDataName;
            }
            string logFilePath = logPath + @"AdminLog\UpdateDataLog.txt";
            string writeText = "[" + DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss") + "]: Veri güncelleme yapan Yetkili No: " + UpdateDataUserId + "\tGüncellenen " + title + " No: " + DataFromAdminPanel + "\tEski" + title + " Adı: " + oldDataName + "\tYeni" + title + " Adı: " + columnNameValue;
            FileStream adminLogFS = new FileStream(logFilePath, FileMode.OpenOrCreate, FileAccess.Write);
            adminLogFS.Close();
            File.AppendAllText(logFilePath, Environment.NewLine + writeText);
        }

        public void NewFileLog(int UserId2, string fileName)
        {
            string fileNameDB = "";
            logConnection.Close();
            using (SqlCommand fileNameCmd = new SqlCommand("SELECT belgeNo FROM belgelerim WHERE belgeAdi = @belgeAdi", logConnection))
            {
                fileNameCmd.Parameters.AddWithValue("@belgeAdi", fileName);
                logConnection.Open();
                using (SqlDataReader fileNameReader = fileNameCmd.ExecuteReader())
                {
                    if (fileNameReader.Read())
                    {
                        fileNameDB = fileNameReader["belgeNo"].ToString();
                    }
                }
            }
            logConnection.Close();

            string logFilePath = logPath + @"AddNewFileLog.txt";
            string writeText = "[" + DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss") + "}: Yeni belge ekleyen Kullanıcı No: " + UserId2 + "\tEklenen Belge No: " + fileNameDB + "\t Belge Adı: " + fileName;

            FileStream adminLogFS = new FileStream(logFilePath, FileMode.OpenOrCreate, FileAccess.Write);
            adminLogFS.Close();
            File.AppendAllText(logFilePath, Environment.NewLine + writeText);
        }
        //User, Status, Position
        public void NewDataLog(string TableName, int AdminUserId2, string title, string columnName, string whereColumnName, string inputData)
        {
            string columnSecondName = "";
            if (TableName == "kullanicilar")
            {
                columnSecondName = " ID: ";
            }
            else
            {
                columnSecondName = " Adı: ";
            }

            string dataNo = NewDataLogSQL(TableName, columnName, whereColumnName, inputData);

            string logFilePath = logPath + @"AdminLog\NewDataLog.txt";
            string writeText = "[" + DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss") + "]: " + title + " ekleyen Yetkili No: " + AdminUserId2 + "\tEklenen " + title + " No: " + dataNo + "\t" + title + columnSecondName + inputData;

            FileStream adminLogFS = new FileStream(logFilePath, FileMode.OpenOrCreate, FileAccess.Write);
            adminLogFS.Close();
            File.AppendAllText(logFilePath, Environment.NewLine + writeText);
        }
        private string NewDataLogSQL(string TableName, string columnName, string whereColumnName, string inputData)
        {
            string dataNo = "";

            logConnection.Close();
            using (SqlCommand dataNoNoCmd = new SqlCommand("SELECT " + columnName + "No FROM " + TableName + " WHERE " + whereColumnName + "= @gelenVeri", logConnection))
            {
                dataNoNoCmd.Parameters.AddWithValue("@gelenVeri", inputData);
                logConnection.Open();
                using (SqlDataReader dataNoReader = dataNoNoCmd.ExecuteReader())
                {
                    if (dataNoReader.Read())
                    {
                        dataNo = dataNoReader[columnName + "No"].ToString();
                    }
                }
            }
            logConnection.Close();

            return dataNo;
        }
        //Delete, Active, Exit
        public void AdminLog(string buttonName, string columnTitle, string currentCellValue, int AdminUserId, string tableName, string datagridColumnName)
        {
            string logFilePath = "";
            string writeText = "";
            string columnSecondName = "";
            if (tableName == "kullanicilar")
            {
                columnSecondName = " No:";

            }
            else
            {
                columnSecondName = " Adı:";
            }

            if (buttonName == "delete")//Delete Button
            {
                logFilePath = logPath + @"AdminLog\AdminDeleteDataLog.txt";
                writeText = "[" + DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss") + "]: Silme işlemini yapan Yetkili No: " + AdminUserId + "\tSilinen Veri= Tablo Adı: " + tableName + "\tSütun Adı: " + datagridColumnName + "\t" + columnTitle + columnSecondName + currentCellValue;
            }
            else if (buttonName == "active")//Active or Inactive Button
            {
                logFilePath = logPath + @"AdminLog\AdminChangeUserActiveLog.txt";
                writeText = "[" + DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss") + "]: Aktiflik işlemini yapan Yetkili No: " + AdminUserId + "\tAktifliği Değiştirilen Kullanıcı No: " + currentCellValue;
            }
            else if (buttonName == "exit")//Exit Button
            {
                logFilePath = logPath + @"UserExitLog.txt";
                writeText = "[" + DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss") + "]: Çıkış yapan Yetkili No: " + AdminUserId;
            }
            else
            {
                //returnValue = "false";
            }
            FileStream adminLogFS = new FileStream(logFilePath, FileMode.OpenOrCreate, FileAccess.Write);
            adminLogFS.Close();
            File.AppendAllText(logFilePath, Environment.NewLine + writeText);
        }
        //fileName = currentCellValue
        //Delete, Exit
        public void NormalUserLog(string buttonName, string fileName, int UserId)
        {
            string logFilePath = "";
            string writeText = "";
            if (buttonName == "delete")
            {
                string fileNoinDB = NormalUserLogSQL(fileName);
                logFilePath = logPath + @"NormalUserDeleteFileLog.txt";
                writeText = "[" + DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss") + "]: Belge silme işlemini yapan Kullanıcı No: " + UserId + "\tSilinen Belge No: " + fileNoinDB + "\tBelge Adı: " + fileName;
            }
            else if (buttonName == "exit")
            {
                logFilePath = logPath + @"UserExitLog.txt";
                writeText = "[" + DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss") + "]: Çıkış yapan Kullanıcı No: " + UserId;
            }
            else
            {
                logFilePath = "";
                writeText = "";
            }
            FileStream adminLogFS = new FileStream(logFilePath, FileMode.OpenOrCreate, FileAccess.Write);
            adminLogFS.Close();
            File.AppendAllText(logFilePath, Environment.NewLine + writeText);
        }
        private string NormalUserLogSQL(string fileName)
        {
            string fileNoinDB = "";
            logConnection.Close();
            using (SqlCommand fileNoCmd = new SqlCommand("SELECT silinenBelgeNo FROM silinenBelgeler WHERE silinenBelgeAdi = @belgeAdi", logConnection))
            {
                fileNoCmd.Parameters.AddWithValue("@belgeAdi", fileName);
                logConnection.Open();
                using (SqlDataReader fileNoReader = fileNoCmd.ExecuteReader())
                {
                    if (fileNoReader.Read())
                    {
                        fileNoinDB = fileNoReader["silinenBelgeNo"].ToString();
                    }
                    else
                    {
                        fileNoinDB = "";
                    }
                }
            }
            logConnection.Close();

            return fileNoinDB;
        }

        public void DownloadLog(int UserId5, string BelgeNo, string fileName)
        {
            string userIdName = DownloadLogSQL(UserId5);
            
            string logFilePath = logPath + @"DownloadFileLog.txt";
            string writeText = "[" + DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss") + "]: Dosyayı indiren Kullanıcı No: " + UserId5 + "\tKullanıcı Adı: " + userIdName + "\t İndirilen Dosya No: " + BelgeNo + "\tBelge Adı: " + fileName;
            FileStream adminLogFS = new FileStream(logFilePath, FileMode.OpenOrCreate, FileAccess.Write);
            adminLogFS.Close();
            File.AppendAllText(logFilePath, Environment.NewLine + writeText);
        }
        private string DownloadLogSQL(int UserId5)
        {
            string userNameSurname = "";
            logConnection.Close();

            string userNameSurnameCmdText = "SELECT klnc.kullaniciAdi, klnc.kullaniciSoyadi FROM kullanicilar AS klnc WHERE klnc.kullaniciNo = @kullaniciNo";
            using (SqlCommand userNameSurnameCmd = new SqlCommand(userNameSurnameCmdText, logConnection))
            {
                userNameSurnameCmd.Parameters.AddWithValue("@kullaniciNo", UserId5);
                logConnection.Open();
                using (SqlDataReader userNameSurnameReader = userNameSurnameCmd.ExecuteReader())
                {
                    if (userNameSurnameReader.Read())
                    {
                        userNameSurname = userNameSurnameReader["kullaniciAdi"].ToString() + ' ' + userNameSurnameReader["kullaniciSoyadi"].ToString();
                    }
                    else
                    {
                        userNameSurname = "Kullanıcı Adı Bulunamadı";
                    }
                }
            }
            logConnection.Close();

            return userNameSurname;
        }
    }
}
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
        SqlConnection logConnection = new SqlConnection("server=ERU; Initial Catalog=deneme;Integrated Security=SSPI");


        public void LoginLog(int kullaniciNo, string userAuthority, string kullaniciAdi, string kullaniciSoyadi)
        {
            string logFilePath = @"C:\Users\Fatih\Desktop\ServerLogKaydi\LoginLog.txt";
            string writeText = "[" + DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss") + "]: Giriş " + userAuthority + " No: " + kullaniciNo + "\tAdı: " + kullaniciAdi + ' ' + kullaniciSoyadi;
            FileStream loginLogFS = new FileStream(logFilePath, FileMode.OpenOrCreate, FileAccess.Write);
            loginLogFS.Close();
            File.AppendAllText(logFilePath, Environment.NewLine + writeText);
        }

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
                logFilePath = @"C:\Users\Fatih\Desktop\ServerLogKaydi\AdminLog\AdminDeleteDataLog.txt";
                writeText = "[" + DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss") + "]: Silme işlemini yapan Yetkili No: " + AdminUserId + "\tSilinen Veri= Tablo Adı: " + tableName + "\tSütun Adı: " + datagridColumnName + "\t" + columnTitle + columnSecondName + currentCellValue;
            }
            else if (buttonName == "active")//Active or Inactive Button
            {
                logFilePath = @"C:\Users\Fatih\Desktop\ServerLogKaydi\AdminLog\AdminChangeUserActiveLog.txt";
                writeText = "[" + DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss") + "]: Aktiflik işlemini yapan Yetkili No: " + AdminUserId + "\tAktifliği Değiştirilen Kullanıcı No: " + currentCellValue;
            }
            else if (buttonName == "exit")//Exit Button
            {
                logFilePath = @"C:\Users\Fatih\Desktop\ServerLogKaydi\UserExitLog.txt";
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

        public void UpdateFileLog(int UserId, string BelgeNo, string fileName)
        {
            string logFilePath = @"C:\Users\Fatih\Desktop\ServerLogKaydi\UpdateFileLog.txt";
            string writeText = "[" + DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss") + "]: Belge güncelleme işlemini yapan Kullanıcı No: " + UserId + "\tBelge No: " + BelgeNo + "\tBelge Adı: " + fileName;
            FileStream adminLogFS = new FileStream(logFilePath, FileMode.OpenOrCreate, FileAccess.Write);
            adminLogFS.Close();
            File.AppendAllText(logFilePath, Environment.NewLine + writeText);
        }

        public void UpdateDataLog(string TableName, int UpdateDataUserId, string DataFromAdminPanel, string title, string userName, string userSurname, string comboBoxDataName)
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
            string logFilePath = @"C:\Users\Fatih\Desktop\ServerLogKaydi\AdminLog\UpdateDataLog.txt";
            string writeText = "[" + DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss") + "]: Veri güncelleme yapan Yetkili No: " + UpdateDataUserId + "\tGüncellenen " + title + " No: " + DataFromAdminPanel + "\t" + title + " Adı: " + columnNameValue;
            Console.WriteLine(writeText);
            FileStream adminLogFS = new FileStream(logFilePath, FileMode.OpenOrCreate, FileAccess.Write);
            adminLogFS.Close();
            File.AppendAllText(logFilePath, Environment.NewLine + writeText);
        }

        public void NewFileLog(int UserId2, string fileName)
        {
            string fileNameDB = "";
            logConnection.Close();
            SqlCommand fileNameCmd = new SqlCommand("SELECT belgeNo FROM belgelerim WHERE belgeAdi = @belgeAdi", logConnection);
            fileNameCmd.Parameters.AddWithValue("@belgeAdi", fileName);
            logConnection.Open();
            SqlDataReader fileNameReader = fileNameCmd.ExecuteReader();
            if (fileNameReader.Read())
            {
                fileNameDB = fileNameReader["belgeNo"].ToString();
            }
            fileNameReader.Close();
            logConnection.Close();

            string logFilePath = @"C:\Users\Fatih\Desktop\ServerLogKaydi\AddNewFileLog.txt";
            string writeText = "[" + DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss") + "}: Yeni belge ekleyen Kullanıcı No: " + UserId2 + "\tEklenen Belge No: " + fileNameDB + "\t Belge Adı: " + fileName;

            FileStream adminLogFS = new FileStream(logFilePath, FileMode.OpenOrCreate, FileAccess.Write);
            adminLogFS.Close();
            File.AppendAllText(logFilePath, Environment.NewLine + writeText);
        }
    }
}

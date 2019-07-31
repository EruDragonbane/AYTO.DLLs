﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace AYTO.MainPage
{
    public class MainPageDLL
    {
        SqlConnection mainPageConnection = new SqlConnection("server=ERU; Initial Catalog=deneme;Integrated Security=SSPI");

        public string UserIdCheckForPermissin(string currentCellValue, int UserId)
        {
            string returnValue = "";
            mainPageConnection.Close();

            string userCheckCmdText = "SELECT blg.kullaniciNo FROM belgelerim AS blg WHERE blg.belgeAdi = @belgeAdi";
            SqlCommand userCheckCmd = new SqlCommand(userCheckCmdText, mainPageConnection);
            userCheckCmd.Parameters.AddWithValue("@belgeAdi", currentCellValue);
            mainPageConnection.Open();
            SqlDataReader userCheckReader = userCheckCmd.ExecuteReader();
            if (userCheckReader.Read() && userCheckReader["kullaniciNo"].ToString() == UserId.ToString())
            {
                returnValue = "true";
            }
            else
            {
                returnValue = "false";
            }
            userCheckReader.Close();
            mainPageConnection.Close();

            return returnValue;
        }

        public Tuple<string , string> CellDoubleClick(string selectedRowName)
        {
            string returnValue = "";
            string fileNo = "";

            mainPageConnection.Close();

            string detailFileCmdText = "SELECT blg.belgeNo FROM belgelerim AS blg WHERE blg.belgeAdi = @belgeAdi";
            SqlCommand detailFileCmd = new SqlCommand(detailFileCmdText, mainPageConnection);
            detailFileCmd.Parameters.AddWithValue("@belgeAdi", selectedRowName);
            mainPageConnection.Open();
            SqlDataReader detailFileReader = detailFileCmd.ExecuteReader();
            if (detailFileReader.Read())
            {
                returnValue = "true";
                fileNo = detailFileReader["belgeNo"].ToString();
            }
            else
            {
                returnValue = "false";
                fileNo = "";
            }
            detailFileReader.Close();
            mainPageConnection.Close();

            var cellDoubleClickTuple = new Tuple<string, string>(returnValue, fileNo);
            return cellDoubleClickTuple;
        }
        //Send ve Update Eventleri
        public Tuple<string, string> SendUpdateButtonsClickForBelgeNo(string selectedRowName)
        {
            string returnValue = "";
            string fileNo = "";

            mainPageConnection.Close();

            string selectedFileCmdText = "SELECT blg.belgeNo FROM belgelerim AS blg WHERE blg.belgeAdi = @belgeAdi";
            SqlCommand selectedFileCmd = new SqlCommand(selectedFileCmdText, mainPageConnection);
            selectedFileCmd.Parameters.AddWithValue("@belgeAdi", selectedRowName);
            mainPageConnection.Open();
            SqlDataReader selectedFileReader = selectedFileCmd.ExecuteReader();
            if (selectedFileReader.Read())
            {
                returnValue = "true";
                fileNo = selectedFileReader["belgeNo"].ToString();
            }
            else
            {
                returnValue = "";
                fileNo = "";
            }
            selectedFileReader.Close();
            mainPageConnection.Close();

            var SendUpdateButtonsClickForBelgeNo = new Tuple<string, string>(returnValue, fileNo);
            return SendUpdateButtonsClickForBelgeNo;
        }

        public void InsertFileBeforeDelete(string currentCellValue, int UserId)
        {
            mainPageConnection.Close();

            string deletedFileInsertCmdText = "INSERT INTO silinenBelgeler (silinenKullaniciNo, silinenBelgeNo, silinenBelgeBasligi, silinenBelgeAdi, silinenBelgeDizini, silinenBelgeVeriTipiveAdi, silinenBelgeServerDizini, silinenBelgeAciklamasi, silinenEklenmeTarihi, silinenSistemEklenmeTarihi, silinenGuncellenmeTarihi, silinenSistemGuncellenmeTarihi, silinenGuncelleyenKisiNo, silinenDurumNo) SELECT kullaniciNo, belgeNo, belgeBasligi, belgeAdi, belgeDizini, belgeVeriTipiveAdi, belgeServerDizini, belgeAciklamasi, eklenmeTarihi, sistemEklenmeTarihi, guncellenmeTarihi, sistemGuncellenmeTarihi, guncelleyenKisiNo, durumNo FROM belgelerim WHERE belgeAdi = @gelenVeri";

            SqlCommand deletedFileInsertCmd = new SqlCommand(deletedFileInsertCmdText, mainPageConnection);
            deletedFileInsertCmd.Parameters.AddWithValue("@gelenVeri", currentCellValue);
            mainPageConnection.Open();
            deletedFileInsertCmd.ExecuteNonQuery();
            mainPageConnection.Close();

            UpdateTableBeforeDelete(currentCellValue, UserId);

        }
        public void UpdateTableBeforeDelete(string currentCellValue, int UserId)
        {
            mainPageConnection.Close();

            string deletedByCmdText = "UPDATE silinenBelgeler SET silenKisi = @silenKisi WHERE silinenBelgeAdi = @gelenVeri";

            SqlCommand deletedByCmd = new SqlCommand(deletedByCmdText, mainPageConnection);
            deletedByCmd.Parameters.AddWithValue("@silenKisi", UserId);
            deletedByCmd.Parameters.AddWithValue("@gelenVeri", currentCellValue);
            mainPageConnection.Open();
            deletedByCmd.ExecuteNonQuery();
            mainPageConnection.Close();

            DeleteButtonClick(currentCellValue);
        }
        public void DeleteButtonClick(string currentCellValue)
        {
            mainPageConnection.Close();

            string deleteFileCmdText = "DELETE FROM belgelerim WHERE belgeAdi = @belgeAdi";

            SqlCommand deleteFileCmd = new SqlCommand(deleteFileCmdText, mainPageConnection);
            deleteFileCmd.Parameters.AddWithValue("@belgeAdi", currentCellValue);
            mainPageConnection.Open();
            deleteFileCmd.ExecuteNonQuery();
            mainPageConnection.Close();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;

namespace AYTO.FileDetail
{
    public class FileDetailDLL
    {
        SqlConnection fileDetailConnection = new SqlConnection("server=ERU; Initial Catalog=deneme;Integrated Security=SSPI");

        public Tuple<string, string, string, string, string> LabelGridFromDataGridView(string BelgeNo)
        {
            string fileTitle = "";
            string fileName = "";
            string fileExplain = "";
            string fileDate = "";
            string addedFromUser = "";

            fileDetailConnection.Close();
            string detailFileCmdText = "SELECT blg.belgeBasligi, blg.belgeVeriTipiveAdi, blg.belgeAciklamasi, blg.belgeDizini, blg.sistemEklenmeTarihi, klnc.kullaniciAdi, klnc.kullaniciSoyadi FROM belgelerim AS blg INNER JOIN kullanicilar AS klnc ON blg.kullaniciNo = klnc.kullaniciNo WHERE belgeNo = @belgeNo";
            using (SqlCommand detailFileCmd = new SqlCommand(detailFileCmdText, fileDetailConnection))
            {
                detailFileCmd.Parameters.AddWithValue("@belgeNo", BelgeNo);
                fileDetailConnection.Open();
                using (SqlDataReader detailFileReader = detailFileCmd.ExecuteReader())
                {
                    if (detailFileReader.Read())
                    {
                        fileTitle = detailFileReader["belgeBasligi"].ToString();
                        fileName = detailFileReader["belgeVeriTipiveAdi"].ToString();
                        fileExplain = detailFileReader["belgeAciklamasi"].ToString();
                        fileDate = DateTime.Parse(detailFileReader["sistemEklenmeTarihi"].ToString()).ToString("dddd, dd/MM/yyyy, HH:mm");
                        addedFromUser = detailFileReader["kullaniciAdi"].ToString() + " " + detailFileReader["kullaniciSoyadi"].ToString();
                    }
                }
            }
            fileDetailConnection.Close();

            var labelGridTuple = new Tuple<string, string, string, string, string>(fileTitle, fileName, fileExplain, fileDate, addedFromUser);
            return labelGridTuple;
        }

        public void LinkClicked_OpenFileEvent(string BelgeNo)
        {
            fileDetailConnection.Close();
            string detailFileCmdText = "SELECT blg.belgeVeriTipiveAdi FROM belgelerim AS blg INNER JOIN kullanicilar AS klnc ON blg.kullaniciNo = klnc.kullaniciNo WHERE belgeNo = @belgeNo";
            using (SqlCommand detailFileCmd = new SqlCommand(detailFileCmdText, fileDetailConnection))
            {
                detailFileCmd.Parameters.AddWithValue("@belgeNo", BelgeNo);
                fileDetailConnection.Open();
                using (SqlDataReader detailFileReader = detailFileCmd.ExecuteReader())
                {
                    if (detailFileReader.Read())
                    {
                        string filePath = (@"C:\Users\Fatih\Desktop\ServerDosyaOrnegi\" + detailFileReader["belgeVeriTipiveAdi"].ToString());

                        var readOnlyAttributes = File.GetAttributes(filePath);
                        File.SetAttributes(filePath, readOnlyAttributes | FileAttributes.ReadOnly);
                        Process.Start(filePath);
                        File.SetAttributes(filePath, readOnlyAttributes);
                    }
                }
            }
            fileDetailConnection.Close();
        }

        public Tuple<string, string, string> DownloadButtonClick(string BelgeNo)
        {
            string returnValue = "";
            string fileTypeAndName = "";
            string fileDirectory = "";

            fileDetailConnection.Close();

            string fileNameCmdText = "SELECT blg.belgeAdi, klnc.kullaniciGiris, blg.belgeVeriTipiveAdi, blg.belgeServerDizini FROM belgelerim AS blg INNER JOIN kullanicilar AS klnc ON blg.kullaniciNo = klnc.kullaniciNo WHERE belgeNo = @belgeNo";
            using (SqlCommand fileNameCmd = new SqlCommand(fileNameCmdText, fileDetailConnection))
            {
                fileNameCmd.Parameters.AddWithValue("@belgeNo", BelgeNo);
                fileDetailConnection.Open();
                using (SqlDataReader fileNameCmdReader = fileNameCmd.ExecuteReader())
                {
                    if (fileNameCmdReader.Read())
                    {
                        returnValue = "true";
                        fileTypeAndName = fileNameCmdReader["belgeVeriTipiveAdi"].ToString();
                        fileDirectory = fileNameCmdReader["belgeServerDizini"].ToString();
                    }
                    else
                    {
                        returnValue = "false";
                    }
                }
            }
            fileDetailConnection.Close();

            var downloadFileTuple = new Tuple<string, string, string>(returnValue, fileTypeAndName, fileDirectory);

            return downloadFileTuple;
        }
    }
}

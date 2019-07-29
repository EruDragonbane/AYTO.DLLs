using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;

namespace AYTO.SendFile
{
    public class SendFileDLL
    {
        SqlConnection sendFileConnection = new SqlConnection("server=ERU; Initial Catalog=deneme;Integrated Security=SSPI");

        public Tuple<string, string, string, string, string> LabelGridFromDataGridView(string BelgeNo)
        {
            string fileTitle = "";
            string fileName = "";
            string fileExplain = "";
            string fileDate = "";
            string sendedFromUser = "";

            sendFileConnection.Close();
            string detailFileCmdText = "SELECT blg.belgeBasligi, blg.belgeVeriTipiveAdi, blg.belgeAciklamasi, blg.belgeDizini, blg.sistemEklenmeTarihi, klnc.kullaniciAdi, klnc.kullaniciSoyadi FROM belgelerim AS blg INNER JOIN kullanicilar AS klnc ON blg.kullaniciNo = klnc.kullaniciNo WHERE belgeNo = @belgeNo";
            SqlCommand detailFileCmd = new SqlCommand(detailFileCmdText, sendFileConnection);
            detailFileCmd.Parameters.AddWithValue("@belgeNo", BelgeNo);
            sendFileConnection.Open();
            SqlDataReader detailFileReader = detailFileCmd.ExecuteReader();
            if (detailFileReader.Read())
            {
                fileTitle = detailFileReader["belgeBasligi"].ToString();
                fileName = detailFileReader["belgeVeriTipiveAdi"].ToString();
                fileExplain = detailFileReader["belgeAciklamasi"].ToString();
                fileDate = DateTime.Parse(detailFileReader["sistemEklenmeTarihi"].ToString()).ToString("dddd, dd/MM/yyyy, HH:mm");
                sendedFromUser = detailFileReader["kullaniciAdi"].ToString() + " " + detailFileReader["kullaniciSoyadi"].ToString();
            }
            detailFileReader.Close();
            sendFileConnection.Close();

            var labelGridTuple = new Tuple<string, string, string, string, string>(fileTitle, fileName, fileExplain, fileDate, sendedFromUser);
            return labelGridTuple;
        }

        public void LinkClicked_OpenFileEvent(string BelgeNo)
        {
            sendFileConnection.Close();
            string detailFileCmdText = "SELECT blg.belgeVeriTipiveAdi FROM belgelerim AS blg INNER JOIN kullanicilar AS klnc ON blg.kullaniciNo = klnc.kullaniciNo WHERE belgeNo = @belgeNo";
            SqlCommand detailFileCmd = new SqlCommand(detailFileCmdText, sendFileConnection);
            detailFileCmd.Parameters.AddWithValue("@belgeNo", BelgeNo);
            sendFileConnection.Open();
            SqlDataReader detailFileReader = detailFileCmd.ExecuteReader();
            if (detailFileReader.Read())
            {
                Process.Start(@"C:\Users\Fatih\Desktop\ServerDosyaOrnegi\" + detailFileReader["belgeVeriTipiveAdi"]);
            }
            detailFileReader.Close();
            sendFileConnection.Close();
        }
    }
}

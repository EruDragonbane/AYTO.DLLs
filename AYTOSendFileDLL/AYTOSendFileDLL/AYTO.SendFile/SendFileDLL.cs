using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Diagnostics;

namespace AYTO.SendFile
{
    public class SendFileDLL
    {
        SqlConnection sendFileConnection = new SqlConnection("server=ERU; Initial Catalog=deneme;Integrated Security=SSPI");
        
        public Tuple<string, string> TextGridFromMainPage(string BelgeNo)
        {
            string fileName = "";
            string fileDirectory = "";

            sendFileConnection.Close();

            string sendFileCmdText = "SELECT blg.belgeDizini, blg.belgeAdi FROM belgelerim AS blg INNER JOIN kullanicilar AS klnc ON blg.kullaniciNo = klnc.kullaniciNo WHERE belgeNo = @belgeNo";
            SqlCommand sendFileCmd = new SqlCommand(sendFileCmdText, sendFileConnection);
            sendFileCmd.Parameters.AddWithValue("@belgeNo", BelgeNo);
            sendFileConnection.Open();
            SqlDataReader sendFileReader = sendFileCmd.ExecuteReader();
            if (sendFileReader.Read())
            {
                fileName = sendFileReader["belgeAdi"].ToString();
                fileDirectory = sendFileReader["belgeDizini"].ToString();

            }
            sendFileReader.Close();
            sendFileConnection.Close();

            var textGridTuple = new Tuple<string, string>(fileName, fileDirectory);
            return textGridTuple;
        }

    }
}

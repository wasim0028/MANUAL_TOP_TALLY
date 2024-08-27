using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        int a = 0;
        int b = 0;

        VizCon vc = new VizCon();

        public void getVizHost()
        {
            string[] dbName = ConfigurationManager.AppSettings.AllKeys.Where(key => key.StartsWith("VIZ")).Select(key => ConfigurationManager.AppSettings[key]).ToArray();
            cmbVizHost.Items.AddRange(dbName);
            cmbVizHost.SelectedIndex = 0;
        }

        public void getStates()
        {
            string tblname = "";
            if(WB.Checked == true)
            {
                tblname = ConfigurationManager.AppSettings["WB"];
            }
            string con = ConfigurationManager.AppSettings["DB"];
            SqlConnection mycon = new SqlConnection(con);
            mycon.Open();
            SqlCommand cmd = new SqlCommand("Select distinct CONST_ID,CONST_NAME,TOTAL_CONST_SEATS,PARTY_NO,CONST_NAME_BANGLA FROM " + tblname + " Order by CONST_ID", mycon);
            SqlDataReader myReader = cmd.ExecuteReader();
            listState.Items.Clear();
            while (myReader.Read())
            {
                string scode = myReader.GetValue(0).ToString();
                string sname = myReader.GetString(1);
                string stotal = myReader.GetString(2).ToString();
                string spartyno = myReader.GetString(3);
                string sname_b = myReader.GetString(4);
                string cmdval = scode + " | " + sname + " | " + stotal + " | " + spartyno + " |  " + sname_b;
                listState.Items.Add(cmdval);
            }
            mycon.Close();

        }


        public void getStateTally()
        {
            string tblname = "";
            if (WB.Checked == true)
            {
                tblname = ConfigurationManager.AppSettings["WB"];
            }
            string con = ConfigurationManager.AppSettings["DB"];
            SqlConnection mycon = new SqlConnection(con);
            mycon.Open();
            SqlCommand cmd = new SqlCommand("SELECT PARTY, SEATS, SORT_ID FROM " + tblname + " WHERE CONST_ID='" + tbID.Text + "' order by SORT_ID", mycon);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            dgvConstTally.DataSource = dt;
            mycon.Close();
        }

        public void FillBoxes()
        {
            tbParty1.Text = "";
            tbParty2.Text = "";
            tbParty3.Text = "";
            tbParty4.Text = "";
            tbParty5.Text = "";

            tbSeats1.Text = "0";
            tbSeats2.Text = "0";    
            tbSeats3.Text = "0";    
            tbSeats4.Text = "0";
            tbSeats5.Text = "0";

            tbConstTotal.Text = "0";

            if (tbConstPartyNo.Text == "5")
            {
                tbParty1.Text = dgvConstTally.Rows[0].Cells[0].Value.ToString();
                tbParty2.Text = dgvConstTally.Rows[1].Cells[0].Value.ToString();
                tbParty3.Text = dgvConstTally.Rows[2].Cells[0].Value.ToString();
                tbParty4.Text = dgvConstTally.Rows[3].Cells[0].Value.ToString();
                tbParty5.Text = dgvConstTally.Rows[4].Cells[0].Value.ToString();

                tbSeats1.Text = dgvConstTally.Rows[0].Cells[1].Value.ToString();
                tbSeats2.Text = dgvConstTally.Rows[1].Cells[1].Value.ToString();
                tbSeats3.Text = dgvConstTally.Rows[2].Cells[1].Value.ToString();
                tbSeats4.Text = dgvConstTally.Rows[3].Cells[1].Value.ToString();
                tbSeats5.Text = dgvConstTally.Rows[4].Cells[1].Value.ToString();

                int tot5 = Convert.ToInt32(tbSeats1.Text) + Convert.ToInt32(tbSeats2.Text) + Convert.ToInt32(tbSeats3.Text) + Convert.ToInt32(tbSeats4.Text) + Convert.ToInt32(tbSeats5.Text);
                tbConstTotal.Text = tot5.ToString();
            }

            if (tbConstPartyNo.Text == "4")
            {
                tbParty1.Text = dgvConstTally.Rows[0].Cells[0].Value.ToString();
                tbParty2.Text = dgvConstTally.Rows[1].Cells[0].Value.ToString();
                tbParty3.Text = dgvConstTally.Rows[2].Cells[0].Value.ToString();
                tbParty4.Text = dgvConstTally.Rows[3].Cells[0].Value.ToString();

                tbSeats1.Text = dgvConstTally.Rows[0].Cells[1].Value.ToString();
                tbSeats2.Text = dgvConstTally.Rows[1].Cells[1].Value.ToString();
                tbSeats3.Text = dgvConstTally.Rows[2].Cells[1].Value.ToString();
                tbSeats4.Text = dgvConstTally.Rows[3].Cells[1].Value.ToString();

                int tot4 = Convert.ToInt32(tbSeats1.Text) + Convert.ToInt32(tbSeats2.Text) + Convert.ToInt32(tbSeats3.Text) + Convert.ToInt32(tbSeats4.Text);
                tbConstTotal.Text = tot4.ToString();
            }
        }

        

        private void Form1_Load(object sender, EventArgs e)
        {
            label2.Text = ConfigurationManager.AppSettings["Scene1"];
            WB.Checked = true;
            getStates();
            getVizHost();
        }

        private void listState_SelectedIndexChanged(object sender, EventArgs e)
        {
            string[] sinfo = listState.Text.Split('|');
            tbID.Text = sinfo[0].Trim();
            tbConstName.Text = sinfo[1].Trim();
            tbConstSeats.Text = sinfo[2].Trim();
            tbConstPartyNo.Text = sinfo[3].Trim();
            tbConstName_b.Text = sinfo[4].Trim();
            getStateTally();
            FillBoxes();
        }

        private void WB_CheckedChanged(object sender, EventArgs e)
        {
            getStates();
            listState.SelectedIndex = 0;
            getStateTally();
            FillBoxes();
            checkBox1.Checked = false;
        }

        private void cmbVizHost_SelectedIndexChanged(object sender, EventArgs e)
        {
            string[] host = cmbVizHost.Text.Split('|');
            tbIP.Text = host[1].Trim();
        }

        public void Tally5_DataToViz()
        {
            string layer = ConfigurationManager.AppSettings["Layer"];
            string myip = tbIP.Text;
            string dpcmd = "0 RENDERER*" + layer + "*FUNCTION*DataPool*Data SET ";

            string stateselector = dpcmd + "HEADER_ELE = " + label1.Text;
            string statename = dpcmd + "DIST_NAME = " + tbConstName_b.Text;

            //string sall1 = dpcmd + "lsbottomtallyalliance1 = " + tbParty1.Text;
            //string sall2 = dpcmd + "lsbottomtallyalliance2 = " + tbParty2.Text;
            //string sall3 = dpcmd + "lsbottomtallyalliance3 = " + tbParty3.Text;
            //string sall4 = dpcmd + "lsbottomtallyalliance4 = " + tbParty4.Text;
            //string sall5 = dpcmd + "lsbottomtallyalliance5 = " + tbParty5.Text;

            string sseats1 = dpcmd + "SEAT_VAL1 = " + tbSeats1.Text;
            string sseats2 = dpcmd + "SEAT_VAL2 = " + tbSeats2.Text;
            string sseats3 = dpcmd + "SEAT_VAL3 = " + tbSeats3.Text;
            string sseats4 = dpcmd + "SEAT_VAL4 = " + tbSeats4.Text;
            string sseats5 = dpcmd + "SEAT_VAL5 = " + tbSeats5.Text;

            //string sTotalVal = dpcmd + "lsbottomtallytotalvalue = " + tbSTotal.Text;
            string sResultVal = dpcmd + " RESULT_VAL = " + tbConstTotal.Text + "/" + tbConstSeats.Text;

            string keyval = statename + ";" + sseats1 + ";" + sseats2 + ";" + sseats3 + ";" + sseats4 + ";" + sseats5 + ";" + sResultVal + ";" + stateselector;

            vc.getVizdata(myip, "0 RENDERER*" + layer + "*STAGE*DIRECTOR*LS_DIR_ELECTION*ACTION*TRIGGER*KEY*$stateplay*VALUE SET " + keyval);

            vc.getVizdata(myip, "0 RENDERER*" + layer + "*STAGE*DIRECTOR*LS_DIR_ELECTION START");
        }

        public void SendDataToViz()

        {
            string layer = ConfigurationManager.AppSettings["Layer"];

            string myip = tbIP.Text;
            vc.getVizdata(myip, "0 RENDERER*" + layer + "*FUNCTION*DataPool*Data SET SEAT_VAL1 = " + tbSeats1.Text);
            vc.getVizdata(myip, "0 RENDERER*" + layer + "*FUNCTION*DataPool*Data SET SEAT_VAL2 = " + tbSeats2.Text);
            vc.getVizdata(myip, "0 RENDERER*" + layer + "*FUNCTION*DataPool*Data SET SEAT_VAL3 = " + tbSeats3.Text);
            vc.getVizdata(myip, "0 RENDERER*" + layer + "*FUNCTION*DataPool*Data SET SEAT_VAL4 = " + tbSeats4.Text);
            vc.getVizdata(myip, "0 RENDERER*" + layer + "*FUNCTION*DataPool*Data SET SEAT_VAL5 = " + tbSeats5.Text);

            vc.getVizdata(myip, "0 RENDERER*" + layer + "*FUNCTION*DataPool*Data SET RESULT_VAL = " + tbConstTotal.Text + "/" + tbConstSeats.Text);
            vc.getVizdata(myip, "0 RENDERER*" + layer + "*FUNCTION*DataPool*Data SET HEADER_ELE = " + label1.Text.Trim());
            vc.getVizdata(myip, "0 RENDERER*" + layer + "*FUNCTION*DataPool*Data SET DIST_NAME = " + tbConstName_b.Text);

        }

        public void SceneLoad()

        {
            string layer = ConfigurationManager.AppSettings["Layer"];
            string myip = tbIP.Text;

            string myscene = label2.Text.Trim();

            vc.getVizdata(myip, "0 RENDERER*" + layer + " SET_OBJECT SCENE*" + myscene);
            vc.getVizdata(myip, "0 RENDERER*" + layer + "*STAGE SHOW 0.0");
            vc.getVizdata(myip, "0 RENDERER*" + layer + "*STAGE START");
            // vc.getVizdata(myip, "0 RENDERER*STAGE*DIRECTOR*MOVEMENT START");
            // vc.getVizdata(myip, "0 RENDERER*STAGE*DIRECTOR*TIME CONTINUE");
            // vc.getVizdata(myip, "0 RENDERER*STAGE*DIRECTOR*LIVE CONTINUE");
            // vc.getVizdata(myip, "0 RENDERER*STAGE*DIRECTOR*VFI_OPEN START");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            a++;
            b = listState.Items.Count;
            listState.SelectedIndex = a - 1;
            if (a == b)
            {
                a = 0;
            }
            SendDataToViz();
            Tally5_DataToViz();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            { 
                timer1.Enabled = true;
            }

            if (checkBox1.Checked == false)  
            { 
                timer1.Enabled= false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
           SceneLoad();
           SendDataToViz();
           Tally5_DataToViz();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string layer = ConfigurationManager.AppSettings["Layer"];
            string myip = tbIP.Text;
            vc.getVizdata(myip, "0 RENDERER*" + layer + "SET_OBJECT");
        }

       
    }
}

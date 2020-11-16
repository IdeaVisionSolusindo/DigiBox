using ReaderB;
using System;
using System.Collections.Generic;
using System.Text;

namespace digibox.lib
{
    public class CT1808Reader : ICT1808Reader
    {
        public string[] data { get ; set ; }
        public string readerAddress { get ; set ; }
        public byte comAddress { get; set ; } = 0xff;
        public byte baudRate { get; set; }
        public string Port { get; set; }
        public string IPAddress { get; set ; }


        //private variable
        private int fOpenComIndex = -1; //Open serial port index number
        private byte fComAdr = 0xff; //        ComAdr currently operating
        private int ferrorcode;
        private byte fBaud;
        private int frmcomportindex;
        private bool ComIsOpen;
        private int fCmdRet = 30; //The return value of all executed instructions

        public CT1808Reader()
        {
            fOpenComIndex = -1;
            fComAdr = 0;
            ferrorcode = -1;
            fBaud = 5;
        }

        public void ReaderInfo()
        {
            byte[] TrType = new byte[2];
            byte[] VersionInfo = new byte[2];
            byte ReaderType = 0;
            byte ScanTime = 0;
            byte dmaxfre = 0;
            byte dminfre = 0;
            byte powerdBm = 0;
            byte FreBand = 0;
            ReaderModel readerModel = new ReaderModel();
            fCmdRet = StaticClassReaderB.GetReaderInformation(ref fComAdr, VersionInfo, ref ReaderType, TrType, ref dmaxfre, ref dminfre, ref powerdBm, ref ScanTime, frmcomportindex);

            
            /*if (fCmdRet == 0)
            {
                readerModel.version = Convert.ToString(VersionInfo[0], 10).PadLeft(2, '0') + "." + Convert.ToString(VersionInfo[1], 10).PadLeft(2, '0');
                if (VersionInfo[1] >= 30)
                {
                    for (int i = 0; i < 31; i++)
                        ComboBox_PowerDbm.Items.Add(Convert.ToString(i));
                    if (powerdBm > 30)
                        ComboBox_PowerDbm.SelectedIndex = 30;
                    else
                        ComboBox_PowerDbm.SelectedIndex = powerdBm;
                }
                else
                {
                    for (int i = 0; i < 19; i++)
                        ComboBox_PowerDbm.Items.Add(Convert.ToString(i));
                    if (powerdBm > 18)
                        ComboBox_PowerDbm.SelectedIndex = 18;
                    else
                        ComboBox_PowerDbm.SelectedIndex = powerdBm;
                }
                readerModel.Address = Convert.ToString(fComAdr, 16).PadLeft(2, '0');
                readerModel.MaxInventoryScanTime = Convert.ToString(ScanTime, 10).PadLeft(2, '0') + "*100ms";
                ComboBox_scantime.SelectedIndex = ScanTime - 3;
                readerModel.Power = Convert.ToString(powerdBm, 10).PadLeft(2, '0');

                FreBand = Convert.ToByte(((dmaxfre & 0xc0) >> 4) | (dminfre >> 6));
                switch (FreBand)
                {
                    case 0:
                        {
                            radioButton_band1.Checked = true;
                            fdminfre = 902.6 + (dminfre & 0x3F) * 0.4;
                            fdmaxfre = 902.6 + (dmaxfre & 0x3F) * 0.4;
                        }
                        break;
                    case 1:
                        {
                            radioButton_band2.Checked = true;
                            fdminfre = 920.125 + (dminfre & 0x3F) * 0.25;
                            fdmaxfre = 920.125 + (dmaxfre & 0x3F) * 0.25;
                        }
                        break;
                    case 2:
                        {
                            radioButton_band3.Checked = true;
                            fdminfre = 902.75 + (dminfre & 0x3F) * 0.5;
                            fdmaxfre = 902.75 + (dmaxfre & 0x3F) * 0.5;
                        }
                        break;
                    case 3:
                        {
                            radioButton_band4.Checked = true;
                            fdminfre = 917.1 + (dminfre & 0x3F) * 0.2;
                            fdmaxfre = 917.1 + (dmaxfre & 0x3F) * 0.2;
                        }
                        break;
                    case 4:
                        {
                            radioButton_band5.Checked = true;
                            fdminfre = 865.1 + (dminfre & 0x3F) * 0.2;
                            fdmaxfre = 865.1 + (dmaxfre & 0x3F) * 0.2;
                        }
                        break;
                }
                Edit_dminfre.Text = Convert.ToString(fdminfre) + "MHz";
                Edit_dmaxfre.Text = Convert.ToString(fdmaxfre) + "MHz";
                if (fdmaxfre != fdminfre)
                    CheckBox_SameFre.Checked = false;
                ComboBox_dminfre.SelectedIndex = dminfre & 0x3F;
                ComboBox_dmaxfre.SelectedIndex = dmaxfre & 0x3F;
                if (ReaderType == 0x03)
                    Edit_Type.Text = "";
                if (ReaderType == 0x06)
                    Edit_Type.Text = "";
                if (ReaderType == 0x09)
                    Edit_Type.Text = "UHFReader18";
                if ((TrType[0] & 0x02) == 0x02) //第二个字节低第四位代表支持的协议“ISO/IEC 15693”
                {
                    ISO180006B.Checked = true;
                    EPCC1G2.Checked = true;
                }
                else
                {
                    ISO180006B.Checked = false;
                    EPCC1G2.Checked = false;
                }
            }*/
        }

        public void OpenPort()
        {
           /* int openresult = 0;
            fComAdr = Convert.ToByte(readerAddress, 16); // $FF;
            openresult = StaticClassReaderB.OpenNetPort(Port, IPAddress, ref fComAdr, ref frmcomportindex);
            fOpenComIndex = frmcomportindex;
            if (openresult == 0)
            {
                ComIsOpen = true;
                ReaderInfo(); //Reader Info
            }
            if ((openresult == 0x35) || (openresult == 0x30))
            {
                MessageBox.Show("TCPIP error", "Information");
                StaticClassReaderB.CloseNetPort(frmcomportindex);
                ComOpen = false;
                return;
            }
            if ((fOpenComIndex != -1) && (openresult != 0X35) && (openresult != 0X30))
            {
                Button3.Enabled = true;
                button20.Enabled = true;
                Button5.Enabled = true;
                Button1.Enabled = true;
                button2.Enabled = true;
                Button_WriteEPC_G2.Enabled = true;
                Button_SetMultiReadProtect_G2.Enabled = true;
                Button_RemoveReadProtect_G2.Enabled = true;
                Button_CheckReadProtected_G2.Enabled = true;
                button4.Enabled = true;
                SpeedButton_Query_6B.Enabled = true;
                button6.Enabled = true;
                button8.Enabled = true;
                button9.Enabled = true;
                button12.Enabled = true;
                button_OffsetTime.Enabled = true;
                button_settigtime.Enabled = true;
                button_gettigtime.Enabled = true;
                ComOpen = true;
            }
            if ((fOpenComIndex == -1) && (openresult == 0x30))
                MessageBox.Show("TCPIP Communication Error", "Information");
            RefreshStatus();
            */
        }

        public void ReadData()
        {
           /* byte[] ScanModeData = new byte[40960];
            int ValidDatalength, i;
            string temp, temps;
            ValidDatalength = 0;
            fCmdRet = StaticClassReaderB.ReadActiveModeData(ScanModeData, ref ValidDatalength, frmcomportindex);
            if (fCmdRet == 0)
            {
                temp = "";
                temps = ByteArrayToHexString(ScanModeData);
                for (i = 0; i < ValidDatalength; i++)
                {
                    temp = temp + temps.Substring(i * 2, 2) + " ";
                }
                if (ValidDatalength > 0)
                    listBox3.Items.Add(temp);
                listBox3.SelectedIndex = listBox3.Items.Count - 1;
                StatusBar1.Panels[0].Text = DateTime.Now.ToLongTimeString() + " 操作成功";
            }
            else
                StatusBar1.Panels[0].Text = DateTime.Now.ToLongTimeString() + " 操作失败";
                */
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SKYPE4COMLib;

namespace Skype_Messenger_v_0._4_by_3v1L_XakFor
{
    public partial class Form1 : Form
    {
        [DllImport("Gdi32.dll")]
        public static extern IntPtr CreateRoundRectRgn(int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateEllipticRgn(
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect);

        [DllImport("user32.dll")]
        public static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);

        private List<string> ContactList = new List<string>();
        private int spamMess { get; set; }
        private int spamMessMass { get; set; }
        public Call call;
        Random rnd = new Random();
        private Skype skype = new Skype();
        private List<string> allContacts = new List<string>();
        ImageList imageList = new ImageList();
        private bool str = true;
        private bool blnName = true;

        int id;

        public Form1()
        {
            InitializeComponent();
            panel4.Location = new Point(200, 88);
            Height = 537;
            Width = 678;
        }

        private void bunifuFlatButton1_Click(object sender, EventArgs e)
        {
            panel5.Visible = false;
            panel7.Visible = false;
            Spammer.Visible = false;
            panel6.Visible = false;
            panel4.Visible = true;
            panel4.Location = new Point(200, 88);
        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                skype.Attach(5, true);
                if (!skype.Client.IsRunning)
                {
                    MessageBox.Show("Skype не запущен. Запустите или установите Skype", "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                else
                {
                    GetContacts();
                    // делаем круглык аватары
                    IntPtr rgn = CreateEllipticRgn(0, 0, 97, 97);
                    SetWindowRgn(pictureBox1.Handle, rgn, true);

                    // Округлые края
                    //IntPtr hRgn = CreateRoundRectRgn(0, 0, pictureBox1.Width, pictureBox1.Height, 50, 50);
                    //SetWindowRgn(pictureBox1.Handle, hRgn, true);

                    imageList.ImageSize = new Size(15, 15);
                    imageList.TransparentColor = TransparencyKey;

                    imageList.Images.Add(Properties.Resources.OnlineNew);
                    imageList.Images.Add(Properties.Resources.DoNotDisturbNew);
                    imageList.Images.Add(Properties.Resources.AwayNew);
                    imageList.Images.Add(Properties.Resources.OfflineNew);
                    imageList.Images.Add(Properties.Resources.SkypeMe);
                    listView1.SmallImageList = imageList;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void GetContacts()
        {
            foreach (User user in skype.Friends)
            {
                allContacts.Add(user.Handle + "|" + user.OnlineStatus + "|" + user.FullName);
                cbContactSpam.Items.Add(user.Handle);
                cbAutoCallBack.Items.Add(user.Handle);
                cbCall.Items.Add(user.Handle);
            }
            AddFriendsList();
        }

        private void AddFriendsList()
        {
            foreach (var user in allContacts)
            {
                string[] name = user.Split('|');
                ListViewItem lsv = new ListViewItem(name[0]);
                lsv.ImageIndex = CheckStatus(name[1]);
                lsv.SubItems.Add(name[2]);
                listView1.Items.Add(lsv);
            }
        }

        private int CheckStatus(string status)
        {
            int ret = 0;
            switch (status)
            {
                case "olsOnline":
                    return 0;
                    break;
                case "olsDoNotDisturb":
                    return 1;
                    break;
                case "olsAway":
                    return 2;
                    break;
                case "olsOffline":
                    return 3;
                    break;
                case "olsSkypeMe":
                    return 4;
                    break;
                case "olsUnknown":
                    return 3;
                    break;
            }
            return ret;
        }

        private void bunifuFlatButton8_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            allContacts.Clear();
            GetContacts();
            btFriendAll.Text = string.Format("All Friends ({0})", allContacts.Count);
        }

        private void bunifuFlatButton7_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            allContacts.Clear();
            foreach (User user in skype.Friends)
            {
                if (user.OnlineStatus == TOnlineStatus.olsOnline ||
                    user.OnlineStatus == TOnlineStatus.olsNotAvailable ||
                    user.OnlineStatus == TOnlineStatus.olsDoNotDisturb || user.OnlineStatus == TOnlineStatus.olsAway)
                {
                    allContacts.Add(user.Handle + "|" + user.OnlineStatus + "|" + user.FullName);
                }
            }
            AddFriendsList();
            btOnline.Text = string.Format("Friends Online ({0})", allContacts.Count);
        }

        private void bunifuFlatButton5_Click(object sender, EventArgs e)
        {
            ListViewItem Items = listView1.FindItemWithText(bunifuTextbox2.text);
            try
            {
                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    string allReadyText = listView1.Items[i].Text;
                    if (allReadyText == Items.Text)
                    {
                        listView1.Select();
                        listView1.Items[i].Selected = true;
                        listView1.EnsureVisible(i);
                        break;
                    }
                }
            }
            catch (Exception)
            {
                bunifuTextbox2.ForeColor = Color.OrangeRed;
            }

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Avatar avatar = new Avatar(skype);
            if (listView1.SelectedItems.Count > 0)
            {
                pictureBox1.ImageLocation = avatar.grabAvatarOnline(listView1.SelectedItems[0].Text);
            }
        }

        private void bunifuTextbox2_OnTextChange(object sender, EventArgs e)
        {
            bunifuTextbox2.ForeColor = Color.FromArgb(0, 179, 239);
        }

        private void bunifuFlatButton2_Click(object sender, EventArgs e)
        {
            panel7.Visible = false;
            panel4.Visible = false;
            panel6.Visible = false;
            Spammer.Visible = false;
            panel5.Visible = true;
            panel5.Location = new Point(200, 88);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            skype.SendMessage(cbContactSpam.Text, tbSingle.Text);
            spamMess++;
            if (spamMess >= piecesMessage.Value)
            {
                timer1.Stop();
                bunifuFlatButton8.Text = "Start";
                MessageBox.Show("Spam successfully completed", "pam successfully", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        public void Herringbone()
        {
            while (str)
            {
                for (;;)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                skype.CurrentUserStatus = TUserStatus.cusOnline;
                                break;
                            case 1:
                                skype.CurrentUserStatus = TUserStatus.cusAway;
                                break;
                            case 2:
                                skype.CurrentUserStatus = TUserStatus.cusDoNotDisturb;
                                break;
                            case 3:
                                skype.CurrentUserStatus = TUserStatus.cusInvisible;
                                break;
                            case 4:
                                skype.CurrentUserStatus = TUserStatus.cusOffline;
                                break;
                        }
                        Thread.Sleep(800);
                    }
                    if (!str)
                        break;
                }
            }
        }

        private void bunifuFlatButton9_Click(object sender, EventArgs e)
        {
            Thread th = new Thread(Herringbone);
            if (bunifuFlatButton9.Text == "Start Blink Status")
            {
                th.Start();
                str = true;
                bunifuFlatButton9.Text = "Stop Blink Status";
            }
            else
            {
                th.Abort();
                bunifuFlatButton9.Text = "Start Blink Status";
                str = false;
            }
        }

        private void bunifuFlatButton6_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == "Online")
                skype.CurrentUserStatus = TUserStatus.cusOnline;
            if (comboBox1.SelectedItem == "Away")
                skype.CurrentUserStatus = TUserStatus.cusAway;
            if (comboBox1.SelectedItem == "Do Not Disturb")
                skype.CurrentUserStatus = TUserStatus.cusDoNotDisturb;
            if (comboBox1.SelectedItem == "Invisible")
                skype.CurrentUserStatus = TUserStatus.cusInvisible;
            if (comboBox1.SelectedItem == "Offline")
                skype.CurrentUserStatus = TUserStatus.cusOffline;

        }

        private void bunifuFlatButton10_Click(object sender, EventArgs e)
        {
            skype.CurrentUserProfile.RichMoodText = textBox2.Text;
        }

        private void bunifuImageButton2_Click(object sender, EventArgs e)
        {
            if (textBox2.SelectionLength == 0)
            {
                MessageBox.Show("Select text!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                StringBuilder s = new StringBuilder(textBox2.Text);
                {
                    s.Insert(textBox2.SelectionStart, "<b>");
                    s.Insert(textBox2.SelectionStart + textBox2.SelectionLength + 3, "</b>");
                }
                textBox2.Text = s.ToString();
            }
        }

        private void bunifuImageButton3_Click(object sender, EventArgs e)
        {
            if (textBox2.SelectionLength == 0)
            {
                MessageBox.Show("Select text!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                StringBuilder s = new StringBuilder(textBox2.Text);
                {
                    s.Insert(textBox2.SelectionStart, "<center>");
                    s.Insert(textBox2.SelectionStart + textBox2.SelectionLength + 8, "</center>");
                }
                textBox2.Text = s.ToString();
            }
        }

        private void bunifuImageButton4_Click(object sender, EventArgs e)
        {
            if (textBox2.SelectionLength == 0)
            {
                MessageBox.Show("Select text!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                StringBuilder s = new StringBuilder(textBox2.Text);
                {
                    s.Insert(textBox2.SelectionStart, "<i>");
                    s.Insert(textBox2.SelectionStart + textBox2.SelectionLength + 3, "</i>");
                }
                textBox2.Text = s.ToString();
            }
        }

        private void bunifuImageButton5_Click(object sender, EventArgs e)
        {
            if (textBox2.SelectionLength == 0)
            {
                MessageBox.Show("Select text!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                StringBuilder s = new StringBuilder(textBox2.Text);
                {
                    s.Insert(textBox2.SelectionStart, "<s>");
                    s.Insert(textBox2.SelectionStart + textBox2.SelectionLength + 3, "</s>");
                }
                textBox2.Text = s.ToString();
            }
        }

        private void bunifuImageButton6_Click(object sender, EventArgs e)
        {
            if (textBox2.SelectionLength == 0)
            {
                MessageBox.Show("Select text!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                StringBuilder s = new StringBuilder(textBox2.Text);
                {
                    s.Insert(textBox2.SelectionStart, "<u>");
                    s.Insert(textBox2.SelectionStart + textBox2.SelectionLength + 3, "</u>");
                }
                textBox2.Text = s.ToString();
            }
        }

        private void bunifuImageButton7_Click(object sender, EventArgs e)
        {
            if (textBox2.SelectionLength == 0)
            {
                MessageBox.Show("Select text!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                StringBuilder s = new StringBuilder(textBox2.Text);
                {
                    s.Insert(textBox2.SelectionStart, "<blink>");
                    s.Insert(textBox2.SelectionStart + textBox2.SelectionLength + 7, "</blink>");
                }
                textBox2.Text = s.ToString();
            }
        }

        private void bunifuFlatButton4_Click(object sender, EventArgs e)
        {
            panel4.Visible = false;
            panel6.Visible = false;
            Spammer.Visible = true;
            panel5.Visible = false;
            panel7.Visible = false;
            Spammer.Location = new Point(200, 88);
        }

        private void bunifuFlatButton7_Click_1(object sender, EventArgs e)
        {

        }

        private void groupBox8_Enter(object sender, EventArgs e)
        {

        }

        private void bunifuFlatButton8_Click_1(object sender, EventArgs e)
        {
            Thread th = new Thread(SendSingleMessage);
            if (bunifuFlatButton8.Text == "Start")
            {
                spamMess = 0;
                th.Start();
                bunifuFlatButton8.Text = "Stop";
            }
            else
            {
                th.Abort();
                timer1.Stop();
                spamMess = 0;
                bunifuFlatButton8.Text = "Start";
            }
        }

        void SendSingleMessage()
        {
            Invoke(new MethodInvoker(() =>
            {
                if (cbContactSpam.Text != "")
                {
                    try
                    {
                        timer1.Interval = Int32.Parse(timeMessage.Text)*1000;
                        timer1.Start();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }));
        }

        void SendMassMessage()
        {
            Invoke(new MethodInvoker(() =>
            {
                if (tbMass.Text != "")
                {
                    try
                    {
                        timer2.Interval = rnd.Next(Decimal.ToInt32(fromDelay.Value)*1000,
                            Decimal.ToInt32(toDelay.Value)*1000);
                        timer2.Start();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }));
        }

        private void bunifuFlatButton7_Click_2(object sender, EventArgs e)
        {
            Thread thr = new Thread(SendMassMessage);
            if (bunifuFlatButton7.Text == "Start")
            {
                spamMessMass = 0;
                thr.Start();
                bunifuFlatButton7.Text = "Stop";
            }
            else
            {
                thr.Abort();
                timer2.Stop();
                spamMessMass = 0;
                bunifuFlatButton7.Text = "Start";
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            try
            {
                skype.SendMessage(cbContactSpam.Items[spamMessMass].ToString(), tbMass.Text);
                spamMessMass++;
                timer2.Interval = rnd.Next(Decimal.ToInt32(fromDelay.Value)*1000, Decimal.ToInt32(toDelay.Value)*1000);
                if (spamMessMass >= allContacts.Count)
                {
                    timer2.Stop();
                    bunifuFlatButton7.Text = "Start";
                    MessageBox.Show("Distribution completed successfully ", "Distribution completed",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Distribution completed successfully " + exc.Message);
            }
        }

        private void bunifuFlatButton12_Click(object sender, EventArgs e)
        {
            if (bunifuFlatButton12.Text == "Start")
            {
                CallSpam();
                bunifuFlatButton12.Text = "Stop";
            }
            else
            {
                try
                {
                    timer3.Stop();
                    skype.Call[id].Finish();
                    bunifuFlatButton12.Text = "Start";
                }
                catch (Exception)
                {
                   
                }
            }
        }

        private void CallSpam()
        {
            call = skype.PlaceCall(cbAutoCallBack.Text, "", "", "");
            timer3.Start();
            id = call.Id;
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            try
            {
                if (call.Status == TCallStatus.clsInProgress)
                {
                    skype.Call[id].Finish();
                }
                else if (call.Status == TCallStatus.clsRefused)
                {
                    CallSpam();
                }
                CallSpam();
            }
            catch (Exception)
            {
                
            }
        }

        private void bunifuFlatButton13_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbCall.Text != "")
                {
                    skype.PlaceCall(cbCall.Text, "", "", "");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void bunifuFlatButton3_Click(object sender, EventArgs e)
        {
            panel6.Visible = true;
            panel4.Visible = false;
            Spammer.Visible = false;
            panel5.Visible = false;
            panel7.Visible = false;
            panel6.Location = new Point(200, 88);
        }

        private void bunifuFlatButton14_Click(object sender, EventArgs e)
        {
            listView2.View = View.Details;
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.FileName = "";
                dialog.Filter = "txt files(*.txt)|*.txt";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string[] lines = File.ReadAllLines(dialog.FileName);
                    foreach (string s in lines)
                        ContactList.Add(s);
                }
                {
                    string[] lines = File.ReadAllLines(dialog.FileName);
                    foreach (string s in lines)
                        listView2.Items.Add(s);
                }
                label4.Text = listView2.Items.Count.ToString();
            }
        }

        private void bunifuFlatButton16_Click(object sender, EventArgs e)
        {
            Thread th = new Thread(FriendsAdd);
            th.IsBackground = true;
            if (bunifuFlatButton16.Text == "Add as Friend")
            {
                th.Start();
                bunifuFlatButton16.Text = "Stop";
            }
            else
            {
                th.Abort();
                timer4.Stop();
                bunifuFlatButton16.Text = "Add as Friend";
            }
        }

        void FriendsAdd()
        {
            timer4.Start();
        }

        private void bunifuFlatButton15_Click(object sender, EventArgs e)
        {
            Thread th = new Thread(SenderMessageContact);
            th.IsBackground = true;
            if (bunifuFlatButton15.Text == "Start")
            {
                th.Start();
                bunifuFlatButton15.Text = "Stop";
            }
            else
            {
                th.Abort();
                bunifuFlatButton15.Text = "Start";
            }
        }

        void SenderMessageContact()
        {
            Invoke(new MethodInvoker(() =>
            {
                string message = textBox1.Text;
                foreach (string contact in ContactList)
                {
                    try
                    {
                        skype.SendMessage(contact, message);
                    }
                    catch (Exception exec)
                    {
                        MessageBox.Show(String.Format("Failed to send message to a contact {0}: {1} ", contact,
                            exec.Message));
                    }
                }
                MessageBox.Show("Messages successfully sent ");

            }));
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            int friend = 0;
            try
            {
                IUserCollection friends = skype.SearchForUsers(ContactList[friend]);
                if (friends.Count >= 1)
                {
                    friends[1].BuddyStatus = TBuddyStatus.budPendingAuthorization;
                    friend++;
                    label5.Text += friend.ToString();
                    label5.Refresh();
                }

                if (friend >= ContactList.Count)
                {
                    timer4.Stop();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Skype user does not exist: {0} ", ex.Message));
            }
            MessageBox.Show("Users successfully added to friends", "Friends added", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void bunifuFlatButton17_Click(object sender, EventArgs e)
        {
            Thread th = new Thread(BlinkName);
            th.IsBackground = true;
            if (bunifuFlatButton17.Text == "Blink Name Start")
            {
                blnName = true;
                th.Start();
                bunifuFlatButton17.Text = "Blink Name Stop";
            }
            else
            {
                th.Abort();
                blnName = false;
                bunifuFlatButton17.Text = "Blink Name Start";
            }
        }

        void BlinkName()
        {
            while (blnName)
            {
                skype.CurrentUserProfile.FullName = textBox3.Text;
                Thread.Sleep(Int32.Parse(numericUpDown1.Text) * 1000);
                skype.CurrentUserProfile.FullName = textBox4.Text;
                Thread.Sleep(Int32.Parse(numericUpDown1.Text) * 1000);
            }
        }

        private void timer5_Tick(object sender, EventArgs e)
        {
            
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://xakfor.net/forum/");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://infomania.su/");
        }

        private void bunifuFlatButton11_Click(object sender, EventArgs e)
        {
            panel6.Visible = false;
            panel4.Visible = false;
            Spammer.Visible = false;
            panel5.Visible = false;
            panel7.Visible = true;
            panel7.Location = new Point(200, 88);
        }
    }
}
    



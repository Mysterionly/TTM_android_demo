using System;
using System.Collections.Generic;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Widget;
using App7;
using MySql.Data.MySqlClient;
using static App7.MainActivity;
using System.IO;

namespace AppHehe
{
    public class BGSender
    {
        string gid = "";
        UserCrendels mineProf = new UserCrendels();
        //MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder();
        MySqlConnection con;
        string conStr;
        bool imIn = false;

        public void Identify(Context cont)
        {
            //builder.Database = "b95381pr_ttm";
            //builder.Server = "b95381pr.beget.tech";
            //builder.PersistSecurityInfo = false;
            //builder.UserID = "b95381pr_ttm";
            //builder.Password = "000000";
            //builder.CharacterSet = "utf8";

            //conStr = builder.ConnectionString;
            conStr = "server=b95381pr.beget.tech;user=b95381pr_ttm;database=b95381pr_ttm;port=3306;password=000000;";
            con = new MySqlConnection(conStr);

            if (imIn)
                SendNotification(cont);
            else
                BootLogin(cont);
        }
        void BootLogin(Context cont)
        {
            //string path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).ToString();
            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            string filename = System.IO.Path.Combine(path, "0012");

            string filename2 = System.IO.Path.Combine(path, "ormids");
            using (var streamWriter = new StreamWriter(filename2, true))
            {
                streamWriter.Write("");
            }

            if (System.IO.File.Exists(filename))
            {
                using (var streamReader = new StreamReader(filename))
                {
                    string content = streamReader.ReadToEnd();
                    gid = content;
                }
            }
            else
            {
                using (var streamWriter = new StreamWriter(filename, true))
                {
                    streamWriter.Write("");
                }
            }

            if (gid.Length > 0)
            {
                int cntr = 0;
                string log = "SELECT COUNT(*) FROM `user_profs` WHERE `gid` = '" + gid + "'";
                string getUser = "SELECT `user_id`, `username`, `birthday`, `gender`, `cash` FROM `user_profs` WHERE `gid` = '" + gid + "'";
                con = new MySqlConnection(conStr);
                try
                {
                    con.Open();

                    MySqlCommand cmd = new MySqlCommand(getUser, con);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        cntr = Int32.Parse(reader[0].ToString());
                        if (cntr != 0)
                        {
                            //Toast.MakeText(this, "you logged in successfully!", ToastLength.Long).Show();

                            mineProf.user_id = Int32.Parse(reader[0].ToString());
                            mineProf.user_name = reader[1].ToString();
                            //string date = reader[2].ToString();
                            mineProf.birthday = DateTime.Parse(reader[2].ToString());
                            mineProf.gender = Int32.Parse(reader[3].ToString());
                            //mineProf.cash = Int32.Parse(reader[4].ToString());

                            reader.Close();
                            con.Close();
                            SendNotification(cont);
                            imIn = true;
                            break;
                        }
                        else
                        {
                            //Toast.MakeText(this, "TTM: you havent login! 111", ToastLength.Long).Show();
                        }
                    }
                }
                catch (Exception e)
                {
                    string em = e.Message;
                }
            }
            else
            {
                // Toast.MakeText(cont, "TTM: you havent login! 222", ToastLength.Long).Show();
            }
        }
        void MNotify(string s, Context cont)
        {
            NotificationManager notificationManager = (NotificationManager)cont.GetSystemService(Context.NotificationService);

            string CHANNEL_ID = "my_channel_01";

            if (Build.VERSION.SdkInt >= Build.VERSION_CODES.O)
            {
                NotificationChannel mChannel = new NotificationChannel(CHANNEL_ID, "TTM BG-notify", NotificationImportance.High);
                //mChannel.SetDescription(Description);
                mChannel.EnableLights(true);
                //mChannel.SetLightColor(Color.Red);
                mChannel.EnableVibration(true);
                mChannel.SetVibrationPattern(new long[] { 100, 200, 300, 400, 500, 400, 300, 200, 400 });
                mChannel.SetShowBadge(false);
                notificationManager.CreateNotificationChannel(mChannel);
            }

            Intent i = new Intent(cont, typeof(MainActivity));
            PendingIntent pip = PendingIntent.GetActivity(cont, 0, i, PendingIntentFlags.OneShot);

            NotificationCompat.Builder builder2 = new NotificationCompat.Builder(cont, CHANNEL_ID)
                            .SetSmallIcon(Resource.Drawable.test)
                            .SetContentTitle("Hey, " + mineProf.user_name + "!")
                            .SetContentIntent(pip)
                            .SetContentText(s);

            Intent intentTL = new Intent(cont, typeof(JobSrvs));

            Android.Support.V4.App.TaskStackBuilder stackBuilder = Android.Support.V4.App.TaskStackBuilder.Create(cont);
            stackBuilder.AddParentStack(Java.Lang.Class.FromType(typeof(MainActivity)));
            stackBuilder.AddNextIntent(intentTL);

            PendingIntent resultPendingIntent = stackBuilder.GetPendingIntent(0, (int)PendingIntentFlags.UpdateCurrent);

            builder2.SetContentIntent(resultPendingIntent);

            notificationManager.Notify(9999, builder2.Build());

        }
        void SendNotification(Context cont)
        {
            List<DialogItem> inboxRecieved = new List<DialogItem>();
            List<int> oldRecieved = new List<int>();

            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            string filename = System.IO.Path.Combine(path, "ormids");
            using (var streamReader = new StreamReader(filename))
            {
                string content = streamReader.ReadToEnd();
                while (content.IndexOf("@") > -1)
                {
                    int ey = Int32.Parse(content.Substring(0, content.IndexOf("@")));
                    content = content.Substring(content.IndexOf("@") + 1);
                    oldRecieved.Add(ey);
                }
            }

            try
            {
                //while (true)
                //{
                //MNotify("yay?", cont, intent);

                string getAllDialogues = "SELECT DISTINCT a.dial_id FROM (SELECT * FROM `dialogues` NATURAL JOIN dial_msgs) a NATURAL JOIN messages WHERE a.user1_id = '" + mineProf.user_id + "' OR  a.user2_id = '" + mineProf.user_id + "'";
                List<int> all = new List<int>();

                try
                {
                    con.Open();

                    MySqlCommand cmd = new MySqlCommand(getAllDialogues, con);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        all.Add(Int32.Parse(reader[0].ToString()));
                    }
                    reader.Close();
                    con.Close();
                }
                catch
                {

                }

                try
                {
                    inboxRecieved.Clear();

                    con.Open();
                    foreach (int d in all)
                    {
                        string getLastMsg = "SELECT * FROM (SELECT * FROM `dialogues` NATURAL JOIN dial_msgs) a NATURAL JOIN messages WHERE a.dial_id = '" + d + "' ORDER BY msg_id DESC LIMIT 1";

                        MySqlCommand cmd = new MySqlCommand(getLastMsg, con);
                        MySqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            string thisUID = reader[2].ToString();
                            string senderIndex = reader[5].ToString();
                            int u2i = 0;
                            string lastMCut;
                            bool isreaden = false;

                            if (reader[6].ToString().Length <= 30)
                                lastMCut = reader[6].ToString();
                            else
                                lastMCut = reader[6].ToString().Substring(0, 30) + "...";

                            if (reader[7].ToString() == "1")
                                isreaden = true;

                            if (Int32.Parse(thisUID) == mineProf.user_id)
                            {
                                if (Int32.Parse(senderIndex) == 1)
                                {

                                }
                                else
                                {
                                    inboxRecieved.Add(new DialogItem()
                                    {
                                        msgid = Int32.Parse(reader[0].ToString()),
                                        dial_id = Int32.Parse(reader[1].ToString()),
                                        u1_id = Int32.Parse(reader[2].ToString()),
                                        u2_id = u2i,
                                        last_msg = lastMCut,
                                        sentByMe = false,
                                        readen = isreaden
                                    });
                                }
                            }
                            else
                            {
                                if (Int32.Parse(senderIndex) == 2)
                                {

                                }
                                else
                                {
                                    inboxRecieved.Add(new DialogItem()
                                    {
                                        msgid = Int32.Parse(reader[0].ToString()),
                                        dial_id = Int32.Parse(reader[1].ToString()),
                                        u1_id = Int32.Parse(reader[2].ToString()),
                                        u2_id = u2i,
                                        last_msg = lastMCut,
                                        sentByMe = false,
                                        readen = isreaden
                                    });
                                }
                            }
                        }
                        reader.Close();
                    }
                    con.Close();
                }
                catch (Exception e)
                {
                    string nen = e.Message;
                }

                int cntr1 = 0;
                int cntr2 = 0;

                foreach (var m in inboxRecieved)
                {
                    if (!m.readen) cntr2++;
                    if (!oldRecieved.Contains(m.msgid))
                        cntr1++;
                }

                List<DialogItem> torefresh = new List<DialogItem>();
                oldRecieved.Clear();
                string oldmsgs = "";
                foreach (var mam in inboxRecieved)
                {
                    oldmsgs += mam.msgid.ToString() + "@";
                    //oldRecieved.Add(mam.msgid);
                }
                using (var streamWriter = new StreamWriter(filename))
                {
                    streamWriter.Write(oldmsgs);
                }

                if (cntr1 > 0)
                {
                    if (cntr2 > 1)
                    {
                        MNotify("You've got " + cntr2 + " new replies! Take a look...", cont);
                    }
                    else
                        MNotify("You've got a new reply! Take a look...", cont);
                }
                //Thread.Sleep(7000);
                //}
            }
            catch (Exception qqq)
            {

            }
        }
    }

    [BroadcastReceiver]
    public class TimerBR : BroadcastReceiver
    {
        BGSender bgs = new BGSender();
        public override void OnReceive(Context context, Intent intent)
        {
            AlarmManager am = (AlarmManager)context.GetSystemService(Context.AlarmService);
            PendingIntent pendingIntent = PendingIntent.GetBroadcast(context, 0, intent, PendingIntentFlags.CancelCurrent);
            am.Set(AlarmType.RtcWakeup, DateTimeOffset.Now.ToUnixTimeMilliseconds() + 7000, pendingIntent);

            Thread t1 = new Thread(Meh)
            {
                IsBackground = true
            };
            t1.Start();

            void Meh()
            {
                bgs.Identify(context);
            }

            //Toast.MakeText(context, "trying to make a popup", ToastLength.Long).Show();
            //NotificationManager nm = (NotificationManager)context.GetSystemService(Context.NotificationService);
            //string CHANNEL_ID = "TEST NOTIFY0";
            //if (Build.VERSION.SdkInt >= Build.VERSION_CODES.O)
            //{
            //    NotificationChannel mChannel = new NotificationChannel(CHANNEL_ID, "TEST NOTIFY1", NotificationImportance.High);
            //    //mChannel.SetDescription(Description);
            //    mChannel.EnableLights(true);
            //    //mChannel.SetLightColor(Color.Red);
            //    mChannel.EnableVibration(true);
            //    mChannel.SetVibrationPattern(new long[] { 100, 200, 300, 400, 500, 400, 300, 200, 400 });
            //    mChannel.SetShowBadge(false);
            //    nm.CreateNotificationChannel(mChannel);
            //}
            //NotificationCompat.Builder builder = new NotificationCompat.Builder(context, CHANNEL_ID)
            //                .SetSmallIcon(Resource.Drawable.test)
            //                .SetContentTitle("alarmmsg!")
            //                .SetContentText("its timeBr");

            //Intent intentTL = new Intent(context, typeof(JobSrvs));

            //Android.Support.V4.App.TaskStackBuilder stackBuilder = Android.Support.V4.App.TaskStackBuilder.Create(context);
            //stackBuilder.AddParentStack(Java.Lang.Class.FromType(typeof(MainActivity)));
            //stackBuilder.AddNextIntent(intentTL);

            //PendingIntent resultPendingIntent = stackBuilder.GetPendingIntent(0, (int)PendingIntentFlags.UpdateCurrent);

            //builder.SetContentIntent(resultPendingIntent);

            //NotificationManager notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);
            //notificationManager.Notify(9999, builder.Build());
        }
    }
}
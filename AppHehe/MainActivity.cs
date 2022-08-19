using Android.App;
using Android.OS;
using Android.Widget;
using System.Collections.Generic;
using Android.Views;
using Android.Support.Design.Widget;
using static Android.Widget.AdapterView;
using Android.Content;
using Android.Graphics;
using MySql.Data.MySqlClient;
using System;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common.Apis;
using Android.Gms.Auth.Api;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using System.Threading;
using AppHehe;
using Android.Views.InputMethods;
using Android.App.Job;
using System.IO;

namespace App7
{
    public class UserCrendels
    {
        public int user_id;
        public string gid;
        public string user_name;
        public string email;
        public DateTime birthday;
        public int gender;
        public int cash;
    }

    public class CurrentDial
    {
        public int dialId;
        public DateTime match;
        public int u1;
        public int u2;
    }

    //// adb root am broadcast -a android.intent.action.BOOT_COMPLETED AppHehe.AppHehe
    //[BroadcastReceiver(Enabled = true, Exported = true, DirectBootAware = true)]
    //[IntentFilter(new string[] { Intent.ActionBootCompleted, Intent.ActionLockedBootCompleted, "android.intent.action.QUICKBOOT_POWERON", "com.htc.intent.action.QUICKBOOT_POWERON" })]
    //public class GetMsgBR : BroadcastReceiver
    //{
    //    public override void OnReceive(Context context, Intent intent)
    //    {
    //        Toast.MakeText(context, "i've boot! meh0000", ToastLength.Long).Show();


    //        if (intent.Action.Equals(Intent.ActionBootCompleted))
    //        {

    //            Toast.MakeText(context, "i've boot! meh1111", ToastLength.Long).Show();

    //            JobScheduler mJobScheduler = (JobScheduler)context.GetSystemService(Context.JobSchedulerService);
    //            JobInfo.Builder mJobBuilder = new JobInfo.Builder(1, new ComponentName(context, "AppHehe.AppHehe.JobSrvs"));
    //            mJobBuilder.SetMinimumLatency(3000);

    //            if (mJobScheduler != null && mJobScheduler.Schedule(mJobBuilder.Build()) != JobScheduler.ResultSuccess)
    //            {
    //                int i = -1;
    //            }
    //            //TTM_notificator.EnqueueWork(context, new Intent(context, typeof(TTM_notificator)));
    //        }
    //    }
    //}

    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : Android.Support.V7.App.AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {

        public UserCrendels mineProf = new UserCrendels();
        public CurrentDial currentDial = new CurrentDial();

        public bool imIn = false;

        //string conStr = а вот))

        MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder();
        string conStr;

        MySqlConnection con;

        List<DialogItem> inboxRecieved = new List<DialogItem>();
        List<DialogItem> oldRecieved = new List<DialogItem>();
        List<DialogItem> inboxSent = new List<DialogItem>();
        List<TagItem> tags = new List<TagItem>();
        ListView listView1;
        ListView listView2;
        ListView listView3;
        TabHost iobox;
        TabHost create;
        ScrollView profile;
        ListView msgTagList;
        TextInputEditText newL;
        LinearLayout dialPage;

        Android.Support.V7.Widget.Toolbar tb;

        Button nickname;
        Button gender;
        Button age;
        Button mainLang;
        Button sendAnswer;

        LinearLayout extraSigninMenu;
        BottomNavigationView navigation1;
        FrameLayout mainFrame;
        GoogleApiClient mGoogleApiClient;
        ListView dialList;


        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.tbMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            tb.Title = "Inbox";
            dialPage.Visibility = ViewStates.Invisible;
            mainFrame.Visibility = ViewStates.Visible;
            navigation1.Visibility = ViewStates.Visible;
            SupportActionBar.SetDisplayHomeAsUpEnabled(false);
            SupportActionBar.SetHomeButtonEnabled(false);
            currentDial.dialId = 0;
            return base.OnOptionsItemSelected(item);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            //----
            //StartService(new Intent(this, typeof(TTM_notificator)));
            //TTM_notificator.EnqueueWork(this, new Intent(this, typeof(TTM_notificator)));

            JobScheduler mJobScheduler = (JobScheduler)this.GetSystemService(Context.JobSchedulerService);
            JobInfo.Builder mJobBuilder = new JobInfo.Builder(1, new ComponentName(this, "appHehe.appHehe.JobSrvs"));
            mJobBuilder.SetMinimumLatency(3000);

            if (mJobScheduler != null && mJobScheduler.Schedule(mJobBuilder.Build()) != JobScheduler.ResultSuccess) {
                int i = -1;
            }

            //JobScheduler mJobScheduler = (JobScheduler)this.GetSystemService(Context.JobSchedulerService);
            //        JobInfo job = new JobInfo.Builder(0 /*jobid*/, new ComponentName(this, Java.Lang.Class.FromType(typeof(Jop))))
            //                    //.SetMinimumLatency(1000)
            //                    .SetRequiredNetworkType(NetworkType.Unmetered)
            //                    .SetRequiresCharging(true)
            //                    .SetPeriodic(2000)
            //                    .Build();
            //        mJobScheduler.Schedule(job);


            mainFrame = FindViewById<FrameLayout>(Resource.Id.contentScreen);
            navigation1 = FindViewById<BottomNavigationView>(Resource.Id.bottom_navigation);
            extraSigninMenu = FindViewById<LinearLayout>(Resource.Id.regExtra);

            dialPage = FindViewById<LinearLayout>(Resource.Id.dialAnswer);
            tb = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar1);
            SetSupportActionBar(tb);
            SupportActionBar.Title = "Is it really works? ";
            tb.InflateMenu(Resource.Menu.tbMenu);

            SupportActionBar.SetDisplayHomeAsUpEnabled(false);
            SupportActionBar.SetHomeButtonEnabled(false);
            //Below you can set you preferred icon              
            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.backArrow);
            tb.SetPadding(0, 0, 0, 0);
            tb.SetPadding(0, 0, 0, 0);
            tb.SetContentInsetsAbsolute(0, 0);

            //builder.Database = "b95381pr_ttm";
            //builder.Server = "b95381pr.beget.tech";
            //builder.PersistSecurityInfo = false;
            //builder.UserID = "b95381pr_ttm";
            //builder.Password = "000000";
            //builder.CharacterSet = "utf8";

            //conStr = builder.ConnectionString;
            conStr = "тут строка подключения";

            GoogleSignInOptions gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                .RequestEmail()
                .Build();

            mGoogleApiClient = new GoogleApiClient.Builder(this)
                    //.EnableAutoManage(this /* FragmentActivity */, this /* OnConnectionFailedListener */)
                    .AddApi(Auth.GOOGLE_SIGN_IN_API, gso)
                    .Build();

            var signInIntent = Auth.GoogleSignInApi.GetSignInIntent(mGoogleApiClient);
            StartActivityForResult(signInIntent, 9001);


            navigation1.Visibility = ViewStates.Invisible;
            mainFrame.Visibility = ViewStates.Invisible;
            extraSigninMenu.Visibility = ViewStates.Invisible;
            dialPage.Visibility = ViewStates.Invisible;

            nickname = FindViewById<Button>(Resource.Id.button5);
            gender = FindViewById<Button>(Resource.Id.button3);
            age = FindViewById<Button>(Resource.Id.button4);
            mainLang = FindViewById<Button>(Resource.Id.button6);
            sendAnswer = FindViewById<Button>(Resource.Id.sendMsg);

            Button newFlySend = FindViewById<Button>(Resource.Id.button1);

            TabHost tabHost = FindViewById<TabHost>(Resource.Id.tabHost1);
            tabHost.Setup();
            TabHost.TabSpec tabSpec = tabHost.NewTabSpec("tag1");
            tabSpec.SetContent(Resource.Id.inbox1);
            tabSpec.SetIndicator("Recieved");
            tabHost.AddTab(tabSpec);
            tabSpec = tabHost.NewTabSpec("tag2");
            tabSpec.SetContent(Resource.Id.inbox2);
            tabSpec.SetIndicator("Sent");
            tabHost.AddTab(tabSpec);
            tabHost.SetCurrentTabByTag("tag1");

            tabHost = FindViewById<TabHost>(Resource.Id.tabHost2);
            tabHost.Setup();
            tabSpec = tabHost.NewTabSpec("tag1");
            tabSpec.SetContent(Resource.Id.linearLayout2);
            tabSpec.SetIndicator("New");
            tabHost.AddTab(tabSpec);
            tabSpec = tabHost.NewTabSpec("tag2");
            tabSpec.SetContent(Resource.Id.linearLayout3);
            tabSpec.SetIndicator("Feed");
            tabHost.AddTab(tabSpec);
            tabHost.SetCurrentTabByTag("tag1");
            tabHost.Visibility = ViewStates.Invisible;

            var font = Typeface.CreateFromAsset(Assets, "Peterbuilt.ttf");

            msgTagList = FindViewById<ListView>(Resource.Id.listView1);
            newL = FindViewById<TextInputEditText>(Resource.Id.textInputEditText1);
            newL.Typeface = font;
            newL.TextSize = 35;

            //msgTagList.Visibility = ViewStates.Invisible;

            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.bottom_navigation);
            navigation.SetOnNavigationItemSelectedListener(this);

            listView1 = FindViewById<ListView>(Resource.Id.inbox1);
            listView2 = FindViewById<ListView>(Resource.Id.inbox2);
            listView3 = FindViewById<ListView>(Resource.Id.listView1);
            dialList = FindViewById<ListView>(Resource.Id.dialList);

            iobox = FindViewById<TabHost>(Resource.Id.tabHost1);
            create = FindViewById<TabHost>(Resource.Id.tabHost2);
            profile = FindViewById<ScrollView>(Resource.Id.profilePage);


            var extraSigninBut = FindViewById<Button>(Resource.Id.proceedReg);

            BottomMenu(1);

            tags.Add(new TagItem()
            {
                tag_name = "ADD TAG",
                cat_ico = "000",
                isService = true
            });
            listView3.Adapter = new TagAdapter(this, tags);
            listView3.VerticalScrollBarEnabled = false;
            void AddTag()
            {
                tags.Reverse();
                Android.Support.V7.Widget.PopupMenu menu = new Android.Support.V7.Widget.PopupMenu(this, navigation);
                menu.Inflate(Resource.Menu.bottom_navigation_main);
                menu.Show();
                tags.Add(new TagItem()
                {
                    tag_name = "sample tag1",
                    cat_ico = "000",
                    isService = false
                });
                tags.Reverse();
                listView3.Adapter = new TagAdapter(this, tags);
            }
            listView1.ItemClick += (object sender, ItemClickEventArgs e) =>
                {
                    currentDial.dialId = inboxRecieved[(int)e.Id].dial_id;
                    currentDial.u1 = inboxRecieved[(int)e.Id].u1_id;
                    currentDial.u2 = inboxRecieved[(int)e.Id].u2_id;

                    FillDialList(inboxRecieved[(int)e.Id].msgid);

                    tb.Title = "Dialogue";
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    dialPage.Visibility = ViewStates.Visible;
                    mainFrame.Visibility = ViewStates.Invisible;
                    navigation1.Visibility = ViewStates.Invisible;
                };
            listView2.ItemClick += (object sender, ItemClickEventArgs e) =>
            {
                currentDial.dialId = inboxSent[(int)e.Id].dial_id;
                currentDial.u1 = inboxSent[(int)e.Id].u1_id;
                currentDial.u2 = inboxSent[(int)e.Id].u2_id;

                FillDialList(inboxRecieved[(int)e.Id].msgid);

                tb.Title = "Dialogue";
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.SetHomeButtonEnabled(true);
                dialPage.Visibility = ViewStates.Visible;
                mainFrame.Visibility = ViewStates.Invisible;
                navigation1.Visibility = ViewStates.Invisible;
            };
            void FillDialList(int msgid)
            {
                int u2i = 0;

                string setreaden = "UPDATE `b95381pr_ttm`.`messages` SET `readen` = '1' WHERE `messages`.`msg_id` = " + msgid;
                string getDialMsgs = "SELECT * FROM (SELECT * FROM `dialogues` NATURAL JOIN dial_msgs) a NATURAL JOIN messages WHERE a.dial_id = " + currentDial.dialId;
                List<DialogItem> dialMsgsList = new List<DialogItem>();

                try
                {
                    con.Open();

                    MySqlCommand cmd = new MySqlCommand(getDialMsgs, con);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string thisUID = reader[2].ToString();
                        string senderIndex = reader[5].ToString();
                        if (Int32.Parse(thisUID) == mineProf.user_id)
                        {
                            if (Int32.Parse(senderIndex) == 1)
                            {
                                dialMsgsList.Add(new DialogItem()
                                {
                                    msgid = Int32.Parse(reader[0].ToString()),
                                    dial_id = Int32.Parse(reader[1].ToString()),
                                    u1_id = Int32.Parse(reader[2].ToString()),
                                    u2_id = u2i,
                                    last_msg = reader[6].ToString(),
                                    sentByMe = true
                                });
                            }
                            else
                            {
                                dialMsgsList.Add(new DialogItem()
                                {
                                    msgid = Int32.Parse(reader[0].ToString()),
                                    dial_id = Int32.Parse(reader[1].ToString()),
                                    u1_id = Int32.Parse(reader[2].ToString()),
                                    u2_id = u2i,
                                    last_msg = reader[6].ToString(),
                                    sentByMe = false
                                });
                            }
                        }
                        else
                        {
                            if (Int32.Parse(senderIndex) == 2)
                            {
                                dialMsgsList.Add(new DialogItem()
                                {
                                    msgid = Int32.Parse(reader[0].ToString()),
                                    dial_id = Int32.Parse(reader[1].ToString()),
                                    u1_id = Int32.Parse(reader[2].ToString()),
                                    u2_id = u2i,
                                    last_msg = reader[6].ToString(),
                                    sentByMe = true
                                });
                            }
                            else
                            {
                                dialMsgsList.Add(new DialogItem()
                                {
                                    msgid = Int32.Parse(reader[0].ToString()),
                                    dial_id = Int32.Parse(reader[1].ToString()),
                                    u1_id = Int32.Parse(reader[2].ToString()),
                                    u2_id = u2i,
                                    last_msg = reader[6].ToString(),
                                    sentByMe = false
                                });
                            }
                        }
                    }
                    reader.Close();
                    con.Close();

                    dialList.Adapter = new ColorAdapter(this, dialMsgsList);
                    dialList.SetSelection(dialList.Adapter.Count - 1);

                    con.Open();
                    cmd = new MySqlCommand(setreaden, con);
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                catch (Exception e)
                {
                    Toast.MakeText(this, "cant get dial msgs :((", ToastLength.Long).Show();
                }

                //Thread t2 = new Thread(Rser);
                //t2.IsBackground = true;
                //t2.Start();

                //IntentFilter filter = new IntentFilter();
                //filter.AddAction("SOME_ACTION");
                //filter.AddAction("SOME_OTHER_ACTION");

                //getMsgBR receiver = new getMsgBR();
                //RegisterReceiver(receiver, filter);
            }
            void SendNewAnswer()
            {
                TextInputEditText dialAndwerText = FindViewById<TextInputEditText>(Resource.Id.textInputEditText3);
                int senderInd = 0;
                if (currentDial.u1 == mineProf.user_id) senderInd = 1;
                else senderInd = 2;

                string newM = "INSERT INTO `b95381pr_ttm`.`messages` (`msg_id`, `senderIndex`, `mess_text`) VALUES (NULL, '" + senderInd + "', \"" + dialAndwerText.Text + "\");";
                MySqlCommand cmd;
                con = new MySqlConnection(conStr);
                try
                {
                    con.Open();
                    cmd = new MySqlCommand(newM, con);
                    cmd.ExecuteNonQuery();
                    long thisMsgId = cmd.LastInsertedId;
                    con.Close();

                    string newDialMsg = "INSERT INTO `b95381pr_ttm`.`dial_msgs` (`dial_id`, `msg_id`) VALUES ('" + currentDial.dialId.ToString() + "', '" + thisMsgId.ToString() + "');";

                    con.Open();
                    cmd = new MySqlCommand(newDialMsg, con);
                    cmd.ExecuteNonQuery();
                    con.Close();

                    currentDial.dialId = 0;
                    dialAndwerText.Text = "";

                    tb.Title = "Inbox";
                    dialPage.Visibility = ViewStates.Invisible;
                    mainFrame.Visibility = ViewStates.Visible;
                    navigation1.Visibility = ViewStates.Visible;
                    SupportActionBar.SetDisplayHomeAsUpEnabled(false);
                    SupportActionBar.SetHomeButtonEnabled(false);

                    InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                    imm.HideSoftInputFromWindow(dialAndwerText.WindowToken, 0);

                    Toast.MakeText(this, "successfully!", ToastLength.Long).Show();

                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, "cant sent the answer for some reason :((", ToastLength.Long).Show();
                }
            }

            sendAnswer.Click += (sender, e) =>
            {
                SendNewAnswer();
            };
            msgTagList.ItemClick += (object sender, ItemClickEventArgs e) =>
            {
                if (e.Id < tags.Count - 1)
                {
                    tags.RemoveAt((int)e.Id);
                    listView3.Adapter = new TagAdapter(this, tags);
                }
                else
                {
                    if (tags.Count < 4)
                        AddTag();
                }
            };
            EditText birthdate = FindViewById<EditText>(Resource.Id.editText1);
            extraSigninBut.Click += delegate
            {
                TextInputEditText username = FindViewById<TextInputEditText>(Resource.Id.textInputEditText2);
                RadioButton genRB1 = FindViewById<RadioButton>(Resource.Id.radioButton1);
                RadioButton genRB2 = FindViewById<RadioButton>(Resource.Id.radioButton2);
                RadioButton genRB3 = FindViewById<RadioButton>(Resource.Id.radioButton3);

                int gender = 0;

                if (genRB1.Checked) gender = 1;
                else if (genRB2.Checked) gender = 2;
                else if (genRB3.Checked) gender = 3;

                string nickname = username.Text;
                DateTime bdate = DateTime.Parse(birthdate.Text);


                Register(nickname, mineProf.gid, mineProf.email, gender, bdate);
            };
            birthdate.Click += (sender, e) => {
                DateTime today = DateTime.Today;
                DatePickerDialog dialog = new DatePickerDialog(this, OnDateSet, today.Year, today.Month - 1, today.Day);
                dialog.DatePicker.MinDate = today.Millisecond;
                dialog.Show();
            };
            newFlySend.Click += (sender, e) => {
                if (newL.Text.Length > 10)
                {
                    bool sS = CreateDialogue(newL.Text);
                    if (sS)
                    {
                        newL.Text = "";
                        Toast.MakeText(this, "you did it!", ToastLength.Long).Show();
                        //+ get new random fly
                    }
                    else
                    {
                        Toast.MakeText(this, "somethin went wrong :(", ToastLength.Long).Show();
                    }
                    InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                    imm.HideSoftInputFromWindow(newL.WindowToken, 0);
                }
                else Toast.MakeText(this, "your letter must me at least 10 symbols long!", ToastLength.Long).Show();
            };
            void OnDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
            {
                birthdate.Text = e.Date.ToShortDateString();
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            //Log.Debug(TAG, "onActivityResult:" + requestCode + ":" + resultCode + ":" + data);

            // Result returned from launching the Intent from GoogleSignInApi.getSignInIntent(...);
            if (requestCode == 9001)
            {
                var result = Auth.GoogleSignInApi.GetSignInResultFromIntent(data);
                HandleSignInResult(result);
                //Toast.MakeText(this, "google login successfully proceed", ToastLength.Long).Show();
            }
            else Toast.MakeText(this, "cant do google login", ToastLength.Long).Show();
        }
        public void HandleSignInResult(GoogleSignInResult result)
        {
            //Log.Debug(TAG, "handleSignInResult:" + result.IsSuccess);
            if (result.IsSuccess)
            {
                //AlertDialog.Builder alert = new AlertDialog.Builder(this);
                //alert.SetTitle("all right!");
                //alert.SetMessage("r u ready to continue?");
                //alert.SetPositiveButton("Proceed", (senderAlert, args) => {
                //    Toast.MakeText(this, "okay!", ToastLength.Short).Show();

                // Signed in successfully, show authenticated UI.
                var acct = result.SignInAccount;
                mineProf.gid = acct.Id;
                mineProf.email = acct.Email;
                Toast.MakeText(this, "hey, " + acct.Id, ToastLength.Long).Show();
                Login(mineProf.gid);
                //});
                //alert.SetNegativeButton("Cancel", (senderAlert, args) => {
                //    Toast.MakeText(this, "Cancelled!", ToastLength.Short).Show();
                //});
                //alert.Show();
            }
            else
            {
                //Toast.MakeText(this, "A PROBLEM WITH GOOGLE LOGIN!!!", ToastLength.Long).Show();
                //AlertDialog.Builder alert = new AlertDialog.Builder(this);
                //alert.SetTitle("We have a problem signing you in...");
                //alert.SetMessage("okay i got fucked...");
                //alert.SetPositiveButton("Delete", (senderAlert, args) => {
                //    Toast.MakeText(this, "Deleted!", ToastLength.Short).Show();
                //});
                //alert.SetNegativeButton("Cancel", (senderAlert, args) => {
                //    Toast.MakeText(this, "Cancelled!", ToastLength.Short).Show();
                //});
                //alert.Show();
            }
        }

        public void Register(string username, string gid, string email, int gender, DateTime birthd)
        {
            //string path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).ToString();
            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            string filename2 = System.IO.Path.Combine(path, "ormids");
            using (var streamWriter = new StreamWriter(filename2))
            {
                streamWriter.Write("");
                streamWriter.Close();
            }
            string reg = "INSERT INTO `b95381pr_ttm`.`user_profs` (`user_id`, `gid`, `email`, `username`, `gender`, `birthday`, `cash`, `partnership`, `userpic`) VALUES (NULL, '" + gid + "','" + email + "', '" + username + "', " + gender + ", '" + birthd.Year.ToString() + "." + birthd.Month.ToString() + "." + birthd.Day.ToString() + "' , NULL, NULL, NULL)";
            MySqlCommand cmd = new MySqlCommand(reg, con);
            con = new MySqlConnection(conStr);
            try
            {
                con.Open();

                cmd = new MySqlCommand(reg, con);
                cmd.ExecuteNonQuery();
                con.Close();

                //Toast.MakeText(this, "you registered successfully!", ToastLength.Long).Show();
                Login(gid);
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.ToString(), ToastLength.Long).Show();
            }
        }
        public void Login(string gid)
        {
            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            string filename = System.IO.Path.Combine(path, "0012");

            using (var streamWriter = new StreamWriter(filename))
            {
                streamWriter.Write(gid);
                streamWriter.Close();
            }

            //using (var streamReader = new StreamReader(filename))
            //{
            //    string content = streamReader.ReadToEnd();
            //    Toast.MakeText(this, content, ToastLength.Long).Show();
            //}

            int cntr = 0;
            string log = "SELECT COUNT(*) FROM `user_profs` WHERE `gid` = '" + gid + "'";
            string getUser = "SELECT `user_id`, `username`, `birthday`, `gender`, `cash` FROM `user_profs` WHERE `gid` = '" + gid + "'";
            con = new MySqlConnection(conStr);
            try
            {
                con.Open();

                MySqlCommand cmd = new MySqlCommand(log, con);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cntr = Int32.Parse(reader[0].ToString());
                }
                reader.Close();
                con.Close();

                if (cntr != 0)
                {
                    //Toast.MakeText(this, "you logged in successfully!", ToastLength.Long).Show();
                    extraSigninMenu.Visibility = ViewStates.Invisible;
                    navigation1.Visibility = ViewStates.Visible;
                    mainFrame.Visibility = ViewStates.Visible;

                    mineProf = new UserCrendels();

                    con.Open();
                    cmd = new MySqlCommand(getUser, con);
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        mineProf.user_id = Int32.Parse(reader[0].ToString());
                        mineProf.user_name = reader[1].ToString();
                        //string date = reader[2].ToString();
                        mineProf.birthday = DateTime.Parse(reader[2].ToString());
                        mineProf.gender = Int32.Parse(reader[3].ToString());
                        //mineProf.cash = Int32.Parse(reader[4].ToString());
                    }
                    reader.Close();
                    con.Close();
                    FillInfo();
                    imIn = true;
                }
                else
                {
                    Toast.MakeText(this, "oh.. is it your first time?", ToastLength.Long).Show();
                    extraSigninMenu.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "A PROBLEM WITH database", ToastLength.Long).Show();
                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetTitle("We have a problem accessing to database");
                alert.SetMessage(ex.Message + "/n " + conStr);
                alert.SetPositiveButton("Delete", (senderAlert, args) =>
                {
                    Toast.MakeText(this, "Deleted!", ToastLength.Short).Show();
                });
                alert.SetNegativeButton("Cancel", (senderAlert, args) =>
                {
                    Toast.MakeText(this, "Cancelled!", ToastLength.Short).Show();
                });
                alert.Show();

                //Toast.MakeText(this, ex.ToString(), ToastLength.Long).Show();
            }
        }

        public bool CreateDialogue(string frstmsg)
        {
            string newM = "INSERT INTO `b95381pr_ttm`.`messages` (`msg_id`, `senderIndex`, `mess_text`) VALUES (NULL, '1', \"" + frstmsg + "\");";
            string newDial = "INSERT INTO `b95381pr_ttm`.`dialogues` (`dial_id`, `user1_id`, `user2_id`, `match_date`) VALUES (NULL, '" + mineProf.user_id + "', NULL, '');";
            MySqlCommand cmd;
            con = new MySqlConnection(conStr);
            try
            {
                con.Open();
                cmd = new MySqlCommand(newM, con);
                cmd.ExecuteNonQuery();
                long thisMsgId = cmd.LastInsertedId;
                con.Close();

                con.Open();
                cmd = new MySqlCommand(newDial, con);
                cmd.ExecuteNonQuery();
                long thisDialId = cmd.LastInsertedId;
                con.Close();

                string newDialMsg = "INSERT INTO `b95381pr_ttm`.`dial_msgs` (`dial_id`, `msg_id`) VALUES ('" + thisDialId.ToString() + "', '" + thisMsgId.ToString() + "');";

                con.Open();
                cmd = new MySqlCommand(newDialMsg, con);
                cmd.ExecuteNonQuery();
                con.Close();

                return true;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.ToString(), ToastLength.Long).Show();
                return false;
            }
        }

        public void RefreshFlyLists()
        {
            try
            {
                while (true)
                {
                    string getAllDialogues = "SELECT DISTINCT a.dial_id FROM (SELECT * FROM `dialogues` NATURAL JOIN dial_msgs) a NATURAL JOIN messages WHERE a.user1_id = '" + mineProf.user_id + "' OR  a.user2_id = '" + mineProf.user_id + "'";
                    List<int> all = new List<int>();

                    con = new MySqlConnection(conStr);
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
                        Toast.MakeText(this, "EROR1", ToastLength.Long).Show();
                    }

                    try
                    {
                        inboxRecieved.Clear();
                        inboxSent.Clear();

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
                                        inboxSent.Add(new DialogItem()
                                        {
                                            msgid = Int32.Parse(reader[0].ToString()),
                                            dial_id = Int32.Parse(reader[1].ToString()),
                                            u1_id = Int32.Parse(reader[2].ToString()),
                                            u2_id = u2i,
                                            last_msg = lastMCut,
                                            sentByMe = true,
                                            readen = isreaden

                                        });
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
                                        inboxSent.Add(new DialogItem()
                                        {
                                            msgid = Int32.Parse(reader[0].ToString()),
                                            dial_id = Int32.Parse(reader[1].ToString()),
                                            u1_id = Int32.Parse(reader[2].ToString()),
                                            u2_id = u2i,
                                            last_msg = lastMCut,
                                            sentByMe = true,
                                            readen = isreaden
                                        });
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
                        Toast.MakeText(this, "EROR2" + e.Message, ToastLength.Long).Show();
                    }

                    //kek++;
                    //inboxRecieved.Add(new DialogItem()
                    //{
                    //    sender_id = 1,
                    //    last_msg = "received msg text " + kek.ToString()
                    //});

                    int cntr1 = 0;
                    int cntr2 = 0;

                    foreach (var m in inboxRecieved)
                    {
                        if (!m.readen) cntr2++;
                        if (oldRecieved.FindIndex(msg => msg.msgid == m.msgid) == -1) cntr1++;
                    }

                    List<DialogItem> torefresh = new List<DialogItem>();
                    oldRecieved.Clear();
                    oldRecieved.AddRange(inboxRecieved);

                    RunOnUiThread(() => {
                        listView1.Adapter = new ColorAdapter(this, torefresh);
                        listView2.Adapter = new ColorAdapter(this, torefresh);
                        listView1.Adapter = new ColorAdapter(this, inboxRecieved);
                        listView2.Adapter = new ColorAdapter(this, inboxSent);

                        //if (cntr1 > 0)
                        //{
                        //    if (cntr2 > 1)
                        //        MNotify("You've got " + cntr2 + " new messages! Take a look...");
                        //    else
                        //        MNotify("You've got a reply! Take a look...");
                        //}
                    });
                    System.Threading.Thread.Sleep(5000);
                }
            }
            catch (Exception qqq)
            {
                string wat = qqq.Message;
                //Toast.MakeText(this, qqq.Message, ToastLength.Long).Show();
            }
        }

        public void FillInfo()
        {
            nickname.Text = mineProf.user_name;
            if (age.Text != null) age.Text = ((DateTime.Today - mineProf.birthday).Days / 365).ToString() + " y.o.";
            else age.Text = "undefined";

            switch (mineProf.gender)
            {
                case 1:
                    gender.Text = "male";
                    break;
                case 2:
                    gender.Text = "female";
                    break;
                case 3:
                    gender.Text = "non binary";
                    break;
                default:
                    gender.Text = "undefined";
                    break;
            }

            Thread t1 = new Thread(RefreshFlyLists);
            t1.IsBackground = true;
            t1.Start();
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.navigation_home:
                    BottomMenu(1);
                    return true;

                case Resource.Id.navigation_dashboard:
                    BottomMenu(2);
                    return true;

                case Resource.Id.navigation_notifications:
                    BottomMenu(3);
                    return true;

                case Resource.Id.navigation_user:
                    BottomMenu(4);
                    return true;
            }
            return false;
        }
        public void BottomMenu(int i)
        {
            iobox.Visibility = ViewStates.Invisible;
            create.Visibility = ViewStates.Invisible;
            profile.Visibility = ViewStates.Invisible;
            switch (i)
            {
                case 1:
                    tb.Title = "Inbox";
                    iobox.Visibility = ViewStates.Visible;
                    break;
                case 2:
                    tb.Title = "Post";
                    create.Visibility = ViewStates.Visible;
                    break;
                case 3:
                    tb.Title = "Store";
                    break;
                case 4:
                    tb.Title = "Profile";
                    profile.Visibility = ViewStates.Visible;
                    break;
                default:

                    break;
            }
        }
    }
    
    public class DialogItem
    {
        public string last_msg { get; set; }
        public int msgid { get; set; }
        public int u1_id { get; set; }
        public int u2_id { get; set; }
        public int dial_id { get; set; }
        public bool readen;
        public bool sentByMe;
    }
    public class TagItem
    {
        //int resourceId = (int)typeof(Resource.Drawable).GetField("tagbg").GetValue(null);
        public string tag_name { get; set; }
        public string cat_ico { get; set; }
        public bool isService { get; set; }
    }

    public class ColorAdapter : BaseAdapter<DialogItem>
    {
        List<DialogItem> items;
        Activity context;
        public ColorAdapter(Activity context, List<DialogItem> items)
            : base()
        {
            this.context = context;
            this.items = items;
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override DialogItem this[int position]
        {
            get { return items[position]; }
        }
        public override int Count
        {
            get { return items.Count; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = items[position];

            View view = convertView;

            if (item.sentByMe)
            {
                //if (view == null) // no view to re-use, create new
                view = context.LayoutInflater.Inflate(Resource.Layout.layout3, null);
                view.FindViewById<TextView>(Resource.Id.textView1).Text = item.last_msg;
                //view.FindViewById<ImageView>(Resource.Id.imageView1).SetBackgroundColor(item.Color);

                if (item.readen) view.FindViewById<ImageView>(Resource.Id.imageView50).SetColorFilter(Color.LightGreen);
                else view.FindViewById<ImageView>(Resource.Id.imageView50).SetColorFilter(Color.LightGray);
            }
            else
            {
                //if (view == null) // no view to re-use, create new
                view = context.LayoutInflater.Inflate(Resource.Layout.layout1, null);
                view.FindViewById<TextView>(Resource.Id.textView1).Text = item.last_msg;

                if (item.readen) view.FindViewById<ImageView>(Resource.Id.imageView50).SetColorFilter(Color.LightGreen);
                else view.FindViewById<ImageView>(Resource.Id.imageView50).SetColorFilter(Color.LightGray);
            }
            return view;
        }
    }
    public class TagAdapter : BaseAdapter<TagItem>
    {
        List<TagItem> items;
        Activity context;
        public TagAdapter(Activity context, List<TagItem> items)
            : base()
        {
            this.context = context;
            this.items = items;
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override TagItem this[int position]
        {
            get { return items[position]; }
        }
        public override int Count
        {
            get { return items.Count; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = items[position];

            View view = convertView;
            if (view == null) // no view to re-use, create new
                view = context.LayoutInflater.Inflate(Resource.Layout.layout2, null);
            view.FindViewById<TextView>(Resource.Id.textView1).Text = item.tag_name;
            //view.FindViewById<ImageView>(Resource.Id.imageView1).SetImageResource(item.cat_ico);
            if (item.isService)
            {
                view.FindViewById<TextView>(Resource.Id.textView1).SetTextColor(Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.LightGray));
                view.FindViewById<ImageView>(Resource.Id.imageView1).SetImageResource(Resource.Drawable.ic_tag);
            }
            else
            {
                view.FindViewById<TextView>(Resource.Id.textView1).SetTextColor(Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.DarkGray));
                view.FindViewById<ImageView>(Resource.Id.imageView1).SetImageResource(Resource.Drawable.abc_vector_test);
            }
            return view;
        }
    }
}
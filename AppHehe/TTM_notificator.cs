using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using App7;
using MySql.Data.MySqlClient;
using static App7.MainActivity;

namespace AppHehe
{
    [Service(Permission = "android.permission.BIND_JOB_SERVICE", Exported = true)]
    public class TTM_notificator : JobIntentService
    {

        private static string Tag = typeof(TTM_notificator) + ": ";
        private CancellationTokenSource _cts;
        private static int MY_JOB_ID = 1000;

        public static void EnqueueWork(Context context, Intent work)
        {
            Java.Lang.Class cls = Java.Lang.Class.FromType(typeof(TTM_notificator));
            try
            {
                EnqueueWork(context, cls, MY_JOB_ID, work);
            }
            catch (Exception ex)
            {
                //...
            }
        }
        protected override void OnHandleWork(Intent intent)
        {
            _cts = new CancellationTokenSource(30 * 1000);

            Task.Run(async () =>
            {
                try
                {
                    //do your work here

                    List<Java.Lang.Integer> oldRecieved = new List<Java.Lang.Integer>();

                    AlarmManager am = (AlarmManager)GetSystemService(Context.AlarmService);
                    Intent iii = new Intent(this, typeof(TimerBR));
                    iii.PutIntegerArrayListExtra("oldRecieved", oldRecieved);
                    PendingIntent pendingIntent = PendingIntent.GetBroadcast(this, 0, iii, PendingIntentFlags.CancelCurrent);
                    am.Cancel(pendingIntent);
                    am.Set(AlarmType.RtcWakeup, DateTimeOffset.Now.ToUnixTimeMilliseconds() + 5000, pendingIntent);

                }
                catch (Exception e)
                {
                    //...
                }
            });
        }

    }
}
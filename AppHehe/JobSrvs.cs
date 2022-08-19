using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.App.Job;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace AppHehe
{
    [Service(Name = "appHehe.appHehe.JobSrvs")]
    public class JobSrvs : JobService
    {
        public override bool OnStartJob(JobParameters jobParameters)
        {
            //...
            Toast.MakeText(this, "job well done!", ToastLength.Long).Show();

            TTM_notificator.EnqueueWork(this, new Intent(this, typeof(TTM_notificator)));
            return false;
        }
        public override bool OnStopJob(JobParameters jobParameters)
        {
            Toast.MakeText(this, "job well finished!", ToastLength.Long).Show();
            return false;
        }
    }
}
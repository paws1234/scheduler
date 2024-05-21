using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;


namespace sched
{
    [Activity(Label = "Scheduler Activity")]
    public class SchedulerActivity : Activity
    {
        private EditText titleEditText;
        private EditText descriptionEditText;
        private EditText dateEditText;
        private ListView schedulerListView;
        private List<Scheduler> schedulerList;
        private SchedulerAdapter schedulerAdapter;
        private int selectedSchedulerId;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Scheduler);

            titleEditText = FindViewById<EditText>(Resource.Id.titleEditText);
            descriptionEditText = FindViewById<EditText>(Resource.Id.descriptionEditText);
            dateEditText = FindViewById<EditText>(Resource.Id.dateEditText);
            schedulerListView = FindViewById<ListView>(Resource.Id.schedulerListView);

            schedulerList = new List<Scheduler>();
            schedulerAdapter = new SchedulerAdapter(this, schedulerList);
            schedulerListView.Adapter = schedulerAdapter;
            schedulerListView.ItemClick += SchedulerListView_ItemClick;

            LoadSchedulers();

            Button addButton = FindViewById<Button>(Resource.Id.addButton);
            addButton.Click += OnAddClicked;

        }

        private void SchedulerListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            selectedSchedulerId = schedulerAdapter[e.Position].Id;

            
            ShowSchedulerDetailsPopup(schedulerAdapter[e.Position]);
        }

        private void LoadSchedulers()
        {
            schedulerList = DatabaseHelper.Instance.SchedulerDatabaseHelper.GetAllSchedulers().ToList();
            schedulerAdapter.Clear();
            schedulerAdapter.AddRange(schedulerList);
            schedulerAdapter.NotifyDataSetChanged();
        }

        private void OnAddClicked(object sender, EventArgs e)
        {
            Scheduler scheduler = new Scheduler
            {
                Title = titleEditText.Text,
                Description = descriptionEditText.Text,
                Date = dateEditText.Text
            };
            DatabaseHelper.Instance.SchedulerDatabaseHelper.CreateScheduler(scheduler);
            LoadSchedulers();
        }

        private void OnUpdateClicked(object sender, EventArgs e)
        {
            if (selectedSchedulerId == 0)
            {
                
                return;
            }

          
            Scheduler schedulerToUpdate = schedulerList.FirstOrDefault(s => s.Id == selectedSchedulerId);
            if (schedulerToUpdate != null)
            {
                
                schedulerToUpdate.Title = titleEditText.Text.Trim();
                schedulerToUpdate.Description = descriptionEditText.Text.Trim();
                schedulerToUpdate.Date = dateEditText.Text.Trim();

              
                DatabaseHelper.Instance.SchedulerDatabaseHelper.UpdateScheduler(schedulerToUpdate);

              
                schedulerAdapter.NotifyDataSetChanged();
            }
        }

        private void OnDeleteClicked(object sender, EventArgs e)
        {
            if (selectedSchedulerId == 0)
            {

                return;
            }

            DatabaseHelper.Instance.SchedulerDatabaseHelper.DeleteScheduler(selectedSchedulerId);
            LoadSchedulers();
        }

        private void ShowSchedulerDetailsPopup(Scheduler scheduler)
        {
            AlertDialog.Builder dialogBuilder = new AlertDialog.Builder(this);
            LayoutInflater inflater = this.LayoutInflater;

            View dialogView = inflater.Inflate(Resource.Layout.SchedulerDetailsPopup, null);
            dialogBuilder.SetView(dialogView);

            EditText popupTitleEditText = dialogView.FindViewById<EditText>(Resource.Id.popupTitleEditText);
            EditText popupDescriptionEditText = dialogView.FindViewById<EditText>(Resource.Id.popupDescriptionEditText);
            EditText popupDateEditText = dialogView.FindViewById<EditText>(Resource.Id.popupDateEditText);
            Button updateButton = dialogView.FindViewById<Button>(Resource.Id.popupUpdateButton);
            Button deleteButton = dialogView.FindViewById<Button>(Resource.Id.popupDeleteButton);

            popupTitleEditText.Text = scheduler.Title;
            popupDescriptionEditText.Text = scheduler.Description;
            popupDateEditText.Text = scheduler.Date;

            AlertDialog dialog = dialogBuilder.Create();

            updateButton.Click += (sender, e) =>
            {
                // Update the selected scheduler directly
                scheduler.Title = popupTitleEditText.Text.Trim();
                scheduler.Description = popupDescriptionEditText.Text.Trim();
                scheduler.Date = popupDateEditText.Text.Trim();

                // Update the scheduler object in the database
                DatabaseHelper.Instance.SchedulerDatabaseHelper.UpdateScheduler(scheduler);

                // Notify the adapter that the data has changed
                schedulerAdapter.NotifyDataSetChanged();

                dialog.Dismiss();
            };

            deleteButton.Click += (sender, e) =>
            {
                OnDeleteClicked(sender, e);
                dialog.Dismiss();
            };

            dialog.Show();
        }

        public class SchedulerAdapter : BaseAdapter<Scheduler>
        {
            private readonly Context context;
            private readonly LayoutInflater inflater;
            public List<Scheduler> Schedulers { get; private set; }

            public SchedulerAdapter(Context context, List<Scheduler> schedulers)
            {
                this.context = context;
                inflater = LayoutInflater.FromContext(context);
                Schedulers = schedulers;
            }

            public override int Count => Schedulers.Count;

            public override Scheduler this[int position] => Schedulers[position];

            public override long GetItemId(int position) => position;

            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                convertView = convertView ?? inflater.Inflate(Resource.Layout.SchedulerRow, parent, false);

                var textViewTitle = convertView.FindViewById<TextView>(Resource.Id.schedulerTitleTextView);
                var textViewDescription = convertView.FindViewById<TextView>(Resource.Id.schedulerDescriptionTextView);
                var textViewDate = convertView.FindViewById<TextView>(Resource.Id.schedulerDateTextView);

                textViewTitle.Text = Schedulers[position].Title;
                textViewDescription.Text = Schedulers[position].Description;
                textViewDate.Text = Schedulers[position].Date;

                return convertView;
            }

            public void Clear()
            {
                Schedulers.Clear();
                NotifyDataSetChanged();
            }

            public void AddRange(IEnumerable<Scheduler> schedulers)
            {
                Schedulers.AddRange(schedulers);
                NotifyDataSetChanged();
            }
        }
    }
}
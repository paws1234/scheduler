using Android.App;
using Android.Content;
using Android.Database;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;
using System.IO;
using Android.Database.Sqlite;
using sched;

namespace sched
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
    public class Scheduler
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }

        public ContentValues ToContentValues()
        {
            var values = new ContentValues();
            values.Put("Title", Title);
            values.Put("Description", Description);
            values.Put("Date", Date);
            return values;
        }
    }
    public class DatabaseHelper : SQLiteOpenHelper
    {
        private const string DbName = "sched.db3";
        private static readonly Lazy<DatabaseHelper> _instance = new Lazy<DatabaseHelper>(() => new DatabaseHelper(Application.Context));
        public SchedulerDatabaseHelper SchedulerDatabaseHelper => new SchedulerDatabaseHelper(this);
        public static DatabaseHelper Instance
        {
            get { return _instance.Value; }
        }

        public DatabaseHelper(Context context) : base(context, DbName, null, 1)
        {
        }

        public override void OnCreate(SQLiteDatabase db)
        {
            db.ExecSQL("CREATE TABLE IF NOT EXISTS User (Id INTEGER PRIMARY KEY, Username TEXT, Password TEXT, Email TEXT);");
            db.ExecSQL("CREATE TABLE IF NOT EXISTS Scheduler (Id INTEGER PRIMARY KEY, Title TEXT, Description TEXT, Date TEXT);");
        }

        public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
        {
            
        }

        public SQLiteDatabase GetConnection()
        {
            return this.WritableDatabase;
        }

        public User GetUser(int id)
        {
            using (var db = GetConnection())
            {
                string[] selectionArgs = new string[] { id.ToString() };
                var userCursor = db.Query("User", null, "Id = ?", selectionArgs, null, null, null);

                if (userCursor.MoveToFirst())
                {
                    return new User
                    {
                        Id = userCursor.GetInt(userCursor.GetColumnIndex("Id")),
                        Username = userCursor.GetString(userCursor.GetColumnIndex("Username")),
                        Password = userCursor.GetString(userCursor.GetColumnIndex("Password")),
                        Email = userCursor.GetString(userCursor.GetColumnIndex("Email"))
                    };
                }

                return null;
            }
        }

        public void SaveUser(User user)
        {
            using (var db = GetConnection())
            {
                db.Insert("User", null, user.ToContentValues());
            }
        }

        public void UpdateUser(User user)
        {
            using (var db = GetConnection())
            {
                db.Update("User", user.ToContentValues(), "Id = ?", new string[] { user.Id.ToString() });
            }
        }

        public void DeleteUser(User user)
        {
            using (var db = GetConnection())
            {
                db.Delete("User", "Id = ?", new string[] { user.Id.ToString() });
            }
        }

        public void DeleteAllUsers()
        {
            using (var db = GetConnection())
            {
                db.ExecSQL("DELETE FROM User;");
            }
        }

        public IEnumerable<User> GetAllUsers()
        {
            using (var db = GetConnection())
            {
                var userList = new List<User>();
                var userCursor = db.Query("User", null, null, null, null, null, null);

                while (userCursor.MoveToNext())
                {
                    userList.Add(new User
                    {
                        Id = userCursor.GetInt(userCursor.GetColumnIndex("Id")),
                        Username = userCursor.GetString(userCursor.GetColumnIndex("Username")),
                        Password = userCursor.GetString(userCursor.GetColumnIndex("Password")),
                        Email = userCursor.GetString(userCursor.GetColumnIndex("Email"))
                    });
                }

                return userList;
            }
        }
    }

    public static class Extensions
    {
        public static ContentValues ToContentValues(this User user)
        {
            var values = new ContentValues();
            values.Put("Username", user.Username);
            values.Put("Password", user.Password);
            values.Put("Email", user.Email);
            return values;
        }
    }
}
public class SchedulerDatabaseHelper
{
    private readonly DatabaseHelper _databaseHelper;
    private SQLiteDatabase _database;


    public SchedulerDatabaseHelper(DatabaseHelper databaseHelper)
    {
        _databaseHelper = databaseHelper;
    }

    public SQLiteDatabase GetDatabase()
    {
        if (_database == null || !_database.IsOpen)
        {
            _database = _databaseHelper.GetConnection();
        }

        return _database;
    }

    public void CreateScheduler(Scheduler scheduler)
    {
        using (var db = GetDatabase())
        {
            db.Insert("Scheduler", null, scheduler.ToContentValues());
        }
    }
    public void UpdateScheduler(Scheduler scheduler)
    {
        using (var db = GetDatabase())
        {
            db.Update("Scheduler", scheduler.ToContentValues(), "Id = ?", new string[] { scheduler.Id.ToString() });
        }
    }

    public void DeleteScheduler(int id)
    {
        using (var db = GetDatabase())
        {
            db.Delete("Scheduler", "Id = ?", new string[] { id.ToString() });
        }
    }

    public IEnumerable<Scheduler> GetAllSchedulers()
    {
        using (var db = GetDatabase())
        {
            var schedulerList = new List<Scheduler>();
            var schedulerCursor = db.Query("Scheduler", null, null, null, null, null, null);

            while (schedulerCursor.MoveToNext())
            {
                schedulerList.Add(new Scheduler
                {
                    Id = schedulerCursor.GetInt(schedulerCursor.GetColumnIndex("Id")),
                    Title = schedulerCursor.GetString(schedulerCursor.GetColumnIndex("Title")),
                    Description = schedulerCursor.GetString(schedulerCursor.GetColumnIndex("Description")),
                    Date = schedulerCursor.GetString(schedulerCursor.GetColumnIndex("Date"))
                });
            }

            return schedulerList;
        }
    }
}
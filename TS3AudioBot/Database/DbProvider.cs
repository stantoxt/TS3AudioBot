using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Data.SQLite;
using System.Data.Linq.Mapping;
using TS3AudioBot.ResourceFactories;
using System.Globalization;

namespace TS3AudioBot.Database
{
	class DbProvider
	{
		private const string connectionString = "Data Source=MyDatabase.sqlite;Version=3;";

		public DbProvider()
		{
			using (var sql = new SQLiteConnection(connectionString))
			{
				var ctx = new DataContext(sql);
				ctx.Log = Console.Out;

				//ctx.ExecuteCommand("DROP TABLE PlayHistory;");

				ctx.ExecuteCommand(
					"CREATE TABLE IF NOT EXISTS PlayHistory (" +
					"  Id            INTEGER      NOT NULL," +
					"  UserInvokeId  INTEGER      NOT NULL," +
					"  PlayCount     INTEGER      NOT NULL," +
					"  Timestamp     DATETIME     NOT NULL," +
					"  AudioType     VARCHAR(16)  NOT NULL," +
					"  ResourceId    TEXT         NOT NULL," +
					"  ResourceTitle TEXT         NOT NULL," +
					"  PRIMARY KEY (Id)" +
					");"
					);
				ctx.SubmitChanges();

				var x = ctx.GetTable<AudioLogEntry2>();

				//x.DeleteAllOnSubmit(from a in x select a);
				//ctx.SubmitChanges();

				x.InsertOnSubmit(new AudioLogEntry2(new AudioResource("asdf", "sc_ar1", AudioType.Soundcloud))
				{
					Id = 0,
					PlayCount = 0,
					Timestamp = DateTime.Now,
					UserInvokeId = 42,
				});

				ctx.SubmitChanges();

				var res = from a in x
						  select a;
				Console.WriteLine(string.Join("\n", res));
			}
		}

		private void CreateDatabaseSchemaV1()
		{

		}

		private void Create()
		{

		}
	}

	[Table(Name = "PlayHistory")]
	public class AudioLogEntry2
	{
		/// <summary>A unique id for each <see cref="ResourceFactories.AudioResource"/>, given by the history system.</summary>
		[Column(IsPrimaryKey = true)] public int Id { get; set; }
		/// <summary>The dbid of the teamspeak user, who played this song first.</summary>
		[Column] public ulong UserInvokeId { get; set; }
		/// <summary>How often the song has been played.</summary>
		[Column] public uint PlayCount { get; set; }
		/// <summary>The last time this song has been played.</summary>
		[Column] public DateTime Timestamp { get; set; }

		/// <summary>The resource type.</summary>
		[Column] public AudioType AudioType { get => AudioResource.AudioType; set => AudioResource.AudioType = value; }
		/// <summary>An identifier to create the song. This id is uniqe among same <see cref="TS3AudioBot.AudioType"/> resources.</summary>
		[Column] public string ResourceId { get => AudioResource.ResourceId; set => AudioResource.ResourceId = value; }
		/// <summary>The display title.</summary>
		[Column] public string ResourceTitle { get => AudioResource.ResourceTitle; set => AudioResource.ResourceTitle = value; }

		public AudioResource AudioResource { get; set; }

		public AudioLogEntry2()
		{
			AudioResource = new AudioResource();
		}

		public AudioLogEntry2(AudioResource resource)
		{
			AudioResource = resource;
		}

		public AudioLogEntry2(int id, AudioResource resource) : this(resource)
		{
			Id = id;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "[{0}] @ {1} by {2}: {3}, ({4})", Id, Timestamp, UserInvokeId, AudioResource.ResourceTitle, AudioResource);
		}
	}
}

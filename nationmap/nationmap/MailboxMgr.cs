using System;
using System.Collections;
using System.Text;
using LitJson;

namespace Prj301.MailSystem
{
	public interface IJsonString
	{
		string ToJsonString ();
	}
	#region Mail Attachment
	abstract class MailAttachment : IJsonString
	{
		public virtual string ToJsonString ()
		{
			return string.Empty;
		}
	}

	class CoordinatesAttachment : MailAttachment
	{
		public CoordinatesAttachment (int x, int y)
		{
			this.PosX = x;
			this.PosY = y;
		}

		public int PosX{ get; set; }

		public int PosY{ get; set; }

		public override string ToJsonString ()
		{
			StringBuilder sb = new StringBuilder ();
			sb.Append (this.PosX);
			sb.Append (',');
			sb.Append (this.PosY);
			return sb.ToString ();
		}
	}
	#endregion
	#region Mail
	class Mail : IJsonString
	{
		public Mail (string mailID, Type type)
		{
			this.MailID = mailID;
			this.MailType = type;
			this.SendDate = System.TimeZone.CurrentTimeZone.ToLocalTime (new System.DateTime (1970, 1, 1));
			m_attachments = new ArrayList ();
		}

		~Mail ()
		{
		}

		public enum Type
		{
			CUSTOM = 0,
			BATTLE = CUSTOM + 1,
			SCOUT = BATTLE + 1,
			BE_SCOUT = SCOUT + 1,
			COOLLECTION = BE_SCOUT + 1,
			FORGING = COOLLECTION + 1,
			DEFENCE = FORGING + 1,
			REWARD = DEFENCE + 1,
			NOTIFY = REWARD + 1,
			NUM = NOTIFY + 1,
		}
		//private string m_mailID;
		public string MailID{ get; set; }

		public Type MailType{ get; set; }

		public string Title{ get; set; }

		public string Content{ get; set; }

		public string Sender{ get; set; }

		public bool Read{ get; set; }

		public bool Favorite{ get; set; }

		public string Recipient{ get; set; }

		public DateTime SendDate{ get; set; }

		private ArrayList m_attachments;

		public bool IsCustomMail ()
		{
			return this.MailType == Type.CUSTOM;
		}

		public bool IsReportMail ()
		{
			return this.MailType >= Type.BATTLE && this.MailType <= Type.REWARD;
		}

		public bool IsNotifyMail ()
		{
			return this.MailType == Type.NOTIFY;
		}

		public void AddCoordinates (CoordinatesAttachment att)
		{
			m_attachments.Add (att);
		}

		public ArrayList GetAttachments ()
		{
			return m_attachments;
		}

		public int AttachmentsLength ()
		{
			return m_attachments.Count;
		}

		public MailAttachment GetAttachment (int index)
		{
			if (index < 0 || index > this.AttachmentsLength () - 1)
				throw new Exception ("Index out of range");

			return m_attachments [index] as MailAttachment;
		}

		public string ToJsonString ()
		{
			// build json data
			StringBuilder sb = new StringBuilder ();

			JsonWriter writer = new JsonWriter (sb);


			writer.WriteObjectStart ();
			{
				writer.WritePropertyName ("MailID");
				writer.Write (this.MailID);

				writer.WritePropertyName ("MailType");
				writer.Write ((int)this.MailType);

				writer.WritePropertyName ("Title");
				writer.Write (this.Title);

				writer.WritePropertyName ("Content");
				writer.Write (this.Content);

				writer.WritePropertyName ("Recipient");
				writer.Write (this.Recipient);

				writer.WritePropertyName ("SendDate");
				writer.Write (ConvertToUnixTime (this.SendDate));

				writer.WritePropertyName ("Read");
				writer.Write (this.Read);

				writer.WritePropertyName ("Favorite");
				writer.Write (this.Favorite);

				writer.WritePropertyName ("Attachments");
				writer.WriteArrayStart ();
				{
					for (int i = 0; i < this.AttachmentsLength(); i++)
						writer.Write ((m_attachments [i] as MailAttachment).ToJsonString ());
				}
				writer.WriteArrayEnd ();
			}
			writer.WriteObjectEnd ();

			return sb.ToString ();
		}

		public double ConvertToUnixTime (DateTime time)
		{
			double intResult = 0;
			DateTime startTime = System.TimeZone.CurrentTimeZone.ToLocalTime (new System.DateTime (1970, 1, 1));
			intResult = (time - startTime).TotalSeconds;
			return intResult;
		}
	}
	#endregion
	#region Mailbox
	class MailboxMgr
	{
		public MailboxMgr ()
		{
			m_mailHolder = new ArrayList ();
			for (int i = 0; i < (int)Category.NUM; i++) {
				m_mailHolder.Add (new ArrayList ());
			}

			m_favoriteMailRef = new ArrayList ();
		}

		~MailboxMgr ()
		{
		}

		public enum Category
		{
			OUTBOX = 0,
			CUSTOM = OUTBOX + 1,
			NOTIFY = CUSTOM + 1,
			REPORT = NOTIFY + 1,
			NUM = REPORT + 1,
			FAVORITE = NUM + 1,
		}

		private static readonly MailboxMgr _instance = new MailboxMgr ();

		public static MailboxMgr getInstance ()
		{
			return _instance;
		}

		private ArrayList m_mailHolder;
		private ArrayList m_favoriteMailRef;

		public void AddMail (Mail mail)
		{
			if (mail.IsCustomMail ())
				(m_mailHolder [(int)Category.CUSTOM] as ArrayList).Add (mail);
			else if (mail.IsNotifyMail ())
				(m_mailHolder [(int)Category.NOTIFY] as ArrayList).Add (mail);
			else if (mail.IsReportMail ())
				(m_mailHolder [(int)Category.REPORT] as ArrayList).Add (mail);
			else
				throw new Exception ("Unhandled type.");

			if (mail.Favorite)
				m_favoriteMailRef.Add (mail);
		}

		public int MailLength (Category cate)
		{
			if (cate == Category.NUM)
				return 0;

			if (cate == Category.FAVORITE)
				return m_favoriteMailRef.Count;
			else {
				return (m_mailHolder [(int)cate] as ArrayList).Count;
			}
		}

		public Mail GetMail (Category cate, int index)
		{
			if (cate == Category.NUM)
				throw new Exception ("Bad category");

			if (index < 0 || index > this.MailLength (cate) - 1)
				throw new Exception ("Index out of range");

			if (cate == Category.FAVORITE)
				return m_favoriteMailRef [index] as Mail;
			else 
				return (m_mailHolder [(int)cate] as ArrayList) [index] as Mail;
		}
		/*
		public string InboxToJsonString (int startIndex, int endIndex)
		{
			if (startIndex < 0 || startIndex > this.InboxLength () - 1)
				throw new Exception ("startIndex out of range");

			if (endIndex < 0 || endIndex > this.InboxLength () - 1)
				throw new Exception ("endIndex out of range");

			if (endIndex < startIndex)
				throw new Exception ("endIndex < startIndex");

			bool isInbox = true;
			StringBuilder sb = BuildMailJsonData (isInbox, startIndex, endIndex);
			return sb.ToString ();
		}

		public string OutboxToJsonString (int startIndex, int endIndex)
		{
			if (startIndex < 0 || startIndex > this.InboxLength () - 1)
				throw new Exception ("startIndex out of range");

			if (endIndex < 0 || endIndex > this.InboxLength () - 1)
				throw new Exception ("endIndex out of range");

			if (endIndex < startIndex)
				throw new Exception ("endIndex < startIndex");

			bool isInbox = false;
			StringBuilder sb = BuildMailJsonData (isInbox, startIndex, endIndex);
			return sb.ToString ();
		}
*/
		public StringBuilder MailToJsonString (Category cate, int startIndex, int endIndex)
		{
			if (cate == Category.NUM)
				throw new Exception ("Bad category");

			ArrayList mailList = null;
			if (cate == Category.FAVORITE)
				mailList = m_favoriteMailRef;
			else
				mailList = m_mailHolder [(int)cate] as ArrayList;

			if (startIndex < 0 || startIndex > this.MailLength (cate) - 1)
				throw new Exception ("startIndex out of range");

			if (endIndex < 0 || endIndex > this.MailLength (cate) - 1)
				throw new Exception ("endIndex out of range");

			if (endIndex < startIndex)
				throw new Exception ("endIndex < startIndex");

			// build json data
			StringBuilder sb = new StringBuilder ();

			sb.Append ("{\"Mails\":[");
			for (int i = startIndex; i<=endIndex; i++) {
				sb.Append ((mailList [i] as Mail).ToJsonString ());
				if (i < endIndex)
					sb.Append (",");
			}
			sb.Append ("]");
			sb.Append (",");
			sb.Append ("\"MailsCount\":");
			sb.Append (endIndex - startIndex + 1);
			sb.Append ("}");

			return sb;

			/*
			StringBuilder sb = new StringBuilder ();

			JsonWriter writer = new JsonWriter (sb);
			writer.WriteObjectStart ();

			writer.WritePropertyName ("name");
			writer.Write ("ethan");

			writer.WritePropertyName ("age");
			writer.Write (31);

			writer.WritePropertyName ("ArrayTset");
			writer.WriteObjectStart ();

			writer.WritePropertyName ("name1");
			writer.Write (1);
			writer.WritePropertyName ("name2");
			writer.Write (2);
			writer.WritePropertyName ("name3");
			writer.Write (3);

			writer.WriteObjectEnd ();


			writer.WriteObjectEnd ();

			return sb.ToString ();

			 */
			/*
			JsonData jd = new JsonData ();
			jd ["age"] = 30;
			jd ["name"] = "ethan";

			JsonData book = new JsonData ();
			book ["bookname"] = "C++";
			jd ["books"] = book;




			jd["array"][0] = 0;
			jd["array"][1] = 1;


			return jd.ToJson ();*/
		}

		public ArrayList SearchMail (string key)
		{
			ArrayList al = new ArrayList ();
			foreach (Mail m in (ArrayList)m_mailHolder[(int)Category.CUSTOM]) {
				if (m.Title.Contains (key) || m.Content.Contains (key))
					al.Add (m);
			}

			return al;
		}
		/*
		public string SearchMail(string key)
		{
			ArrayList al = SearchMail (key);
			return "";
		}
*/
		public bool setFavorite (Category cate, string mailID)
		{
			if (cate == Category.NUM)
				throw new Exception ("Bad category");

			ArrayList mailList = null;
			if (cate == Category.FAVORITE)
				mailList = m_favoriteMailRef;
			else
				mailList = m_mailHolder [(int)cate] as ArrayList;

			foreach (Mail m in mailList) {
				if (m.MailID == mailID) {
					m.Favorite = true;
					m_favoriteMailRef.Add (m);
					return true;
				}
			}

			return false;
		}

		public bool unsetFavorite (string mailID)
		{
			foreach (Mail m in m_favoriteMailRef) {
				if (m.MailID == mailID) {
					m.Favorite = false;
					m_favoriteMailRef.Remove (m);
					return true;
				}
			}

			return false;
		}

		public bool unsetFavorite (Mail mail)
		{
			return unsetFavorite (mail.MailID);
		}
	}
	#endregion
}


























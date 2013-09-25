using System;
using System.Collections;

namespace bigmap
{
	#region Mail Attachment
	abstract class MailAttachment
	{

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
	}
	#endregion
	#region Mail
	class Mail
	{
		public Mail ()
		{
			m_attachments = new ArrayList ();
		}

		~Mail ()
		{
		}

		public enum Type
		{
			CUSTOM_MAIL = 0,
			BATTLE_REPORT = 1,
			SCOUT_REPORT = 2,
			NUM = 3,
		}

		public Type MailType{ get; set; }

		public string MailID{ get; set; }

		public string Title{ get; set; }

		public string Content{ get; set; }

		public string Sender{ get; set; }

		public bool Read{ get; set; }

		public string Recipient{ get; set; }

		public DateTime SendDate{ get; set; }

		private ArrayList m_attachments;

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
	}
	#endregion
	#region Mailbox
	class MailboxMgr
	{
		public MailboxMgr ()
		{
			m_mailList = new ArrayList ();
		}

		~MailboxMgr ()
		{
		}

		private static readonly MailboxMgr _instance = new MailboxMgr ();

		public static MailboxMgr getInstance ()
		{
			return _instance;
		}

		private ArrayList m_mailList;

		public void AddMail (Mail mail)
		{
			m_mailList.Add (mail);
		}

		public int MailLength ()
		{
			return m_mailList.Count;
		}

		public Mail GetMail (int index)
		{
			if (index < 0 || index > this.MailLength () - 1)
				throw new Exception ("Index out of range");

			return m_mailList [index] as Mail;
		}
	}
	#endregion
}


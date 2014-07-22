/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using TextMetal.Common.Core;
using TextMetal.HostImpl.AspNetSample.Common;

namespace TextMetal.HostImpl.AspNetSample.DomainModel.Tables
{
	public partial class User
	{
		#region Fields/Constants

		private const int SALT_SIZE = 64;
		public const int SYSTEM_USER_ID = 0;

		#endregion

		#region Properties/Indexers/Events

		private string TransientAnswerClearText
		{
			get;
			set;
		}

		private string TransientPassswordClearText
		{
			get;
			set;
		}

		#endregion

		#region Methods/Operators

		public static bool Exists(User user)
		{
			IEnumerable<IUser> users;

			if ((object)user == null)
				throw new ArgumentNullException("user");

			users =
				Stuff.Get<IRepository>("").FindUsers(
					q => q.Where(u => (u.UserName == user.UserName || u.EmailAddress == user.EmailAddress) && ((object)user.UserId == null || u.UserId != user.UserId)));

			return users.Count() > 0;
		}

		public static string GenerateCryptoSafeSaltValue()
		{
			//UnicodeEncoding encoding;
			RNGCryptoServiceProvider random;
			byte[] saltBits;
			string saltValue;

			//encoding = new UnicodeEncoding();
			using (random = new RNGCryptoServiceProvider())
			{
				saltBits = new byte[SALT_SIZE];
				random.GetBytes(saltBits);
				//saltValue = encoding.GetString(saltBits);

				saltValue = "";
				foreach (byte saltBit in saltBits)
					saltValue += saltBit.ToString("X2", CultureInfo.InvariantCulture.NumberFormat);
			}

			return saltValue;
		}

		private static string HashWithSalt(string clearText, string saltValue)
		{
			HashAlgorithm hash;
			UnicodeEncoding encoding;
			byte[] clearTextPasswordBits;
			byte[] hashBits;
			string saltedHash;

			if ((object)clearText == null)
				throw new ArgumentNullException("clearText");

			if ((object)saltValue == null)
				throw new ArgumentNullException("saltValue");

			using (hash = new SHA512Managed())
			{
				encoding = new UnicodeEncoding();

				clearTextPasswordBits = encoding.GetBytes(clearText + saltValue);
				hashBits = hash.ComputeHash(clearTextPasswordBits);

				saltedHash = "";
				foreach (byte hashBit in hashBits)
					saltedHash += hashBit.ToString("X2", CultureInfo.InvariantCulture.NumberFormat);
			}

			return saltedHash;
		}

		public static IEnumerable<Message> ValidateForAuthentication(string username, string password)
		{
			List<Message> messages;

			messages = new List<Message>();

			if (DataType.IsNullOrWhiteSpace(username))
				messages.Add(new Message("", "Username is required.", Severity.Error));

			if (DataType.IsNullOrWhiteSpace(password))
				messages.Add(new Message("", "Password is required.", Severity.Error));

			return messages;
		}

		public static IEnumerable<Message> ValidateForForgotPassword(string username)
		{
			List<Message> messages;

			messages = new List<Message>();

			if (DataType.IsNullOrWhiteSpace(username))
				messages.Add(new Message("", "Username is required.", Severity.Error));

			return messages;
		}

		public static IEnumerable<Message> ValidateForForgotUserName(string emailAddress)
		{
			List<Message> messages;

			messages = new List<Message>();

			if (DataType.IsNullOrWhiteSpace(emailAddress))
				messages.Add(new Message("", "Email address is required.", Severity.Error));
			else
			{
				if (!DataType.IsValidEmailAddress(emailAddress))
					messages.Add(new Message("", "Email address is invalid.", Severity.Error));
			}

			return messages;
		}

		public bool CheckAnswer(string answerClearText)
		{
			string value;

			value = HashWithSalt(answerClearText ?? "", this.SaltValue ?? "");

			return this.AnswerHash == value;
		}

		public bool CheckPassword(string passwordClearText)
		{
			string value;

			value = HashWithSalt(passwordClearText ?? "", this.SaltValue ?? "");

			return this.PasswordHash == value;
		}

		partial void OnMark()
		{
			DateTime now;

			now = DateTime.UtcNow;

			this.CreationTimestamp = this.CreationTimestamp ?? now;
			this.ModificationTimestamp = !this.IsNew ? now : this.CreationTimestamp;
			this.CreationUserId = ((this.IsNew ? Current.UserId : this.CreationUserId) ?? this.CreationUserId) ?? SYSTEM_USER_ID;
			this.ModificationUserId = ((!this.IsNew ? Current.UserId : this.CreationUserId) ?? this.ModificationUserId) ?? SYSTEM_USER_ID;
			this.LogicalDelete = this.LogicalDelete ?? false;

			this.FailedLoginCount = this.FailedLoginCount ?? 0;
			this.MustChangePassword = this.MustChangePassword ?? false;
		}

		partial void OnValidate(ref IEnumerable<Message> messages)
		{
			List<Message> _messages;
			bool exists;

			_messages = new List<Message>();

			if (DataType.IsNullOrWhiteSpace(this.EmailAddress))
				_messages.Add(new Message("", "Email address is required.", Severity.Error));
			else
			{
				if (!DataType.IsValidEmailAddress(this.EmailAddress))
					_messages.Add(new Message("", "Email address is invalid.", Severity.Error));
			}

			if (DataType.IsNullOrWhiteSpace(this.UserName))
				_messages.Add(new Message("", "Username is required.", Severity.Error));
			else if (this.UserName.Length < 3)
				_messages.Add(new Message("", "Username minimum length is 3.", Severity.Error));
			//else if (Regex.IsMatch(this.UserName, "[a-zA-Z][a-zA-Z0-9\\.-_]{1,255}", RegexOptions.IgnorePatternWhitespace))
			//_messages.Add(new Message("", "Username is invalid.", Severity.Error));

			if (DataType.IsNullOrWhiteSpace(this.SaltValue))
				_messages.Add(new Message("", "Salt value is required.", Severity.Error));

			if (DataType.IsNullOrWhiteSpace(this.PasswordHash))
				_messages.Add(new Message("", "Password is required.", Severity.Error));

			if (!DataType.IsNullOrWhiteSpace(this.TransientPassswordClearText) &&
				this.TransientPassswordClearText.Length < 4)
				_messages.Add(new Message("", "Password minimum length is 4.", Severity.Error));

			if (DataType.IsNullOrWhiteSpace(this.Question))
				_messages.Add(new Message("", "Security question is required.", Severity.Error));
			else if (this.Question.Length < 3)
				_messages.Add(new Message("", "Security question minimum length is 3.", Severity.Error));

			if (DataType.IsNullOrWhiteSpace(this.AnswerHash))
				_messages.Add(new Message("", "Security answer is required.", Severity.Error));

			if (!DataType.IsNullOrWhiteSpace(this.TransientAnswerClearText) &&
				this.TransientAnswerClearText.Length < 3)
				_messages.Add(new Message("", "Security answer minimum length is 3.", Severity.Error));

			if (_messages.Count > 0)
			{
				messages = _messages;
				return;
			}

			exists = Exists(this);

			if (exists)
				_messages.Add(new Message("", "Username and email address each must be unique.", Severity.Error));

			messages = _messages;
		}

		public void SetAnswer(string value)
		{
			this.TransientAnswerClearText = value;

			value = HashWithSalt(this.TransientPassswordClearText ?? "", this.SaltValue ?? "");

			this.AnswerHash = value;
		}

		public void SetPassword(string value)
		{
			this.TransientPassswordClearText = value;

			value = HashWithSalt(this.TransientPassswordClearText ?? "", this.SaltValue ?? "");

			this.PasswordHash = value;
		}

		#endregion
	}
}
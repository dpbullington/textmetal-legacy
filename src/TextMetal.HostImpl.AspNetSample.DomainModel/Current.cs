/*
	Copyright ©2002-2014 Daniel Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;

using TextMetal.HostImpl.AspNetSample.Common;

namespace TextMetal.HostImpl.AspNetSample.DomainModel
{
	public static class Current
	{
		#region Fields/Constants

		private static readonly Dictionary<string, Tuple<int?, int?>> volatileCookieToUserMap = new Dictionary<string, Tuple<int?, int?>>();

		#endregion

		#region Properties/Indexers/Events

		public static int? ChildId
		{
			get
			{
				int? value;

				value = Stuff.Session.GetValue<int?>("CurrentChildId");

				return value;
			}
			set
			{
				Stuff.Session.FreeValue("CurrentChildId");
				Stuff.Session.SetValue<int?>("CurrentChildId", value);
			}
		}

		public static int? CircleId
		{
			get
			{
				int? value;

				value = Stuff.Session.GetValue<int?>("CurrentCircleId");

				return value;
			}
			set
			{
				Stuff.Session.FreeValue("CurrentCircleId");
				Stuff.Session.SetValue<int?>("CurrentCircleId", value);
			}
		}

		public static int FailedLoginCount
		{
			get
			{
				int? value;

				value = Stuff.Session.GetValue<int?>("CurrentFailedLoginCount");

				return value ?? 0;
			}
			set
			{
				Stuff.Session.FreeValue("CurrentFailedLoginCount");
				Stuff.Session.SetValue<int?>("CurrentFailedLoginCount", value);
			}
		}

		public static int? FamilyId
		{
			get
			{
				int? value;

				value = Stuff.Session.GetValue<int?>("CurrentFamilyId");

				return value ?? ShouldRememberMeFamilyId; // fallback
			}
			set
			{
				Stuff.Session.FreeValue("CurrentFamilyId");
				Stuff.Session.SetValue<int?>("CurrentFamilyId", value);
			}
		}

		public static bool MustChangePassword
		{
			get
			{
				bool? value;

				value = Stuff.Session.GetValue<bool?>("CurrentMustChangePassword");

				return value ?? false;
			}
			set
			{
				Stuff.Session.FreeValue("CurrentMustChangePassword");
				Stuff.Session.SetValue<bool?>("CurrentMustChangePassword", value);
			}
		}

		public static int? ParentId
		{
			get
			{
				int? value;

				value = Stuff.Session.GetValue<int?>("CurrentParentId");

				return value;
			}
			set
			{
				Stuff.Session.FreeValue("CurrentParentId");
				Stuff.Session.SetValue<int?>("CurrentParentId", value);
			}
		}

		public static int? ShouldRememberMeFamilyId
		{
			get
			{
				if ((object)ShouldRememberMeToken != null)
					return ShouldRememberMeToken.Item2;
				else
					return null;
			}
		}

		public static Tuple<int?, int?> ShouldRememberMeToken
		{
			get
			{
				Tuple<int?, int?> pair;
				string token;

				token = Stuff.StickySession.GetValue<string>("ShouldRememberMeToken");

				if ((object)token == null)
					return null;

				lock (volatileCookieToUserMap)
				{
					if (!volatileCookieToUserMap.TryGetValue(token, out pair))
						return null;
				}

				return pair;
			}
			set
			{
				string token;
				Tuple<int?, int?> pair;

				token = Stuff.StickySession.GetValue<string>("ShouldRememberMeToken");
				Stuff.StickySession.FreeValue("ShouldRememberMeToken");

				if ((object)token != null)
				{
					lock (volatileCookieToUserMap)
					{
						if (volatileCookieToUserMap.TryGetValue(token, out pair))
							volatileCookieToUserMap.Remove(token);
					}
				}

				if ((object)value != null)
				{
					token = Guid.NewGuid().ToString();

					lock (volatileCookieToUserMap)
						volatileCookieToUserMap.Add(token, value);

					Stuff.StickySession.SetValue<string>("ShouldRememberMeToken", token);
				}
			}
		}

		public static int? ShouldRememberMeUserId
		{
			get
			{
				if ((object)ShouldRememberMeToken != null)
					return ShouldRememberMeToken.Item1;
				else
					return null;
			}
		}

		public static bool? SignUpFamilyNotFinalized
		{
			get
			{
				bool? value;

				value = Stuff.Session.GetValue<bool?>("SignUpFamilyNotFinalized");

				return value ?? false;
			}
			set
			{
				Stuff.Session.FreeValue("SignUpFamilyNotFinalized");
				Stuff.Session.SetValue<bool?>("SignUpFamilyNotFinalized", value);
			}
		}

		public static int? UserId
		{
			get
			{
				int? value;

				value = Stuff.Session.GetValue<int?>("CurrentUserId");

				return value ?? ShouldRememberMeUserId; // fallback
			}
			set
			{
				Stuff.Session.FreeValue("CurrentUserId");
				Stuff.Session.SetValue<int?>("CurrentUserId", value);
			}
		}

		#endregion

		#region Methods/Operators

		public static void Exit()
		{
			ShouldRememberMeToken = null;

			UserId = null;
			FamilyId = null;
			ChildId = null;
			ParentId = null;
			CircleId = null;

			FailedLoginCount = 0;
			MustChangePassword = false;
		}

		#endregion
	}
}